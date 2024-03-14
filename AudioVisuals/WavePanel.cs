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
using System.Diagnostics;

namespace AudioVisuals
{
    using AudioData;
    using AudioData.Interfaces;
    using AudioVisuals.Tools;
    using NAudio.Wave;

    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:AudioVisuals"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:AudioVisuals;assembly=AudioVisuals"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class WavePanel : Control
    {
        Pen _waveBorderPen = new Pen(Brushes.Black, 1);
        Pen _waveMiddleLinePen = new Pen(Brushes.Black, 1);
        Pen _waveMidPointPen = new Pen(Brushes.DarkGreen, 1);
        IAudioStream? _stream;
        const double _minWaveValue = (double)short.MinValue;
        const double _maxWaveValue = (double)short.MaxValue;
        private TimeSpan _timeSpan = new TimeSpan(0, 0, 2);
        SelectionView? _selectionView;

        static WavePanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WavePanel), new FrameworkPropertyMetadata(typeof(WavePanel)));
        }

        public WavePanel()
        {
            Height = 50;

            _waveMidPointPen.DashStyle = DashStyles.Dash;
        }

        #region DRAWING
        protected void DrawMarkings(DrawingContext drawingContext, double waveHeight, double midPoint, double quarterPoint)
        {
            drawingContext.DrawLine(_waveMiddleLinePen, new Point(0, midPoint), new Point(ActualWidth, midPoint));
            drawingContext.DrawLine(_waveMidPointPen, new Point(0, quarterPoint), new Point(ActualWidth, quarterPoint));
            drawingContext.DrawLine(_waveMidPointPen, new Point(0, midPoint + quarterPoint), new Point(ActualWidth, midPoint + quarterPoint));
            //drawingContext.DrawLine(_waveMidPointPen, new Point(5, 5), new Point(ActualWidth - 5, midPoint));
        }

        protected void DrawWaveForm(DrawingContext drawingContext, double samplesToDisplay, double midPoint)
        {
            if (Stream != null)
            {
                // A non-compressed wave form will be the actual waveform zoomed in such that it is a contiguous point to point line
                long sampleCount = (long)Math.Round(samplesToDisplay, 0);
                double pixelsPerSample = ActualWidth / samplesToDisplay;
                double xPixelPosition = 0;

                short[] buffer = new short[sampleCount];
                int samplesRead = Stream.ReadChannel(Channel, Position, buffer, (int)sampleCount);
                if (samplesRead == 0)
                {
                    return;
                }
                short value = buffer[0];
                for (int waveDataIndex = 1; waveDataIndex < samplesRead; waveDataIndex++)
                {
                    double y1Position = (double)value / _minWaveValue * midPoint;
                    value = buffer[waveDataIndex];
                    double y2Position = (double)value / _maxWaveValue * midPoint;
                    drawingContext.DrawLine(_waveMiddleLinePen, new Point(xPixelPosition, midPoint + y1Position), new Point(xPixelPosition + pixelsPerSample, midPoint - y2Position));
                    xPixelPosition += pixelsPerSample;
                }
            }
        }

        protected void DrawCompressedWaveForm(DrawingContext drawingContext, double samplesToDisplay, double midPoint)
        {
            if (Stream != null)
            {
                // Window is laid out 0,0 top left, ActualWidth, ActualHeight bottom right
                // A compressed wave form will be vertical bars expressing the extent of the data in both positive and negative directions
                // Determine how many samples per pixel
                long samplesPerPixel = (long)samplesToDisplay / (long)ActualWidth;

                int xPixelPosition = 0;
                short[] buffer = new short[samplesPerPixel];
                short maxExtent;
                short minExtent;

                long displayPosition = Position;

                // Trace.WriteLine($"------------------------>S:{samplesToDisplay}, MP: {midPoint}");

                while (xPixelPosition < (int)ActualWidth)
                {
                    maxExtent = short.MinValue;
                    minExtent = short.MaxValue;
                    // Read in the next samplesPerPixel assuming the position will increment for the offset
                    int samplesRead = Stream.ReadChannel(Channel, displayPosition, buffer, (int)samplesPerPixel);
                    if (samplesRead == 0)
                    {
                        return;
                    }
                    for (int waveDataIndex = 0; waveDataIndex < samplesRead; waveDataIndex++)
                    {
                        if (maxExtent < buffer[waveDataIndex])
                        {
                            maxExtent = buffer[waveDataIndex];
                        }
                        if (minExtent > buffer[waveDataIndex])
                        {
                            minExtent = buffer[waveDataIndex];
                        }
                    }

                    double y1Position = minExtent / _minWaveValue * midPoint;
                    double y2Position = maxExtent / _maxWaveValue * midPoint;

                    drawingContext.DrawLine(_waveMiddleLinePen, new Point(xPixelPosition, midPoint + y1Position), new Point(xPixelPosition, midPoint - y2Position));
                    xPixelPosition++;
                    displayPosition += samplesPerPixel;
                }
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            double waveHeight = ActualHeight - 1;

            double midPoint = waveHeight / 2.0;
            double quarterPoint = midPoint / 2.0;

            // make it so
            drawingContext.DrawRoundedRectangle(Brushes.Silver, _waveBorderPen, new Rect(0, 0, ActualWidth, waveHeight), 3.0, 3.0);

            if (Stream != null)
            {
                double samplesToDisplay = Stream.WaveFormat.SampleRate * TimeSpan.TotalSeconds;

                // Draw wave
                // Get total count
                if (samplesToDisplay > ActualWidth)
                {
                    // Wave is zoomed out where we have more samples per pixel
                    DrawCompressedWaveForm(drawingContext, samplesToDisplay, midPoint);
                }
                else
                {
                    // Wave is zoomed where we have more than one pixel per sample
                    DrawWaveForm(drawingContext, samplesToDisplay, midPoint);
                }
            }
            
            SelectionView?.OnRender(drawingContext, ActualHeight);

            DrawMarkings(drawingContext, waveHeight, midPoint, quarterPoint);
        }
        #endregion

        private void UpdateTimeSpan(TimeSpan timeSpan)
        {
            _timeSpan = timeSpan;
            InvalidateVisual();
        }
        #region PROPERTIES

        public IAudioStream? Stream
        {
            get
            {
                return _stream;
            }
            set
            {
                _stream = value;
                InvalidateVisual();
            }
        }

        public int Channel { get; set; } = 0;
        public long Position { get; set; } = 0;

        internal SelectionView? SelectionView
        {
            get
            {
                return _selectionView;
            }
            set
            {
                _selectionView = value;
                InvalidateVisual();
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