using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasBuddy.Model
{
    [Table("ProcessQueueArchive")]
    public class ProcessQueueArchive
    {
        [Required]
        [Key]
        public int ID { get; set; }
        [Required]
        public int UserID { get; set; }
        [Required]
        public int Priority { get; set; }
        [Required]
        public int FailCount { get; set; }
        [Required]
        public int RetryCount { get; set; }
        [Required]
        public bool Successful { get; set; }

        [Required]
        public bool Processing { get; set; }

        [Required]
        public DateTime DateAdded { get; set; }
    }
}
