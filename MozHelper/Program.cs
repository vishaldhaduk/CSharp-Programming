﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MozHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            KeyPressHelper kph = new KeyPressHelper();
            kph.InitializeEventSubscription();

            Console.ReadKey();
            Console.ReadLine();
            //KeyPressHelper.PressKeyForFF();
            //Mozlz4Helper.GetCurrentSearch();
        }
    }
}
