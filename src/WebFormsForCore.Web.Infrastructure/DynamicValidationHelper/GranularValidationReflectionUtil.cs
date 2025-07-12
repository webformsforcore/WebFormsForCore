using System;
using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Web;
using System.Web.Util;

namespace Microsoft.Web.Infrastructure.DynamicValidationHelper;

[SecurityCritical]
internal sealed class GranularValidationReflectionUtil
{
	private Func<NameObjectCollectionBase, ArrayList> _del_get_NameObjectCollectionBase_entriesArray;
	private Action<NameObjectCollectionBase, ArrayList> _del_set_NameObjectCollectionBase_entriesArray;
	private Func<NameObjectCollectionBase, Hashtable> _del_get_NameObjectCollectionBase_entriesTable;
	private Action<NameObjectCollectionBase, Hashtable> _del_set_NameObjectCollectionBase_entriesTable;
	private Func<object, string> _del_get_NameObjectEntry_Key;
	private Func<object, object> _del_get_NameObjectEntry_Value;
	private Action<object, object> _del_set_NameObjectEntry_Value;
	private Func<HttpRequest, ValidateStringCallback> _del_validateStringCallback;
	private Func<HttpRequest, int, bool> _del_BitVector32_get_Item;
	private Action<HttpRequest, int, bool> _del_BitVector32_set_Item;
	private Func<HttpRequest, NameValueCollection> _del_get_HttpRequest_form;
	private Action<object, object> _del_set_HttpRequest_form;
	private Func<HttpRequest, NameValueCollection> _del_get_HttpRequest_queryString;
	private Action<object, object> _del_set_HttpRequest_queryString;
	private Func<NameValueCollection> _del_HttpValueCollection_ctor;
	public static readonly GranularValidationReflectionUtil Instance = GranularValidationReflectionUtil.GetInstance();

	public ArrayList GetNameObjectCollectionEntriesArray(NameObjectCollectionBase target)
	{
		return this._del_get_NameObjectCollectionBase_entriesArray(target);
	}

	public void SetNameObjectCollectionEntriesArray(NameObjectCollectionBase target, ArrayList array)
	{
		this._del_set_NameObjectCollectionBase_entriesArray(target, array);
	}

	public Hashtable GetNameObjectCollectionEntriesTable(NameObjectCollectionBase target)
	{
		return this._del_get_NameObjectCollectionBase_entriesTable(target);
	}

	public void SetNameObjectCollectionEntriesTable(NameObjectCollectionBase target, Hashtable table)
	{
		this._del_set_NameObjectCollectionBase_entriesTable(target, table);
	}

	public string GetNameObjectEntryKey(object target) => this._del_get_NameObjectEntry_Key(target);

	public object GetNameObjectEntryValue(object target)
	{
		return this._del_get_NameObjectEntry_Value(target);
	}

	public void SetNameObjectEntryValue(object target, object newValue)
	{
		this._del_set_NameObjectEntry_Value(target, newValue);
	}

	public ValidateStringCallback MakeValidateStringCallback(HttpRequest request)
	{
		return this._del_validateStringCallback(request);
	}

	public bool GetRequestValidationFlag(HttpRequest target, ValidationSourceFlag flag)
	{
		return this._del_BitVector32_get_Item(target, (int)flag);
	}

	public void SetRequestValidationFlag(HttpRequest target, ValidationSourceFlag flag, bool value)
	{
		this._del_BitVector32_set_Item(target, (int)flag, value);
	}

	public NameValueCollection GetHttpRequestFormField(HttpRequest target)
	{
		return this._del_get_HttpRequest_form(target);
	}

	public void SetHttpRequestFormField(HttpRequest target, NameValueCollection value)
	{
		this._del_set_HttpRequest_form((object)target, (object)value);
	}

	public NameValueCollection GetHttpRequestQueryStringField(HttpRequest target)
	{
		return this._del_get_HttpRequest_queryString(target);
	}

	public void SetHttpRequestQueryStringField(HttpRequest target, NameValueCollection value)
	{
		this._del_set_HttpRequest_queryString((object)target, (object)value);
	}

