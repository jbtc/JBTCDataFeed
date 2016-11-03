namespace JBTCDataFeedUserConfiguraton
{
    partial class frmEditDataFeedTagGroups
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditDataFeedTagGroups));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOwner = new System.Windows.Forms.TextBox();
            this.txtSite = new System.Windows.Forms.TextBox();
            this.txtTagGroup = new System.Windows.Forms.TextBox();
            this.cmdInsert = new System.Windows.Forms.Button();
            this.cmdUpdate = new System.Windows.Forms.Button();
            this.cmdDelete = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.cmdLoadCsv = new System.Windows.Forms.Button();
            this.cmdExportDataToCSV = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.label4 = new System.Windows.Forms.Label();
            this.txtEnabled = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Owner";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Site";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Tag Group";
            // 
            // txtOwner
            // 
            this.txtOwner.Location = new System.Drawing.Point(84, 10);
            this.txtOwner.Name = "txtOwner";
            this.txtOwner.Size = new System.Drawing.Size(613, 20);
            this.txtOwner.TabIndex = 3;
            // 
            // txtSite
            // 
            this.txtSite.Location = new System.Drawing.Point(84, 36);
            this.txtSite.Name = "txtSite";
            this.txtSite.Size = new System.Drawing.Size(613, 20);
            this.txtSite.TabIndex = 4;
            // 
            // txtTagGroup
            // 
            this.txtTagGroup.Location = new System.Drawing.Point(84, 62);
            this.txtTagGroup.Name = "txtTagGroup";
            this.txtTagGroup.Size = new System.Drawing.Size(613, 20);
            this.txtTagGroup.TabIndex = 5;
            // 
            // cmdInsert
            // 
            this.cmdInsert.Location = new System.Drawing.Point(19, 120);
            this.cmdInsert.Name = "cmdInsert";
            this.cmdInsert.Size = new System.Drawing.Size(111, 23);
            this.cmdInsert.TabIndex = 6;
            this.cmdInsert.Text = "Insert";
            this.cmdInsert.UseVisualStyleBackColor = true;
            this.cmdInsert.Click += new System.EventHandler(this.cmdInsert_Click);
            // 
            // cmdUpdate
            // 
            this.cmdUpdate.Location = new System.Drawing.Point(309, 120);
            this.cmdUpdate.Name = "cmdUpdate";
            this.cmdUpdate.Size = new System.Drawing.Size(111, 23);
            this.cmdUpdate.TabIndex = 7;
            this.cmdUpdate.Text = "Update";
            this.cmdUpdate.UseVisualStyleBackColor = true;
            this.cmdUpdate.Click += new System.EventHandler(this.cmdUpdate_Click);
            // 
            // cmdDelete
            // 
            this.cmdDelete.Location = new System.Drawing.Point(586, 120);
            this.cmdDelete.Name = "cmdDelete";
            this.cmdDelete.Size = new System.Drawing.Size(111, 23);
            this.cmdDelete.TabIndex = 8;
            this.cmdDelete.Text = "Delete";
            this.cmdDelete.UseVisualStyleBackColor = true;
            this.cmdDelete.Click += new System.EventHandler(this.cmdDelete_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(19, 164);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(678, 189);
            this.dataGridView1.TabIndex = 9;
            this.dataGridView1.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_RowHeaderMouseClick);
            // 
            // cmdLoadCsv
            // 
            this.cmdLoadCsv.Location = new System.Drawing.Point(11, 358);
            this.cmdLoadCsv.Name = "cmdLoadCsv";
            this.cmdLoadCsv.Size = new System.Drawing.Size(678, 23);
            this.cmdLoadCsv.TabIndex = 10;
            this.cmdLoadCsv.Text = "Load CSV";
            this.cmdLoadCsv.UseVisualStyleBackColor = true;
            this.cmdLoadCsv.Click += new System.EventHandler(this.cmdLoadCsv_Click);
            // 
            // cmdExportDataToCSV
            // 
            this.cmdExportDataToCSV.Location = new System.Drawing.Point(12, 388);
            this.cmdExportDataToCSV.Name = "cmdExportDataToCSV";
            this.cmdExportDataToCSV.Size = new System.Drawing.Size(678, 23);
            this.cmdExportDataToCSV.TabIndex = 11;
            this.cmdExportDataToCSV.Text = "Export Data To CSV";
            this.cmdExportDataToCSV.UseVisualStyleBackColor = true;
            this.cmdExportDataToCSV.Click += new System.EventHandler(this.cmdExportDataToCSV_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Enabled";
            // 
            // txtEnabled
            // 
            this.txtEnabled.Location = new System.Drawing.Point(84, 90);
            this.txtEnabled.Name = "txtEnabled";
            this.txtEnabled.Size = new System.Drawing.Size(613, 20);
            this.txtEnabled.TabIndex = 13;
            // 
            // frmEditDataFeedTagGroups
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 419);
            this.Controls.Add(this.txtEnabled);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmdExportDataToCSV);
            this.Controls.Add(this.cmdLoadCsv);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.cmdDelete);
            this.Controls.Add(this.cmdUpdate);
            this.Controls.Add(this.cmdInsert);
            this.Controls.Add(this.txtTagGroup);
            this.Controls.Add(this.txtSite);
            this.Controls.Add(this.txtOwner);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmEditDataFeedTagGroups";
            this.Text = "Edit Data Feed Tag Groups";
            this.Load += new System.EventHandler(this.frmEditDataFeedTagGroups_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOwner;
        private System.Windows.Forms.TextBox txtSite;
        private System.Windows.Forms.TextBox txtTagGroup;
        private System.Windows.Forms.Button cmdInsert;
        private System.Windows.Forms.Button cmdUpdate;
        private System.Windows.Forms.Button cmdDelete;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button cmdLoadCsv;
        private System.Windows.Forms.Button cmdExportDataToCSV;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtEnabled;
    }
}