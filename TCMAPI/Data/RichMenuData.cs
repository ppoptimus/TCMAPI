using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TCMAPI.Data
{
    public class Size
    {
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Bounds
    {
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Action
    {
        public string type { get; set; }
        public string uri { get; set; }
        public string text { get; set; }
    }

    public class Area
    {
        public Bounds bounds { get; set; }
        public Action action { get; set; }
    }

    public class RichMenu
    {
        public Size size { get; set; }
        public bool selected { get; set; }
        public string name { get; set; }
        public string chatBarText { get; set; }
        public List<Area> areas { get; set; }
    }


}
