using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioData.Interfaces;

namespace AudioData
{
    internal class FillBlock<T> : IAudioBlock<T>
    {
        private long _positionOffset = 0;
        private long _sampleCount = 0;
        private AudioChannel<T> _channel;

        public FillBlock(long bufferSize, int channels, long positionOffset = 0)
        {
            BufferSize = bufferSize;
            Channels = channels;
            _channel = new AudioChannel<T>();
            _channel.BufferSize = bufferSize;
        }


        #region IAudioBlock

        public void AddSample(params T[] samples)
        {
            _sampleCount++;
        }

        public void AddSample(params byte[] samples)
        {
            _sampleCount++;
        }

        public void AddSample(params short[] samples)
        {
            _sampleCount++;
        }

        public void AddSample(params float[] samples)
        {
            _sampleCount++;
        }

        public int GetSamples(long position, byte[] values, int offset, int count)
        {
            return 0; // for now, need to fill in blanks
        }

        public long BufferSize { get; set; } = IAudioBlock<T>.DefaultBufferSize;

        public bool IsFull { get; private set; } = false;

        public long Samples
        {
            get
            {
                return _sampleCount;
            }
            private set
            {
                _sampleCount = value;
            }
        }

        public int Channels { get; set; }

        public AudioChannel<T> this[int index] => _channel;
        #endregion
    }
}
