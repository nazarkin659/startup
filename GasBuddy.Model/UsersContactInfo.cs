using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GasBuddy.Model
{
    [Table("UsersContactInfo")]
    public partial class UsersContactInfo
    {
        [Key]
        [Required]
        public int ID { get; set; }

        public int UserID { get; set; }
        [ForeignKey("UserID")]
        public virtual User User { get; set; }
        public int ContactInfoID { get; set; }
        [ForeignKey("ContactInfoID")]
        public virtual ContactInfo ContactInfo { get; set; }
    }
}
