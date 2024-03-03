using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioData.Interfaces;

namespace AudioData
{
    internal class AudioChannel
    {
        private long _bufferSize = IAudioBlock.DefaultBufferSize;
        private long _sampleCount = 0;

        public void AddSample(short sample)
        {
            Data[_sampleCount++] = sample;

            IsFull = _sampleCount == _bufferSize;
        }

        public long BufferSize
        {
            get
            {
                return _bufferSize;
            }
            set
            {
                _bufferSize = value;
                Data = new short[_bufferSize];
            }
        }

        public short[] Data { get; private set; } = new short[IAudioBlock.DefaultBufferSize];

        public bool IsFull { get; private set; }
    }
}
