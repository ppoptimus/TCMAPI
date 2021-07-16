using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCMAPI.Models
{
    public class AppSettingModel
    {
        public string GetRichMenuListUrl { get; set; }
        public string GetDefaultRichMenuIDUrl { get; set; }
        public string CreateRichMenuUrl { get; set; }
        public string LineChannelSecret { get; set; }
        public string LineChannelAccessToken { get; set; }
        public string UploadImageUrl { get; set; }
        public string SetDefaultMenu { get; set; }
        public string LinkRichMenuToUser { get; set; }
        public string LinkRichmenuToMultipleUser { get; set; }
        public string DeleteRichMenu { get; set; }
        public string PushFlexMessageUrl { get; set; }
        public string BroadcastMessageUrl { get; set; }
    }
}
