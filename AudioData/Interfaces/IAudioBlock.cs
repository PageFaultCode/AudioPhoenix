using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("AudioTest")]
namespace AudioData.Interfaces
{
    internal interface IAudioBlock
    {
        public static long DefaultBufferSize = 44100 * 60 * 5; // 5 minutes per channel;

        public void AddSample(params short[] samples);

        public int GetSamples(long position, byte[] values, int offset, int count);

        public int GetSamples(int channel, long position, short[] values, int count);

        public long BufferSize { get; set; }

        public bool IsFull { get; }

        public long Samples { get; }

        public AudioChannel this[int index] { get; }

        int Channels { get; set; }
    }
}
