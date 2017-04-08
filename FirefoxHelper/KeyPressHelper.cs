using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace FirefoxHelper
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
        internal static bool PressKeyForFF(string Shortcut, string WindowName)
        {
            Console.WriteLine("Checking if window exist or not...");
            IntPtr calculatorHandle = FindWindow("MozillaWindowClass", WindowName);

            if (calculatorHandle == IntPtr.Zero)
            {
                Console.WriteLine("FF window doesnt exist...");
                return false;
            }

            SetForegroundWindow(calculatorHandle);

            SendKeys.SendWait("ت");
            MessageBox.Show("Nailed it");
            //SendKeys.SendWait("J");
            //SendKeys.SendWait("+{TAB}");
            //SendKeys.SendWait("{ENTER}");

            //SendKeys.SendWait(Shortcut);


            Console.WriteLine("Successfully press the key...");
            return true;
            //MessageBox.Show("Nailed it");
        }
    }
}
