using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace mediaplayer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Playlists : Page
    {
        private string playlistName = null;
        private string coverPath = "Assets/StoreLogo.png";
        private List<string> playlistIdSave = new List<string>();
        private List<string> playlistNameSave = new List<string>();
        private List<string> coverPathSave = new List<string>();
        private ViewModel viewModel = new ViewModel();
        public static Playlist selectedPlaylist;

        public class ViewModel
        {
            public ObservableCollection<Playlist> playLists { get; set; } = new ObservableCollection<Playlist>();
        }
        public Playlists()
        {
            this.InitializeComponent();
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("playlists")
                && ApplicationData.Current.LocalSettings.Values.ContainsKey("covers")
                && ApplicationData.Current.LocalSettings.Values.ContainsKey("playlistID"))
            {
                List<string> nameses = JsonConvert.DeserializeObject<List<string>>((string)ApplicationData.Current.LocalSettings.Values["playlists"]);
                List<string> images = JsonConvert.DeserializeObject<List<string>>((string)ApplicationData.Current.LocalSettings.Values["covers"]);
                List<string> idplaylists = JsonConvert.DeserializeObject<List<string>>((string)ApplicationData.Current.LocalSettings.Values["playlistID"]);

                //nameses.Clear();
                //imagess.Clear();    

                for (int i = 0; i < nameses.Count; i++)
                {
                    var name = nameses[i];
                    var image = images[i];
                    var playlistids = idplaylists[i];
                    playlistNameSave.Add(name);
                    coverPathSave.Add(image);
                    playlistIdSave.Add(playlistids);
                    viewModel.playLists.Add(new Playlist(playlistids,name, image));
                }
            }
            listView.ItemsSource = viewModel.playLists;
            viewModel.playLists.CollectionChanged += AudioTracks_CollectionChanged;
        }

        private void AudioTracks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Save_playlist();
        }

        private void Save_playlist()
        {
            ApplicationData.Current.LocalSettings.Values["playlistID"] = JsonConvert.SerializeObject(playlistNameSave);
            ApplicationData.Current.LocalSettings.Values["playlists"] = JsonConvert.SerializeObject(playlistNameSave);
            ApplicationData.Current.LocalSettings.Values["covers"] = JsonConvert.SerializeObject(coverPathSave);
        }

        private void RefreshDataGrid()
        {
            if (playlistName != null)
            {
                string Id = Guid.NewGuid().ToString();
                playlistNameSave.Add(playlistName);
                coverPathSave.Add(coverPath);
                playlistIdSave.Add(Id);
                viewModel.playLists.Add(new Playlist(Id,playlistName, coverPath));
                playlistName = null;
                coverPath = null;
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            playlistNameSave.Remove(selectedPlaylist.Name);
            coverPathSave.Remove(selectedPlaylist.ImagePath);
            playlistIdSave.Remove(selectedPlaylist.PlaylistId);
            viewModel.playLists.Remove(selectedPlaylist);
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            popupPlaylist.IsOpen = true;
        }

        private async void importImage_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".jpg");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)

            {
                String a;
                if (await IsFilePresent(file.Name))
                {
                    a = "ms-appdata:///local/" + file.Name;
                }
                else
                {
                    await file.CopyAsync(ApplicationData.Current.LocalFolder);
                }
                a = "ms-appdata:///local/" + file.Name;
                coverPath = a;

            }
        }

        public async Task<bool> IsFilePresent(string fileName)
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            return item != null;
        }

        private void closeImport_Click(object sender, RoutedEventArgs e)
        {
            playlistName = nazwaPlaylisty.Text;
            RefreshDataGrid();
            popupPlaylist.IsOpen = false;
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listView.SelectedItems.Count != 0)
            {
                selectedPlaylist = listView.SelectedItem as Playlist;
                Delete.Visibility = Visibility.Visible;
                Goto.Visibility = Visibility.Visible;
            }
                
        }

        private void Goto_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }

        private void HistoryNav_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(History));
        }
    }
}
