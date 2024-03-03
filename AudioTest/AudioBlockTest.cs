using AudioData;
using AudioData.Interfaces;

namespace AudioTest
{
    [TestClass]
    public class AudioBlockTest
    {
        [TestMethod]
        public void AudioBlock8Test()
        {
            short testData = 0;

            IAudioBlock iAudioBlock = new AudioBlock(2, 2, 0);

            iAudioBlock.AddSample(testData, testData);

            Assert.AreEqual(1, iAudioBlock.Samples);

            Assert.IsFalse(iAudioBlock.IsFull);

            iAudioBlock.AddSample(testData, testData);

            Assert.AreEqual(2, iAudioBlock.Samples);

            iAudioBlock.AddSample(testData, testData);

            Assert.AreEqual(2, iAudioBlock.Samples);

            Assert.IsTrue(iAudioBlock.IsFull);

            byte[] testData2 = new byte[4];

            Assert.AreEqual(4, iAudioBlock.GetSamples(0, testData2, 0, testData2.Length));
        }

        [TestMethod]
        public void AudioBlock16Test()
        {
            short testData = 0xA5;

            IAudioBlock iAudioBlock = new AudioBlock(2, 2, 0);

            iAudioBlock.AddSample(testData, testData);
            iAudioBlock.AddSample(testData, testData);

            short[] testData2 = new short[4];

            int samplesRead = iAudioBlock.GetSamples(0, 0, testData2, 2);

            Assert.AreEqual(2, samplesRead);

            samplesRead = iAudioBlock.GetSamples(1, 0, testData2, 2);

            Assert.AreEqual(2, samplesRead);

            samplesRead = iAudioBlock.GetSamples(1, 0, testData2, 4);

            Assert.AreEqual(2, samplesRead);

            samplesRead = iAudioBlock.GetSamples(1, 1, testData2, 4);

            Assert.AreEqual(1, samplesRead);
        }
    }
}