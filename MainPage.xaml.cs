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
        private ICollection<string> playerNames = new HashSet<string>();
        private ViewModel viewModel = new ViewModel();


        public class ViewModel
        {
            public ObservableCollection<string> audioTracks { get; set; } = new ObservableCollection<string>(); //{ new Obiekt { Nazwa = "a" }, new Obiekt { Nazwa = "b" } };

        }
        public MainPage()
        {
            this.InitializeComponent();
            //dataGrid.ItemsSource = viewModel.audioTracks;
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("listaPiosenek"))
            {
                foreach (var item in JsonConvert.DeserializeObject<List<string>>((string)ApplicationData.Current.LocalSettings.Values["listaPiosenek"]))
                {
                    viewModel.audioTracks.Add(item);
                }
            }
            listView.ItemsSource = viewModel.audioTracks;
            viewModel.audioTracks.CollectionChanged += AudioTracks_CollectionChanged;



        }

        private void AudioTracks_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Save_songs();
        }

        private void RefreshDataGrid()
        {
            viewModel.audioTracks.Clear();
            foreach (string playerName in playerNames)
            {
                viewModel.audioTracks.Add(playerName);
            }

        }
        private async void Button_ClickAsync(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".mp3");


            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {

                mediaPlayer.SetFileSource(file);
                playerNames.Add(file.Path);
                RefreshDataGrid();
                mediaPlayer.Play();
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += timer_Tick;
                timer.Start();
            }


        }



        private void Save_songs()
        {
            ApplicationData.Current.LocalSettings.Values["listaPiosenek"] = JsonConvert.SerializeObject(playerNames);
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


    }
}
