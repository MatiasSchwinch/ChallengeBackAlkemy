namespace ChallengeBackendCSharp.Entities
{
    public class GenreAudiovisualWork
    {
        public int GenreID { get; set; }
        public int AudiovisualWorkID { get; set; }

        public Genre? Genre { get; set; }
        public AudiovisualWork? AudiovisualWork { get; set; }
    }
}