using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace WebCompanionHelper.Core
{
    class InstallerProcessHelper
    {
        public static string InstllerProcessName = "webcompanion.exe";
        public static void GetInstallerProcess()
        {
            bool result1 = Process.GetProcessesByName("WcInstaller.exe").Count() > 0;


            var processes = Process.GetProcesses();

            foreach (var item in processes)
            {
                if (item.ProcessName.Contains("WcInstaller"))
                    Debug.WriteLine(item);
                else if (item.ProcessName.Contains("WebCompanionInstaller"))
                    Debug.WriteLine(item);
            }
        }
    }
}
