using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EstrellasDeEsperanza.WebFormsForCore.Build;


public class AssemblyResolver: DefaultAssemblyResolver {

	readonly List<string> directories = new List<string>();
	
	public new void AddSearchDirectory(string directory)
	{
		directories.Add(directory);
	}

	public new void RemoveSearchDirectory(string directory)
	{
		directories.Remove(directory);
	}
	static bool IsZero(Version version)
	{
		return version.Major == 0 && version.Minor == 0 && version.Build == 0 && version.Revision == 0;
	}
	public override AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
	{
		var assembly = SearchDirectory(name, directories, parameters);
		if (assembly != null)
			return assembly;

		throw new AssemblyResolutionException(name);
	}

	public class Pack
	{
		public string Name { get; set; }
		public Version Version { get; set; }
		public string Path { get; set; }
	}

	public void AddRuntimeConfig(params IEnumerable<string> filenames)
	{
		var dotnetInfo = Shell.Standard.Exec("dotnet --list-runtimes").Output().Result;

		var matches = Regex.Matches(dotnetInfo, @"\s+(?<name>[a-zA-Z0-9.]+)\s+(?<version>[0-9.]+)\s+\[(?<path>[^\]]+)\]");
		var packs = matches
			.OfType<Match>()
			.Select(m => new Pack
			{
				Name = m.Groups["name"].Value,
				Version = Version.Parse(m.Groups["version"].Value),
				Path = m.Groups["path"].Value
			}).ToList();

		var paths = filenames
			.SelectMany(filename =>
			{
				if (string.IsNullOrEmpty(filename) || !File.Exists(filename)) return Enumerable.Empty<string>();

				var json = File.ReadAllText(filename);
				var config = JsonConvert.DeserializeObject<dynamic>(json);
				var used = new List<Pack>();
				try
				{
					var framework = config.runtimeOptions?.framework;
					used.Add(new Pack
					{
						Name = framework.name,
						Version = framework.version
					});
				}
				catch { }
				
				try
				{
					foreach (var framework in config.runtimeOptions?.frameworks)
					{
						used.Add(new Pack
						{
							Name = framework.name,
							Version = framework.version
						});
					}
				} catch { }

				var uses = packs
					.Join(used, p => new { p.Name, Version = p.Version.Major }, u => new { u.Name, Version = u.Version.Major }, (p, u) => p)
					.GroupBy(p => p.Name)
					.Select(p => new { Name = p.Key, Max = p.OrderByDescending(q => q.Version).FirstOrDefault() });

				return uses.Select(use => Path.Combine(use.Max.Path, use.Max.Version.ToString()));
			})
			.Distinct();

		directories.AddRange(paths);
	}
}

