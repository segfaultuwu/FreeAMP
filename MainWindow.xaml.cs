using FreeAMP.Core.Music;
using FreeAMP.Resources;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Wpf.Ui.Controls;

namespace FreeAMP
{
    public partial class MainWindow : FluentWindow
    {
        private readonly LocalPlayer _player = new();
        private readonly DispatcherTimer _timer = new();

        public MainWindow()
        {
            InitializeComponent();

            Finder.LoadMusic(Finder.AllowedExts);
            TracksListView.ItemsSource = Finder.Tracks;

            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void TracksListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TracksListView.SelectedItem is not Track selectedTrack)
                return;

            _player.Play(selectedTrack.Path);

            NowPlayingTitleText.Text = selectedTrack.Title;

            NowPlayingArtistText.Text =
                string.IsNullOrWhiteSpace(selectedTrack.Artist)
                    ? Strings.UnknownArtist
                    : selectedTrack.Artist;

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

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsOverlay.Visibility = Visibility.Visible;
        }

        private void CloseSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            SettingsOverlay.Visibility = Visibility.Collapsed;
        }

        protected override void OnClosed(EventArgs e)
        {
            _timer.Stop();
            _player.Dispose();
            base.OnClosed(e);
        }
    }
}