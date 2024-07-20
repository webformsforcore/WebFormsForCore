#region Assembly System.CodeDom, Version=8.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51
// C:\Users\simon\.nuget\packages\system.codedom\8.0.0\lib\net8.0\System.CodeDom.dll
// Decompiled with ICSharpCode.Decompiler 8.1.1.7464
#endregion

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
#if NETCOREAPP
using W = EstrellasDeEsperanza.WebFormsCore.CodeDom.Compiler;
#else
using W = System.CodeDom.Compiler;
#endif

namespace EstrellasDeEsperanza.WebFormsCore.CodeDom.Compiler;

//
// Summary:
//     Provides access to instances of the C# code generator and code compiler.
public class CSharpCodeProviderBase : W.CodeDomProvider
{
#if NETCOREAPP
	const string CodeDomAssembly = "System.CodeDom";
#else
	const string CodeDomAssembly  = "System"; 
#endif
	private Type Type => Type.GetType($"Microsoft.CSharp.CSharpCodeGenerator, {CodeDomAssembly}");
	private Type typeAttributeConverterType => Type.GetType($"Microsoft.CSharp.CSharpTypeAttributeConverter, {CodeDomAssembly}");
	private Type memberAttributeConverterType => Type.GetType($"Microsoft.CSharp.CSharpMemberAttributeConverter, {CodeDomAssembly}");

	private readonly ICodeGenerator _generator;

	//
	// Summary:
	//     Gets the file name extension to use when creating source code files.
	//
	// Returns:
	//     The file name extension to use for generated source code files.
	public override string FileExtension => "cs";

	//
	// Summary:
	//     Initializes a new instance of the Microsoft.CSharp.CSharpCodeProvider class.
	public CSharpCodeProviderBase(): base()
	{
		_generator = (ICodeGenerator)Activator.CreateInstance(Type, true);
	}

	//
	// Summary:
	//     Initializes a new instance of the Microsoft.CSharp.CSharpCodeProvider class by
	//     using the specified provider options.
	//
	// Parameters:
	//   providerOptions:
	//     A System.Collections.Generic.IDictionary`2 object that contains the provider
	//     options.
	//
	// Exceptions:
	//   T:System.ArgumentNullException:
	//     providerOptions is null.
	public CSharpCodeProviderBase(IDictionary<string, string> providerOptions): base()
	{
		if (providerOptions == null)
		{
			throw new ArgumentNullException("providerOptions");
		}

		_generator = (ICodeGenerator)Activator.CreateInstance(Type, BindingFlags.NonPublic | BindingFlags.Public,
			null, new object[] { providerOptions }, null);
	}

	//
	// Summary:
	//     Gets an instance of the C# code generator.
	//
	// Returns:
	//     An instance of the C# System.CodeDom.Compiler.ICodeGenerator implementation.
	[Obsolete("ICodeGenerator has been deprecated. Use the methods directly on the CodeDomProvider class instead.")]
	public override ICodeGenerator CreateGenerator()
	{
		return _generator;
	}

	//
	// Summary:
	//     Gets an instance of the C# code compiler.
	//
	// Returns:
	//     An instance of the C# System.CodeDom.Compiler.ICodeCompiler implementation.
	[Obsolete("ICodeCompiler has been deprecated. Use the methods directly on the CodeDomProvider class instead.")]
	public override ICodeCompiler CreateCompiler()
	{
		return (ICodeCompiler)_generator;
	}

	//
	// Summary:
	//     Gets a System.ComponentModel.TypeConverter for the specified type of object.
	//
	//
	// Parameters:
	//   type:
	//     The type of object to retrieve a type converter for.
	//
	// Returns:
	//     A System.ComponentModel.TypeConverter for the specified type.
	public override TypeConverter GetConverter(Type type)
	{
		if (!(type == typeof(MemberAttributes)))
		{
			if (!(type == typeof(TypeAttributes)))
			{
				return base.GetConverter(type);
			}

			return (TypeConverter)typeAttributeConverterType.GetProperty("Default",
				BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Static)
				.GetValue(null);
		}

		return (TypeConverter)memberAttributeConverterType.GetProperty("Default",
			BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Static)
			.GetValue(null);
	}

	//
	// Summary:
	//     Generates code for the specified class member using the specified text writer
	//     and code generator options.
	//
	// Parameters:
	//   member:
	//     A System.CodeDom.CodeTypeMember to generate code for.
	//
	//   writer:
	//     The System.IO.TextWriter to write to.
	//
	//   options:
	//     The System.CodeDom.Compiler.CodeGeneratorOptions to use when generating the code.
	public override void GenerateCodeFromMember(CodeTypeMember member, TextWriter writer, CodeGeneratorOptions options)
	{
		var method = _generator.GetType().GetMethod("GenerateCodeFromMember");
		method.Invoke(_generator, new object[] { member, writer, options });
		//_generator.GenerateCodeFromMember(member, writer, options);
	}
}