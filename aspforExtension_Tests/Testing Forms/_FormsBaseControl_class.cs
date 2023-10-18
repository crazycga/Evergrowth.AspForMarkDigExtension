using TestingProject.Models;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace TestingProject.Database.FormObjects;

/// <summary>
/// This is the basic forms class that contains the coordinates that are required to identify the form that is being recorded.  The IBasicGenericFormData is meant to be overriden to house the actual data captured on the form.
/// </summary>
public abstract class _FormsBaseControl_class
{
    public Guid FormRequestId { get; set; }
    public FunctionalFormType? FunctionalFormType { get; set; }
    public int? MajorVersion { get; set; }
    public int? MinorVersion { get; set; }
    public int? PatchVersion { get; set; }
    public string? FormTemplate { get; set; }

    public IBasicGenericFormData? formData { get; set; }

    public abstract string ConvertToJson();
    public abstract bool ConvertFromJson(string input);
}
