using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Gapi.Charting
{
    public abstract class Chart
    {
        const string CHART_API = "http://chart.apis.google.com/chart?";

        int _width;
        int _height;

        public int Width
        {
            get { return _width; }
            set 
            {
                if (value <= 0)
                    throw new Exception("Width value out of range: " + value.ToString());

                _width = value; 
            }
        }

        public int Height
        {
            get { return _height; }
            set 
            {
                if (value <= 0)
                    throw new Exception("Height value out of range: " + value.ToString());

                _height = value; 
            }
        }

        protected Chart(int width, int height)
        {
            this.Width = width;
            this.Height = height;
        }
    }
}
