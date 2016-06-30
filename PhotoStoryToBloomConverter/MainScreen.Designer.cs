namespace PhotoStoryToBloomConverter
{
    partial class MainScreen
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
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.selectProjectXmlButton = new System.Windows.Forms.Button();
			this.selectCollectionButton = new System.Windows.Forms.Button();
			this.convertProjectButton = new System.Windows.Forms.Button();
			this.photoStoryProjectTextBox = new System.Windows.Forms.TextBox();
			this.bloomCollectionTextBox = new System.Windows.Forms.TextBox();
			this.projectNameTextBox = new System.Windows.Forms.TextBox();
			this.wordDocTextBox = new System.Windows.Forms.TextBox();
			this.selectWordDocButton = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.linkLabelBatchConversion = new System.Windows.Forms.LinkLabel();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(188, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Select the Photo Story Project Xml File";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 171);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(71, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Project Name";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(12, 121);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(136, 13);
			this.label3.TabIndex = 2;
			this.label3.Text = "Select the Bloom Collection";
			// 
			// selectProjectXmlButton
			// 
			this.selectProjectXmlButton.Location = new System.Drawing.Point(424, 35);
			this.selectProjectXmlButton.Name = "selectProjectXmlButton";
			this.selectProjectXmlButton.Size = new System.Drawing.Size(75, 23);
			this.selectProjectXmlButton.TabIndex = 3;
			this.selectProjectXmlButton.Text = "Select";
			this.selectProjectXmlButton.UseVisualStyleBackColor = true;
			this.selectProjectXmlButton.Click += new System.EventHandler(this.selectProjectXmlButton_Click);
			// 
			// selectCollectionButton
			// 
			this.selectCollectionButton.Location = new System.Drawing.Point(424, 135);
			this.selectCollectionButton.Name = "selectCollectionButton";
			this.selectCollectionButton.Size = new System.Drawing.Size(75, 23);
			this.selectCollectionButton.TabIndex = 5;
			this.selectCollectionButton.Text = "Select";
			this.selectCollectionButton.UseVisualStyleBackColor = true;
			this.selectCollectionButton.Click += new System.EventHandler(this.selectCollectionButton_Click);
			// 
			// convertProjectButton
			// 
			this.convertProjectButton.Location = new System.Drawing.Point(424, 185);
			this.convertProjectButton.Name = "convertProjectButton";
			this.convertProjectButton.Size = new System.Drawing.Size(75, 23);
			this.convertProjectButton.TabIndex = 6;
			this.convertProjectButton.Text = "Convert";
			this.convertProjectButton.UseVisualStyleBackColor = true;
			this.convertProjectButton.Click += new System.EventHandler(this.convertProjectButton_Click);
			// 
			// photoStoryProjectTextBox
			// 
			this.photoStoryProjectTextBox.Enabled = false;
			this.photoStoryProjectTextBox.Location = new System.Drawing.Point(15, 37);
			this.photoStoryProjectTextBox.Name = "photoStoryProjectTextBox";
			this.photoStoryProjectTextBox.Size = new System.Drawing.Size(403, 20);
			this.photoStoryProjectTextBox.TabIndex = 7;
			// 
			// bloomCollectionTextBox
			// 
			this.bloomCollectionTextBox.Enabled = false;
			this.bloomCollectionTextBox.Location = new System.Drawing.Point(15, 137);
			this.bloomCollectionTextBox.Name = "bloomCollectionTextBox";
			this.bloomCollectionTextBox.Size = new System.Drawing.Size(403, 20);
			this.bloomCollectionTextBox.TabIndex = 8;
			// 
			// projectNameTextBox
			// 
			this.projectNameTextBox.Location = new System.Drawing.Point(15, 187);
			this.projectNameTextBox.Name = "projectNameTextBox";
			this.projectNameTextBox.Size = new System.Drawing.Size(403, 20);
			this.projectNameTextBox.TabIndex = 9;
			// 
			// wordDocTextBox
			// 
			this.wordDocTextBox.Enabled = false;
			this.wordDocTextBox.Location = new System.Drawing.Point(15, 88);
			this.wordDocTextBox.Name = "wordDocTextBox";
			this.wordDocTextBox.Size = new System.Drawing.Size(403, 20);
			this.wordDocTextBox.TabIndex = 12;
			// 
			// selectWordDocButton
			// 
			this.selectWordDocButton.Location = new System.Drawing.Point(424, 86);
			this.selectWordDocButton.Name = "selectWordDocButton";
			this.selectWordDocButton.Size = new System.Drawing.Size(75, 23);
			this.selectWordDocButton.TabIndex = 11;
			this.selectWordDocButton.Text = "Select";
			this.selectWordDocButton.UseVisualStyleBackColor = true;
			this.selectWordDocButton.Click += new System.EventHandler(this.selectWordDocButton_Click);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(12, 72);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(312, 13);
			this.label4.TabIndex = 10;
			this.label4.Text = "Select the Word Document (.docx) Containing the Text (optional)";
			// 
			// linkLabelBatchConversion
			// 
			this.linkLabelBatchConversion.AutoSize = true;
			this.linkLabelBatchConversion.Location = new System.Drawing.Point(15, 223);
			this.linkLabelBatchConversion.Name = "linkLabelBatchConversion";
			this.linkLabelBatchConversion.Size = new System.Drawing.Size(91, 13);
			this.linkLabelBatchConversion.TabIndex = 13;
			this.linkLabelBatchConversion.TabStop = true;
			this.linkLabelBatchConversion.Text = "Batch Conversion";
			this.linkLabelBatchConversion.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
			// 
			// MainScreen
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(511, 255);
			this.Controls.Add(this.linkLabelBatchConversion);
			this.Controls.Add(this.wordDocTextBox);
			this.Controls.Add(this.selectWordDocButton);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.projectNameTextBox);
			this.Controls.Add(this.bloomCollectionTextBox);
			this.Controls.Add(this.photoStoryProjectTextBox);
			this.Controls.Add(this.convertProjectButton);
			this.Controls.Add(this.selectCollectionButton);
			this.Controls.Add(this.selectProjectXmlButton);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MainScreen";
			this.Text = "Photo Story to Bloom Converter";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button selectProjectXmlButton;
        private System.Windows.Forms.Button selectCollectionButton;
        private System.Windows.Forms.Button convertProjectButton;
        private System.Windows.Forms.TextBox photoStoryProjectTextBox;
        private System.Windows.Forms.TextBox bloomCollectionTextBox;
        private System.Windows.Forms.TextBox projectNameTextBox;
		private System.Windows.Forms.TextBox wordDocTextBox;
		private System.Windows.Forms.Button selectWordDocButton;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.LinkLabel linkLabelBatchConversion;
    }
}