using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace AudioVisuals.Tools
{
    internal class SelectionView
    {
        private Pen _selectionBorderPen = new Pen(Brushes.Black, 1);
        private Brush _selectionBrush = new System.Windows.Media.SolidColorBrush(Color.FromArgb(87, 0, 0, 127));

        #region properties
        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }

        public Color Color { get; set; }

        public double Opacity { get; set; }

        #endregion

        public void OnRender(DrawingContext drawingContext, double height)
        {
            var startPoint = StartPoint.X < EndPoint.X ? StartPoint.X : EndPoint.X;
            var width = Math.Abs(EndPoint.X - StartPoint.X);

            drawingContext.DrawRectangle(_selectionBrush, null, new Rect(startPoint, 0, width, height));
        }
    }
}
