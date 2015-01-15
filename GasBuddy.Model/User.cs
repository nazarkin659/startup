using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsQuery;
using HelperFunctions;
using Gurock.SmartInspect;

namespace GasBuddy.Model
{
    [Serializable]
    [Table("Users")]
    public partial class User
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }
        [MaxLength(24)]
        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int TodayPointsReceived { get; set; }
        public int? PrizeEntriesReported { get; set; }

        public int PrizesToReport { get; set; }

        public virtual Mobile Mobile { get; set; }
        public virtual WebSite Website { get; set; }
    }
}
