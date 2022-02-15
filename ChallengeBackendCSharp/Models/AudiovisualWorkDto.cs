﻿namespace ChallengeBackendCSharp.Models
{
    public class AudiovisualWorkDto
    {
        public string? Image { get; set; }
        public string? Title { get; set; }
        public DateTime ReleaseDate { get; set; }
        public float Rating { get; set; }
    }

    public class AudiovisualWorkWithIdDto : AudiovisualWorkDto
    {
        public int AudiovisualWorkID { get; set; }
    }

    public class AudiovisualWorkWithCharactersDto : AudiovisualWorkDto
    {
        public IList<string>? Characters { get; set; }
    }

    public class AudiovisualWorkWithCharactersAndGenresDto : AudiovisualWorkWithCharactersDto
    {
        public IList<string>? Genres { get; set; }
    }
}
