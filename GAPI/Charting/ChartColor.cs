using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Gapi.Charting
{
    public class ChartColor
    {
        double _alpha;
        Color _color;

        public double Alpha
        {
            get { return _alpha; }
            set
            {
                if ((value < 0) || (value > 1))
                    throw new Exception("Alpha value must be [0..1]");

                _alpha = value;
            }
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public ChartColor(Color color)
            : this(color, 1.0)
        {
        }

        public ChartColor(Color color, double alpha)
        {
            this.Color = color;
            this.Alpha = alpha;
        }

        public override string ToString()
        {
            return 
                string.Format("{0}{1}{2}{3}",
                    this.Color.R.ToString("X2"),
                    this.Color.G.ToString("X2"),
                    this.Color.B.ToString("X2"),
                    Convert.ToInt32(this.Alpha * 0xFF).ToString("X2"));
        }
    }
}
