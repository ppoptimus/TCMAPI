using System.Collections.Generic;

namespace TCMAPI.Models
{
    public class BroadcastMessageModel
    {
        public List<Message> messages { get; set; }
        public class Message
        {
            public string type { get; set; }
            public string text { get; set; }
        }
    }
}
