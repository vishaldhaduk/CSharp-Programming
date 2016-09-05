﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace MozHelper
{
    public class KeyPressHelper
    {
        // Get a handle to an application window.
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(string lpClassName,
            string lpWindowName);

        // Activate an application window.
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        internal static void PressKeyForFF()
        {
            // Get a handle to the Calculator application. The window class 
            // and window name were obtained using the Spy++ tool.
            IntPtr calculatorHandle = FindWindow("MozillaWindowClass", "Restore Search Settings - Mozilla Firefox");

            // Verify that Calculator is a running process. 
            if (calculatorHandle == IntPtr.Zero)
            {
                //MessageBox.Show("Calculator is not running.");
                return;
            }

            // Make Calculator the foreground application and send it  
            // a set of calculations.
            SetForegroundWindow(calculatorHandle);
            SendKeys.SendWait("D");
            //SendKeys.SendWait("111");
            //SendKeys.SendWait("*");
            //SendKeys.SendWait("11");
            //SendKeys.SendWait("=");
        }
    }
}
