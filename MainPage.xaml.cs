using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x415

namespace mediaplayer
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private readonly string saveFileName = ".\\mp3.json";
        private string playerNames = null;
        //private List<string> playerNames = new List<string>();
        private string playerPath = null;
        private string imagePath = null;
        private List<string> playerNamesIsoSto = new List<string>();
        private List<string> playerPathIsoSto = new List<string>();
        private List<string> playerImageIsoSto = new List<string>();
        private ViewModel viewModel = new ViewModel();
        //List<MySong> songLists = new List<MySong>();


        public class ViewModel
        {
            //public ObservableCollection<string> audioTracks { get; set; } = new ObservableCollection<string>(); //{ new Obiekt { Nazwa = "a" }, new Obiekt { Nazwa = "b" } };
            public ObservableCollection<MySong> songLists { get; set; } = new ObservableCollection<MySong>(); //{ new Obiekt { Nazwa = "a" }, new Obiekt { Nazwa = "b" } };


        }
        public MainPage()
        {
            this.InitializeComponent();
            //dataGrid.ItemsSource = viewModel.audioTracks;
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("listaPiosenek")
                && ApplicationData.Current.LocalSettings.Values.ContainsKey("listaSciezek"))
            {
                List<string> nameses = JsonConvert.DeserializeObject<List<string>>((string)ApplicationData.Current.LocalSettings.Values["listaPiosenek"]);
                List<string> pathes = JsonConvert.DeserializeObject<List<string>>((string)ApplicationData.Current.LocalSettings.Values["listaSciezek"]);
                for (int i = 0; i < nameses.Count; i++)
                {
                    var song = nameses[i];
                    var path = pathes[i];
                    playerNamesIsoSto.Add(song);
                    playerPathIsoSto.Add(path);
                    viewModel.songLists.Add(new MySong(song, path, "Assets/white2115.jpg"));
                }
                /*
                foreach (var item in JsonConvert.DeserializeObject<List<string>>((string)ApplicationData.Current.LocalSettings.Values["listaPiosenek"]))
                {
                    playerNamesIsoSto.Add(item);
                    viewModel.songLists.Add(new MySong(item, item, "Assets/white2115.jpg"));
                }
                foreach (var item in JsonConvert.DeserializeObject<List<string>>((string)ApplicationData.Current.LocalSettings.Values["listaSciezek"]))
                {
                    playerPathIsoSto.Add(item);
                    viewModel.songLists.Add(new MySong(item, item, "Assets/white2115.jpg"));
                }
                */
            }
            listView.ItemsSource = viewModel.songLists;
            viewModel.songLists.CollectionChanged += AudioTracks_CollectionChanged;
        }

        private void AudioTracks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Save_songs();
        }

        private void RefreshDataGrid()
        {
            //viewModel.songLists.Clear();

            //foreach (string playerName in playerNames)
            //{
            //viewModel.audioTracks.Add(playerName);
            //viewModel.songLists.Add(new MySong(playerName, playerName, "Assets/white2115.jpg"));
            //}
            if(playerNames!=null && playerPath != null)
            {
                playerNamesIsoSto.Add(playerNames);
                playerPathIsoSto.Add(playerPath);
                playerImageIsoSto.Add(imagePath);
                viewModel.songLists.Add(new MySong(playerNames, playerPath, "Assets/white2115.jpg"));
                playerNames = null;
                playerPath = null;
            }
            
            
            //playerNames.ForEach(names => playerNamesIsoSto.Add(names));
            //playerNames.Clear();
            //playerPath.Clear();
            
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
                //playerPath.Add(file.Path);
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
                imagePath = file.Path;
                //playerPath.Add(file.Path);
            }
        }
        private void closeImport_Click(object sender, RoutedEventArgs e)
        {
            playerNames = nazwaUtworu.Text;
            RefreshDataGrid();
            popup.IsOpen = false;
            //mediaPlayer.Play();
            //DispatcherTimer timer = new DispatcherTimer();
            //timer.Interval = TimeSpan.FromSeconds(1);
            //timer.Tick += timer_Tick;
            //timer.Start();
        }
        private void Save_songs()
        {
            ApplicationData.Current.LocalSettings.Values["listaPiosenek"] = JsonConvert.SerializeObject(playerNamesIsoSto);
            ApplicationData.Current.LocalSettings.Values["listaSciezek"] = JsonConvert.SerializeObject(playerPathIsoSto);
            ApplicationData.Current.LocalSettings.Values["obrazy"] = JsonConvert.SerializeObject(playerNames);
            //Debug.WriteLine(JsonConvert.SerializeObject(playerNames));

        }
        void timer_Tick(object sender, object e)
        {
            if (mediaPlayer.GetAsCastingSource() != null)
                lblStatus.Text = String.Format("{0} / {1}", mediaPlayer.Position.ToString(@"mm\:ss"), mediaPlayer.NaturalDuration.Duration().ToString(@"mm\:ss"));
            else
                lblStatus.Text = "No file selected...";
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Play();
            Play.Visibility = Visibility.Collapsed;
            Pause.Visibility = Visibility.Visible;

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Pause();
            Pause.Visibility = Visibility.Collapsed;
            Play.Visibility = Visibility.Visible;
        }
        /*
        private async void TextBlock_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(((TextBlock)sender).Text);

            if (file != null)
            {
                string faToken = StorageApplicationPermissions.FutureAccessList.Add(file);
                mediaPlayer.SetFileSource(await StorageApplicationPermissions.FutureAccessList.GetFileAsync(faToken));

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += timer_Tick;
                timer.Start();
            }
            mediaPlayer.Play();
        }
        */
        private async void listView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var selectedSong = listView.SelectedItem as MySong;

            StorageFile file = await StorageFile.GetFileFromPathAsync(selectedSong.PathName);

            if (file != null)
            {
                string faToken = StorageApplicationPermissions.FutureAccessList.Add(file);
                mediaPlayer.SetFileSource(await StorageApplicationPermissions.FutureAccessList.GetFileAsync(faToken));

                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += timer_Tick;
                timer.Start();
            }
            mediaPlayer.Play();
        }
    }
}
