using Microsoft.AspNetCore.Mvc;

namespace ChallengeBackendCSharp.Models
{
    public class CharacterQueryDto
    {
        [FromQuery(Name = "name")]
        public string? Name { get; set; }
        [FromQuery(Name = "age")]
        public int? Age { get; set; }
        [FromQuery(Name = "movie")]
        public int? Movie { get; set; }

        public bool GetState()
        {
            return (Name is null && Age is null && Movie is null);
        }
    }
}
