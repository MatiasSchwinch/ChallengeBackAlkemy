using Microsoft.AspNetCore.Mvc;

namespace ChallengeBackendCSharp.Models
{
    public class AudiovisualWorkQueryDto
    {
        [FromQuery(Name = "name")]
        public string? Name { get; set; }
        [FromQuery(Name = "genre")]
        public int? GenreID { get; set; }
        [FromQuery(Name = "order")]
        public Order? Order { get; set; }

        public bool GetState()
        {
            return (Name is null && GenreID is null && Order is null);
        }
    }

    public enum Order
    {
        ASC,
        DESC
    }
}