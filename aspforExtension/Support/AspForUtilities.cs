using Evergrowth.AspForMarkDigExtension.Decorators;
using Evergrowth.AspForMarkDigExtension.Enums;
using System.ComponentModel;
using System.Data;
using System.Text;

namespace Evergrowth.AspForMarkDigExtension.Support;
public static class AspForUtilities
{
    public class AspForObjectInfo_struct
    {
        public bool? ReadOnly { get; set; }
        public string? CheckedValue { get; set; }
        public NullHandling? ObjectNullHandling { get; set; }
        public string? DerivedHTMLType { get; set; }
        public Type? PropertyType { get; set; }
        public bool? Ignore { get; set; }
        public bool? Required { get; set; }
        public DateTimeOverride? DateTimeOverride { get; set; }

        /// <summary>
        /// Sets defaults on struct.
        /// </summary>
        public AspForObjectInfo_struct()
        {
            this.ReadOnly = false;
            this.CheckedValue = "True";
            this.ObjectNullHandling = NullHandling.ShowBlank;
            this.DerivedHTMLType = "text";
            this.PropertyType = typeof(AspForObjectInfo_struct);
            this.Ignore = false;
            this.Required = false;
        }
    }

    /// <summary>
    /// Resursive function that will walk through a dot-separated reference field name and try to extract the value from the ASPForm object that was passed during instatiation.
    /// </summary>
    /// <param name="src">Initial source should be the root object ASPForm that was passed in through the <see cref="AspForGeneratorOptions">AspForGeneratorOptions</see> object.</param>
    /// <param name="propName">Initial source should be the entire field reference from the markdown document.</param>
    /// <returns>The value of <see cref="object">the field</see> that was found; null if it did not find a value.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static object GetPropertyValueAndAttributes(AspForGeneratorOptions _options, object src, string propName, out AspForObjectInfo_struct? aspForObjectInfo)
    {
        //aspForObjectInfo = null;
        if (src == null) throw new ArgumentException("Value cannot be null.", "src");
        if (propName == null) throw new ArgumentException("Value cannot be null.", "propName");

        if (propName.Contains("."))//complex type nested
        {
            var temp = propName.Split(new char[] { '.' }, 2);
            return GetPropertyValueAndAttributes(_options, GetPropertyValueAndAttributes(_options, src, temp[0], out aspForObjectInfo), temp[1], out aspForObjectInfo);
        }
        else
        {
            try
            {
                var objType = src.GetType();
                var allProps = objType.GetProperties().Where(s => s.Name == propName);
                if (allProps.Any())
                {
                    var prop = allProps.FirstOrDefault(s => s?.DeclaringType == objType) ?? allProps.First();
                    aspForObjectInfo = prop != null ? ExtractObjectInfo(_options, prop) : null;
                    return prop != null ? prop.GetValue(src, null) : null;
                }
                else
                {
                    aspForObjectInfo = null;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
            }
        }
        aspForObjectInfo = null;
        return new object();
    }

    public static string BooleanCheckedValue(object? referenceObject, string? referenceName)
    {
        Type propertyType = Nullable.GetUnderlyingType(referenceObject.GetType()) ?? referenceObject.GetType();
        string propertyTypeName = propertyType.Name;

        if (propertyTypeName != "Boolean")
        {
            throw new ArgumentException("CheckedValue attribute is only valid on boolean type properties.", referenceName);
        }

        //AspForCheckedValueAttribute checkAttribute
        return String.Empty;
    }

    public static AspForObjectInfo_struct ExtractObjectInfo(AspForGeneratorOptions _options, System.Reflection.PropertyInfo? referenceObject)
    {
        AspForObjectInfo_struct tempReturn = new AspForObjectInfo_struct();

        // extract readonly tag
        AspForReadOnlyAttribute checkReadOnly = (AspForReadOnlyAttribute)Attribute.GetCustomAttribute(referenceObject, typeof(AspForReadOnlyAttribute));
        if (checkReadOnly != null)
        {
            tempReturn.ReadOnly = checkReadOnly.ReadOnly;
        }
        else
        {
            tempReturn.ReadOnly = false;
        }

        // extract object type
        tempReturn.PropertyType = referenceObject != null ? Nullable.GetUnderlyingType(referenceObject.PropertyType) ?? referenceObject.PropertyType : null;

        // extract checkbox value tag (only on boolean types)
        if (tempReturn.PropertyType.Name == "Boolean")
        {
            AspForCheckedValueAttribute checkCheckValue = (AspForCheckedValueAttribute)Attribute.GetCustomAttribute(referenceObject, typeof(AspForCheckedValueAttribute));
            if (checkCheckValue != null)
            {
                tempReturn.CheckedValue = checkCheckValue.Value;
            }
            else
            {
                tempReturn.CheckedValue = _options.DefaultCheckedCheckboxValue;
            }
        }

        // extract DateTimeOverride attribute
        AspForDateTimeTypeAttribute checkDateTimeType = (AspForDateTimeTypeAttribute)Attribute.GetCustomAttribute(referenceObject, typeof(AspForDateTimeTypeAttribute));
        if (checkDateTimeType != null)
        {
            tempReturn.DateTimeOverride = checkDateTimeType.Value;
        }
        else
        {
            tempReturn.DateTimeOverride = _options.DateTimeOverride;
        }

        // extract derived HTML type
        switch (tempReturn.PropertyType.GenericTypeArguments.Any() ? tempReturn.PropertyType.GenericTypeArguments.First().Name : tempReturn.PropertyType.Name)
        {
            case "String": tempReturn.DerivedHTMLType = "text"; break;
            case "Int16": tempReturn.DerivedHTMLType = "number"; break;
            case "Int32": tempReturn.DerivedHTMLType = "number"; break;
            case "Int64": tempReturn.DerivedHTMLType = "number"; break;
            case "Boolean": tempReturn.DerivedHTMLType = "checkbox"; break;
            case "Decimal": tempReturn.DerivedHTMLType = "number"; break;
            case "DateTime": tempReturn.DerivedHTMLType = ParseDateTime_UsingOverride(tempReturn.DateTimeOverride ?? _options.DateTimeOverride); break;
            default: tempReturn.DerivedHTMLType = "number"; break;
        }

        // extract ignore flag
        AspForIgnoreAttribute checkIgnore = (AspForIgnoreAttribute)Attribute.GetCustomAttribute(referenceObject, typeof(AspForIgnoreAttribute));
        if (checkIgnore != null)
        {
            tempReturn.Ignore = checkIgnore.IgnoreTotally;
        }
        else 
        {
            tempReturn.Ignore = false;
        }

        // extract required attribute
        AspForRequiredInputAttribute checkRequiredInput = (AspForRequiredInputAttribute)Attribute.GetCustomAttribute(referenceObject, typeof(AspForRequiredInputAttribute));
        if (checkRequiredInput != null)
        {
            tempReturn.Required = checkRequiredInput.Required;
        }
        else
        {
            tempReturn.Required = false;
        }

        // extract null handling for property type
        tempReturn.ObjectNullHandling = null;                   // TODO: placeholder

        return tempReturn;
    }

    private static string ParseDateTime_UsingOverride(DateTimeOverride _option)
    {
        switch (_option)
        {
            case DateTimeOverride.AsDate: return "date";
            case DateTimeOverride.AsTime: return "time";
            case DateTimeOverride.AsDateTimeLocal: return "datetime-local";
            default: return "date";
        }
    }
}
