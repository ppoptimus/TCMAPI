using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCMAPI.Models
{
    public class RootEventMessageModel
    {
        public string destination { get; set; }
        public List<Event> events { get; set; }
    }
    public class Source
    {
        public string type { get; set; }
        public string userId { get; set; }
    }

    public class Emoji
    {
        public int index { get; set; }
        public int length { get; set; }
        public string productId { get; set; }
        public string emojiId { get; set; }
    }

    public class Mentionee
    {
        public int index { get; set; }
        public int length { get; set; }
        public string userId { get; set; }
    }

    public class Mention
    {
        public List<Mentionee> mentionees { get; set; }
    }

    public class Message
    {
        public string id { get; set; }
        public string type { get; set; }
        public string text { get; set; }
        public List<Emoji> emojis { get; set; }
        public Mention mention { get; set; }
    }

    public class Event
    {
        public string replyToken { get; set; }
        public string type { get; set; }
        public string mode { get; set; }
        public long timestamp { get; set; }
        public Source source { get; set; }
        public Message message { get; set; }
    }


}
