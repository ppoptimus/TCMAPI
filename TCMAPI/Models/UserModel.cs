using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TCMAPI.Models
{
    public class UserModel
    {
        public int Id { get; set; }
        public string LineUserId { get; set; }
        public string AppUserId { get; set; }
        public string UserStatus { get; set; }

        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; }
    }
}
