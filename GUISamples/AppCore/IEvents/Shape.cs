using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUISamples.AppCore.IEvents
{
    public class Shape : IDrawingObjectcs, IShape
    {
        public object lockObject = new object();

        public event EventHandler PreDrawEvent;
        public event EventHandler PostDrawEvent;

        event EventHandler IDrawingObjectcs.OnDraw
        {
            add
            {
                lock (lockObject)
                {
                    PreDrawEvent += value;
                }
            }
            remove
            {
                lock (lockObject)
                {
                    PreDrawEvent -= value;

                }
            }
        }

        event EventHandler IShape.OnDraw
        {
            add
            {
                lock (lockObject)
                {
                    PostDrawEvent += value;
                }
            }
            remove
            {
                lock (lockObject)
                {
                    PostDrawEvent -= value;
                }
            }
        }

        // For the sake of simplicity this one method
        // implements both interfaces. 
        public void Draw()
        {
            // Raise IDrawingObject's event before the object is drawn.
            EventHandler handler = PreDrawEvent;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
            Console.WriteLine("Drawing a shape.");

            // RaiseIShape's event after the object is drawn.
            handler = PostDrawEvent;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }
    }
}
