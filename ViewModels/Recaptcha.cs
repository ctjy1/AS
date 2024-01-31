using Newtonsoft.Json;

namespace Assignment2.ViewModels
{
    public class Recaptcha
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
    }
}
