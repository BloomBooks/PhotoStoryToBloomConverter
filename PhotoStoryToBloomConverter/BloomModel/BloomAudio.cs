namespace PhotoStoryToBloomConverter.BloomModel
{
    public class BloomAudio
    {
        public const string kAudioDirectory = "audio";

        public string NarrationPath;
        public string BackgroundAudioPath;
        public double BackgroundVolume;

        public BloomAudio(string narrationPath, string backgroundAudioPath, double backgroundVolume)
        {
            NarrationPath = narrationPath;
            BackgroundAudioPath = backgroundAudioPath;
            BackgroundVolume = backgroundVolume;
        }
    }
}