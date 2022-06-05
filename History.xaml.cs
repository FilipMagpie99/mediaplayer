using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static mediaplayer.MainPage;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace mediaplayer
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public class Record
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }  
    }
    public sealed partial class History : Page
    {
        private ViewModel vModel = new ViewModel();

        public class ViewModel
        {
            //public ObservableCollection<string> audioTracks { get; set; } = new ObservableCollection<string>(); //{ new Obiekt { Nazwa = "a" }, new Obiekt { Nazwa = "b" } };
            public ObservableCollection<Record> ListenedSongs { get; set; } = new ObservableCollection<Record>(); //{ new Obiekt { Nazwa = "a" }, new Obiekt { Nazwa = "b" } };


        }
        public History()
        {
            this.InitializeComponent();
            for(int i = 0; i < Listened.historiaOdtwarzania.Count(); i++)
            {
                vModel.ListenedSongs.Add(new Record { Name = Listened.historiaOdtwarzania[i], Date = Listened.kiedyOdtwarzane[i] });
            }
            listView.ItemsSource = vModel.ListenedSongs;

        }

    }
}
