using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Web;
using System.Web.Util;

namespace Microsoft.Web.Infrastructure.DynamicValidationHelper;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class ValidationUtility
{
	[SecuritySafeCritical]
	public static void EnableDynamicValidation(HttpContext context)
	{
		if (DynamicValidationShimReflectionUtil.Instance != null)
			DynamicValidationShimReflectionUtil.Instance.EnableDynamicValidation(context);
		else
			ValidationUtility.CollectionReplacer.MakeCollectionsLazy(context);
	}

	[SecuritySafeCritical]
	public static bool? IsValidationEnabled(HttpContext context)
	{
		return DynamicValidationShimReflectionUtil.Instance != null ? new bool?(DynamicValidationShimReflectionUtil.Instance.IsValidationEnabled(context)) : ValidationUtility.CollectionReplacer.IsValidationEnabled(context);
	}

	[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "2#", Justification = "Users aren't expected to call this code directly.")]
	[SecuritySafeCritical]
	[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#", Justification = "Users aren't expected to call this code directly.")]
	public static void GetUnvalidatedCollections(
	  HttpContext context,
	  out Func<NameValueCollection> formGetter,
	  out Func<NameValueCollection> queryStringGetter)
	{
		bool? nullable = ValidationUtility.IsValidationEnabled(context);
		bool flag = true;
		if (nullable.GetValueOrDefault() == flag & nullable.HasValue)
			ValidationUtility.EnableDynamicValidation(context);
		if (DynamicValidationShimReflectionUtil.Instance != null)
		{
			DynamicValidationShimReflectionUtil.Instance.GetUnvalidatedCollections(context, out formGetter, out queryStringGetter);
		}
		else
		{
			ValidationUtility.UnvalidatedCollections unvalidatedCollections = ValidationUtility.CollectionReplacer.GetUnvalidatedCollections(context);
			formGetter = new Func<NameValueCollection>(unvalidatedCollections.EnsureForm);
			queryStringGetter = new Func<NameValueCollection>(unvalidatedCollections.EnsureQueryString);
		}
	}

	[SecurityCritical]
	private static class CollectionReplacer
	{
		private static readonly GranularValidationReflectionUtil _reflectionUtil = GranularValidationReflectionUtil.Instance;
		private static readonly object _unvalidatedCollectionsKey = new object();

		public static void MakeCollectionsLazy(HttpContext context)
		{
			if (context == null || context.Items[ValidationUtility.CollectionReplacer._unvalidatedCollectionsKey] != null || ValidationUtility.CollectionReplacer._reflectionUtil == null)
				return;
			HttpRequest request = context.Request;
			request.ValidateInput();
			ValidationUtility.UnvalidatedCollections unvalidatedCollections = new ValidationUtility.UnvalidatedCollections(request);
			context.Items[ValidationUtility.CollectionReplacer._unvalidatedCollectionsKey] = (object)unvalidatedCollections;
			ValidationUtility.CollectionReplacer.ReplaceCollection(context, new FieldAccessor<NameValueCollection>((Func<NameValueCollection>)(() => ValidationUtility.CollectionReplacer._reflectionUtil.GetHttpRequestFormField(request)), (Action<NameValueCollection>)(val => ValidationUtility.CollectionReplacer._reflectionUtil.SetHttpRequestFormField(request, val))), (Func<NameValueCollection>)(() => request.Form), (Action<NameValueCollection>)(col => unvalidatedCollections.Form = col), RequestValidationSource.Form, ValidationSourceFlag.needToValidateForm);
			ValidationUtility.CollectionReplacer.ReplaceCollection(context, new FieldAccessor<NameValueCollection>((Func<NameValueCollection>)(() => ValidationUtility.CollectionReplacer._reflectionUtil.GetHttpRequestQueryStringField(request)), (Action<NameValueCollection>)(val => ValidationUtility.CollectionReplacer._reflectionUtil.SetHttpRequestQueryStringField(request, val))), (Func<NameValueCollection>)(() => request.QueryString), (Action<NameValueCollection>)(col => unvalidatedCollections.QueryString = col), RequestValidationSource.QueryString, ValidationSourceFlag.needToValidateQueryString);
		}

		private static void ReplaceCollection(
		  HttpContext context,
		  FieldAccessor<NameValueCollection> fieldAccessor,
		  Func<NameValueCollection> propertyAccessor,
		  Action<NameValueCollection> storeInUnvalidatedCollection,
		  RequestValidationSource validationSource,
		  ValidationSourceFlag validationSourceFlag)
		{
			HttpRequest request = context.Request;
			Func<bool> getValidationFlag = (Func<bool>)(() => ValidationUtility.CollectionReplacer._reflectionUtil.GetRequestValidationFlag(request, validationSourceFlag));
			Func<bool> hasValidationFired = (Func<bool>)(() => !getValidationFlag());
			Action<bool> setValidationFlag = (Action<bool>)(value => ValidationUtility.CollectionReplacer._reflectionUtil.SetRequestValidationFlag(request, validationSourceFlag, value));
			if (fieldAccessor.Value != null && hasValidationFired())
			{
				storeInUnvalidatedCollection(fieldAccessor.Value);
			}
			else
			{
				NameValueCollection originalBackingCollection = fieldAccessor.Value;
				ValidateStringCallback validateString = ValidationUtility.CollectionReplacer._reflectionUtil.MakeValidateStringCallback(context.Request);
				SimpleValidateStringCallback simpleValidateString = (SimpleValidateStringCallback)((value, key) =>
				{
					if (key != null && key.StartsWith("__", StringComparison.Ordinal) || string.IsNullOrEmpty(value))
						return;
					validateString(value, key, validationSource);
				});
				Func<NameValueCollection> getActualCollection = (Func<NameValueCollection>)(() =>
				{
					fieldAccessor.Value = originalBackingCollection;
					bool flag = getValidationFlag();
					setValidationFlag(false);
					NameValueCollection col = propertyAccessor();
					setValidationFlag(flag);
					storeInUnvalidatedCollection(new NameValueCollection(col));
					return col;
				});
				Action<NameValueCollection> makeCollectionLazy = (Action<NameValueCollection>)(col =>
				{
					simpleValidateString(col[(string)null], (string)null);
					LazilyValidatingArrayList array = new LazilyValidatingArrayList(ValidationUtility.CollectionReplacer._reflectionUtil.GetNameObjectCollectionEntriesArray((NameObjectCollectionBase)col), simpleValidateString);
					ValidationUtility.CollectionReplacer._reflectionUtil.SetNameObjectCollectionEntriesArray((NameObjectCollectionBase)col, (ArrayList)array);
					LazilyValidatingHashtable table = new LazilyValidatingHashtable(ValidationUtility.CollectionReplacer._reflectionUtil.GetNameObjectCollectionEntriesTable((NameObjectCollectionBase)col), simpleValidateString);
					ValidationUtility.CollectionReplacer._reflectionUtil.SetNameObjectCollectionEntriesTable((NameObjectCollectionBase)col, (Hashtable)table);
				});
				DeferredCountArrayList array1 = new DeferredCountArrayList(hasValidationFired, (Action)(() => setValidationFlag(false)), (Func<int>)(() =>
				{
					NameValueCollection nameValueCollection = getActualCollection();
					makeCollectionLazy(nameValueCollection);
					return nameValueCollection.Count;
				}));
				NameValueCollection target = ValidationUtility.CollectionReplacer._reflectionUtil.NewHttpValueCollection();
				ValidationUtility.CollectionReplacer._reflectionUtil.SetNameObjectCollectionEntriesArray((NameObjectCollectionBase)target, (ArrayList)array1);
				fieldAccessor.Value = target;
			}
		}

		public static bool? IsValidationEnabled(HttpContext context)
		{
			return context == null || context.Request == null || ValidationUtility.CollectionReplacer._reflectionUtil == null ? new bool?() : new bool?(ValidationUtility.CollectionReplacer._reflectionUtil.GetRequestValidationFlag(context.Request, ValidationSourceFlag.hasValidateInputBeenCalled));
		}

		public static ValidationUtility.UnvalidatedCollections GetUnvalidatedCollections(
		  HttpContext context)
		{
			return (ValidationUtility.UnvalidatedCollections)context.Items[ValidationUtility.CollectionReplacer._unvalidatedCollectionsKey] ?? new ValidationUtility.UnvalidatedCollections(context.Request);
		}
	}

	[SecurityCritical]
	private sealed class UnvalidatedCollections
	{
		private readonly HttpRequest _request;
		public NameValueCollection Form;
		public NameValueCollection QueryString;

		public UnvalidatedCollections(HttpRequest request) => this._request = request;

		public NameValueCollection EnsureForm()
		{
			NameValueCollection nameValueCollection = (NameValueCollection)null;
			if (this.Form == null)
				nameValueCollection = this._request.Form;
			return this.Form ?? nameValueCollection;
		}

		public NameValueCollection EnsureQueryString()
		{
			NameValueCollection nameValueCollection = (NameValueCollection)null;
			if (this.QueryString == null)
				nameValueCollection = this._request.QueryString;
			return this.QueryString ?? nameValueCollection;
		}
	}
}
