namespace ChallengeBackendCSharp.Entities
{
    public class Character
    {
        public int CharacterID { get; set; }
        public string? Image { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
        public float Weight { get; set; }
        public string? History { get; set; }
        public IList<CharacterAudiovisualWork>? CharacterAudiovisualWorks { get; set; }
    }
}
