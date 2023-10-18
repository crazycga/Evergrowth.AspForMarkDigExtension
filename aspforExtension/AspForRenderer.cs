using Evergrowth.AspForMarkDigExtension.Decorators;
using Evergrowth.AspForMarkDigExtension.Enums;
using Evergrowth.AspForMarkDigExtension.Support;
using Markdig.Helpers;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using System.ComponentModel;
using System.Text;

namespace Evergrowth.AspForMarkDigExtension;
public class AspForRenderer : HtmlObjectRenderer<AspForGenerator>
{
    private AspForGeneratorOptions _aspForGeneratorOptions;
    private const char LEFTSIDE_INDICATOR = '[';
    private const char RIGHTSIDE_INDICATOR = ']';

    // {0} = HTML input type
    // {1} = Id as specified by object name
    // {2} = Exact object property name (i.e. formData.MyString)
    // {3} = Value of property name
    // {4} = Any additional requirement (i.e. "readonly")
    private const string VALUE_REPLACEMENTTOKEN = "%VALUE%";
    private const string VALUE_PROTOTYPE = "value=\"" + VALUE_REPLACEMENTTOKEN + "\"";

    private const string REQUIRED_REPLACEMENTTOKEN = "%REQUIRED%";
    private const string REQUIRED_PROTOTYPE = "required";

    private const string READONLY_REPLACEMENTTOKEN = "%READONLY%";
    private const string READONLY_PROTOTYPE = "readonly ";

    private const string TYPE_REPLACEMENTTOKEN = "%TYPE%";
    private const string ID_REPLACEMENTTOKEN = "%ID%";
    private const string NAME_REPLACEMENTTOKEN = "%NAME%";

    private const string CHECKED_REPLACEMENTTOKEN = "%CHECKBOX_VALUE%";
    private const string CHECKED_PROTOTYPE = "checked ";

    private const string SYNTACTICAL_PROTOTYPE_REPLACEMENT_TOKENS = "<input type=\"" + TYPE_REPLACEMENTTOKEN +
                                                                    "\" id=\"" + ID_REPLACEMENTTOKEN +
                                                                    "\" name=\"" + NAME_REPLACEMENTTOKEN + "\" " +
                                                                    VALUE_REPLACEMENTTOKEN + " "
                                                                    + CHECKED_REPLACEMENTTOKEN
                                                                    + READONLY_REPLACEMENTTOKEN
                                                                    + REQUIRED_REPLACEMENTTOKEN
                                                                    + "/>";

    public AspForRenderer(AspForGeneratorOptions aspForGeneratorOptions)
    {
        _aspForGeneratorOptions = aspForGeneratorOptions;
    }

    protected override void Write(HtmlRenderer renderer, AspForGenerator obj)
    {
        // TODO: the RENDERER needs to know that it was just passed the ENTIRE tag, including the model name.  Parse out the model name.

        // first step: determine if model is empty; if so, bust out.
        if (_aspForGeneratorOptions == null) { return; }

        StringSlice AspForReference;

        AspForReference = obj.InputString;

        string modelReference = ExtractReferenceName(obj.InputString.ToString());

        if (String.IsNullOrEmpty(modelReference)) { return; }

        if (renderer.EnableHtmlForInline)
        {
            string inputType = String.Empty;
            string valueOutput = String.Empty;
            AspForUtilities.AspForObjectInfo_struct? objInfo;

            string inputId = modelReference.Replace('.', '_');
            string inputName = modelReference;

            var valueResponse = AspForUtilities.GetPropertyValueAndAttributes(_aspForGeneratorOptions, _aspForGeneratorOptions.ASPModel, modelReference, out objInfo);

            // if the attribute is set to ignore, write it out and return.  Also, if the object isn't found, skip it and leave the original code.
            if ((objInfo != null) && (objInfo.Ignore == true))
            {
                renderer.Write(obj.InputString);
                return;
            }
            
            if (valueResponse == null)
            {
                switch (_aspForGeneratorOptions.NullHandling)
                {
                    case NullHandling.ShowBlank:
                        valueOutput = String.Empty;
                        inputType = objInfo != null ? objInfo.DerivedHTMLType : "text";
                        break;

                    case NullHandling.ShowWarning:
                        valueOutput = String.Format(this._aspForGeneratorOptions.WarningMessage, modelReference);
                        inputType = "text";
                        break;

                    case NullHandling.ShowError:
                        throw new ArgumentNullException(nameof(modelReference));
                        break;

                    case NullHandling.RemoveEntirely:
                        renderer.Write(String.Empty);
                        return;

                    default:
                        throw new ArgumentNullException(nameof(modelReference));
                        break;
                }
            }
            else
            {
                if (objInfo != null)
                {
                    inputType = objInfo.DerivedHTMLType;
                    valueOutput = GetValueOutput(valueResponse, inputType, objInfo);
                }
                else
                {
                    inputType = "text";
                    valueOutput = String.Empty;
                }
            }

            string proposedOutput = PrepareInputHTML(inputType, inputId, inputName, valueOutput, _aspForGeneratorOptions.DesignatePopulatedFieldsReadOnly, objInfo);
            renderer.Write(proposedOutput);
        }
        else
        {
            renderer.Write(obj.InputString);
        }
    }

    private string GetValueOutput(object? referenceObject, string referenceType, AspForUtilities.AspForObjectInfo_struct? objInfo)
    {
        if (referenceObject == null) { return String.Empty; }

        switch(referenceType)
        {
            case "date": return ConvertDateValue_Conditional((DateTime)referenceObject, objInfo);
            case "time": return ConvertDateValue_Conditional((DateTime)referenceObject, objInfo);
            case "datetime-local": return ConvertDateValue_Conditional((DateTime)referenceObject, objInfo);
            default: return String.IsNullOrEmpty(referenceObject.ToString()) ? String.Empty : referenceObject.ToString();
        }
    }

