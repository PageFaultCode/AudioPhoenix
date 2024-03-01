using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioData.Interfaces;

namespace AudioData
{
    internal class AudioChannel<T>
    {
        private long _bufferSize = IAudioBlock<T>.DefaultBufferSize;
        private long _sampleCount = 0;

        public void AddSample(T sample)
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
                Data = new T[_bufferSize];
            }
        }

        public T[] Data { get; private set; } = new T[IAudioBlock<T>.DefaultBufferSize];

        public bool IsFull { get; private set; }
    }
}
