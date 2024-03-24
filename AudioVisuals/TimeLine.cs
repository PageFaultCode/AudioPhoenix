using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
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
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:TimeLine/>
    ///
    /// </summary>
    public class TimeLine : Control
    {
        private readonly Pen _ctrlBorderPen = new(Brushes.Black, 1);
        private long _position;
        private FormattedText _text = new FormattedText("1.00", System.Globalization.CultureInfo.InvariantCulture, FlowDirection.LeftToRight, new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal), 7.5, Brushes.Black, 1.0);

        static TimeLine()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TimeLine), new FrameworkPropertyMetadata(typeof(TimeLine)));
        }

        public TimeLine()
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (WaveFormat != null)
            {
                drawingContext.DrawRoundedRectangle(Brushes.Silver, _ctrlBorderPen, new Rect(0, 0, ActualWidth, ActualHeight), 2.0, 2.0);

                // need to draw divits for each second?
                if (WaveFormat != null)
                {
                    int tickCount = (int)(TimeSpan.TotalSeconds * 4); // quarter second divits
                    int tickSpacing = (int)ActualWidth / tickCount;
                    int xSpot = 0;
                    for (int i = 1; i < tickCount; i++)
                    {
                        xSpot += tickSpacing;
                        if (i % 4 != 0)
                        {
                            drawingContext.DrawLine(_ctrlBorderPen, new Point(xSpot, 0), new Point(xSpot, ActualHeight / 2));
                        }
                        else
                        {
                            drawingContext.DrawLine(_ctrlBorderPen, new Point(xSpot, 0), new Point(xSpot, ActualHeight * 0.75));
                            drawingContext.DrawText(_text, new Point(xSpot, ActualHeight / 2));
                        }
                    }
                }
            }
        }

        #region PROPERTIES
        public WaveFormat? WaveFormat {  get; set; }

        public long Position
        {
            get { return _position; } 
            set
            {
                _position = value;
                InvalidateVisual();
            }
        }
        public TimeSpan TimeSpan { get; set; }

        public long SamplesPerSecond { get; set; }
        #endregion
    }
}
