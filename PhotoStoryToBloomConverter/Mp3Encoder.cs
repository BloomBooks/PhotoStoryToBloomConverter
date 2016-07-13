using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Lame;
using NAudio.Wave;

namespace PhotoStoryToBloomConverter
{
	class Mp3Encoder
	{

		public string Encode(string sourcePath, string destPathWithoutExtension)
		{
			var outputPath = destPathWithoutExtension + "." + FormatName;
			var reader = new WaveFileReader(sourcePath);
			using (var writer = new LameMP3FileWriter(outputPath, reader.WaveFormat, LAMEPreset.VBR_50))
			{
				reader.CopyTo(writer);
			}
			return outputPath;
		}

		public string FormatName
		{
			get { return "mp3"; }
		}
	}
}
