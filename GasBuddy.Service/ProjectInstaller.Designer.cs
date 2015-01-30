namespace GasBuddy.Service
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            this.Prepare = new System.ServiceProcess.ServiceInstaller();
            this.Main = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller1
            // 
            this.serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalService;
            this.serviceProcessInstaller1.Password = null;
            this.serviceProcessInstaller1.Username = null;
            // 
            // Prepare
            // 
            this.Prepare.DisplayName = "GasBuddy.Prepare";
            this.Prepare.ServiceName = "GasBuddy.Prepare";
            this.Prepare.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.Prepare.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.Prepare_AfterInstall);
            // 
            // Main
            // 
            this.Main.DisplayName = "GasBuddy.Main";
            this.Main.ServiceName = "GasBuddy.Main";
            this.Main.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            this.Main.AfterInstall += new System.Configuration.Install.InstallEventHandler(this.Main_AfterInstall);
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller1,
            this.Prepare,
            this.Main});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
        private System.ServiceProcess.ServiceInstaller Prepare;
        private System.ServiceProcess.ServiceInstaller Main;
    }
}