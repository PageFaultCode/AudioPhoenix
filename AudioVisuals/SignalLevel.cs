﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
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
    ///     <MyNamespace:SignalLevel/>
    ///
    /// </summary>
    public class SignalLevel : Control
    {
        private readonly Pen _ctrlBorderPen = new (Brushes.Black, 1);
        private readonly SolidColorBrush _volumeBrush = new (Colors.GreenYellow);

        static SignalLevel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SignalLevel), new FrameworkPropertyMetadata(typeof(SignalLevel)));
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (ActualWidth > 0 && ActualHeight > 0)
            {
                drawingContext.DrawRectangle(Brushes.DarkGray, _ctrlBorderPen, new Rect(new Point(0, 0), new Point(ActualWidth, ActualHeight)));

                // Upto 10 blocks with 10% spacing between
                double blockSize = ActualWidth / 10.0;
                double blockWidth = blockSize * 0.9;
                double spacer = blockSize * 0.05;

                for (int i = 0; i < (int)Volume; i++)
                {
                    double offset = blockSize * i + spacer;
                    drawingContext.DrawRectangle(_volumeBrush, null, new Rect(new Point(offset, 3.0), new Point(offset + blockWidth, ActualHeight - 3.0)));
                }
            }
        }

        public double Volume { get; set; } = 0.0;
    }
}
