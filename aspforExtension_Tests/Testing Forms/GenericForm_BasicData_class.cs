namespace TestingProject.Database.FormObjects;

using Evergrowth.AspForMarkDigExtension.Enums;
using Evergrowth.AspForMarkDigExtension.Decorators;
using System.Text.Json;
using System.Text.Json.Serialization;

public class GenericForm_BasicData_class : IBasicGenericFormData
{
    public bool? cbCheckCondition1 { get; set; }
    public bool? cbCheckCondition2 { get; set; }
    public bool? cbCheckCondition3 { get; set; }
    public string? tbInitials1 { get; set; }
    public string? tbInitials2 { get; set; }
    public string? tbInitials3 { get; set; }
    public bool? cbYesNo1 { get; set; }
    public bool? cbYesNo2 { get; set; }

    [AspForCheckedValue("Excluded")]
    [AspForReadOnly]
    public bool? cbYesNo3 { get; set; }
    public string? tbYesNoInitials1 { get; set; }
    public string? tbYesNoInitials2 { get; set; }
    public string? tbYesNoInitials3 { get; set; }
    public bool? cbAcceptance1 { get; set; }
    public bool? cbAcceptance2 { get; set; }
    
    [AspForCheckedValue("Shadows")]
    public bool? cbAcceptance3 { get; set; }

    [AspForIgnore]
    public string? tbAcceptanceInitials1 { get; set; }

    [AspForReadOnly]
    public string? tbAcceptanceInitials2 { get; set; }

    [AspForCheckedValue("ThisIsInvalid")]
    public string? tbAcceptanceInitials3 { get; set; }
    public string? tbRandomName { get; set; }
    public decimal? tbRandomRate { get; set; }
    
    [AspForRequiredInput]
    public string? tbRateInitials1 { get; set; }

    [AspForRequiredInput]
    public string? tbName1 { get; set; }
    public DateTime? dtName1 { get; set; }
    public string? tbName2 { get; set; }
    public DateTime? dtName2 { get; set; }
    public string? tbName3 { get; set; }
    public DateTime? dtName3 { get; set; }
    public string? tbName4 { get; set; }
    public DateTime? dtName4 { get; set; }
    public string? tbSubName1 { get; set; }

    [AspForDateTimeType(DateTimeOverride.AsTime)]
    public DateTime? dtSubName1 { get; set; }
    public string? tbSubName2 { get; set; }
    public DateTime? dtSubName2 { get; set; }
    public string? tbSubName3 { get; set; }
    public DateTime? dtSubName3 { get; set; }
    public string? tbManagerName1 { get; set; }
    public DateTime? dtManagerName1 { get; set; }
    public string? tbManagerName2 { get; set; }
    public DateTime? dtManagerName2 { get; set; }
}
