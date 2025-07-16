using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace SemiConductor_Equipment.Coverter
{
    public class GraphToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type t, object p, CultureInfo color)
        {
            if (value is LiveChartsCore.SkiaSharpView.Painting.SolidColorPaint paint)
            {
                var c = paint.Color;
                return new SolidColorBrush(Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue));
            }
            return Brushes.Black;
        }
        public object ConvertBack(object v, Type t, object p, CultureInfo color) => throw new NotImplementedException();
    }
}
