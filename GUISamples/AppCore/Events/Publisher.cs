using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUISamples.AppCore
{
    public class Publisher
    {
        public event EventHandler<CustomEventArgs> RaiseCustomEvents;

        public void DoSomething()
        {
            OnRaiseCustomEvent(new CustomEventArgs("Did Something"));
        }

        public virtual void OnRaiseCustomEvent(CustomEventArgs e)
        {
            EventHandler<CustomEventArgs> handle = RaiseCustomEvents;

            if (handle != null)
            {
                e.Message = "Did somethin here";
                handle(this, e);
            }
        }
    }
}
