namespace JBTCDataFeedUserConfiguraton
{
    partial class frmEditSubscribedUsers
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditSubscribedUsers));
            this.cmbOwner = new System.Windows.Forms.ComboBox();
            this.lblOwner = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtSMSEmail = new System.Windows.Forms.TextBox();
            this.cmdNew = new System.Windows.Forms.Button();
            this.cmdSave = new System.Windows.Forms.Button();
            this.txtNewOwner = new System.Windows.Forms.TextBox();
            this.lblNewOwner = new System.Windows.Forms.Label();
            this.cmdClose = new System.Windows.Forms.Button();
            this.lblRid = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmbOwner
            // 
            this.cmbOwner.FormattingEnabled = true;
            this.cmbOwner.Location = new System.Drawing.Point(110, 16);
            this.cmbOwner.Name = "cmbOwner";
            this.cmbOwner.Size = new System.Drawing.Size(242, 21);
            this.cmbOwner.TabIndex = 0;
            this.cmbOwner.SelectedIndexChanged += new System.EventHandler(this.cmbOwner_SelectedIndexChanged);
            // 
            // lblOwner
            // 
            this.lblOwner.AutoSize = true;
            this.lblOwner.Location = new System.Drawing.Point(66, 19);
            this.lblOwner.Name = "lblOwner";
            this.lblOwner.Size = new System.Drawing.Size(38, 13);
            this.lblOwner.TabIndex = 1;
            this.lblOwner.Text = "Owner";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(66, 50);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Phone";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(72, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Email";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(49, 110);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "SMSEmail";
            // 
            // txtPhone
            // 
            this.txtPhone.Location = new System.Drawing.Point(110, 47);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(242, 20);
            this.txtPhone.TabIndex = 5;
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(110, 77);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(242, 20);
            this.txtEmail.TabIndex = 6;
            // 
            // txtSMSEmail
            // 
            this.txtSMSEmail.Location = new System.Drawing.Point(110, 107);
            this.txtSMSEmail.Name = "txtSMSEmail";
            this.txtSMSEmail.Size = new System.Drawing.Size(242, 20);
            this.txtSMSEmail.TabIndex = 7;
            // 
            // cmdNew
            // 
            this.cmdNew.Location = new System.Drawing.Point(12, 183);
            this.cmdNew.Name = "cmdNew";
            this.cmdNew.Size = new System.Drawing.Size(75, 23);
            this.cmdNew.TabIndex = 8;
            this.cmdNew.Text = "New";
            this.cmdNew.UseVisualStyleBackColor = true;
            this.cmdNew.Click += new System.EventHandler(this.cmdNew_Click);
            // 
            // cmdSave
            // 
            this.cmdSave.Location = new System.Drawing.Point(276, 183);
            this.cmdSave.Name = "cmdSave";
            this.cmdSave.Size = new System.Drawing.Size(75, 23);
            this.cmdSave.TabIndex = 9;
            this.cmdSave.Text = "Save";
            this.cmdSave.UseVisualStyleBackColor = true;
            this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
            // 
            // txtNewOwner
            // 
            this.txtNewOwner.Location = new System.Drawing.Point(113, 17);
            this.txtNewOwner.Name = "txtNewOwner";
            this.txtNewOwner.Size = new System.Drawing.Size(238, 20);
            this.txtNewOwner.TabIndex = 10;
            // 
            // lblNewOwner
            // 
            this.lblNewOwner.AutoSize = true;
            this.lblNewOwner.Location = new System.Drawing.Point(41, 19);
            this.lblNewOwner.Name = "lblNewOwner";
            this.lblNewOwner.Size = new System.Drawing.Size(63, 13);
            this.lblNewOwner.TabIndex = 11;
            this.lblNewOwner.Text = "New Owner";
            // 
            // cmdClose
            // 
            this.cmdClose.Location = new System.Drawing.Point(144, 183);
            this.cmdClose.Name = "cmdClose";
            this.cmdClose.Size = new System.Drawing.Size(75, 23);
            this.cmdClose.TabIndex = 12;
            this.cmdClose.Text = "Close";
            this.cmdClose.UseVisualStyleBackColor = true;
            this.cmdClose.Click += new System.EventHandler(this.cmdClose_Click);
            // 
            // lblRid
            // 
            this.lblRid.AutoSize = true;
            this.lblRid.Location = new System.Drawing.Point(9, 65);
            this.lblRid.Name = "lblRid";
            this.lblRid.Size = new System.Drawing.Size(26, 13);
            this.lblRid.TabIndex = 13;
            this.lblRid.Text = "RID";
            // 
            // frmEditSubscribedUsers
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(364, 213);
            this.Controls.Add(this.lblRid);
            this.Controls.Add(this.cmdClose);
            this.Controls.Add(this.lblNewOwner);
            this.Controls.Add(this.txtNewOwner);
            this.Controls.Add(this.cmdSave);
            this.Controls.Add(this.cmdNew);
            this.Controls.Add(this.txtSMSEmail);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lblOwner);
            this.Controls.Add(this.cmbOwner);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmEditSubscribedUsers";
            this.Text = "Edit Subscribed Users";
            this.Load += new System.EventHandler(this.frmEditSubscribedUsers_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbOwner;
        private System.Windows.Forms.Label lblOwner;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.TextBox txtSMSEmail;
        private System.Windows.Forms.Button cmdNew;
        private System.Windows.Forms.Button cmdSave;
        private System.Windows.Forms.TextBox txtNewOwner;
        private System.Windows.Forms.Label lblNewOwner;
        private System.Windows.Forms.Button cmdClose;
        private System.Windows.Forms.Label lblRid;
    }
}