# PhotoStoryToBloomTools

Tools to help convert the Photo Story 3 format into a Bloom book format

## About Bloom

[Bloom Website](https://bloomlibrary.org)

On [github.com](https://github.com/BloomBooks/BloomDesktop)

## Building PhotoStoryToBloomTools

This project is built in Visual Studio to run on Windows. 

There is a dependency on libpalaso which can be downloaded from build.palaso.org. Run build/getDependencies-windows.sh.

Open the PhotoStoryToBloomTools.sln file and build the two projects. Two executable files will be created, CABExtracter and PhotoStoryToBloomConverter. They will be at CABExtracter/output/{Debug|Release}/CABExtracter.exe and PhotoStoryToBloomConverter/output/{Debug|Release}/PhotoStoryToBloomConverter.exe respectively.

## Converting from .doc to .docx (if needed)

We have included a Word macro which can be used to convert from .doc format to .docx format. The tool requires the Word documents containing the text to be .docx files. 

See the macro code and instructions in BatchDoctoDocxMacro.txt.

This macro is for mass conversion. A user can also convert files one at a time by opening the files and saving them as .docx.

## Conversion from Photo Story 3 to Bloom

You will need a copy of Bloom to perform the conversion. Install it from [bloomlibrary.org/installers](https://bloomlibrary.org/installers).

Run
```
PhotoStoryToBloomConverter.exe -h
```
to see usage details. 
