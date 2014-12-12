using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gurock.SmartInspect;
using HelperFunctions;
using GasDuddy.Database.Model;

namespace GasDuddy
{
    class Program : BaseClass
    {
        static void Main(string[] args)
        {
            try
            {
                List<User> users = new List<User>
                {
                    new User{UserName = "nazarkin659", Password="Hakers659", UserID="0001"},
                    new User{UserName = "gasbuddy659", Password = "Hakers659",UserID="0002"}
                };

                User u = users[0];
                //Authorization.LoginMobile(ref u);


                using (var db = new GasDuddy.Database.Model.Model1())
                {
                    var a = db.Users.First();

                    foreach (var ve in db.GetValidationErrors())
                    {
                        var bla = "";

                    }
                    try
                    {
                        db.Users.Add(u);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        SiAuto.Main.LogException(e);
                    }

                }


                Model1 m = new Model1();

                //var a1 = JSONHelper.ToJSON(u, typeof(User));

                //var a2 = JSONHelper.FromJSON<User>(a1);


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
    }
}
