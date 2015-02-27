using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Gurock.SmartInspect;

namespace GasBuddy.Service
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new GasBuddyMain(),
                new Service2()
            };
            ServiceBase.Run(ServicesToRun);
        }

      
    }
}
