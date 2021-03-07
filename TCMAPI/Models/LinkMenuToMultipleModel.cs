using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCMAPI.Models
{
    public class LinkMenuToMultipleModel
    {
        public string richMenuId { get; set; }
        public List<string> userIds { get; set; }
    }
}
