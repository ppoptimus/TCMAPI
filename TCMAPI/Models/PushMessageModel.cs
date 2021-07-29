using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCMAPI.Models
{
    public class PushMessageModel
    {
        public List<string> to { get; set; }
        public List<Message> messages { get; set; }

        public class Message
        {
            public string type { get; set; }
            public string text { get; set; }
        }
    }
}
