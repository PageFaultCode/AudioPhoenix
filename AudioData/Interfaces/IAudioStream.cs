using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using NAudio.Wave;

namespace AudioData.Interfaces
{
    public interface IAudioStream : IWaveProvider
    {
        WaveStream WaveStream { get; }

        long SampleCount { get; }

        int ReadChannel(int channel, long position, short[] buffer, int count);
    }
}
