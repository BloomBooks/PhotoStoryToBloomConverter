# PhotoStoryToBloomTools

Tools to convert the Photo Story 3 format into a Bloom book format

## How to Use

### Conversion from Photo Story 3 to Bloom

You will need a copy of Bloom to perform the conversion. Install it from [bloomlibrary.org/installers](https://bloomlibrary.org/installers).

Run

```
PhotoStoryToBloomConverter.exe -h
```

to see usage details.

### Converting from .doc to .docx (if needed)

We have included a Word macro which can be used to convert from .doc format to .docx format. The tool requires the Word documents containing the text to be .docx files.

See the macro code and instructions in BatchDoctoDocxMacro.txt.

This macro is for mass conversion. A user can also convert files one at a time by opening the files and saving them as .docx.

## Development

### Building

This project is built in Visual Studio to run on Windows.

There is a dependency on libpalaso which can be downloaded from build.palaso.org. Run `build/getDependencies-windows.sh`.

Open the PhotoStoryToBloomTools.sln file in Visual Studio.

### Releases

1. Commit and push the code to `master`.
2. Commit and push a new tag, e.g. `v1.0.17`.
3. Update the version number in `CABExtracter/Properties/AssemblyInfo.cs` and `PhotoStoryToBloomConverter/Properties/AssemblyInfo.cs`.
4. In VS, in the Installer properties, update the version number.
5. In VS, build Release.
6. In VS, right-click Installer, "Build".
7. The installer output is `Installer/Release/PhotoStoryToBloomConverterInstaller.msi`.
8. Upload for IMS to [their Drive](https://drive.google.com/drive/u/0/folders/1woEgOdg3vOPv5mPx7UXOIxn_mujGfnv0).

## About Bloom

[Bloom Website](https://bloomlibrary.org)

On [github.com](https://github.com/BloomBooks/BloomDesktop)
