using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mediaplayer
{
    public class Playlist
    {
        public string PlaylistId = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string ImagePath { get; set; }
        List<MySong> mySongs { get; set; } = new List<MySong>();

        public Playlist()
        {
        }

        public Playlist(string name, string imagePath)
        {
            this.Name = name;
            this.ImagePath = imagePath;
        }
    }
}
