using FreeAMP.Core.Music;
using FreeAMP.Core.Youtube;
using FreeAMP.Resources;
using FreeAMP.Views;
using System;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Wpf.Ui.Controls;

namespace FreeAMP
{
    [SupportedOSPlatform("windows")]
    public partial class MainWindow : FluentWindow
    {
        private readonly LocalPlayer _player = new();
        private readonly DispatcherTimer _timer = new();

        public MainWindow()
        {
            InitializeComponent();

            MainContent.Content = new LibraryView();

            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void LibraryButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new LibraryView();
        }

        private void YoutubeButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new YouTubeView();
        }

        private void PlaylistsButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new PlaylistsView();
        }

        private void AlbumsButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new AlbumsView();
        }

        private void ArtistsButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new ArtistsView();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            MainContent.Content = new SettingsView();
        }

        public void PlayYoutube(string title, string author, string streamUrl)
        {
            _player.Play(streamUrl);

            NowPlayingTitleText.Text = title;
            NowPlayingArtistText.Text = author;

            DurationText.Text = "0:00";
            CurrentTimeText.Text = "0:00";
            ProgressSlider.Value = 0;
        }

        public void PlayTrack(Track track)
        {
            _player.Play(track.Path);

            NowPlayingTitleText.Text = track.Title;

            NowPlayingArtistText.Text =
                string.IsNullOrWhiteSpace(track.Artist)
                    ? Strings.UnknownArtist
                    : track.Artist;

            DurationText.Text = "0:00";
            CurrentTimeText.Text = "0:00";
            ProgressSlider.Value = 0;
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            _player.TogglePause();
        }

        private void VolumeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _player.Volume = (float)(e.NewValue / 100.0);
        }

        private void ProgressSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _player.Seek(ProgressSlider.Value);
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (_player.Duration.TotalSeconds <= 0)
                return;

            ProgressSlider.Maximum = _player.Duration.TotalSeconds;
            ProgressSlider.Value = _player.Position.TotalSeconds;

            CurrentTimeText.Text = _player.Position.ToString(@"m\:ss");
            DurationText.Text = _player.Duration.ToString(@"m\:ss");
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer.Stop();
            _player.Dispose();

            base.OnClosed(e);
        }
    }
}