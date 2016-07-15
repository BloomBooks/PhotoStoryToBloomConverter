# PhotoStoryToBloomTools

Tools to help convert the Photo Story 3 format into a bloom book format

## Building

This project is built in Visual Studio to run on Windows. 

There is a dependency on libpalaso which can be downloaded from build.palaso.org. Run build/getDependencies-windows.sh.

Open the PhotoStoryToBloomTools.sln file and build the two projects. Two executable files will be created, CABExtracter and PhotoStoryToBloomConverter. They will be at CABExtracter/output/Debug/CABExtracter.exe and PhotoStoryToBloomConverter/output/Debug/PhotoStoryToBloomConverter.exe respectively.

## Conversion

If the project is in .wp3 format, you must first extract the files from the .wp3 file into a folder using the CABExtracter.exe file. (Alternatively, just extract the files from Windows Explorer) 

    ./path/to/extracter /path/to/wp3 /path/to/outputfolder

This will create a folder with all the image and audio assets, along with an xml file called project.xml.

The next prerequisite is to install Bloom and create a collection. Download from http://bloomlibrary.org/installers. Then open bloom and fill out the necessary information to create a collection.

Next, run the PhotoStoryToBloomConverter command line tool.

    ./path/to/converter projectXmlPath bloomCollectionPath bloomAppPath [-f] [-pn projectName] [-t narrativeDocxPath]

## Batch Conversion

Running the converter for a batch of projects is simple. To use the tool in batch mode, with all your .wp3 and .docx files in the same directory.

    ./path/to/converter -b batchDirectoryPath bloomAppPath [-f]
