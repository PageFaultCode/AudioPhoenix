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
            byte testData = 0;

            IAudioBlock<byte> iAudioBlock = new AudioBlock<byte>(2, 2, 0);

            iAudioBlock.AddSample(testData, testData);

            Assert.AreEqual(1, iAudioBlock.Samples);

            Assert.IsFalse(iAudioBlock.IsFull);

            iAudioBlock.AddSample(testData, testData);

            Assert.AreEqual(2, iAudioBlock.Samples);

            iAudioBlock.AddSample(testData, testData);

            Assert.AreEqual(2, iAudioBlock.Samples);

            Assert.IsTrue(iAudioBlock.IsFull);

            byte[] testData2 = new byte[2];

            Assert.AreEqual(2, iAudioBlock.GetSamples(0, testData2, 0, testData2.Length));
        }
    }
}