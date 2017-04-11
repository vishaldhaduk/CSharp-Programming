using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUISamples.AppCore
{
    public class CustomEventArgs : EventArgs
    {
        private string _msg;
        public CustomEventArgs(string message)
        {
            Message = message;
        }

        public string Message
        {
            get { return _msg; }
            set { _msg = value; }
        }
    }
}
