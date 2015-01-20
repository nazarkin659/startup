using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasBuddy.Model
{
    public partial class ZipcodeStations
    {
        [Required]
        [Key]
        public int ID { get; set; }

        [Required]
        public int ZipCode { get; set; }

        public string StationsURL { get; set; }
    }
}
