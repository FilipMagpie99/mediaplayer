using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace mediaplayer
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private string playerNames = null;
        private string playerPath = null;
        private string imagePath = "Assets/StoreLogo.png";
        private string playlistId = null;
        private List<string> playerNamesIsoSto = new List<string>();
        private List<string> playerPathIsoSto = new List<string>();
        private List<string> playerImageIsoSto = new List<string>();
        private List<string> playerPlaylistSave = new List<string>();
        Random rnd = new Random();
        bool czyShuffle =false;
        bool czyPauza = true;
        private ViewModel viewModel = new ViewModel();        
        MySong selectedSong;
        ObservableCollection<MySong> searchSongs = new ObservableCollection<MySong>();

        public class ViewModel
        {
            public ObservableCollection<MySong> songLists { get; set; } = new ObservableCollection<MySong>();
            public ObservableCollection<MySong> filteredSongList { get; set; } = new ObservableCollection<MySong>();

        }
        public MainPage()
        {
            this.InitializeComponent();
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("listaPiosenek")
                && ApplicationData.Current.LocalSettings.Values.ContainsKey("listaSciezek")
                && ApplicationData.Current.LocalSettings.Values.ContainsKey("obrazy")
                && ApplicationData.Current.LocalSettings.Values.ContainsKey("playlista"))
            {
                List<string> nameses = JsonConvert.DeserializeObject<List<string>>((string)ApplicationData.Current.LocalSettings.Values["listaPiosenek"]);
                List<string> pathes = JsonConvert.DeserializeObject<List<string>>((string)ApplicationData.Current.LocalSettings.Values["listaSciezek"]);
                List<string> images = JsonConvert.DeserializeObject<List<string>>((string)ApplicationData.Current.LocalSettings.Values["obrazy"]);
                List<string> playlistID = JsonConvert.DeserializeObject<List<string>>((string)ApplicationData.Current.LocalSettings.Values["playlista"]);

                //nameses.Clear();
                //pathes.Clear();
                //imagess.Clear();
                //playlistID.Clear();
                for (int i = 0; i < nameses.Count; i++)
                {
                    var song = nameses[i];
                    var path = pathes[i];
                    var image = images[i];
                    var IdPlaylist = playlistID[i];
                    playerNamesIsoSto.Add(song);
                    playerPathIsoSto.Add(path);
                    playerImageIsoSto.Add(image);
                    playerPlaylistSave.Add(IdPlaylist);
                    viewModel.songLists.Add(new MySong(song, path, image, IdPlaylist));
                }
            }
            foreach (MySong song in viewModel.songLists)
            {
                if (song.playlistId.Equals(Playlists.selectedPlaylist.PlaylistId))
                {
                    viewModel.filteredSongList.Add(song);
                }
            }
            
            listView.ItemsSource = viewModel.filteredSongList;
            viewModel.filteredSongList.CollectionChanged += AudioTracks_CollectionChanged;
            searchSongs = viewModel.filteredSongList;
        }
        private void AudioTracks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Save_songs();
        }
        private void Save_songs()
        {
            ApplicationData.Current.LocalSettings.Values["listaPiosenek"] = JsonConvert.SerializeObject(playerNamesIsoSto);
            ApplicationData.Current.LocalSettings.Values["listaSciezek"] = JsonConvert.SerializeObject(playerPathIsoSto);
            ApplicationData.Current.LocalSettings.Values["obrazy"] = JsonConvert.SerializeObject(playerImageIsoSto);
            ApplicationData.Current.LocalSettings.Values["playlista"] = JsonConvert.SerializeObject(playerPlaylistSave);

        }

        private void RefreshDataGrid()
        {
            if (playerNames != null && playerPath != null)
            {
                playerNamesIsoSto.Add(playerNames);
                playerPathIsoSto.Add(playerPath);
                playerImageIsoSto.Add(imagePath);
                playerPlaylistSave.Add(Playlists.selectedPlaylist.PlaylistId);
                viewModel.filteredSongList.Add(new MySong(playerNames, playerPath, imagePath, Playlists.selectedPlaylist.PlaylistId));
                viewModel.songLists.Add(new MySong(playerNames, playerPath, imagePath, Playlists.selectedPlaylist.PlaylistId));
                playerNames = null;
                playerPath = null;
                imagePath = null;
            }
        }
        private async void Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            popup.IsOpen = true;
        }

        private async void importSong_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".mp3");

            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                mediaPlayer.SetFileSource(file);
                playerPath = file.Path;
            }
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
                imagePath = a;
                
            }
        }

        public async Task<bool> IsFilePresent(string fileName)
        {
            var item = await ApplicationData.Current.LocalFolder.TryGetItemAsync(fileName);
            return item != null;
        }        

        private void closeImport_Click(object sender, RoutedEventArgs e)
        {
            playerNames = nazwaUtworu.Text;
            RefreshDataGrid();
            popup.IsOpen = false;
        }

        async void  timer_Tick(object sender, object e)
        {
            if (mediaPlayer.GetAsCastingSource() != null)
            { 
                lblStatus.Text = String.Format("{0} / {1}", mediaPlayer.Position.ToString(@"mm\:ss"), mediaPlayer.NaturalDuration.Duration().ToString(@"mm\:ss"));
                if (mediaPlayer.Position.ToString(@"mm\:ss").Equals(mediaPlayer.NaturalDuration.Duration().ToString(@"mm\:ss")) && listView.SelectedIndex < viewModel.filteredSongList.Count() -1)
                {
                    if (czyShuffle != true)
                        listView.SelectedItem = viewModel.filteredSongList[listView.SelectedIndex + 1];
                    else
                        listView.SelectedItem = viewModel.filteredSongList[listView.SelectedIndex + rnd.Next(-listView.SelectedIndex, viewModel.filteredSongList.Count()-listView.SelectedIndex)];
                    OdtwarzanieMuzyki();
                }
                if(czyPauza == false)
                    mediaPlayer.Play();
            }
            else
            {
                lblStatus.Text = "No file selected...";
            }
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItems.Count != 0)
            {
                mediaPlayer.Play();
                Play.Visibility = Visibility.Collapsed;
                Pause.Visibility = Visibility.Visible;
                czyPauza = false;
            }
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
            Pause.Visibility = Visibility.Collapsed;
            Play.Visibility = Visibility.Visible;
            czyPauza = true;
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OdtwarzanieMuzyki();
            //mediaPlayer.Play();
        }

        private async void OdtwarzanieMuzyki()
        {
            mediaPlayer.Play();
            lblStatus.Visibility = Visibility.Visible;
            if (listView.SelectedItems.Count != 0)
            {
                DispatcherTimer timer = new DispatcherTimer();
                selectedSong = listView.SelectedItem as MySong;
                Delete.Visibility = Visibility.Visible;
                StorageFile file = await StorageFile.GetFileFromPathAsync(selectedSong.PathName);

                if (file != null)
                {
                    string faToken = StorageApplicationPermissions.FutureAccessList.Add(file);
                    mediaPlayer.SetFileSource(await StorageApplicationPermissions.FutureAccessList.GetFileAsync(faToken));

                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Tick += timer_Tick;
                    timer.Start();
                }
                Listened.historiaOdtwarzania.Add(selectedSong.Name);
                Listened.kiedyOdtwarzane.Add(DateTime.Now);
                
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {   
            playerImageIsoSto.Remove(selectedSong.ImagePath);
            playerNamesIsoSto.Remove(selectedSong.Name);
            playerPathIsoSto.Remove(selectedSong.PathName);
            playerPlaylistSave.Remove(selectedSong.playlistId);
            viewModel.songLists.Remove(selectedSong);
            viewModel.filteredSongList.Remove(selectedSong);
            mediaPlayer.Pause();
            lblStatus.Visibility = Visibility.Collapsed;
            //Save_songs();
        }

        private void HistoryNav_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(History));
        }

        private void PlaylistNav_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Playlists));
        }

        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            czyShuffle = true;
        }
        private void Regular_Click(object sender, RoutedEventArgs e)
        {
            czyShuffle = false;
        }

        private void sbar_TextChanged(object sender, TextChangedEventArgs e)
        {
            listView.ItemsSource = searchSongs;

        }

        private void Search_Click(object sender, RoutedEventArgs e)
        {
            var cont = from s in searchSongs where s.Name.Contains(sbar.Text) select s;//LINQ Query 

            listView.ItemsSource = cont;
        }
        



    }
}
