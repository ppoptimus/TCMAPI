using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCMAPI.Models
{
    public class FlexMessageModel
    {
        public string to { get; set; }
        public List<Message> messages { get; set; }
        public class Action
        {
            public string type { get; set; }
            public string label { get; set; }
            public string uri { get; set; }
        }

        public class Hero
        {
            public string type { get; set; }
            public string url { get; set; }
            public string size { get; set; }
            public string aspectRatio { get; set; }
            public string aspectMode { get; set; }
            public Action action { get; set; }
        }

        public class Contents4
        {
            public string type { get; set; }
            public string text { get; set; }
            public string size { get; set; }
            public string style { get; set; }
        }

        public class Contents3
        {
            public string type { get; set; }
            public string text { get; set; }
            public string weight { get; set; }
            public string size { get; set; }
            public List<Contents4> contents4 { get; set; }
            public string margin { get; set; }
            public Action action { get; set; }
            public string height { get; set; }
            public string style { get; set; }
            public string gravity { get; set; }
            public string position { get; set; }
        }

        public class Body
        {
            public string type { get; set; }
            public string layout { get; set; }
            public List<Contents3> contents3 { get; set; }
        }

        public class Footer
        {
            public string type { get; set; }
            public string layout { get; set; }
            public int flex { get; set; }
            public string spacing { get; set; }
            public List<Contents3> contents3 { get; set; }
        }

        public class Contents2
        {
            public string type { get; set; }
            public string size { get; set; }
            public Hero hero { get; set; }
            public Body body { get; set; }
            public Footer footer { get; set; }
        }

        public class Contents1
        {
            public string type { get; set; }
            public List<Contents2> contents2 { get; set; }
        }

        public class Message
        {
            public string type { get; set; }
            public string altText { get; set; }
            public Contents1 contents1 { get; set; }
        }       
    }
}
