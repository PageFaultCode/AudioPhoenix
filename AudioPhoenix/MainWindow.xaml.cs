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
using NAudio.CoreAudioApi;
using System.Collections.ObjectModel;

namespace AudioPhoenix
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        WasapiOut _outputPlayer = new WasapiOut();
        MMDevice? _selectedPlaybackDevice = null;

        public MainWindow()
        {
            InitializeComponent();

            loadAudioDevices();
        }
        private void loadAudioDevices()
        {
            MMDeviceEnumerator deviceEnumerator = new MMDeviceEnumerator();
            MMDeviceCollection deviceCollection = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);

            Collection<MMDevice> devices = new Collection<MMDevice>();

            foreach (MMDevice device in deviceCollection)
            {
                devices.Add(device);
            }

            deviceCollection = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            devices = new Collection<MMDevice>();
            foreach (MMDevice device in deviceCollection)
            {
                devices.Add(device);
            }

            _playbackDevice.ItemsSource = devices;
            if (devices.Count == 0)
            {
                // Disable playback
            }
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
                    _wavePanel.Stream = audioData;
/*                    _outputPlayer = new WasapiOut(_selectedPlaybackDevice, AudioClientShareMode.Shared, true, 200);
                    _outputPlayer.Init(audioData);
                    _outputPlayer.Play();*/
                }
            }
        }
        private void _playbackDevice_Selected(object sender, RoutedEventArgs e)
        {
            _selectedPlaybackDevice = _playbackDevice.SelectedItem as MMDevice;
        }
    }
}