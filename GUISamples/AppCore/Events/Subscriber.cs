using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GUISamples.AppCore
{
    public class Subscriber
    {
        public string id;

        public Subscriber(string iD, Publisher pub)
        {
            id = iD;
            pub.RaiseCustomEvents += HandleCustomEvents;
        }

        private void HandleCustomEvents(object sender, CustomEventArgs e)
        {
            Console.WriteLine(id + " received this message: {0}", e.Message);
        }
    }
}
