using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace mediaplayer
{
    public class MySong
    {
        public Guid Id = Guid.NewGuid();    
        public string Name { get; set; }
        public string PathName { get; set; }
        public string ImagePath { get; set; }
        public string playlistId;

        public MySong() { 
        }

        public MySong(string name, string pathName, string imagePath, string playlistId)
        {
            this.Name = name;
            this.PathName = pathName;
            this.ImagePath = imagePath;
            this.playlistId = playlistId;
        }

    }
}
