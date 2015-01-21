using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HelperFunctions;
using WebSpider;
using GasBuddy;
using Blablabla;

namespace TestApp
{
    class Program
    {
        private static Random random = new Random((int)DateTime.Now.Ticks);
        static void Main(string[] args)
        {
            GetNextRunTime(DateTime.Now);

            //Nazar.TestSP();
            //new GasBuddy.Service.Service1().OnStart();
        }

        private static double GetNextRunTime(DateTime datetime)
        {
            if (datetime != null)
            {
                double r = (24 * 60 * 60) - (datetime.TimeOfDay.TotalSeconds);

                int runBetweenStartSeconds = 0;
                int runBetweenStopSeconds = 4 * 60 * 60;
                double toAdd = random.Next(runBetweenStartSeconds, runBetweenStopSeconds); //actual runtime

                double actualStartTime = r + toAdd;

                var test = new TimeSpan(0, 0, (int)(DateTime.Now.TimeOfDay.TotalSeconds + actualStartTime));
                //var t = datetime.TimeOfDay.TotalSeconds + (new DateTime(new TimeSpan(0, 0, 0, r))).TimeOfDay;

                var a = datetime.Subtract(new TimeSpan(0, 0, (int)actualStartTime)).Hour;

                return actualStartTime;
            }

            return 0;
        }
    }
}
