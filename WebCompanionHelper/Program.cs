using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using WebCompanionHelper.Core;

namespace WebCompanionHelper
{
    class Program
    {
        static void Main(string[] args)
        {

            Kill();
            //InstallerProcessHelper.GetInstallerProcess();

            //Process[] p = Process.GetProcessesByName("chrome");


            //for (int i = 0; i < p.Count(); i++)
            //{
            //    ProcessUtilities.KillProcessTree(p[i]);
            //}

            //ProcessExecWaitDemo();
        }

        private static void ProcessExecWaitDemo()
        {
            string path = (Path.Combine(Path.GetTempPath(), "WcInstaller"));
            String DownloadUrl = "https://webcompanion.com/nano_download.php?partner=AG160601&Silent&homepage=12&search=2";
            var dest = Path.GetTempPath() + "WcInstaller.exe";
            TimeSpan _timeout = TimeSpan.FromMinutes(2);

            DownloadToLocation(new Uri(DownloadUrl),
                                           dest, (int)_timeout.TotalMilliseconds);
            Process.Start(dest);

        }

        internal static void DownloadToLocation(Uri WebLocation, string fileLocation, int timeout)
        {
            // create all folders to the file
            Directory.CreateDirectory(Path.GetDirectoryName(fileLocation));

            using (var webClient = new WebClient())
            using (var stream = webClient.OpenRead(WebLocation))
            {
                if (stream != null)
                {
                    using (var file = File.OpenWrite(fileLocation))
                    {
                        // If we had use .net 4 framework, all the following lines could be replaced by:
                        // stream.CopyTo(file)

                        var buffer = new byte[1024];
                        int bytesReceived = 0;
                        stream.ReadTimeout = timeout;

                        while ((bytesReceived = stream.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            file.Write(buffer, 0, bytesReceived);
                        }
                    }
                }
            }
        }

        public static void Kill()
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo("taskkill", "/F /T /IM chrome.exe")
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = System.AppDomain.CurrentDomain.BaseDirectory,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                Process.Start(processStartInfo);
            }
            catch { }
        }
    }
}