	public NameValueCollection NewHttpValueCollection() => this._del_HttpValueCollection_ctor();

	private GranularValidationReflectionUtil()
	{
	}

	[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "We catch and ignore all errors because we do not want to bring down the application.")]
	private static GranularValidationReflectionUtil GetInstance()
	{
		try
		{
			if (DynamicValidationShimReflectionUtil.Instance != null)
				return (GranularValidationReflectionUtil)null;
			GranularValidationReflectionUtil instance = new GranularValidationReflectionUtil();
			FieldInfo field1 = CommonReflectionUtil.FindField(typeof(NameObjectCollectionBase), "_entriesArray", false, typeof(ArrayList));
			instance._del_get_NameObjectCollectionBase_entriesArray = GranularValidationReflectionUtil.MakeFieldGetterFunc<NameObjectCollectionBase, ArrayList>(field1);
			instance._del_set_NameObjectCollectionBase_entriesArray = GranularValidationReflectionUtil.MakeFieldSetterFunc<NameObjectCollectionBase, ArrayList>(field1);
			FieldInfo field2 = CommonReflectionUtil.FindField(typeof(NameObjectCollectionBase), "_entriesTable", false, typeof(Hashtable));
			instance._del_get_NameObjectCollectionBase_entriesTable = GranularValidationReflectionUtil.MakeFieldGetterFunc<NameObjectCollectionBase, Hashtable>(field2);
			instance._del_set_NameObjectCollectionBase_entriesTable = GranularValidationReflectionUtil.MakeFieldSetterFunc<NameObjectCollectionBase, Hashtable>(field2);
			Type type1 = CommonAssemblies.System.GetType("System.Collections.Specialized.NameObjectCollectionBase+NameObjectEntry");
			FieldInfo field3 = CommonReflectionUtil.FindField(type1, "Key", false, typeof(string));
			instance._del_get_NameObjectEntry_Key = GranularValidationReflectionUtil.MakeFieldGetterFunc<string>(type1, field3);
			FieldInfo field4 = CommonReflectionUtil.FindField(type1, "Value", false, typeof(object));
			instance._del_get_NameObjectEntry_Value = GranularValidationReflectionUtil.MakeFieldGetterFunc<object>(type1, field4);
			instance._del_set_NameObjectEntry_Value = GranularValidationReflectionUtil.MakeFieldSetterFunc(type1, field4);
			MethodInfo method = CommonReflectionUtil.FindMethod(typeof(HttpRequest), "ValidateString", false, new Type[3]
			{
		typeof (string),
		typeof (string),
		typeof (RequestValidationSource)
			}, typeof(void));
			instance._del_validateStringCallback = CommonReflectionUtil.MakeFastCreateDelegate<HttpRequest, ValidateStringCallback>(method);
			Type type2 = CommonAssemblies.SystemWeb.GetType("System.Web.HttpValueCollection");
			instance._del_HttpValueCollection_ctor = CommonReflectionUtil.MakeFastNewObject<Func<NameValueCollection>>(type2);
			FieldInfo field5 = CommonReflectionUtil.FindField(typeof(HttpRequest), "_form", false, type2);
			instance._del_get_HttpRequest_form = GranularValidationReflectionUtil.MakeFieldGetterFunc<HttpRequest, NameValueCollection>(field5);
			instance._del_set_HttpRequest_form = GranularValidationReflectionUtil.MakeFieldSetterFunc(typeof(HttpRequest), field5);
			FieldInfo field6 = CommonReflectionUtil.FindField(typeof(HttpRequest), "_queryString", false, type2);
			instance._del_get_HttpRequest_queryString = GranularValidationReflectionUtil.MakeFieldGetterFunc<HttpRequest, NameValueCollection>(field6);
			instance._del_set_HttpRequest_queryString = GranularValidationReflectionUtil.MakeFieldSetterFunc(typeof(HttpRequest), field6);
			Type type3 = CommonAssemblies.SystemWeb.GetType("System.Web.Util.SimpleBitVector32");
			GranularValidationReflectionUtil.MakeRequestValidationFlagsAccessors(CommonReflectionUtil.FindField(typeof(HttpRequest), "_flags", false, type3), CommonReflectionUtil.FindMethod(type3, "get_Item", false, new Type[1]
			{
		typeof (int)
			}, typeof(bool)), CommonReflectionUtil.FindMethod(type3, "set_Item", false, new Type[2]
			{
		typeof (int),
		typeof (bool)
			}, typeof(void)), out instance._del_BitVector32_get_Item, out instance._del_BitVector32_set_Item);
			return instance;
		}
		catch
		{
			return (GranularValidationReflectionUtil)null;
		}
	}

	[ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
	private static Func<TTarget, TFieldType> MakeFieldGetterFunc<TTarget, TFieldType>(
	  FieldInfo fieldInfo)
	{
		ParameterExpression parameterExpression = Expression.Parameter(typeof(TTarget));
		return Expression.Lambda<Func<TTarget, TFieldType>>(Expression.Field(parameterExpression, fieldInfo), new ParameterExpression[] { parameterExpression }).Compile();
	}

	[ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
	private static Func<object, TFieldType> MakeFieldGetterFunc<TFieldType>(Type targetType, FieldInfo fieldInfo)
	{
		ParameterExpression parameterExpression = Expression.Parameter(typeof(object));
		return Expression.Lambda<Func<object, TFieldType>>(Expression.Field(Expression.Convert(parameterExpression, targetType), fieldInfo), new ParameterExpression[] { parameterExpression }).Compile();
	}

	[ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
	private static Action<TTarget, TFieldType> MakeFieldSetterFunc<TTarget, TFieldType>(FieldInfo fieldInfo)
	{
		ParameterExpression parameterExpression = Expression.Parameter(typeof(TTarget));
		ParameterExpression parameterExpression1 = Expression.Parameter(typeof(TFieldType));
		return Expression.Lambda<Action<TTarget, TFieldType>>(Expression.Assign(Expression.Field(parameterExpression, fieldInfo), parameterExpression1), new ParameterExpression[] { parameterExpression, parameterExpression1 }).Compile();
	}

	[ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
	private static Action<object, object> MakeFieldSetterFunc(Type targetType, FieldInfo fieldInfo)
	{
		UnaryExpression unaryExpression = Expression.Convert((Expression)Expression.Parameter(typeof(object)), targetType);
		UnaryExpression right = Expression.Convert((Expression)Expression.Parameter(typeof(object)), fieldInfo.FieldType);
		FieldInfo field = fieldInfo;
		return ((Expression<Action<object, object>>)((parameterExpression1, parameterExpression2) => Expression.Assign((Expression)Expression.Field((Expression)unaryExpression, field), (Expression)right))).Compile();
	}

	[ReflectionPermission(SecurityAction.Assert, MemberAccess = true)]
	private static void MakeRequestValidationFlagsAccessors(FieldInfo flagsFieldInfo, MethodInfo itemGetter,
	  MethodInfo itemSetter, out Func<HttpRequest, int, bool> delGetter, out Action<HttpRequest, int, bool> delSetter)
	{
		ParameterExpression parameterExpression = Expression.Parameter(typeof(HttpRequest));
		ParameterExpression parameterExpression1 = Expression.Parameter(typeof(int));
		ParameterExpression parameterExpression2 = Expression.Parameter(typeof(bool));
		MemberExpression memberExpression = Expression.Field(parameterExpression, flagsFieldInfo);
		delGetter = Expression.Lambda<Func<HttpRequest, int, bool>>(Expression.Call(memberExpression, itemGetter, new Expression[] { parameterExpression1 }), new ParameterExpression[] { parameterExpression, parameterExpression1 }).Compile();
		delSetter = Expression.Lambda<Action<HttpRequest, int, bool>>(Expression.Call(memberExpression, itemSetter, parameterExpression1, parameterExpression2), new ParameterExpression[] { parameterExpression, parameterExpression1, parameterExpression2 }).Compile();
	}
}
