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
                Authorization authorization = new Authorization();
                authorization.Login();

            }
            catch (Exception e)
            {
                SiAuto.Main.LogException(e);
            }
        }
    }
}
