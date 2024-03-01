using AudioData;
using NAudio.Wave;

namespace AudioTest
{
    [TestClass]
    public class AudioStreamTest
    {
        [TestMethod]
        public void AudioStream8Test()
        {
            var waveStream = AudioFactory.Instance.LoadStream("Media\\test.wav");

            Assert.IsNotNull(waveStream);

            Assert.AreEqual(2, waveStream.WaveFormat.Channels);

            Assert.AreEqual(3082940, waveStream.WaveStream.Length);
            Assert.AreEqual(770735, waveStream.SampleCount);
        }

        [TestMethod]
        public void VerifyWaveDataTest()
        {
            WaveFileReader waveFileReader = new WaveFileReader("Media\\test.wav");

            Assert.IsNotNull(waveFileReader);
        }
    }
}
