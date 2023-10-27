using System.Formats.Asn1;
using Evergrowth.AspForMarkDigExtension.Enums;

namespace Evergrowth.AspForMarkDigExtension;

public class AspForGeneratorOptions
{

    private readonly object _aspModel;
    private bool _designatePopulatedFieldsReadOnly = true;
    private bool _populateTagsWithModelData = true;
    private NullHandling _nullHandling = NullHandling.ShowBlank;
    private string _warningMessage = "Field {0} not found.";
    private uint _maxReferenceLength = 100;
    private DateTimeOverride _dateTimeOverride = DateTimeOverride.AsDate;
    private string? _defaultCheckedChecboxValue = true.ToString();
    private EnumHandling _defaultEnumHandling = EnumHandling.AsRadioButtons;

    public AspForGeneratorOptions(object model)
    {
        this._aspModel = model;
    }

    public object ASPModel
    {
        get
        {
            return this._aspModel;
        }
    }

    public NullHandling NullHandling
    {
        get
        {
            return this._nullHandling;
        }
        set
        {
            this._nullHandling = value;
        }
    }

    public string WarningMessage
    {
        get
        {
            return this._warningMessage;
        }
        set
        {
            this._warningMessage = value;
        }
    }

    public uint MaxReferenceLength
    {
        get
        {
            return this._maxReferenceLength;
        }
        set
        {
            this._maxReferenceLength = value;
        }
    }

    public bool PopulateTagsWithModelData
    {
        get
        {
            return this._populateTagsWithModelData;
        }
        set
        {
            this._populateTagsWithModelData = value;
            if ((!value) && this._designatePopulatedFieldsReadOnly)
            {
                this._designatePopulatedFieldsReadOnly = false;
            }
        }
    }

    public bool DesignatePopulatedFieldsReadOnly
    {
        get
        {
            return this._designatePopulatedFieldsReadOnly;
        }
        set
        {
            if (!this._populateTagsWithModelData)
            {
                throw new ArgumentException("Cannot set fields readonly if populate fields is not true.", "DesignatePopulatedFieldsReadOnly");
            }
            this._designatePopulatedFieldsReadOnly = value;
        }
    }

    public DateTimeOverride DateTimeOverride
    {
        get
        {
            return this._dateTimeOverride;
        }
        set
        {
            this._dateTimeOverride = value;
        }
    }

    public string? DefaultCheckedCheckboxValue
    {
        get
        {
            return this._defaultCheckedChecboxValue;
        }
        set
        {
            this._defaultCheckedChecboxValue = value;
        }
    }

    public EnumHandling EnumHandling
    {
        get
        {
            return this._defaultEnumHandling;
        }
        set
        {
            this._defaultEnumHandling = value;
        }
    }
}
