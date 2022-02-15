using System.ComponentModel.DataAnnotations;

namespace ChallengeBackendCSharp.Models
{
    public class RegisterModel
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
