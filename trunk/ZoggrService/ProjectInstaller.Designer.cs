namespace ZoggrService
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
            this.zoggrServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.zoggrServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // zoggrServiceProcessInstaller
            // 
            this.zoggrServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.zoggrServiceProcessInstaller.Password = null;
            this.zoggrServiceProcessInstaller.Username = null;
            // 
            // zoggrServiceInstaller
            // 
            this.zoggrServiceInstaller.Description = "Updates Zoggr web site with current ReplayTV information.";
            this.zoggrServiceInstaller.DisplayName = "Zoggr Service";
            this.zoggrServiceInstaller.ServiceName = "Zoggr Updater";
            this.zoggrServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.zoggrServiceProcessInstaller,
            this.zoggrServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller zoggrServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller zoggrServiceInstaller;
    }
}