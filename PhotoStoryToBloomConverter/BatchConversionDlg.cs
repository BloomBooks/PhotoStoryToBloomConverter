using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
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
			if (directoryPath.Length == 0 || !Directory.Exists(directoryPath))
            {
                MessageBox.Show("Please select an existing directory.");
                return;
            }

	        var outputDirectory = Path.Combine(directoryPath, "Batch Conversion Output");
			Directory.CreateDirectory(outputDirectory);

			var tempFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			Directory.CreateDirectory(tempFolder);
			foreach (var projectPath in Directory.EnumerateFiles(directoryPath, "*.wp3").Union(Directory.EnumerateFiles(directoryPath, "*.cab")))
	        {
				CABExtracter.Program.ExpandCabFile(projectPath, tempFolder);

		        var projectXmlPath = Path.Combine(tempFolder, "project.xml");

				var photoStoryProject = Ps3AndBloomSerializer.DeserializePhotoStoryXml(projectXmlPath);
		        var projectName = photoStoryProject.GetProjectName();
		        if (string.IsNullOrWhiteSpace(projectName))
			        projectName = Path.GetFileNameWithoutExtension(projectPath);
				var convertedProjectDirectory = Path.Combine(outputDirectory, projectName);
				Directory.CreateDirectory(convertedProjectDirectory);

				var matchingDocxFile = GetMatchingDocxFile(directoryPath, projectPath);
				IList<string> text = null;
		        if (matchingDocxFile != null)
			        TextExtractor.TryExtractText(matchingDocxFile, out text);

				if (text == null)
					Debug.WriteLine(string.Format("Could not extract text for {0}", projectName));

		        Program.ConvertToBloom(photoStoryProject, Path.Combine(convertedProjectDirectory, string.Format("{0}.htm", projectName)), projectName, text);
				Program.CopyAssetsAndResources(Path.GetDirectoryName(projectXmlPath), convertedProjectDirectory);
				Program.CopyBloomFiles(convertedProjectDirectory);

				DeleteAllFilesAndFoldersInDirectory(tempFolder);
	        }

			Directory.Delete(tempFolder);
        }

	    private void DeleteAllFilesAndFoldersInDirectory(string directoryPath)
	    {
			foreach (FileInfo file in new DirectoryInfo(directoryPath).GetFiles())
				file.Delete();
	    }

	    private string GetMatchingDocxFile(string directoryPath, string wp3FilePath)
	    {
		    var wp3Name = Path.GetFileNameWithoutExtension(wp3FilePath);
		    var wp3NameCode = wp3Name.Split(' ')[0];

			var directoryInfo = new DirectoryInfo(directoryPath);
		    var matchingFiles = directoryInfo.GetFiles(wp3NameCode + "*.docx");
		    if (matchingFiles.Length == 1)
			    return matchingFiles[0].FullName;
		    return null;
	    }
    }
}
