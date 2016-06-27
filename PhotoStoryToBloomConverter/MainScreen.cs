using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using PhotoStoryToBloomConverter.PS3Model;

namespace PhotoStoryToBloomConverter
{
    public partial class MainScreen : Form
    {
        private PhotoStoryProject photoStoryProject;
	    private IList<string> extractedText; 

        public MainScreen()
        {
            InitializeComponent();
        }

        private void selectProjectXmlButton_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "xml files (*.xml)|*.xml",
                RestoreDirectory = true
            };

            if (fileDialog.ShowDialog() != DialogResult.OK) return;
            var projectPath = fileDialog.FileName;

            photoStoryProject = Ps3AndBloomSerializer.DeserializePhotoStoryXml(projectPath);

            photoStoryProjectTextBox.Text = projectPath;

            var bookName = "";
            foreach (var vunit in photoStoryProject.VisualUnits)
            {
                foreach (var edit in vunit.Image.Edits)
                {
                    if (edit.TextOverlays.Length <= 0) continue;
                    bookName = edit.TextOverlays[0].Text;
                    break;
                }
                if (bookName != "") break;
            }

            if (projectNameTextBox.Text.Length == 0 && bookName.Length > 0)
                projectNameTextBox.Text = bookName;
        }

        private void selectCollectionButton_Click(object sender, EventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "Bloom Collections (*.bloomLibrary *.bloomCollection)|*.bloomLibrary;*.bloomCollection",
                RestoreDirectory = true
            };

            if (fileDialog.ShowDialog() != DialogResult.OK) return;
            bloomCollectionTextBox.Text = fileDialog.FileName;
        }

        private void convertProjectButton_Click(object sender, EventArgs e)
        {
            var projectXmlPath = photoStoryProjectTextBox.Text;
            var bloomCollectionPath = bloomCollectionTextBox.Text;
            var projectName = projectNameTextBox.Text.Trim();
            if (projectXmlPath.Length == 0 || bloomCollectionPath.Length == 0 || projectName.Length == 0)
            {
                MessageBox.Show("Please fill out all fields");
                return;
            }
            if (!File.Exists(projectXmlPath))
            {
                MessageBox.Show("The project does not exist");
                return;
            }
            if (!File.Exists(bloomCollectionPath))
            {
                MessageBox.Show("The bloom file does not exist");
                return;
            }
            var convertedProjectDirectory = Path.Combine(Path.GetDirectoryName(bloomCollectionPath), projectName);
            if (Directory.Exists(convertedProjectDirectory))
            {
                MessageBox.Show("A book already exists with this name in that collection");
                projectNameTextBox.Text = "";
                return;
            }

            Directory.CreateDirectory(convertedProjectDirectory);

            Program.ConvertToBloom(photoStoryProject, Path.Combine(convertedProjectDirectory, string.Format("{0}.htm", projectName)), projectName, extractedText);
            Program.CopyAssetsAndResources(Path.GetDirectoryName(projectXmlPath), convertedProjectDirectory);
            Program.CopyBloomFiles(convertedProjectDirectory);

            var result = MessageBox.Show("Success! Convert another project?", "Photo Story to Bloom Converter", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                photoStoryProject = null;
                photoStoryProjectTextBox.Text = "";
                projectNameTextBox.Text = "";
            }
            else
                Close();
        }

		private void selectWordDocButton_Click(object sender, EventArgs e)
		{
			var fileDialog = new OpenFileDialog
			{
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
				Filter = ".docx files (*.docx)|*.docx",
				RestoreDirectory = true
			};

			if (fileDialog.ShowDialog() != DialogResult.OK)
				return;

			var path = fileDialog.FileName;

			IList<string> text;
			if (!TextExtractor.TryExtractText(path, out text))
			{
				MessageBox.Show("There was a problem extracting text from this file. Please try a different one or continue without text.");
				wordDocTextBox.Text = string.Empty;
				return;
			}
			extractedText = text;

			wordDocTextBox.Text = path;
		}
    }
}
