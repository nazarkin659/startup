using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasBuddy.Model
{
    [Table("ContactInfo")]
    public class ContactInfo 
    {
        [Key]
        [Required]
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Unit { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Email { get; set; }

        public List<int> UserIDs { get; set; }
    }
}
