using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AudioVisuals
{
    using AudioData;
    using AudioData.Interfaces;
    using NAudio.Wave;
    using System.IO;

    /// <summary>
    /// Interaction logic for AudioPanel.xaml
    /// </summary>
    public partial class AudioPanel : UserControl
    {
        private IAudioStream _stream = new AudioStream(new WaveFormat()); // place holder

        public AudioPanel()
        {
            InitializeComponent();
        }

        private void UpdateWavePanels()
        {
            _wavePanels.Children.Clear();
            for (var channel = 0; channel < _stream.WaveFormat.Channels; channel++)
            {

            }
        }

        #region PROPERTIES

        public IAudioStream Stream
        {
            get
            {
                return _stream;
            }
            set
            {
                _stream = value;
                UpdateWavePanels();
            }
        }

        public long Position { get; set; }

        public TimeSpan TimeSpan
        {
            get;
            set;
        }
        #endregion
    }
}
