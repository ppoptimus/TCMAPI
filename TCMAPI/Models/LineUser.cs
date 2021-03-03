using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TCMAPI.Models
{
    [Table("LineUser")]
    public class LineUser
    {
        public int Id { get; set; }
        public string LineUserId { get; set; }
        public string UserStatus { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }

    }
}
