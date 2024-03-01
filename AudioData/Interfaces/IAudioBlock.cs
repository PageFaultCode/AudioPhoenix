using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("AudioTest")]
namespace AudioData.Interfaces
{
    internal interface IAudioBlock<T>
    {
        public static long DefaultBufferSize = 44100 * 60 * 5; // 5 minutes per channel;

        public void AddSample(params T[] samples);

        public void AddSample(params byte[] samples);

        public void AddSample(params short[] samples);

        public void AddSample(params float[] samples);

        public int GetSamples(long position, byte[] values, int offset, int count);

        public long BufferSize { get; set; }

        public bool IsFull { get; }

        public long Samples { get; }

        public AudioChannel<T> this[int index] { get; }

        int Channels { get; set; }
    }
}
