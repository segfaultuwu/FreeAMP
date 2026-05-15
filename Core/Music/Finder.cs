using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text;

namespace FreeAMP.Core.Music
{
    public class Finder
    {
        public static List<Track> Tracks { get; set; } = new List<Track>();
        public static HashSet<string> AllowedExts { get; } = new HashSet<string>(new[] { ".mp3", ".wav", ".flac", ".aac", ".ogg" });

        public static void LoadMusic(HashSet<string> allowedExts)
        {
            var musicPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            for (DirectoryInfo dir = new DirectoryInfo(musicPath); dir != null; dir = dir.Parent)
            {
                foreach (var file in dir.GetFiles())
                {
                    if (!allowedExts.Contains(Path.GetExtension(file.Name).ToLowerInvariant()))
                    {
                        continue;
                    }

                    var track = new Track
                    {
                        Title = Path.GetFileNameWithoutExtension(file.Name),
                        Path = file.FullName
                    };
                    Tracks.Add(track);
                }
            }
        }
    }
}
