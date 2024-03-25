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
    ///     <MyNamespace:WaveStrength/>
    ///
    /// </summary>
    public class WaveStrength : Control
    {
        private readonly Pen _ctrlMarkerPen = new(Brushes.Black, 1);

        static WaveStrength()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WaveStrength), new FrameworkPropertyMetadata(typeof(WaveStrength)));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (ActualWidth > 0 && ActualHeight > 0)
            {
                drawingContext.DrawRectangle(Brushes.DarkGray, null, new Rect(0, 0, ActualWidth, ActualHeight));

                // We need a mark at midpoint to indicate the 0 level
                // then +- 6db == 0.5 volume out of 1.0
                if (Flipped)
                {
                    drawingContext.DrawLine(_ctrlMarkerPen, new Point(0.0, ActualHeight / 2.0), new Point(ActualWidth * 0.8, ActualHeight / 2.0));
                    drawingContext.DrawLine(_ctrlMarkerPen, new Point(0.0, ActualHeight / 4.0), new Point(ActualWidth * 0.5, ActualHeight / 4.0));
                    drawingContext.DrawLine(_ctrlMarkerPen, new Point(0.0, ActualHeight * 0.75), new Point(ActualWidth * 0.5, ActualHeight * 0.75));
                }
                else
                {
                    drawingContext.DrawLine(_ctrlMarkerPen, new Point(ActualWidth * 0.2, ActualHeight / 2.0), new Point(ActualWidth, ActualHeight / 2.0));
                    drawingContext.DrawLine(_ctrlMarkerPen, new Point(ActualWidth * 0.5, ActualHeight / 4.0), new Point(ActualWidth, ActualHeight / 4.0));
                    drawingContext.DrawLine(_ctrlMarkerPen, new Point(ActualWidth * 0.5, ActualHeight * 0.75), new Point(ActualWidth, ActualHeight * 0.75));
                }
            }
        }

        public bool Flipped { get; set; } = false;
    }
}
