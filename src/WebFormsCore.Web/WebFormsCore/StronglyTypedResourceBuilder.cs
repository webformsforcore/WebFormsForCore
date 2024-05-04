using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Web;

namespace System.Resources.Tools;

//
// Summary:
//     Provides support for strongly typed resources. This class cannot be inherited.
public static class StronglyTypedResourceBuilder
{
	internal sealed class ResourceData
	{
		private Type _type;

		private string _valueAsString;

		internal Type Type => _type;

		internal string ValueAsString => _valueAsString;

		internal ResourceData(Type type, string valueAsString)
		{
			_type = type;
			_valueAsString = valueAsString;
		}
	}

	private const string ResMgrFieldName = "resourceMan";

	private const string ResMgrPropertyName = "ResourceManager";

	private const string CultureInfoFieldName = "resourceCulture";

	private const string CultureInfoPropertyName = "Culture";

	private static readonly char[] CharsToReplace = new char[30]
	{
		' ', '\u00a0', '.', ',', ';', '|', '~', '@', '#', '%',
		'^', '&', '*', '+', '-', '/', '\\', '<', '>', '?',
		'[', ']', '(', ')', '{', '}', '"', '\'', ':', '!'
	};

	private const char ReplacementChar = '_';

	private const string DocCommentSummaryStart = "<summary>";

	private const string DocCommentSummaryEnd = "</summary>";

	private const int DocCommentLengthThreshold = 512;

	//
	// Summary:
	//     Generates a class file that contains strongly typed properties that match the
	//     resources referenced in the specified collection.
	//
	// Parameters:
	//   resourceList:
	//     An System.Collections.IDictionary collection where each dictionary entry key/value
	//     pair is the name of a resource and the value of the resource.
	//
	//   baseName:
	//     The name of the class to be generated.
	//
	//   generatedCodeNamespace:
	//     The namespace of the class to be generated.
	//
	//   codeProvider:
	//     A System.CodeDom.Compiler.CodeDomProvider class that provides the language in
	//     which the class will be generated.
	//
	//   internalClass:
	//     true to generate an internal class; false to generate a public class.
	//
	//   unmatchable:
	//     An array that contains each resource name for which a property cannot be generated.
	//     Typically, a property cannot be generated because the resource name is not a
	//     valid identifier.
	//
	// Returns:
	//     A System.CodeDom.CodeCompileUnit container.
	//
	// Exceptions:
	//   T:System.ArgumentNullException:
	//     resourceList, basename, or codeProvider is null.
	//
	//   T:System.ArgumentException:
	//     A resource node name does not match its key in resourceList.
	public static CodeCompileUnit Create(IDictionary resourceList, string baseName, string generatedCodeNamespace, CodeDomProvider codeProvider, bool internalClass, out string[] unmatchable)
	{
		return Create(resourceList, baseName, generatedCodeNamespace, null, codeProvider, internalClass, out unmatchable);
	}

