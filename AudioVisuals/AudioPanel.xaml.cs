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
using System.Diagnostics;

namespace AudioVisuals
{
    using AudioData;
    using AudioData.Interfaces;
    using NAudio.Wave;
    using System.Diagnostics;
    using System.IO;

    /// <summary>
    /// Interaction logic for AudioPanel.xaml
    /// </summary>
    public partial class AudioPanel : UserControl
    {
        Pen _selectionBorderPen = new Pen(Brushes.Black, 1);

        private IAudioStream _stream = new AudioStream(new WaveFormat()); // place holder   
        private TimeSpan _timeSpan = new TimeSpan(0, 0, 2);
        private bool _selectionInProgress;
        private Point _startingSelection;
        private Point _endingSelection;
        private Point _currentPosition;
        private Brush _selectionBrush = new System.Windows.Media.SolidColorBrush(Color.FromArgb(127,0,0,127));

        public AudioPanel()
        {
            InitializeComponent();
        }

        #region User Interface
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
        }

        protected override void OnDragLeave(DragEventArgs e)
        {
            base.OnDragLeave(e);
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            _selectionInProgress = true;
            _startingSelection = e.GetPosition(this);
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_selectionInProgress)
            {
                _currentPosition = e.GetPosition(this);
                InvalidateVisual();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            _selectionInProgress = false;
            _endingSelection = e.GetPosition(this);
            base.OnMouseUp(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            if (_selectionInProgress)
            {
                var startPoint = _startingSelection.X < _currentPosition.X ? _startingSelection.X : _currentPosition.X;
                var width = Math.Abs(_currentPosition.X - _startingSelection.X);

//                Trace.WriteLine($"SP: {startPoint} W: {width} SS: {_startingSelection} CP: {_currentPosition}");
                drawingContext.DrawRectangle(Brushes.Aqua, _selectionBorderPen, new Rect(startPoint, 0, width, 200));
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            foreach (WavePanel panel in _wavePanels.Children)
            {
                panel.InvalidateVisual();
            }
        }
        #endregion

        private void UpdateWavePanels()
        {
            _wavePanels.Children.Clear();
            for (var channel = 0; channel < _stream.WaveFormat.Channels; channel++)
            {
                var wavePanel = new WavePanel();

                wavePanel.Channel = channel;
                _wavePanels.Children.Add(wavePanel);
                wavePanel.TimeSpan = TimeSpan;
                wavePanel.Stream = Stream;
            }
        }

        #region PROPERTIES
        private void UpdateTimeSpan(TimeSpan timeSpan)
        { 
            _timeSpan = timeSpan;
            foreach (WavePanel panel in _wavePanels.Children)
            {
                panel.TimeSpan = timeSpan;
            }
        }
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
            get => _timeSpan;
            set => UpdateTimeSpan(value);
        }
        #endregion
    }
}
