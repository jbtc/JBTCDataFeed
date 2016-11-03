namespace JBTCDataFeedUserConfiguraton
{
    partial class frmEditDataFeedTags
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmEditDataFeedTags));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.cmdInsert = new System.Windows.Forms.Button();
            this.cmdDelete = new System.Windows.Forms.Button();
            this.cmdUpdate = new System.Windows.Forms.Button();
            this.txtOwner = new System.Windows.Forms.TextBox();
            this.txtSite = new System.Windows.Forms.TextBox();
            this.txtTagGroup = new System.Windows.Forms.TextBox();
            this.txtTag = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmdLoadCsv = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.cmdExportDataToCSV = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 150);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.Size = new System.Drawing.Size(686, 179);
            this.dataGridView1.TabIndex = 2;
            this.dataGridView1.RowHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridView1_RowHeaderMouseClick);
            // 
            // cmdInsert
            // 
            this.cmdInsert.Location = new System.Drawing.Point(11, 121);
            this.cmdInsert.Name = "cmdInsert";
            this.cmdInsert.Size = new System.Drawing.Size(109, 23);
            this.cmdInsert.TabIndex = 3;
            this.cmdInsert.Text = "Insert";
            this.cmdInsert.UseVisualStyleBackColor = true;
            this.cmdInsert.Click += new System.EventHandler(this.cmdInsert_Click);
            // 
            // cmdDelete
            // 
            this.cmdDelete.Location = new System.Drawing.Point(588, 121);
            this.cmdDelete.Name = "cmdDelete";
            this.cmdDelete.Size = new System.Drawing.Size(109, 23);
            this.cmdDelete.TabIndex = 4;
            this.cmdDelete.Text = "Delete";
            this.cmdDelete.UseVisualStyleBackColor = true;
            this.cmdDelete.Click += new System.EventHandler(this.cmdDelete_Click);
            // 
            // cmdUpdate
            // 
            this.cmdUpdate.Location = new System.Drawing.Point(308, 121);
            this.cmdUpdate.Name = "cmdUpdate";
            this.cmdUpdate.Size = new System.Drawing.Size(109, 23);
            this.cmdUpdate.TabIndex = 5;
            this.cmdUpdate.Text = "Update";
            this.cmdUpdate.UseVisualStyleBackColor = true;
            this.cmdUpdate.Click += new System.EventHandler(this.cmdUpdate_Click);
            // 
            // txtOwner
            // 
            this.txtOwner.Location = new System.Drawing.Point(84, 10);
            this.txtOwner.Name = "txtOwner";
            this.txtOwner.Size = new System.Drawing.Size(613, 20);
            this.txtOwner.TabIndex = 6;
            // 
            // txtSite
            // 
            this.txtSite.Location = new System.Drawing.Point(84, 36);
            this.txtSite.Name = "txtSite";
            this.txtSite.Size = new System.Drawing.Size(613, 20);
            this.txtSite.TabIndex = 7;
            // 
            // txtTagGroup
            // 
            this.txtTagGroup.Location = new System.Drawing.Point(84, 62);
            this.txtTagGroup.Name = "txtTagGroup";
            this.txtTagGroup.Size = new System.Drawing.Size(613, 20);
            this.txtTagGroup.TabIndex = 8;
            // 
            // txtTag
            // 
            this.txtTag.Location = new System.Drawing.Point(84, 88);
            this.txtTag.Name = "txtTag";
            this.txtTag.Size = new System.Drawing.Size(613, 20);
            this.txtTag.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Owner";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(25, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Site";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(16, 65);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 12;
            this.label3.Text = "Tag Group";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 91);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "OPC Tag";
            // 
            // cmdLoadCsv
            // 
            this.cmdLoadCsv.Location = new System.Drawing.Point(11, 336);
            this.cmdLoadCsv.Name = "cmdLoadCsv";
            this.cmdLoadCsv.Size = new System.Drawing.Size(686, 23);
            this.cmdLoadCsv.TabIndex = 14;
            this.cmdLoadCsv.Text = "Load CSV";
            this.cmdLoadCsv.UseVisualStyleBackColor = true;
            this.cmdLoadCsv.Click += new System.EventHandler(this.cmdLoadCsv_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // cmdExportDataToCSV
            // 
            this.cmdExportDataToCSV.Location = new System.Drawing.Point(12, 366);
            this.cmdExportDataToCSV.Name = "cmdExportDataToCSV";
            this.cmdExportDataToCSV.Size = new System.Drawing.Size(685, 23);
            this.cmdExportDataToCSV.TabIndex = 15;
            this.cmdExportDataToCSV.Text = "Export Data To CSV";
            this.cmdExportDataToCSV.UseVisualStyleBackColor = true;
            this.cmdExportDataToCSV.Click += new System.EventHandler(this.cmdExportDataToCSV_Click);
            // 
            // frmEditDataFeedTags
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(709, 399);
            this.Controls.Add(this.cmdExportDataToCSV);
            this.Controls.Add(this.cmdLoadCsv);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtTag);
            this.Controls.Add(this.txtTagGroup);
            this.Controls.Add(this.txtSite);
            this.Controls.Add(this.txtOwner);
            this.Controls.Add(this.cmdUpdate);
            this.Controls.Add(this.cmdDelete);
            this.Controls.Add(this.cmdInsert);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmEditDataFeedTags";
            this.Text = "Edit Data Feed Tags";
            this.Load += new System.EventHandler(this.frmEditDataFeedTags_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button cmdInsert;
        private System.Windows.Forms.Button cmdDelete;
        private System.Windows.Forms.Button cmdUpdate;
        private System.Windows.Forms.TextBox txtOwner;
        private System.Windows.Forms.TextBox txtSite;
        private System.Windows.Forms.TextBox txtTagGroup;
        private System.Windows.Forms.TextBox txtTag;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button cmdLoadCsv;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button cmdExportDataToCSV;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
    }
}