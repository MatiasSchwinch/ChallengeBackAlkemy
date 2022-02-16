namespace ChallengeBackendCSharp.Entities
{
    public class Genre
    {
        public int GenreID { get; set; }
        public string? Image { get; set; }
        public string? Name { get; set; }
        public IList<GenreAudiovisualWork>? GenreAudiovisualWorks { get; set; }
    }
}