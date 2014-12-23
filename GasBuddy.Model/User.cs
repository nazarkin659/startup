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
using GasBuddy.Model.ComplexTypes;

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

        //[NotMapped]
        //public bool isLoggedIn
        //{
        //    get
        //    {
        //        //if (!string.IsNullOrWhiteSpace(Mobile.CookiesHolder) && !string.IsNullOrWhiteSpace(CheckLoginURL))
        //        //{
        //        //    try
        //        //    {
        //        //        CQ response = WebSpider.SpiderUse.GetResponse(CheckLoginURL, false, Cookies);
        //        //        if (!response.IsNullOrEmpty() &&
        //        //            (!response[string.Format(".login .profile:contains('{0}')", UserName.TrimEnd())].IsNullOrEmpty() || !response[string.Format("#ctl00_TB_lblLogInStatus .MPLink:contains('{0}')", UserName.TrimEnd())].IsNullOrEmpty())
        //        //            )
        //        //        {
        //        //            return true;
        //        //        }
        //        //    }
        //        //    catch (Exception e)
        //        //    {
        //        //        SiAuto.Main.LogException(e);
        //        //    }
        //        //}
        //        return false;
        //    }
        //}

        //public string CookiesHolder { get; set; }




        public DateTime? CreatedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        public Mobile Mobile { get; set; }

        public WebSite WebSite { get; set; }

        public User()
        {
            //this.CookiesHolder = string.Empty;
            Mobile = new Mobile(this.UserName);
            WebSite = new WebSite();
        }
    }
}
