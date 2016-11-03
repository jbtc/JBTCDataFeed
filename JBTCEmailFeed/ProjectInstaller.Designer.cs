namespace JBTCEmailFeed
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
            this.serviceProcessInstaller3 = new System.ServiceProcess.ServiceProcessInstaller();
            this.serviceInstaller3 = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller3
            // 
            this.serviceProcessInstaller3.Password = null;
            this.serviceProcessInstaller3.Username = null;
            // 
            // serviceInstaller3
            // 
            this.serviceInstaller3.DelayedAutoStart = true;
            this.serviceInstaller3.Description = "JBTCEmailFeedService";
            this.serviceInstaller3.DisplayName = "JBTCEmailFeedService";
            this.serviceInstaller3.ServiceName = "JBTCEmailFeedService";
            this.serviceInstaller3.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller3,
            this.serviceInstaller3});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller3;
        private System.ServiceProcess.ServiceInstaller serviceInstaller3;
    }
}