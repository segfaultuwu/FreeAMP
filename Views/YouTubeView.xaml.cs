using FreeAMP.Core.Youtube;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FreeAMP.Views
{
    [SupportedOSPlatform("windows")]
    public partial class YouTubeView : UserControl
    {
        private readonly List<YoutubeTrack> _results = new();

        public YouTubeView()
        {
            InitializeComponent();
        }

        private async void YoutubeSearchButton_Click(object sender, RoutedEventArgs e)
        {
            await PerformSearch();
        }

        private async void YoutubeSearchTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                await PerformSearch();
            }
        }

        private async Task PerformSearch()
        {
            string query = YoutubeSearchTextBox.Text;

            if (string.IsNullOrWhiteSpace(query))
                return;

            YoutubeResultsListView.ItemsSource = null;
            _results.Clear();

            var results = await YoutubeService.SearchAsync(query);

            _results.AddRange(results);
            YoutubeResultsListView.ItemsSource = _results;
        }

        private async void YoutubeResultsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (YoutubeResultsListView.SelectedItem is not YoutubeTrack track)
                return;

            string? streamUrl = await YoutubeService.GetAudioStreamAsync(track.Url);

            if (string.IsNullOrWhiteSpace(streamUrl))
            {
                System.Windows.MessageBox.Show("Could not load audio stream.");
                return;
            }

            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.PlayYoutube(track.Title, track.Author, streamUrl);
            }
        }
    }
}