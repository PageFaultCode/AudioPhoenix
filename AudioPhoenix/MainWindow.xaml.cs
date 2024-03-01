using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AudioData;
using NAudio.Wave;

namespace AudioPhoenix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WaveOut _outputPlayer = new WaveOut();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void _loadWave_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.FileName = "Audio";
            dialog.DefaultExt = "wav";
            dialog.Filter = "Audio Files (.wav)|*.wav|MP3 (.mp3)|*.mp3";

            bool? openFile = dialog.ShowDialog();

            if (openFile == true)
            {
                // Load stream
                var audioData = AudioFactory.Instance.LoadStream(dialog.FileName);

                if (audioData != null)
                {
                    _outputPlayer.Init(audioData);
                    _outputPlayer.Play();
                }
            }
        }
    }
}