# PhotoStoryToBloomTools

Tools to help convert the Photo Story 3 format into a bloom book format

## Building

This project is built in Visual Studio to run on Windows. Open the PhotoStoryToBloomTools.sln file and build the two projects. Two executable files will be created, CABExtracter and PhotoStoryToBloomConverter. They will be at CABExtracter/bin/Debug/CABExtracter.exe and PhotoStoryToBloomConverter/bin/Debug/PhotoStoryToBloomConverter.exe respectively.

## Conversion

First, extract the files from the .wp3 file into a folder using the CABExtracter.exe file. 

    ./path/to/extracter /path/to/wp3 /path/to/outputfolder

This will create a folder with all the image and audio assets, along with an xml file called project.xml.

Next, convert the project.xml into a bloom book htm file using the PhotoStoryToBloomConverter.exe file.

    ./path/to/converter /path/to/project.xml /path/to/bloomBookDirectory bookName

This will create a book that shows up in bloom. Close bloom if it is already open, then open it and see the newly converted book.

