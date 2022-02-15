namespace ChallengeBackendCSharp.Models
{
    public class CharacterDto
    {
        public string? Image { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public float Weight { get; set; }
        public string? History { get; set; }
    }

    public class CharacterWithIdDto : CharacterDto
    {
        public int CharacterID { get; set; }
        //public IList<AudiovisualWorkWithIdDto>? AudiovisualWorks { get; set; }
    }

    public class CharacterWithMoviesDto : CharacterDto
    {
        public IList<string>? AudiovisualWorks { get; set; }
    }
}
