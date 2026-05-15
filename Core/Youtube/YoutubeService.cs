using System;
using System.Collections.Generic;
using System.Text;
using YoutubeExplode;
using YoutubeExplode.Search;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Videos;

namespace FreeAMP.Core.Youtube
{
    public static class YoutubeService
    {
        private static readonly YoutubeClient Client = new();

        public static async Task<List<YoutubeTrack>> SearchAsync(string query)
        {
            var results = new List<YoutubeTrack>();

            await foreach (var video in Client.Search.GetVideosAsync(query))
            {
                results.Add(new YoutubeTrack
                {
                    Title = video.Title,
                    Author = video.Author.ChannelTitle,
                    Url = $"https://youtube.com/watch?v={video.Id}"
                });

                if (results.Count >= 20)
                    break;
            }

            return results;
        }

        public static async Task<string?> GetAudioStreamAsync(string url)
        {
            var videoId = VideoId.Parse(url);

            var manifest = await Client.Videos.Streams.GetManifestAsync(videoId);

            var streamInfo = manifest
                .GetAudioOnlyStreams()
                .GetWithHighestBitrate();

            return streamInfo?.Url;
        }
    }
}
