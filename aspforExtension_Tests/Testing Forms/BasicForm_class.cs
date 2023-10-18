using System.Text.Json.Serialization;
using System.Text.Json;
using TestingProject.Models;

namespace TestingProject.Database.FormObjects;

/// <summary>
/// This is the basic form.
/// </summary>
public class BasicForm_class: _FormsBaseControl_class
{
    // DUDE!  This was a good idea, riffed from a variety of sources, but most straightforward from
    // https://stackoverflow.com/a/48994193
    //
    // Basically, this says that the constructor has to be integrated AS AN ATTRIBUTE not as a variable.  That's WILD!

    public new GenericForm_BasicData_class formData { get; set; } = new GenericForm_BasicData_class();

    /// <summary>
    /// Basic constructor for the class.  Nothing special.
    /// </summary>
    public BasicForm_class() : base() { }

    /// <summary>
    /// This constructor will apply the variables on instantiation.
    /// </summary>
    /// <param name="obviousFormType"></param>
    /// <param name="MajorVersion"></param>
    /// <param name="MinorVersion"></param>
    /// <param name="PatchVersion"></param>
    public BasicForm_class(Guid Id, FunctionalFormType obviousFormType, int MajorVersion, int MinorVersion, int PatchVersion) : base()
    {
        this.FormRequestId = Id;
        this.FunctionalFormType = obviousFormType;
        this.MajorVersion = MajorVersion;
        this.MinorVersion = MinorVersion;
        this.PatchVersion = PatchVersion;
    }

    /// <summary>
    /// This routine will convert the object into a JSON object for storage in the database.
    /// </summary>
    /// <returns>String of JSON object.</returns>
    public override string ConvertToJson()
    {
        JsonSerializerOptions options = new JsonSerializerOptions
        {
            //Converters = { //new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
            //    new CustomDateConverter("yyyy-MM-dd"),
            //    new JsonEnumMemberStringEnumConverter()}
        };
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        options.WriteIndented = true;

        string response = JsonSerializer.Serialize(this, options);

        return response;
    }

    /// <summary>
    /// This routine will convert a JSON string into **this** object.
    /// </summary>
    /// <param name="input">String of JSON text.</param>
    /// <returns>True if succeeded; false if it did not.</returns>
    public override bool ConvertFromJson(string input)
    {
        if (string.IsNullOrEmpty(input)) return false;

        var baseResponse = (BasicForm_class)JsonSerializer.Deserialize(input, typeof(BasicForm_class));
        if (baseResponse == null) return false;

        this.FunctionalFormType = baseResponse.FunctionalFormType;
        this.MajorVersion = baseResponse.MajorVersion;
        this.MinorVersion = baseResponse.MinorVersion;
        this.PatchVersion = baseResponse.PatchVersion;
        this.formData = baseResponse.formData;

        return true;
    }
}
