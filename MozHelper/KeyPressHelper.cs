using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Management;
using Lavasoft.WebBar.UI.AppCore.Utilities;


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

        #region BrowserWatcher
        private ManagementEventWatcher watcherFirefox;
        internal void InitializeEventSubscription()
        {
            try
            {
                Console.WriteLine("Subscribing to browser startup events.");
                string queryString =
               "SELECT TargetInstance " +
               "FROM __InstanceCreationEvent " +
               "WITHIN  10 " +
               "WHERE TargetInstance ISA 'Win32_Process' " +
               "AND TargetInstance.Name = ";

                string queryStringFirefox = queryString + "'firefox.exe'";

                // The dot in the scope means use the current machine
                string scope = @"\\.\root\CIMV2";

                // Create a watcher and listen for events

                watcherFirefox = new ManagementEventWatcher(scope, queryStringFirefox, new EventWatcherOptions { });
                watcherFirefox.EventArrived += FirefoxProcessStarted;
                watcherFirefox.Start();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
        }

        /// <summary>
        /// Process start event. It detects if any of the browser starts 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FirefoxProcessStarted(object sender, EventArrivedEventArgs e)
        {
            try
            {
                MessageBox.Show("Fuck off");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
        }
        #endregion
    }
}
