namespace FreeAMP.Core.Music
{
    public class Track
    {
        public int Number { get; set; }

        public string Title { get; set; } = "";

        public string Artist { get; set; } = "";

        public string Album { get; set; } = "";

        public string Duration { get; set; } = "";

        public string Path { get; set; } = "";
    }
}