	//
	// Summary:
	//     Generates a class file that contains strongly typed properties that match the
	//     resources referenced in the specified collection.
	//
	// Parameters:
	//   resourceList:
	//     An System.Collections.IDictionary collection where each dictionary entry key/value
	//     pair is the name of a resource and the value of the resource.
	//
	//   baseName:
	//     The name of the class to be generated.
	//
	//   generatedCodeNamespace:
	//     The namespace of the class to be generated.
	//
	//   resourcesNamespace:
	//     The namespace of the resource to be generated.
	//
	//   codeProvider:
	//     A System.CodeDom.Compiler.CodeDomProvider object that provides the language in
	//     which the class will be generated.
	//
	//   internalClass:
	//     true to generate an internal class; false to generate a public class.
	//
	//   unmatchable:
	//     A System.String array that contains each resource name for which a property cannot
	//     be generated. Typically, a property cannot be generated because the resource
	//     name is not a valid identifier.
	//
	// Returns:
	//     A System.CodeDom.CodeCompileUnit container.
	//
	// Exceptions:
	//   T:System.ArgumentNullException:
	//     resourceList, basename, or codeProvider is null.
	//
	//   T:System.ArgumentException:
	//     A resource node name does not match its key in resourceList.
	public static CodeCompileUnit Create(IDictionary resourceList, string baseName, string generatedCodeNamespace, string resourcesNamespace, CodeDomProvider codeProvider, bool internalClass, out string[] unmatchable)
	{
		if (resourceList == null)
		{
			throw new ArgumentNullException("resourceList");
		}

		Dictionary<string, ResourceData> dictionary = new Dictionary<string, ResourceData>(StringComparer.InvariantCultureIgnoreCase);
		foreach (DictionaryEntry resource in resourceList)
		{
			ResourceData value;
			if (resource.Value is ResXDataNode resXDataNode)
			{
				string text = (string)resource.Key;
				if (text != resXDataNode.Name)
				{
					throw new ArgumentException(SR.GetString("MismatchedResourceName", text, resXDataNode.Name));
				}

				string valueTypeName = resXDataNode.GetValueTypeName((AssemblyName[])null);
				Type type = Type.GetType(valueTypeName);
				string valueAsString = resXDataNode.GetValue((AssemblyName[])null).ToString();
				value = new ResourceData(type, valueAsString);
			}
			else
			{
				Type type2 = ((resource.Value == null) ? typeof(object) : resource.Value.GetType());
				value = new ResourceData(type2, (resource.Value == null) ? null : resource.Value.ToString());
			}

			dictionary.Add((string)resource.Key, value);
		}

		return InternalCreate(dictionary, baseName, generatedCodeNamespace, resourcesNamespace, codeProvider, internalClass, out unmatchable);
	}

	private static CodeCompileUnit InternalCreate(Dictionary<string, ResourceData> resourceList, string baseName, string generatedCodeNamespace, string resourcesNamespace, CodeDomProvider codeProvider, bool internalClass, out string[] unmatchable)
	{
		if (baseName == null)
		{
			throw new ArgumentNullException("baseName");
		}

		if (codeProvider == null)
		{
			throw new ArgumentNullException("codeProvider");
		}

		ArrayList arrayList = new ArrayList(0);
		Hashtable reverseFixupTable;
		SortedList sortedList = VerifyResourceNames(resourceList, codeProvider, arrayList, out reverseFixupTable);
		string text = baseName;
		if (!codeProvider.IsValidIdentifier(text))
		{
			string text2 = VerifyResourceName(text, codeProvider);
			if (text2 != null)
			{
				text = text2;
			}
		}

		if (!codeProvider.IsValidIdentifier(text))
		{
			throw new ArgumentException(SR.GetString("InvalidIdentifier", text));
		}

		if (!string.IsNullOrEmpty(generatedCodeNamespace) && !codeProvider.IsValidIdentifier(generatedCodeNamespace))
		{
			string text3 = VerifyResourceName(generatedCodeNamespace, codeProvider, isNameSpace: true);
			if (text3 != null)
			{
				generatedCodeNamespace = text3;
			}
		}

		CodeCompileUnit codeCompileUnit = new CodeCompileUnit();
		codeCompileUnit.ReferencedAssemblies.Add("System.dll");
		codeCompileUnit.UserData.Add("AllowLateBound", false);
		codeCompileUnit.UserData.Add("RequireVariableDeclaration", true);
		CodeNamespace codeNamespace = new CodeNamespace(generatedCodeNamespace);
		codeNamespace.Imports.Add(new CodeNamespaceImport("System"));
		codeCompileUnit.Namespaces.Add(codeNamespace);
		CodeTypeDeclaration codeTypeDeclaration = new CodeTypeDeclaration(text);
		codeNamespace.Types.Add(codeTypeDeclaration);
		AddGeneratedCodeAttributeforMember(codeTypeDeclaration);
		TypeAttributes typeAttributes = ((!internalClass) ? TypeAttributes.Public : TypeAttributes.NotPublic);
		codeTypeDeclaration.TypeAttributes = typeAttributes;
		codeTypeDeclaration.Comments.Add(new CodeCommentStatement("<summary>", docComment: true));
		codeTypeDeclaration.Comments.Add(new CodeCommentStatement(SR.GetString("ClassDocComment"), docComment: true));
		codeTypeDeclaration.Comments.Add(new CodeCommentStatement("</summary>", docComment: true));
		CodeTypeReference codeTypeReference = new CodeTypeReference(typeof(DebuggerNonUserCodeAttribute));
		codeTypeReference.Options = CodeTypeReferenceOptions.GlobalReference;
		codeTypeDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration(codeTypeReference));
		CodeTypeReference codeTypeReference2 = new CodeTypeReference(typeof(CompilerGeneratedAttribute));
		codeTypeReference2.Options = CodeTypeReferenceOptions.GlobalReference;
		codeTypeDeclaration.CustomAttributes.Add(new CodeAttributeDeclaration(codeTypeReference2));
		bool useStatic = internalClass || codeProvider.Supports(GeneratorSupport.PublicStaticMembers);
		bool supportsTryCatch = codeProvider.Supports(GeneratorSupport.TryCatchStatements);
		bool flag = codeProvider is ITargetAwareCodeDomProvider targetAwareCodeDomProvider && !targetAwareCodeDomProvider.SupportsProperty(typeof(Type), "Assembly", isWritable: false);
		if (flag)
		{
			codeNamespace.Imports.Add(new CodeNamespaceImport("System.Reflection"));
		}

