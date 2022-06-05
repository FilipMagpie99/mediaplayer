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
        private readonly string saveFileName = ".\\mp3.json";
        private string playerNames = null;
        //private List<string> playerNames = new List<string>();
        private string playerPath = null;
        private string imagePath = "Assets/StoreLogo.png";
        private List<string> playerNamesIsoSto = new List<string>();
        private List<string> playerPathIsoSto = new List<string>();
        private List<string> playerImageIsoSto = new List<string>();
        //private List<string> historiaOdtwarzania = new List<string>();
        Random rnd = new Random();
        bool czyShuffle =false;
        private ViewModel viewModel = new ViewModel();

        
        MySong selectedSong;
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
                List<string> imagess = JsonConvert.DeserializeObject<List<string>>((string)ApplicationData.Current.LocalSettings.Values["obrazy"]);

                //nameses.Clear();
                //pathes.Clear();
                //imagess.Clear();    
     
                

                for (int i = 0; i < nameses.Count; i++)
                {
                    var song = nameses[i];
                    var path = pathes[i];
                    var imagepaths = imagess[i];
                    playerNamesIsoSto.Add(song);
                    playerPathIsoSto.Add(path);
                    playerImageIsoSto.Add(imagepaths);
                    viewModel.songLists.Add(new MySong(song, path, imagepaths));
                }

            }
            listView.ItemsSource = viewModel.songLists;
            viewModel.songLists.CollectionChanged += AudioTracks_CollectionChanged;
        }

        private void AudioTracks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Save_songs();
        }

        private BitmapSource getImage()
        {
            var wb = new WriteableBitmap(20, 20);
            var imageArray = new byte[wb.PixelWidth * wb.PixelHeight * 4];
            for (int i = 0; i < imageArray.Length; i += 4)
            {
                //BGRA format
                imageArray[i] = 0; // Blue
                imageArray[i + 1] = 0;  // Green
                imageArray[i + 2] = 255; // Red
                imageArray[i + 3] = 255; // Alpha
            }

            using (Stream stream = wb.PixelBuffer.AsStream())
            {
                //write to bitmap
                stream.Write(imageArray, 0, imageArray.Length);
            }

            return wb;
        }

        private void RefreshDataGrid()
        {


            //foreach (string playerName in playerNames)
            //{
            //viewModel.audioTracks.Add(playerName);
            //viewModel.songLists.Add(new MySong(playerName, playerName, "Assets/white2115.jpg"));
            //}
            if (playerNames != null && playerPath != null)
            {
                playerNamesIsoSto.Add(playerNames);
                playerPathIsoSto.Add(playerPath);
                playerImageIsoSto.Add(imagePath);
                viewModel.songLists.Add(new MySong(playerNames, playerPath, imagePath));
                playerNames = null;
                playerPath = null;
                imagePath = null;
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
            ApplicationData.Current.LocalSettings.Values["obrazy"] = JsonConvert.SerializeObject(playerImageIsoSto);
            //Debug.WriteLine(JsonConvert.SerializeObject(playerNames));

        }
        async void  timer_Tick(object sender, object e)
        {
            if (mediaPlayer.GetAsCastingSource() != null)
            { 

                lblStatus.Text = String.Format("{0} / {1}", mediaPlayer.Position.ToString(@"mm\:ss"), mediaPlayer.NaturalDuration.Duration().ToString(@"mm\:ss"));
                if (mediaPlayer.Position.ToString(@"mm\:ss").Equals(mediaPlayer.NaturalDuration.Duration().ToString(@"mm\:ss")) && listView.SelectedIndex < viewModel.songLists.Count())
                {
                    if (czyShuffle != true)
                    {
                        listView.SelectedItem = viewModel.songLists[listView.SelectedIndex + 1];
                        
                    }
                    else
                    {
                        listView.SelectedItem = viewModel.songLists[listView.SelectedIndex + rnd.Next(-listView.SelectedIndex, viewModel.songLists.Count()-listView.SelectedIndex)];
                       
                    }
                    OdtwarzanieMuzyki();
                }
                mediaPlayer.Play();
            }
            else
            {
                lblStatus.Text = "No file selected...";
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItems.Count != 0)
            {
                mediaPlayer.Play();
                Play.Visibility = Visibility.Collapsed;
                Pause.Visibility = Visibility.Visible;
            }

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
        private  void listView_Tapped(object sender, TappedRoutedEventArgs e)
        {

            OdtwarzanieMuzyki();
            mediaPlayer.Play();
           
        }
        private async void OdtwarzanieMuzyki()
        {
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

                //if (Listened.historiaOdtwarzania.Count() != 0 && selectedSong.PathName.Equals(Listened.historiaOdtwarzania.Last())){
                Listened.historiaOdtwarzania.Add(selectedSong.Name);
                Listened.kiedyOdtwarzane.Add(DateTime.Now);




                //}
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            //foreach(var song in viewModel.songLists) { 
                //if(song == selectedSong)
            
            viewModel.songLists.Remove(selectedSong);
            playerImageIsoSto.Remove(selectedSong.ImagePath);
            playerNamesIsoSto.Remove(selectedSong.Name);
            playerPathIsoSto.Remove(selectedSong.PathName);
            mediaPlayer.Pause();
            lblStatus.Visibility = Visibility.Collapsed;
            Save_songs();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(History));
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            czyShuffle = true;
        }

        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            czyShuffle = false;
        }


    }
}