    private string ConvertDateValue_Conditional(DateTime incoming, AspForUtilities.AspForObjectInfo_struct? objInfo)
    {
        switch (objInfo.DateTimeOverride ?? _aspForGeneratorOptions.DateTimeOverride)
        {
            case DateTimeOverride.AsDate: return incoming.ToString("yyyy-MM-dd");
            case DateTimeOverride.AsTime: return incoming.ToString("HH:mm");
            case DateTimeOverride.AsDateTimeLocal: return incoming.ToString("yyyy-MM-ddTHH:mm");
            default: return incoming.ToString("MM-dd-yyyy");
        }
    }

    private string PrepareInputHTML(string inputType, string inputId, string inputName, string inputValue, bool designatePopulatedFieldsReadOnly, AspForUtilities.AspForObjectInfo_struct objInfo)
    {
        string tempReturn = SYNTACTICAL_PROTOTYPE_REPLACEMENT_TOKENS.Replace(TYPE_REPLACEMENTTOKEN, inputType)
                            .Replace(ID_REPLACEMENTTOKEN, inputId)
                            .Replace(NAME_REPLACEMENTTOKEN, inputName);

        if (this._aspForGeneratorOptions.PopulateTagsWithModelData)
        {
            tempReturn = tempReturn.Replace(VALUE_REPLACEMENTTOKEN, VALUE_PROTOTYPE);

            if ((objInfo != null) && (objInfo.PropertyType.Name == "Boolean")) 
            {
                if (!String.IsNullOrEmpty(objInfo.CheckedValue))
                {
                    tempReturn = tempReturn.Replace(VALUE_REPLACEMENTTOKEN, objInfo.CheckedValue);
                }
                else
                {
                    tempReturn = tempReturn.Replace(VALUE_REPLACEMENTTOKEN, _aspForGeneratorOptions.DefaultCheckedCheckboxValue);
                }
            }
            else
            {
                tempReturn = tempReturn.Replace(VALUE_REPLACEMENTTOKEN, inputValue);
            }

            // deal with checkbox values and "checked" attribute
            if ((inputType == "checkbox") && ((inputValue == _aspForGeneratorOptions.DefaultCheckedCheckboxValue) || (inputValue == objInfo.CheckedValue)))
            {
                tempReturn = tempReturn.Replace(CHECKED_REPLACEMENTTOKEN, CHECKED_PROTOTYPE);
            }
            else
            {
                tempReturn = tempReturn.Replace(CHECKED_REPLACEMENTTOKEN, String.Empty);
            }

            // add readonly attribute if required
            if ((this._aspForGeneratorOptions.DesignatePopulatedFieldsReadOnly) || (objInfo != null ? objInfo.ReadOnly == true : false))
            {
                tempReturn = tempReturn.Replace(READONLY_REPLACEMENTTOKEN, READONLY_PROTOTYPE);
            }
            else
            {
                tempReturn = tempReturn.Replace(READONLY_REPLACEMENTTOKEN, String.Empty);
            }

            // add required atrtibute if required
            if ((objInfo == null) || (objInfo.Required == false))
            {
                tempReturn = tempReturn.Replace(REQUIRED_REPLACEMENTTOKEN, String.Empty);
            }
            else
            {
                tempReturn = tempReturn.Replace(REQUIRED_REPLACEMENTTOKEN, REQUIRED_PROTOTYPE);
            }
        }
        else
        {
            if (objInfo.PropertyType.Name == "Boolean")
            {
                tempReturn = tempReturn.Replace(VALUE_REPLACEMENTTOKEN, VALUE_PROTOTYPE);

                if (!String.IsNullOrEmpty(objInfo.CheckedValue))
                {
                    tempReturn = tempReturn.Replace(VALUE_REPLACEMENTTOKEN, objInfo.CheckedValue);
                }
                else
                {
                    tempReturn = tempReturn.Replace(VALUE_REPLACEMENTTOKEN, _aspForGeneratorOptions.DefaultCheckedCheckboxValue);
                }
            }

            // add required atrtibute if required
            if ((objInfo == null) || (objInfo.Required == false))
            {
                tempReturn = tempReturn.Replace(REQUIRED_REPLACEMENTTOKEN, String.Empty);
            }
            else
            {
                tempReturn = tempReturn.Replace(REQUIRED_REPLACEMENTTOKEN, REQUIRED_PROTOTYPE);
            }

            tempReturn = tempReturn.Replace(VALUE_REPLACEMENTTOKEN, String.Empty).Replace(READONLY_REPLACEMENTTOKEN, String.Empty).Replace(CHECKED_REPLACEMENTTOKEN, String.Empty);
        }

        return tempReturn;
    }

    /// <summary>
    /// Extracts the reference name from the markdown tag.  This routine assumes that the entire field name is contained between brackets.
    /// </summary>
    /// <param name="incoming">The markdown tag in its entirety.</param>
    /// <returns>The field name contained betweeen the brackets.</returns>
    private string ExtractReferenceName(string incoming)
    {
        string tempReturn = String.Empty;
        int firstBracket = 0;
        int lastBracket = 0;

        firstBracket = incoming.IndexOf(LEFTSIDE_INDICATOR);
        lastBracket = incoming.IndexOf(RIGHTSIDE_INDICATOR);

        if ((firstBracket == -1) || (lastBracket == -1)) { return tempReturn; }
        tempReturn = incoming.Substring(firstBracket + 1, lastBracket - firstBracket - 1);
        return tempReturn;
    }


}
