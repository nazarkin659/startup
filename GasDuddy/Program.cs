using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gurock.SmartInspect;

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
                    new User{UserName = "nazarkin659", Password="Hakers659"},
                    new User{UserName = "gasbuddy659", Password = "Hakers659"}
                };


                Parallel.For(0, users.Count, i =>
                {
                    try
                    {
                        User user = users[i];
                        Authorization.Login(ref user);
                    }
                    catch (Exception e)
                    {

                    }
                });

                string test = WebSpider.SpiderUse.GetResponse("http://www.chicagogasprices.com/", false, users[0].Cookies);


                for (int i = 0; i < users.Count; i++)
                {
                    try
                    {
                        User user = users[i];
                        Authorization.Login(ref user);
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
