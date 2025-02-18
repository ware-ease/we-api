using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace BusinessLogicLayer.Models.Authentication
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Username is required")]
        //[JsonProperty("user-name")]
        public string Username { get; set; } = string.Empty;
        //[JsonProperty("password")]
        public string Password { get; set; } = string.Empty;
    }
}
