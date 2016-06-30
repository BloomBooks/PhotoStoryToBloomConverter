namespace PhotoStoryToBloomConverter
{
    partial class BatchConversionDlg
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
			this.label1 = new System.Windows.Forms.Label();
			this.selectDirectoryButton = new System.Windows.Forms.Button();
			this.convertProjectButton = new System.Windows.Forms.Button();
			this.directoryTextBox = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(487, 31);
			this.label1.TabIndex = 0;
			this.label1.Text = "Select a directory which contains both .wp3 files and .docx files. The correspond" +
    "ing files should start with the same numeric code (e.g. \'001 Creation.wp3\' and \'" +
    "001 Creation Text.docx\').";
			// 
			// selectDirectoryButton
			// 
			this.selectDirectoryButton.Location = new System.Drawing.Point(424, 73);
			this.selectDirectoryButton.Name = "selectDirectoryButton";
			this.selectDirectoryButton.Size = new System.Drawing.Size(75, 23);
			this.selectDirectoryButton.TabIndex = 3;
			this.selectDirectoryButton.Text = "Select";
			this.selectDirectoryButton.UseVisualStyleBackColor = true;
			this.selectDirectoryButton.Click += new System.EventHandler(this.selectDirectoryButton_Click);
			// 
			// convertProjectButton
			// 
			this.convertProjectButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.convertProjectButton.Location = new System.Drawing.Point(424, 113);
			this.convertProjectButton.Name = "convertProjectButton";
			this.convertProjectButton.Size = new System.Drawing.Size(75, 23);
			this.convertProjectButton.TabIndex = 6;
			this.convertProjectButton.Text = "Convert";
			this.convertProjectButton.UseVisualStyleBackColor = true;
			this.convertProjectButton.Click += new System.EventHandler(this.convertButton_Click);
			// 
			// directoryTextBox
			// 
			this.directoryTextBox.Enabled = false;
			this.directoryTextBox.Location = new System.Drawing.Point(15, 75);
			this.directoryTextBox.Name = "directoryTextBox";
			this.directoryTextBox.Size = new System.Drawing.Size(403, 20);
			this.directoryTextBox.TabIndex = 7;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 52);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(487, 20);
			this.label2.TabIndex = 8;
			this.label2.Text = "NOTE: The converter can only handle .docx files.";
			// 
			// BatchConversionDlg
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(511, 151);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.directoryTextBox);
			this.Controls.Add(this.convertProjectButton);
			this.Controls.Add(this.selectDirectoryButton);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BatchConversionDlg";
			this.Text = "Photo Story to Bloom Converter - Batch";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button selectDirectoryButton;
        private System.Windows.Forms.Button convertProjectButton;
		private System.Windows.Forms.TextBox directoryTextBox;
		private System.Windows.Forms.Label label2;
    }
}