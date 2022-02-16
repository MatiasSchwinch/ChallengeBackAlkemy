using System.ComponentModel.DataAnnotations;

namespace ChallengeBackendCSharp.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}