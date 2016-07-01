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

        private void convertButton_Click(object sender, EventArgs e)
        {
			var directoryPath = directoryTextBox.Text;
	        try
	        {
		        Program.BatchConvert(directoryPath);
	        }
	        catch (ArgumentException ae)
	        {
		        MessageBox.Show(ae.Message);
	        }
        }
    }
}
