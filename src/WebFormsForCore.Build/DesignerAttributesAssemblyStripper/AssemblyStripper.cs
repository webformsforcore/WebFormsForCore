using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mono.Cecil;

namespace EstrellasDeEsperanza.WebFormsForCore.Build;

public class AssemblyStripper
{
	public Action<string> LogWarning;
	public Action<string> LogMessage;

	public IEnumerable<TypeReference> BaseTypes(TypeReference type, Action<TypeDefinition> each = null)
	{
		if (type == null) yield break;
		else {
			yield return type;
			var definition = type.Resolve();
			if (definition != null) each?.Invoke(definition);
			while (definition?.BaseType != null) {
				type = definition.BaseType;
				yield return type;
				definition = type.Resolve();
				if (definition != null) each?.Invoke(definition);
			}
		}
	}

	public bool HasInvalidBaseType(TypeReference type)
	{
		if (type == null) return false;
		try
		{
			foreach (var baseType in BaseTypes(type))
			{
				var aname = baseType.Module.Assembly.Name.Name;
				if (aname == "System.Drawing" ||
					aname == "System.Drawing.Common")
					throw new AssemblyResolutionException(baseType.Module.Assembly.Name);
			}
		} catch (AssemblyResolutionException ex)
		{
			var match = Regex.Match(ex.Message, @":\s*'(?<fullassembly>(?<assembly>[^,]*)[^']*)");
			var assembly = match.Groups["assembly"].Value;
			if (assembly == "System.Configuration" ||
				assembly == "System.Web" ||
				assembly == "System.Web.Services" ||
				assembly == "System.Web.Extensions" ||
				assembly == "System.Web.Optimization" ||
				assembly == "System.Web.Mobile" ||
				assembly == "Microsoft.AspNet.Web.Optimization.WebForms" ||
				assembly == "WebGrease")
			{
				LogWarning($"Could not resolve assembly {match.Groups["fullassembly"].Value}, consider referencing the corresponding WebFormsForCore nuget package.");
				return false;
			}
			else return true;
		}
		return false;
	}	

	public bool Strip(Mono.Collections.Generic.Collection<CustomAttribute> attributes, TypeReference type, AssemblyDefinition assembly)
	{
		bool stripped = false;
		foreach (var attribute in attributes.ToArray())
		{
			var isInvalid = HasInvalidBaseType(attribute.AttributeType) || attribute.ConstructorArguments
				.Any(arg => arg.Type.FullName == "System.Type" && arg.Value is TypeReference typeRef &&
					HasInvalidBaseType(typeRef));
			if (isInvalid)
			{
				// Remove the attribute from the type
				attributes.Remove(attribute);
				LogMessage?.Invoke($"Removed attribute {attribute.AttributeType.Name} from type {type.Name} in assembly {assembly.Name.Name}.");
				stripped = true;
			}
		}
		return stripped;

	}
	public void Strip(AssemblyDefinition assembly)
	{
		bool stripped = false;

		if (assembly == null) return;

		var targetFramework = assembly.CustomAttributes
			.FirstOrDefault(attr => attr.AttributeType.FullName == "System.Runtime.Versioning.TargetFrameworkAttribute")
			?.ConstructorArguments.FirstOrDefault().Value as string;
		if (targetFramework == null || targetFramework.IndexOf("Framework", StringComparison.OrdinalIgnoreCase) < 0)
		{
			// Not a .NET Framework assembly, skip stripping
			return;
		}

		var types = assembly.Modules
			.SelectMany(mod => mod.Types);
		foreach (var type in types)
		{
			stripped |= Strip(type.CustomAttributes, type, assembly);
			foreach (var field in type.Fields) stripped |= Strip(field.CustomAttributes, type, assembly);
			foreach (var method in type.Methods) stripped |= Strip(method.CustomAttributes, type, assembly);
			foreach (var prop in type.Properties) stripped |= Strip(prop.CustomAttributes, type, assembly);
		}

		if (stripped)
		{
			assembly.Write();
			LogMessage?.Invoke($"Stripped assembly {assembly.Name.Name} from designer attributes.");
		}
	}

	public void StripPath(string path)
	{
		var dlls = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);

		var paths = dlls
			.Select(dll => Path.GetDirectoryName(dll))
			.Distinct();

 		var resolver = new AssemblyResolver();
		foreach (var p in paths) resolver.AddSearchDirectory(p);

		var runtimeConfigs = Directory.GetFiles(path, "*.runtimeconfig.json", SearchOption.TopDirectoryOnly);
		resolver.AddRuntimeConfig(runtimeConfigs);

		var parameters = new ReaderParameters
		{
			AssemblyResolver = resolver,
			ReadWrite = true, // Allow writing back to the assembly
			ReadingMode = ReadingMode.Deferred // Use deferred reading for performance
		};
		foreach (var dll in dlls)
		{
			AssemblyDefinition assembly = null;
			try
			{
				try
				{
					assembly = AssemblyDefinition.ReadAssembly(dll, parameters);
				} catch (Exception ex)
				{
					continue;
				}
				Strip(assembly);
			}
			catch (Exception ex)
			{
				LogWarning($"Failed to process {dll}: {ex.Message}");
			} finally
			{
				assembly?.Dispose();
			}
		}	
	}
}
