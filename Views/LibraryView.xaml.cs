using FreeAMP.Core.Music;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Controls;

namespace FreeAMP.Views
{
    [SupportedOSPlatform("windows")]
    public partial class LibraryView : UserControl
    {
        public LibraryView()
        {
            InitializeComponent();

            Finder.LoadMusic(Finder.AllowedExts);

            TracksListView.ItemsSource = Finder.Tracks;
        }

        private void TracksListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TracksListView.SelectedItem is not Track track)
                return;

            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.PlayTrack(track);
            }
        }
    }
}