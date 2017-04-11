using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUISamples.AppCore.Shape
{
    public class ShapeEventArgs : EventArgs
    {
        private double newArea;

        public ShapeEventArgs(double d)
        {
            newArea = d;
        }
        public double NewArea
        {
            get { return newArea; }
        }
    }
}
