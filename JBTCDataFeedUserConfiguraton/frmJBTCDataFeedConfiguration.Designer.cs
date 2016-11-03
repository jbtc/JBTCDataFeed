namespace JBTCDataFeedUserConfiguraton
{
    partial class frmJBTCDataFeedConfiguration
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmJBTCDataFeedConfiguration));
            this.cmdEditSubscribedUsers = new System.Windows.Forms.Button();
            this.cmdEditDataFeedTags = new System.Windows.Forms.Button();
            this.cmdEditDataFeedTagGroups = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cmdEditSubscribedUsers
            // 
            this.cmdEditSubscribedUsers.Location = new System.Drawing.Point(13, 13);
            this.cmdEditSubscribedUsers.Name = "cmdEditSubscribedUsers";
            this.cmdEditSubscribedUsers.Size = new System.Drawing.Size(171, 23);
            this.cmdEditSubscribedUsers.TabIndex = 0;
            this.cmdEditSubscribedUsers.Text = "Edit Subscribed Users";
            this.cmdEditSubscribedUsers.UseVisualStyleBackColor = true;
            this.cmdEditSubscribedUsers.Click += new System.EventHandler(this.cmdEditSubscribedUsers_Click);
            // 
            // cmdEditDataFeedTags
            // 
            this.cmdEditDataFeedTags.Location = new System.Drawing.Point(13, 43);
            this.cmdEditDataFeedTags.Name = "cmdEditDataFeedTags";
            this.cmdEditDataFeedTags.Size = new System.Drawing.Size(171, 23);
            this.cmdEditDataFeedTags.TabIndex = 1;
            this.cmdEditDataFeedTags.Text = "Edit Data Feed Tags";
            this.cmdEditDataFeedTags.UseVisualStyleBackColor = true;
            this.cmdEditDataFeedTags.Click += new System.EventHandler(this.cmdEditDataFeedTags_Click);
            // 
            // cmdEditDataFeedTagGroups
            // 
            this.cmdEditDataFeedTagGroups.Location = new System.Drawing.Point(13, 73);
            this.cmdEditDataFeedTagGroups.Name = "cmdEditDataFeedTagGroups";
            this.cmdEditDataFeedTagGroups.Size = new System.Drawing.Size(171, 23);
            this.cmdEditDataFeedTagGroups.TabIndex = 2;
            this.cmdEditDataFeedTagGroups.Text = "Edit Data Feed Tag Groups";
            this.cmdEditDataFeedTagGroups.UseVisualStyleBackColor = true;
            this.cmdEditDataFeedTagGroups.Click += new System.EventHandler(this.cmdEditDataFeedTagGroups_Click);
            // 
            // frmJBTCDataFeedConfiguration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 562);
            this.Controls.Add(this.cmdEditDataFeedTagGroups);
            this.Controls.Add(this.cmdEditDataFeedTags);
            this.Controls.Add(this.cmdEditSubscribedUsers);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmJBTCDataFeedConfiguration";
            this.Text = "JBTCDataFeedConfiguration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmJBTCDataFeedConfiguration_FormClosing);
            this.Load += new System.EventHandler(this.frmJBTCDataFeedConfiguration_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button cmdEditSubscribedUsers;
        private System.Windows.Forms.Button cmdEditDataFeedTags;
        private System.Windows.Forms.Button cmdEditDataFeedTagGroups;
    }
}

