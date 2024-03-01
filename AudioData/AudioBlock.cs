using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioData.Interfaces;

namespace AudioData
{
    internal class AudioBlock<T> : IAudioBlock<T>
    {
        private long _bufferSize = IAudioBlock<T>.DefaultBufferSize;
        private long _sampleCount = 0;
        private long _positionOffset = 0;
        private AudioChannel<T>[] _channels;

        public AudioBlock(long bufferSize, int numChannels, long positionOffset = 0)
        {
            _positionOffset = positionOffset;
            _channels = new AudioChannel<T>[numChannels];
            for (int i = 0; i < numChannels; i++)
            {
                _channels[i] = new AudioChannel<T>();
            }
            BufferSize = bufferSize;
            IsFull = false;
        }

        #region IAudioBlock
        public void AddSample(params T[] samples)
        {
            if (!IsFull)
            {
                int index = 0;
                foreach (var channel in _channels)
                {
                    if (index < samples.Length)
                    {
                        channel.AddSample(samples[index++]);
                    }
                    else
                    {
                        // fill with silence
                        dynamic zero = 0;
                        channel.AddSample(zero);
                    }
                }
                _sampleCount++;
                if (_sampleCount == _bufferSize)
                {
                    IsFull = true;
                }
            }
        }

        public void AddSample(params byte[] samples)
        {
            if (typeof(T).Equals(typeof(byte)))
            {
                var converted = samples as T[];
                if (converted != null)
                {
                    AddSample(converted);
                }
            }
        }

        public void AddSample(params short[] samples)
        {
            if (typeof(T).Equals(typeof(short)))
            {
                var converted = samples as T[];
                if (converted != null)
                {
                    AddSample(converted);
                }
            }
        }

        public void AddSample(params float[] samples)
        {
            if (typeof(T).Equals(typeof(float)))
            {
                var converted = samples as T[];
                if (converted != null)
                {
                    AddSample(converted);
                }
            }
        }

        public int GetSamples(long position, byte[] values, int offset, int count)
        {
            int bytesRead = 0;

            int bytesNeeded = GetSampleSize() * _channels.Length;

            if (count <= bytesNeeded)
            {
                foreach (var channel in _channels)
                {
                    // copy sample at channel[position - _positionOffset]
                    T value = channel.Data[position - _positionOffset];

                    if (value != null)
                    {
                        byte[] data = GetBytes(value);
                        bytesRead += data.Length;
                        Buffer.BlockCopy(data, 0, values, offset, data.Length);
                        offset += data.Length;
                    }
                }
            }

            return bytesRead;
        }

        public void GetSample(short[] values)
        {

        }

        public void GetSample(float[] values)
        {

        }

        public long BufferSize
        { 
            get
            {
                return _bufferSize;
            }
            set
            {
                // ignore the change if there isn't a change
                if (_bufferSize != value)
                {
                    _bufferSize = value;
                    foreach (var channel in _channels)
                    {
                        channel.BufferSize = value;
                    }
                }
            }
        }

        public bool IsFull { get; private set; }

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

        public int Channels
        {
            get
            {
                return _channels.Length;
            }
            set
            {
                _channels = new AudioChannel<T>[value];
            }
        }

        public AudioChannel<T> this[int index] => _channels[index];
        #endregion

        private int GetSampleSize()
        {
            int sampleSize = 0;

            if (typeof(T).Equals(typeof(byte)))
            {
                sampleSize = 1;
            }
            else if (typeof(T).Equals(typeof(short)))
            {
                sampleSize = 2;
            }
            else if (typeof(T).Equals(typeof(float)))
            {
                sampleSize = 4;
            }
            return sampleSize;
        }

        private byte[] GetBytes(dynamic value)
        {
            byte[] results = new byte[0];

            if (typeof(T).Equals(typeof(short)))
            {
                results = new byte[sizeof(short)];
                results = BitConverter.GetBytes((short)value);
            }
            else if (typeof(T).Equals(typeof(float)))
            {
                results = new byte[sizeof(float)];
                results = BitConverter.GetBytes((float)value);
            }
            else
            {
                results = new byte[1];
                results[0] = (byte)value;
            }
            return results;
        }
    }
}
