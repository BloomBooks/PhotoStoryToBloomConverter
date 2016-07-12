namespace PhotoStoryToBloomConverter.BloomModel
{
    public class BloomAudio
    {
        public const string kAudioDirectory = "audio";

        public string NarrationPath;
        public string BackgroundAudioPath;
        public double BackgroundVolume;
	    public string Duration;

        public BloomAudio(string narrationPath, string backgroundAudioPath, double backgroundVolume, string duration)
        {
            NarrationPath = narrationPath;
            BackgroundAudioPath = backgroundAudioPath;
            BackgroundVolume = backgroundVolume;
	        Duration = duration;
        }
    }
}