		EmitBasicClassMembers(codeTypeDeclaration, generatedCodeNamespace, baseName, resourcesNamespace, internalClass, useStatic, supportsTryCatch, flag);
		foreach (DictionaryEntry item in sortedList)
		{
			string text4 = (string)item.Key;
			string text5 = (string)reverseFixupTable[text4];
			if (text5 == null)
			{
				text5 = text4;
			}

			if (!DefineResourceFetchingProperty(text4, text5, (ResourceData)item.Value, codeTypeDeclaration, internalClass, useStatic))
			{
				arrayList.Add(item.Key);
			}
		}

		unmatchable = (string[])arrayList.ToArray(typeof(string));
		CodeGenerator.ValidateIdentifiers(codeCompileUnit);
		return codeCompileUnit;
	}

	//
	// Summary:
	//     Generates a class file that contains strongly typed properties that match the
	//     resources in the specified .resx file.
	//
	// Parameters:
	//   resxFile:
	//     The name of a .resx file used as input.
	//
	//   baseName:
	//     The name of the class to be generated.
	//
	//   generatedCodeNamespace:
	//     The namespace of the class to be generated.
	//
	//   codeProvider:
	//     A System.CodeDom.Compiler.CodeDomProvider class that provides the language in
	//     which the class will be generated.
	//
	//   internalClass:
	//     true to generate an internal class; false to generate a public class.
	//
	//   unmatchable:
	//     A System.String array that contains each resource name for which a property cannot
	//     be generated. Typically, a property cannot be generated because the resource
	//     name is not a valid identifier.
	//
	// Returns:
	//     A System.CodeDom.CodeCompileUnit container.
	//
	// Exceptions:
	//   T:System.ArgumentNullException:
	//     basename or codeProvider is null.
	public static CodeCompileUnit Create(string resxFile, string baseName, string generatedCodeNamespace, CodeDomProvider codeProvider, bool internalClass, out string[] unmatchable)
	{
		return Create(resxFile, baseName, generatedCodeNamespace, null, codeProvider, internalClass, out unmatchable);
	}

	//
	// Summary:
	//     Generates a class file that contains strongly typed properties that match the
	//     resources in the specified .resx file.
	//
	// Parameters:
	//   resxFile:
	//     The name of a .resx file used as input.
	//
	//   baseName:
	//     The name of the class to be generated.
	//
	//   generatedCodeNamespace:
	//     The namespace of the class to be generated.
	//
	//   resourcesNamespace:
	//     The namespace of the resource to be generated.
	//
	//   codeProvider:
	//     A System.CodeDom.Compiler.CodeDomProvider class that provides the language in
	//     which the class will be generated.
	//
	//   internalClass:
	//     true to generate an internal class; false to generate a public class.
	//
	//   unmatchable:
	//     A System.String array that contains each resource name for which a property cannot
	//     be generated. Typically, a property cannot be generated because the resource
	//     name is not a valid identifier.
	//
	// Returns:
	//     A System.CodeDom.CodeCompileUnit container.
	//
	// Exceptions:
	//   T:System.ArgumentNullException:
	//     basename or codeProvider is null.
	public static CodeCompileUnit Create(string resxFile, string baseName, string generatedCodeNamespace, string resourcesNamespace, CodeDomProvider codeProvider, bool internalClass, out string[] unmatchable)
	{
		if (resxFile == null)
		{
			throw new ArgumentNullException("resxFile");
		}

		Dictionary<string, ResourceData> dictionary = new Dictionary<string, ResourceData>(StringComparer.InvariantCultureIgnoreCase);
		using (ResXResourceReader resXResourceReader = new ResXResourceReader(resxFile))
		{
			resXResourceReader.UseResXDataNodes = true;
			foreach (DictionaryEntry item in resXResourceReader)
			{
				ResXDataNode resXDataNode = (ResXDataNode)item.Value;
				string valueTypeName = resXDataNode.GetValueTypeName((AssemblyName[])null);
				Type type = Type.GetType(valueTypeName);
				string valueAsString = resXDataNode.GetValue((AssemblyName[])null).ToString();
				ResourceData value = new ResourceData(type, valueAsString);
				dictionary.Add((string)item.Key, value);
			}
		}

		return InternalCreate(dictionary, baseName, generatedCodeNamespace, resourcesNamespace, codeProvider, internalClass, out unmatchable);
	}

	private static void AddGeneratedCodeAttributeforMember(CodeTypeMember typeMember)
	{
		CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration(new CodeTypeReference(typeof(GeneratedCodeAttribute)));
		codeAttributeDeclaration.AttributeType.Options = CodeTypeReferenceOptions.GlobalReference;
		CodeAttributeArgument value = new CodeAttributeArgument(new CodePrimitiveExpression(typeof(StronglyTypedResourceBuilder).FullName));
		CodeAttributeArgument value2 = new CodeAttributeArgument(new CodePrimitiveExpression("4.0.0.0"));
		codeAttributeDeclaration.Arguments.Add(value);
		codeAttributeDeclaration.Arguments.Add(value2);
		typeMember.CustomAttributes.Add(codeAttributeDeclaration);
	}

	private static void EmitBasicClassMembers(CodeTypeDeclaration srClass, string nameSpace, string baseName, string resourcesNamespace, bool internalClass, bool useStatic, bool supportsTryCatch, bool useTypeInfo)
	{
		string value = ((resourcesNamespace != null) ? ((resourcesNamespace.Length <= 0) ? baseName : (resourcesNamespace + "." + baseName)) : ((nameSpace == null || nameSpace.Length <= 0) ? baseName : (nameSpace + "." + baseName)));
		CodeCommentStatement value2 = new CodeCommentStatement(SR.GetString("ClassComments1"));
		srClass.Comments.Add(value2);
		value2 = new CodeCommentStatement(SR.GetString("ClassComments2"));
		srClass.Comments.Add(value2);
		value2 = new CodeCommentStatement(SR.GetString("ClassComments3"));
		srClass.Comments.Add(value2);
		value2 = new CodeCommentStatement(SR.GetString("ClassComments4"));
		srClass.Comments.Add(value2);
		CodeAttributeDeclaration codeAttributeDeclaration = new CodeAttributeDeclaration(new CodeTypeReference(typeof(SuppressMessageAttribute)));
		codeAttributeDeclaration.AttributeType.Options = CodeTypeReferenceOptions.GlobalReference;
		codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression("Microsoft.Performance")));
		codeAttributeDeclaration.Arguments.Add(new CodeAttributeArgument(new CodePrimitiveExpression("CA1811:AvoidUncalledPrivateCode")));
		CodeConstructor codeConstructor = new CodeConstructor();
		codeConstructor.CustomAttributes.Add(codeAttributeDeclaration);
		if (useStatic || internalClass)
		{
			codeConstructor.Attributes = MemberAttributes.FamilyAndAssembly;
		}
		else
		{
			codeConstructor.Attributes = MemberAttributes.Public;
		}

		srClass.Members.Add(codeConstructor);
		CodeTypeReference codeTypeReference = new CodeTypeReference(typeof(ResourceManager), CodeTypeReferenceOptions.GlobalReference);
		CodeMemberField codeMemberField = new CodeMemberField(codeTypeReference, "resourceMan");
		codeMemberField.Attributes = MemberAttributes.Private;
		if (useStatic)
		{
			codeMemberField.Attributes |= MemberAttributes.Static;
		}

		srClass.Members.Add(codeMemberField);
		CodeTypeReference type = new CodeTypeReference(typeof(CultureInfo), CodeTypeReferenceOptions.GlobalReference);
		codeMemberField = new CodeMemberField(type, "resourceCulture");
		codeMemberField.Attributes = MemberAttributes.Private;
		if (useStatic)
		{
			codeMemberField.Attributes |= MemberAttributes.Static;
		}

		srClass.Members.Add(codeMemberField);
		CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
		srClass.Members.Add(codeMemberProperty);
		codeMemberProperty.Name = "ResourceManager";
		codeMemberProperty.HasGet = true;
		codeMemberProperty.HasSet = false;
		codeMemberProperty.Type = codeTypeReference;
		if (internalClass)
		{
			codeMemberProperty.Attributes = MemberAttributes.Assembly;
		}
		else
		{
			codeMemberProperty.Attributes = MemberAttributes.Public;
		}

		if (useStatic)
		{
			codeMemberProperty.Attributes |= MemberAttributes.Static;
		}

		CodeTypeReference codeTypeReference2 = new CodeTypeReference(typeof(EditorBrowsableState));
		codeTypeReference2.Options = CodeTypeReferenceOptions.GlobalReference;
		CodeAttributeArgument codeAttributeArgument = new CodeAttributeArgument(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression(codeTypeReference2), "Advanced"));
		CodeAttributeDeclaration codeAttributeDeclaration2 = new CodeAttributeDeclaration("System.ComponentModel.EditorBrowsableAttribute", codeAttributeArgument);
		codeAttributeDeclaration2.AttributeType.Options = CodeTypeReferenceOptions.GlobalReference;
		codeMemberProperty.CustomAttributes.Add(codeAttributeDeclaration2);
		CodeMemberProperty codeMemberProperty2 = new CodeMemberProperty();
		srClass.Members.Add(codeMemberProperty2);
		codeMemberProperty2.Name = "Culture";
		codeMemberProperty2.HasGet = true;
		codeMemberProperty2.HasSet = true;
		codeMemberProperty2.Type = type;
		if (internalClass)
		{
			codeMemberProperty2.Attributes = MemberAttributes.Assembly;
		}
		else
		{
			codeMemberProperty2.Attributes = MemberAttributes.Public;
		}

		if (useStatic)
		{
			codeMemberProperty2.Attributes |= MemberAttributes.Static;
		}

		codeMemberProperty2.CustomAttributes.Add(codeAttributeDeclaration2);
		CodeFieldReferenceExpression codeFieldReferenceExpression = new CodeFieldReferenceExpression(null, "resourceMan");
		CodeMethodReferenceExpression method = new CodeMethodReferenceExpression(new CodeTypeReferenceExpression(typeof(object)), "ReferenceEquals");
		CodeMethodInvokeExpression condition = new CodeMethodInvokeExpression(method, codeFieldReferenceExpression, new CodePrimitiveExpression(null));
		CodePropertyReferenceExpression codePropertyReferenceExpression;
		if (useTypeInfo)
		{
			CodeMethodInvokeExpression targetObject = new CodeMethodInvokeExpression(new CodeTypeOfExpression(new CodeTypeReference(srClass.Name)), "GetTypeInfo");
			codePropertyReferenceExpression = new CodePropertyReferenceExpression(targetObject, "Assembly");
		}
		else
		{
			codePropertyReferenceExpression = new CodePropertyReferenceExpression(new CodeTypeOfExpression(new CodeTypeReference(srClass.Name)), "Assembly");
		}

		CodeObjectCreateExpression initExpression = new CodeObjectCreateExpression(codeTypeReference, new CodePrimitiveExpression(value), codePropertyReferenceExpression);
		CodeStatement[] trueStatements = new CodeStatement[2]
		{
			new CodeVariableDeclarationStatement(codeTypeReference, "temp", initExpression),
			new CodeAssignStatement(codeFieldReferenceExpression, new CodeVariableReferenceExpression("temp"))
		};
		codeMemberProperty.GetStatements.Add(new CodeConditionStatement(condition, trueStatements));
		codeMemberProperty.GetStatements.Add(new CodeMethodReturnStatement(codeFieldReferenceExpression));
		codeMemberProperty.Comments.Add(new CodeCommentStatement("<summary>", docComment: true));
		codeMemberProperty.Comments.Add(new CodeCommentStatement(SR.GetString("ResMgrPropertyComment"), docComment: true));
		codeMemberProperty.Comments.Add(new CodeCommentStatement("</summary>", docComment: true));
		CodeFieldReferenceExpression codeFieldReferenceExpression2 = new CodeFieldReferenceExpression(null, "resourceCulture");
		codeMemberProperty2.GetStatements.Add(new CodeMethodReturnStatement(codeFieldReferenceExpression2));
		CodePropertySetValueReferenceExpression right = new CodePropertySetValueReferenceExpression();
		codeMemberProperty2.SetStatements.Add(new CodeAssignStatement(codeFieldReferenceExpression2, right));
		codeMemberProperty2.Comments.Add(new CodeCommentStatement("<summary>", docComment: true));
		codeMemberProperty2.Comments.Add(new CodeCommentStatement(SR.GetString("CulturePropertyComment1"), docComment: true));
		codeMemberProperty2.Comments.Add(new CodeCommentStatement(SR.GetString("CulturePropertyComment2"), docComment: true));
		codeMemberProperty2.Comments.Add(new CodeCommentStatement("</summary>", docComment: true));
	}

	private static string TruncateAndFormatCommentStringForOutput(string commentString)
	{
		if (commentString != null)
		{
			if (commentString.Length > 512)
			{
				commentString = SR.GetString("StringPropertyTruncatedComment", commentString.Substring(0, 512));
			}

			commentString = SecurityElement.Escape(commentString);
		}

		return commentString;
	}

	private static bool DefineResourceFetchingProperty(string propertyName, string resourceName, ResourceData data, CodeTypeDeclaration srClass, bool internalClass, bool useStatic)
	{
		CodeMemberProperty codeMemberProperty = new CodeMemberProperty();
		codeMemberProperty.Name = propertyName;
		codeMemberProperty.HasGet = true;
		codeMemberProperty.HasSet = false;
		Type type = data.Type;
		if (type == null)
		{
			return false;
		}

		if (type == typeof(MemoryStream))
		{
			type = typeof(UnmanagedMemoryStream);
		}

		while (!type.IsPublic)
		{
			type = type.BaseType;
		}

		CodeTypeReference targetType = (codeMemberProperty.Type = new CodeTypeReference(type));
		if (internalClass)
		{
			codeMemberProperty.Attributes = MemberAttributes.Assembly;
		}
		else
		{
			codeMemberProperty.Attributes = MemberAttributes.Public;
		}

		if (useStatic)
		{
			codeMemberProperty.Attributes |= MemberAttributes.Static;
		}

		CodePropertyReferenceExpression targetObject = new CodePropertyReferenceExpression(null, "ResourceManager");
		CodeFieldReferenceExpression codeFieldReferenceExpression = new CodeFieldReferenceExpression(useStatic ? null : new CodeThisReferenceExpression(), "resourceCulture");
		bool flag = type == typeof(string);
		bool flag2 = type == typeof(UnmanagedMemoryStream) || type == typeof(MemoryStream);
		string empty = string.Empty;
		string empty2 = string.Empty;
		string text = TruncateAndFormatCommentStringForOutput(data.ValueAsString);
		string text2 = string.Empty;
		if (!flag)
		{
			text2 = TruncateAndFormatCommentStringForOutput(type.ToString());
		}

		empty = (flag ? "GetString" : ((!flag2) ? "GetObject" : "GetStream"));
		empty2 = (flag ? SR.GetString("StringPropertyComment", text) : ((text != null && !string.Equals(text2, text)) ? SR.GetString("NonStringPropertyDetailedComment", text2, text) : SR.GetString("NonStringPropertyComment", text2)));
		codeMemberProperty.Comments.Add(new CodeCommentStatement("<summary>", docComment: true));
		codeMemberProperty.Comments.Add(new CodeCommentStatement(empty2, docComment: true));
		codeMemberProperty.Comments.Add(new CodeCommentStatement("</summary>", docComment: true));
		CodeExpression codeExpression = new CodeMethodInvokeExpression(targetObject, empty, new CodePrimitiveExpression(resourceName), codeFieldReferenceExpression);
		CodeMethodReturnStatement value;
		if (flag || flag2)
		{
			value = new CodeMethodReturnStatement(codeExpression);
		}
		else
		{
			CodeVariableDeclarationStatement value2 = new CodeVariableDeclarationStatement(typeof(object), "obj", codeExpression);
			codeMemberProperty.GetStatements.Add(value2);
			value = new CodeMethodReturnStatement(new CodeCastExpression(targetType, new CodeVariableReferenceExpression("obj")));
		}

		codeMemberProperty.GetStatements.Add(value);
		srClass.Members.Add(codeMemberProperty);
		return true;
	}

	//
	// Summary:
	//     Generates a valid resource string based on the specified input string and code
	//     provider.
	//
	// Parameters:
	//   key:
	//     The string to verify and, if necessary, convert to a valid resource name.
	//
	//   provider:
	//     A System.CodeDom.Compiler.CodeDomProvider object that specifies the target language
	//     to use.
	//
	// Returns:
	//     A valid resource name derived from the key parameter. Any invalid tokens are
	//     replaced with the underscore (_) character, or null if the derived string still
	//     contains invalid characters according to the language specified by the provider
	//     parameter.
	//
	// Exceptions:
	//   T:System.ArgumentNullException:
	//     key or provider is null.
	public static string VerifyResourceName(string key, CodeDomProvider provider)
	{
		return VerifyResourceName(key, provider, isNameSpace: false);
	}

	private static string VerifyResourceName(string key, CodeDomProvider provider, bool isNameSpace)
	{
		if (key == null)
		{
			throw new ArgumentNullException("key");
		}

		if (provider == null)
		{
			throw new ArgumentNullException("provider");
		}

		char[] charsToReplace = CharsToReplace;
		foreach (char c in charsToReplace)
		{
			if (!isNameSpace || (c != '.' && c != ':'))
			{
				key = key.Replace(c, '_');
			}
		}

		if (provider.IsValidIdentifier(key))
		{
			return key;
		}

		key = provider.CreateValidIdentifier(key);
		if (provider.IsValidIdentifier(key))
		{
			return key;
		}

		key = "_" + key;
		if (provider.IsValidIdentifier(key))
		{
			return key;
		}

		return null;
	}

	private static SortedList VerifyResourceNames(Dictionary<string, ResourceData> resourceList, CodeDomProvider codeProvider, ArrayList errors, out Hashtable reverseFixupTable)
	{
		reverseFixupTable = new Hashtable(0, StringComparer.InvariantCultureIgnoreCase);
		SortedList sortedList = new SortedList(StringComparer.InvariantCultureIgnoreCase, resourceList.Count);
		foreach (KeyValuePair<string, ResourceData> resource in resourceList)
		{
			string text = resource.Key;
			if (string.Equals(text, "ResourceManager") || string.Equals(text, "Culture") || typeof(void) == resource.Value.Type)
			{
				errors.Add(text);
			}
			else
			{
				if ((text.Length > 0 && text[0] == '$') || (text.Length > 1 && text[0] == '>' && text[1] == '>'))
				{
					continue;
				}

				if (!codeProvider.IsValidIdentifier(text))
				{
					string text2 = VerifyResourceName(text, codeProvider, isNameSpace: false);
					if (text2 == null)
					{
						errors.Add(text);
						continue;
					}

					string text3 = (string)reverseFixupTable[text2];
					if (text3 != null)
					{
						if (!errors.Contains(text3))
						{
							errors.Add(text3);
						}

						if (sortedList.Contains(text2))
						{
							sortedList.Remove(text2);
						}

						errors.Add(text);
						continue;
					}

					reverseFixupTable[text2] = text;
					text = text2;
				}

				ResourceData value = resource.Value;
				if (!sortedList.Contains(text))
				{
					sortedList.Add(text, value);
					continue;
				}

				string text4 = (string)reverseFixupTable[text];
				if (text4 != null)
				{
					if (!errors.Contains(text4))
					{
						errors.Add(text4);
					}

					reverseFixupTable.Remove(text);
				}

				errors.Add(resource.Key);
				sortedList.Remove(text);
			}
		}

		return sortedList;
	}
}

