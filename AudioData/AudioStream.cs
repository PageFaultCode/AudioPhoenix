
namespace AudioData
{
    using AudioData.Interfaces;
    using NAudio.Wave;

    /// <summary>
    /// AudioStream class to manage an audio stream
    /// </summary>
    public class AudioStream : WaveStream, IAudioStream
    {
        private const long _defaultBufferLength = 44100 * 60 * 5; // 5 minutes per channel
        private List<IAudioBlock> _blocks = new List<IAudioBlock>();
        private WaveFormat _waveFormat = new WaveFormat(); // default to stereo 44.1k 16 bit
        private long _bufferSize = _defaultBufferLength;
        private long _position = 0;

        public AudioStream(WaveFormat format)
        {
            _waveFormat = format;
        }

        public AudioStream(WaveFileReader fileReader)
        {
            _waveFormat = fileReader.WaveFormat;

            int sampleSize = _waveFormat.BlockAlign / _waveFormat.Channels;

            switch (sampleSize)
            {
                case 1:
                    ReadBytes(fileReader);
                    break;
                case 2:
                    ReadShorts(fileReader);
                    break;
                case 4:
                    ReadFloats(fileReader);
                    break;
                default:
                    break;
            }
        }

        private void ReadBytes(WaveFileReader fileReader)
        {
            int readSize = (int)_bufferSize;
            IAudioBlock block = new AudioBlock(_bufferSize, _waveFormat.Channels);

            // read the data into the stream
            // While the data block size will be for one audio sample (mono / stereo) etc.
            // we are storing them as whole samples of the correct type
            // so although 16 bit mono is 2 bytes per sample and stereo is 4 bytes per sample
            // we are storing either 1 short or two shorts though each channel is of the 1 short type
            byte[] buffer = new byte[readSize];

            int position = 0;
            int bytesRead = 0;
            while (position < fileReader.Length)
            {
                bytesRead += fileReader.Read(buffer, position, readSize);
                for (int i = 0; i < bytesRead;)
                {
                    if (_waveFormat.Channels == 2)
                    {
                        byte left = buffer[i++];
                        byte right = buffer[i++];
                        block.AddSample(left, right);
                    }
                    else
                    {
                        block.AddSample(buffer[i++]);
                    }
                    if (block.IsFull)
                    {
                        // add this block
                        AddAudioBlock(block);
                        // create the next one
                        block = new AudioBlock(_bufferSize, _waveFormat.Channels);
                    }
                }
                position += bytesRead;
            }
            // add in the tail
            AddAudioBlock(block);
        }

        private void ReadShorts(WaveFileReader fileReader)
        {
            int sampleSize = sizeof(short) * _waveFormat.Channels;
            int readSize = (int)_bufferSize / sampleSize;

            IAudioBlock block = new AudioBlock(_bufferSize, _waveFormat.Channels);

            // read the data into the stream
            // While the data block size will be for one audio sample (mono / stereo) etc.
            // we are storing them as whole samples of the correct type
            // so although 16 bit mono is 2 bytes per sample and stereo is 4 bytes per sample
            // we are storing either 1 short or two shorts though each channel is of the 1 short type
            byte[] buffer = new byte[readSize];

            int position = 0;
            int bytesRead = 0;
            while (position < fileReader.Length)
            {
                bytesRead += fileReader.Read(buffer, position, readSize);
                for (int i = 0; i < bytesRead;)
                {
                    short left = (short)((buffer[i + 1] << 8) | buffer[i]);
                    if (_waveFormat.Channels == 2)
                    {
                        i += 2;

                        short right = (short)((buffer[i + 1] << 8) | buffer[i]);

                        block.AddSample(left, right);
                    }
                    else
                    {
                        block.AddSample(left, 0);
                    }
                    i += 2;
                    if (block.IsFull)
                    {
                        AddAudioBlock(block);
                        block = new AudioBlock(_bufferSize, _waveFormat.Channels, _blocks.Count * _bufferSize);
                    }
                }
                position += bytesRead;
            }
            AddAudioBlock(block);
        }
        private void ReadFloats(WaveFileReader fileReader)
        {
        }

        public int ReadChannel(int channel, long position, short[] buffer, int count)
        {
            int samplesRead = 0;
            var audioBlock = GetAudioBlock(position);

            if ((audioBlock == null) || (channel >= audioBlock.Channels))
                return samplesRead;

            samplesRead = audioBlock.GetSamples(channel, position, buffer, count);

            return samplesRead;
        }

        #region WAVESTREAM
        public override WaveFormat WaveFormat => _waveFormat;

        /// <summary>
        /// Returns the length based on number of blocks (should all be same length) and whatever is hanging in the end
        /// </summary>
        public override long Length => (long)(((_blocks.Count - 1) * _bufferSize + _blocks[_blocks.Count - 1].Samples) * _waveFormat.BlockAlign);

        public override long Position { get => _position; set => _position = value; } // will need to update the position via actions

        /// <summary>
        /// Reads up to a buffer full of data in byte format so stereo is interleaved and
        /// shorts are two bytes each
        /// </summary>
        /// <param name="buffer">where to place the data</param>
        /// <param name="offset">the offset into the passed in buffer for the data</param>
        /// <param name="count">the amount of data to get</param>
        /// <returns>the amount of data retrieved</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;
            IAudioBlock? block = GetAudioBlock(_position);

            if (block != null)
            {
                for (int i = offset; i < count + offset;)
                {
                    // each read depletes the buffer space, so adjust as we go
                    int bytesRead = block.GetSamples(_position, buffer, i, count - i);

                    if (bytesRead == 0)
                    {
                        break;
                    }
                    i += bytesRead;
                    totalBytesRead += bytesRead;
                    _position++;
                }
            }

            return totalBytesRead;
        }
        #endregion

        #region PROPERTIES
        public long SampleCount => (long)(_blocks.Count - 1) * _bufferSize + _blocks[_blocks.Count - 1].Samples;

        public WaveStream WaveStream => this;


        #endregion

        private void AddAudioBlock(IAudioBlock block)
        {
            _blocks.Add(block);
        }

        private IAudioBlock? GetAudioBlock(long position)
        {
            if (_blocks.Count != 0)
            {
                // Assuming each block is buffersize long
                long index = position / _bufferSize;

                // need to test at block boundries
                if (index < _blocks.Count)
                {
                    return _blocks[(int)index];
                }
                return _blocks[_blocks.Count - 1];
            }
            return null;
        }
    }
}
