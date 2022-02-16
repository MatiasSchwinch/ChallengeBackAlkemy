namespace ChallengeBackendCSharp.Entities
{
    public class AudiovisualWork
    {
        public int AudiovisualWorkID { get; set; }
        public string? Image { get; set; }
        public string? Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public float Rating { get; set; }
        public IList<CharacterAudiovisualWork>? CharacterAudiovisualWorks { get; set; }
        public IList<GenreAudiovisualWork>? GenreAudiovisualWorks { get; set; }
    }
}