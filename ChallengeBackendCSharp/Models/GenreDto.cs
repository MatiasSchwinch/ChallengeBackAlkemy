namespace ChallengeBackendCSharp.Models
{
    public class GenreDto
    {
        public string? Image { get; set; }
        public string? Name { get; set; }
    }
    public class GenreWithAudiovisualWorksDto : GenreDto
    {
        public IList<string>? AudiovisualWorks { get; set; }
    }
}
