using System;
using System.Windows.Forms;

namespace PhotoStoryToBloomConverter
{
    public partial class BatchConversionDlg : Form
    {
        public BatchConversionDlg()
        {
            InitializeComponent();
        }

        private void selectDirectoryButton_Click(object sender, EventArgs e)
        {
	        using (var folderDialog = new FolderBrowserDialog{ RootFolder = Environment.SpecialFolder.MyDocuments })
	        {
		        if (folderDialog.ShowDialog(this) != DialogResult.OK)
			        return;

				var directoryPath = folderDialog.SelectedPath;
				directoryTextBox.Text = directoryPath;
	        }
        }
        
        private void selectBloomExePathButton_Clicked(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Filter = "Bloom Executable (*.exe)|*.exe",
                RestoreDirectory = true
            };

            if (fileDialog.ShowDialog() != DialogResult.OK) return;

            bloomExePathTextBox.Text = fileDialog.FileName;
        }

        private void convertButton_Click(object sender, EventArgs e)
        {
			var directoryPath = directoryTextBox.Text;
	        try
	        {
		        Program.BatchConvert(directoryPath, bloomExePathTextBox.Text);
	        }
	        catch (ArgumentException ae)
	        {
		        MessageBox.Show(ae.Message);
	        }
        }

    }
}
