using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gurock.SmartInspect;
using HelperFunctions;
using System.Net;
using GasBuddy.Model;
using System.Data.Entity.Core.Objects;

namespace GasBuddy
{
    class Program : BaseClass
    {
        static void Main(string[] args)
        {
            try
            {
                List<User> users = new List<User>
                {
                    new User{UserName = "nazarkin659", Password="Hakers659", UserID=0001},
                    new User{UserName = "gasbuddy659", Password = "Hakers659",UserID=0002}
                };

                User u = users[0];
                User u2 = users[1];
                Authorization.LoginMobile(ref u);
                Authorization.LoginWebsite(ref u2);
                try
                {
                    AddUser(u);
                    AddUser(u2);



                    //ContactInfo contactInfo = new ContactInfo();
                    //contactInfo.Address = "4049 N Kedvale Ave, Chicago IL";
                    //contactInfo.Email = "nazarkin659@gmail.com";
                    //contactInfo.FirstName = "Nazar";
                    //contactInfo.LastName = "Petriv";
                    //contactInfo.Unit = "APT 48";
                    //contactInfo.ZipCode = "60641";
                    //contactInfo.State = "IL";

                    //dbContext.ContactInfo.Add(contactInfo);

                    //dbContext.SaveChanges();


                    //dbContext.Users.Attach(attemptGetUser);

                    //dbContext.Entry(attemptGetUser).State = System.Data.Entity.EntityState.Modified;

                }
                catch (Exception e)
                {


                }

                try
                {
                    Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
                    settings.TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Full;
                    settings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.All;

                    u.Cookies = new System.Net.CookieCollection();

                    string str = Newtonsoft.Json.JsonConvert.SerializeObject(u, typeof(User), settings);
                    User us = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(str);
                }
                catch (Exception e)
                {

                }



                for (int i = 0; i < users.Count; i++)
                {
                    try
                    {
                        User user = users[1];
                        Authorization.LoginWebsite(ref user);
                        CommonAction c = new CommonAction();
                        List<string> stations = c.GetStations("60641");

                        foreach (string station in stations)
                        {

                            bool ok = c.SuccessReportPrice(station, user);
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }

                //test = WebSpider.SpiderUse.GetResponse("http://www.chicagogasprices.com/", false, users[1].Cookies);
            }
            catch (Exception e)
            {
                SiAuto.Main.LogException(e);
            }
        }

        public static bool AddUser(List<User> users)
        {
            try
            {
                if (!users.IsNullOrEmpty())
                {
                    using (var db = new GasBuddy.Infrastructure.BaseDbContext())
                    {
                        db.Users.AddRange(users);
                        db.SaveChanges();
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                SiAuto.Main.LogException(e);
            }
            return false;
        }

        public static bool AddUser(User user)
        {
            return AddUser(new List<User> { user });
        }

        public static bool UpdateUser(List<User> users)
        {
            try
            {
                if (!users.IsNullOrEmpty())
                {
                    using (var db = new GasBuddy.Infrastructure.BaseDbContext())
                    {
                        foreach (User user in users)
                        {
                            db.Users.Attach(user);
                            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                        }

                        db.SaveChanges();
                    }

                    return true;
                }
            }
            catch (Exception e)
            {
                SiAuto.Main.LogException(e);
            }
            return false;
        }
        public static bool UpdateUser(User user)
        {
            return UpdateUser(new List<User> { user });
        }
    }
}
