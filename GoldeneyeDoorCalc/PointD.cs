using System;
using System.Collections.Generic;
using System.Text;

namespace GoldeneyeDoorCalc
{
    public class PointD
    {
        public PointD()
        { }

        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }

        public PointD(int x, int y)
        {
            X = x;
            Y = y;
        }

        public double X { get; set; }
        public double Y { get; set; }
    }
}
