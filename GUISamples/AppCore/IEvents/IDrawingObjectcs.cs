using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUISamples.AppCore.IEvents
{
    interface IDrawingObjectcs
    {
        event EventHandler OnDraw;
    }
}
