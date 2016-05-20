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
            this.label2.Location = new System.Drawing.Point(12, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(71, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Project Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 67);
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
            this.selectCollectionButton.Location = new System.Drawing.Point(424, 81);
            this.selectCollectionButton.Name = "selectCollectionButton";
            this.selectCollectionButton.Size = new System.Drawing.Size(75, 23);
            this.selectCollectionButton.TabIndex = 5;
            this.selectCollectionButton.Text = "Select";
            this.selectCollectionButton.UseVisualStyleBackColor = true;
            this.selectCollectionButton.Click += new System.EventHandler(this.selectCollectionButton_Click);
            // 
            // convertProjectButton
            // 
            this.convertProjectButton.Location = new System.Drawing.Point(424, 131);
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
            this.bloomCollectionTextBox.Location = new System.Drawing.Point(15, 83);
            this.bloomCollectionTextBox.Name = "bloomCollectionTextBox";
            this.bloomCollectionTextBox.Size = new System.Drawing.Size(403, 20);
            this.bloomCollectionTextBox.TabIndex = 8;
            // 
            // projectNameTextBox
            // 
            this.projectNameTextBox.Location = new System.Drawing.Point(15, 133);
            this.projectNameTextBox.Name = "projectNameTextBox";
            this.projectNameTextBox.Size = new System.Drawing.Size(403, 20);
            this.projectNameTextBox.TabIndex = 9;
            // 
            // MainScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 181);
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
            this.Load += new System.EventHandler(this.MainScreen_Load);
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
    }
}