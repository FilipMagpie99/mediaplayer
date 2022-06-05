using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mediaplayer
{
    internal class Listened
    {
        public static List<string> historiaOdtwarzania { get; set; } = new List<string>();
        public static List<DateTime> kiedyOdtwarzane { get; set; } = new List<DateTime> { DateTime.Now };
    }
}
