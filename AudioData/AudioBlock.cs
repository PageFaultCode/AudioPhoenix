using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AudioData.Interfaces;

namespace AudioData
{
    internal class AudioBlock : IAudioBlock
    {
        private long _bufferSize = IAudioBlock.DefaultBufferSize;
        private long _sampleCount = 0;
        private long _positionOffset = 0;
        private AudioChannel[] _channels;

        public AudioBlock(long bufferSize, int numChannels, long positionOffset = 0)
        {
            _positionOffset = positionOffset;
            _channels = new AudioChannel[numChannels];
            for (int i = 0; i < numChannels; i++)
            {
                _channels[i] = new AudioChannel();
            }
            BufferSize = bufferSize;
            IsFull = false;
        }

        #region IAudioBlock

        public void AddSample(params short[] samples)
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
                        short zero = 0;
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

        public int GetSamples(long position, byte[] values, int offset, int count)
        {
            int bytesRead = 0;

            if (position - _positionOffset < _sampleCount)
            {
                foreach (var channel in _channels)
                {
                    // copy sample at channel[position - _positionOffset]
                    short value = channel.Data[position - _positionOffset];

                    byte[] data = BitConverter.GetBytes(value);
                    bytesRead += data.Length;
                    Buffer.BlockCopy(data, 0, values, offset, data.Length);
                    offset += data.Length;
                }
            }

            return bytesRead;
        }

        public int GetSamples(int channel, long position, short[] values, int count)
        {
            int samplesRead = 0;
            long offset = position - _positionOffset;

            if (offset < _sampleCount)
            {
                if (offset + count < _sampleCount)
                {
                    Buffer.BlockCopy(_channels[channel].Data, (int)offset, values, 0, count * 2);
                    samplesRead = count;
                }
                else // up to the end
                {
                    Buffer.BlockCopy(_channels[channel].Data, (int)offset, values, 0, (int)(_sampleCount - offset) * 2);
                    samplesRead = (int)(_sampleCount - offset);
                }
            }

            return samplesRead;

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
                _channels = new AudioChannel[value];
            }
        }

        public AudioChannel this[int index] => _channels[index];
        #endregion
    }
}
