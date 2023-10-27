using Newtonsoft.Json;

namespace TestingProject.Models;

public enum FunctionalFormType
{
    [JsonProperty("FirstForm")]
    FirstForm = 0,
    [JsonProperty("SecondForm")]
    SecondForm = 1,
    [JsonProperty("ThirdForm")]
    ThirdForm = 2,
    [JsonProperty("FourthForm")]
    FourthForm = 3
}
