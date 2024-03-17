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
    using AudioVisuals.Tools;
    using NAudio.MediaFoundation;
    using NAudio.Wave;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Media.Media3D;

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
        private Point _currentSelection;
        private SelectionView? _selectionView;
        private long _position;

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
            // only on left button press
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _selectionInProgress = true;
                _startingSelection = e.GetPosition(this);
            }
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_selectionInProgress)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    _currentSelection = e.GetPosition(this);
                    InvalidateVisual();
                }
                else
                {
                    _selectionInProgress = false;
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (_selectionInProgress)
            {
                if (e.LeftButton == MouseButtonState.Released)
                {
                    _endingSelection = e.GetPosition(this);

                    if (_endingSelection != _startingSelection)
                    {
                        _selectionView = new SelectionView();

                        _selectionView.Opacity = 0.5;
                        _selectionView.Color = Colors.Aqua;
                        _selectionView.StartPoint = _startingSelection;
                        _selectionView.EndPoint = _endingSelection;
                    }
                    else
                    {
                        _selectionView = null;
                    }
                    foreach (WavePanel panel in _wavePanels.Children)
                    {
                        panel.SelectionView = _selectionView;
                        panel.InvalidateVisual(); // make it so
                    }
                }
                _selectionInProgress = false;
            }
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
                var selectionView = new SelectionView();

                selectionView.Opacity = 0.5;
                selectionView.Color = Colors.Aqua;
                selectionView.StartPoint = _startingSelection;
                selectionView.EndPoint = _currentSelection;

                foreach (WavePanel panel in _wavePanels.Children)
                {
                    panel.SelectionView = selectionView;
                    panel.InvalidateVisual();
                }
            }
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            var newHeight = (sizeInfo.NewSize.Height - _timeLine.Height) / _wavePanels.Children.Count;

            foreach (WavePanel panel in _wavePanels.Children)
            {
                panel.Height = newHeight;
                panel.Width = ActualWidth;
                panel.InvalidateVisual();
            }
            _timeLine.InvalidateVisual();
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
            _timeLine.WaveFormat = Stream.WaveFormat;
            _timeLine.TimeSpan = TimeSpan;
            _timeLine.Position = Position;
        }

        private void UpdateTimeSpan(TimeSpan timeSpan)
        { 
            _timeSpan = timeSpan;
            foreach (WavePanel panel in _wavePanels.Children)
            {
                panel.TimeSpan = timeSpan;
            }
            _timeLine.TimeSpan = timeSpan;
        }

        private void UpdatePosition(long newPosition)
        {
            _position = newPosition;
            foreach (WavePanel wavePanel in _wavePanels.Children)
            {
                wavePanel.Position = newPosition;
            }
            _timeLine.Position = newPosition;
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

        public long Position
        {
            get { return _position; }
            set
            {
                UpdatePosition(value);
            }
        }

        public TimeSpan TimeSpan
        {
            get => _timeSpan;
            set => UpdateTimeSpan(value);
        }
        #endregion
    }
}
