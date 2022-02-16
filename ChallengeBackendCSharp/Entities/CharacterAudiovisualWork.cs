namespace ChallengeBackendCSharp.Entities
{
    public class CharacterAudiovisualWork
    {
        public int CharacterID { get; set; }
        public int AudiovisualWorkID { get; set; }

        public Character? Character { get; set; }
        public AudiovisualWork? AudiovisualWork { get; set; }
    }
}