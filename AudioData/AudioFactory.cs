using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioData
{
    using Interfaces;

    public sealed class AudioFactory
    {
        #region singleton
        private static readonly Lazy<AudioFactory> lazy =
            new Lazy<AudioFactory>(() => new AudioFactory());

        public static AudioFactory Instance { get { return lazy.Value; } }
        #endregion

        private AudioFactory()
        {
        }

        public IAudioStream NewStream(WaveFormat format)
        {
            return new AudioStream(format);
        }

        public IAudioStream LoadStream(string filename)
        {
            WaveFileReader waveFileReader = new WaveFileReader(filename);

            switch (waveFileReader.WaveFormat.Encoding)
            {
                case WaveFormatEncoding.Pcm:
                case WaveFormatEncoding.Extensible:
                    {
                        if (waveFileReader.WaveFormat.BitsPerSample == 8)
                        {
                            AudioStream stream = new AudioStream(waveFileReader);
                            return stream;
                        }
                        else
                        {
                            AudioStream stream = new AudioStream(waveFileReader);
                            return stream;
                        }
                        throw new Exceptions.AudioDataException($"Unhandled bits per sample: {waveFileReader.WaveFormat.BitsPerSample}");
                    }
                case WaveFormatEncoding.IeeeFloat:
                    {
                        AudioStream stream = new AudioStream(waveFileReader);
                        return stream;
                    }
                default:
                    throw new Exceptions.AudioDataException($"Unhandled encoding: {waveFileReader.WaveFormat.Encoding}");
            }
        }
    }
}
