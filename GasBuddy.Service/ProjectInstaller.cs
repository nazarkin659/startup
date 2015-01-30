using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace GasBuddy.Service
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        private void Main_AfterInstall(object sender, InstallEventArgs e)
        {
            using (ServiceController sc = new ServiceController("GasBuddy.Main"))
            {
                sc.Start();
            }
        }

        private void Prepare_AfterInstall(object sender, InstallEventArgs e)
        {
            using (ServiceController sc = new ServiceController("GasBuddy.Prepare"))
            {
                sc.Start();
            }
        }
    }
}
