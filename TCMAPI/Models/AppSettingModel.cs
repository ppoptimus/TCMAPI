using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCMAPI.Models
{
    public class AppSettingModel
    {
        public string GetRichMenuListUrl { get; set; }
        public string LineChannelSecret { get; set; }
        public string LineChannelAccessToken { get; set; }
        public string UploadImageUrl { get; set; }
    }
}
