namespace PhotoStoryToBloomConverter.BloomModel
{
    public class BloomAudio
    {
        public const string kAudioDirectory = "audio";

        public string NarrationPath;
        public string BackgroundAudioPath;

        public BloomAudio(string narrationPath, string backgroundAudioPath)
        {
            NarrationPath = narrationPath;
            BackgroundAudioPath = backgroundAudioPath;
        }
    }
}