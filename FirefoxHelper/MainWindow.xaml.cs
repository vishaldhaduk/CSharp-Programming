using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Management;
using System.Windows.Threading;
using System.Timers;
using System.Diagnostics;
using System.IO;
using System.Reflection;


namespace FirefoxHelper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ManagementEventWatcher watcherFirefox;
        private WindowFocusDetector windowsFocusWatcher;
        public static readonly TimeSpan PressKeyTimer = new TimeSpan(0, 0, 5);
        public Timer timer;
        public static string WindowName { get; set; }
        public static string Language { get; set; }
        public static string Shortcut { get; set; }

        public string GIFPATH
        {
            get
            {
                return string.Concat(Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory())),
                    "\\progress-animation-final.gif");
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            GetFirefoxVersion();

            //System.Drawing.Image img;

            //img = System.Drawing.Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            //                @".Resources\\progress-animation-final.gif"));

            //img = System.Drawing.Image.FromFile(@"D:\Personal\Files\CSharp-Programming\FirefoxHelper\progress-animation-final.gif");
            //aimg.AnimatedBitmap = (System.Drawing.Bitmap)img;

            //BitmapImage image = new BitmapImage(new Uri(base.BaseUri, @"/Assets/favorited.png"));
            //string wanted_path = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));

            //DisableAutoUpdate();

            //InitializeLanguageRelatedData();

            //InitializeEventSubscription();
        }

        public static string GetFirefoxVersion()
        {
            string path = @"C:\Program Files (x86)\Mozilla Firefox\firefox.exe";
            int bVersion = 46;
            try
            {
                Microsoft.Win32.RegistryKey registry = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\firefox.exe");

                if (registry != null)
                    path = registry.GetValue("").ToString();

                if (path != null)
                {
                    string sVersion = FileVersionInfo.GetVersionInfo(path.ToString()).FileVersion;
                    //bVersion = Int32.Parse(sVersion.Substring(0, sVersion.IndexOf('.')));
                }
                return bVersion.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while getting firefox version during first run : " + ex.Message, ex);
                return bVersion.ToString();
            }
        }

        private void myGif_MediaEnded(object sender, RoutedEventArgs e)
        {
            myGif.Position = new TimeSpan(0,0,1);
            myGif.Play();
        }

        private static void DisableAutoUpdate()
        {
            updateSettingsFile(GetPathOfPrefsFile());
        }

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

                // Need to use a windows watcher for chrome since is creates multiple processes
                windowsFocusWatcher = new WindowFocusDetector();
                watcherFirefox = new ManagementEventWatcher(scope, queryStringFirefox, new EventWatcherOptions { });
                watcherFirefox.EventArrived += FirefoxProcessStarted;
                watcherFirefox.Start();

                Application.Current.Exit += CleanUp;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
        }
        private void FirefoxProcessStarted(object sender, EventArrivedEventArgs e)
        {
            try
            {
                Console.WriteLine("Firefox process has started.");
                ManagementBaseObject targetInstance = (ManagementBaseObject)e.NewEvent.Properties["TargetInstance"].Value;
                string processName = targetInstance.Properties["Name"].Value.ToString();
                if (processName == "firefox.exe")
                {
                    Application.Current.Dispatcher.BeginInvoke(
                         DispatcherPriority.Background,
                         new Action(() =>
                    {
                        //MessageBox.Show("Boom Firefox Started");
                        StartTimerToPressKey();
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
        }
        private void StartTimerToPressKey()
        {
            timer = new Timer(PressKeyTimer.TotalMilliseconds);
            timer.Elapsed += t_Elapsed;
            timer.Start();
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            KeyPressHelper.PressKeyForFF(Shortcut, WindowName);
            timer = new Timer(PressKeyTimer.TotalMilliseconds);
            timer.Start();
        }

        private void CleanUp(object sender, EventArgs args)
        {
            if (watcherFirefox != null)
            {
                watcherFirefox.Stop();
                watcherFirefox.Dispose();
                watcherFirefox = null;
            }
        }

        public static void InitializeLanguageRelatedData()
        {
            string culture = GetCurrentCulture();

            if (culture == "fr")
            {
                Language = "fr";
                Shortcut = "N";
                WindowName = "Restaurer les paramètres de recherche - Mozilla Firefox";
            }
            else if (culture == "de")
            {
                Language = "de";
                Shortcut = "N";
                WindowName = "Wiederherstellen Sucheinstellungen - Mozilla Firefox";
            }
            else if (culture == "es")
            {
                Language = "es";
                Shortcut = "N";
                WindowName = "Restaurar la configuración de búsqueda - Mozilla Firefox";
            }
            else
            {
                Language = "en";
                Shortcut = "D";
                WindowName = "Restore Search Settings - Mozilla Firefox";
            }
        }

        public static string GetCurrentCulture()
        {
            //return "de";
            return "en";
            //return "fr";
            return "es";
        }

        private static void updateSettingsFile(string pathOfPrefsFile)
        {
            string[] contentsOfFile = File.ReadAllLines(pathOfPrefsFile);
            // We are looking for "user_pref("browser.download.useDownloadDir", true);"
            // This needs to be set to:
            // "user_pref("browser.download.useDownloadDir", false);"
            List<String> outputLines = new List<string>();
            foreach (string line in contentsOfFile)
            {
                if (line.StartsWith("user_pref(\"app.update.enabled\""))
                {
                    Console.WriteLine("Found it already in file, replacing");
                }
                else
                {
                    outputLines.Add(line);
                }
            }

            // Finally add the value we want to the end
            outputLines.Add("user_pref(\"app.update.enabled\", false);");
            // Rename the old file preferences for safety...
            File.Move(pathOfPrefsFile, System.IO.Path.GetDirectoryName(pathOfPrefsFile) + @"\" + System.IO.Path.GetFileName(pathOfPrefsFile) + ".OLD." + Guid.NewGuid().ToString());
            // Write the new file.
            File.WriteAllLines(pathOfPrefsFile, outputLines.ToArray());
        }

        private static string GetPathOfPrefsFile()
        {
            // Get roaming folder, and get the profiles.ini
            string iniFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Mozilla\Firefox\profiles.ini";
            // Profiles.ini tells us what folder the preferences file is in.
            string contentsOfIni = File.ReadAllText(iniFilePath);

            int locOfPath = contentsOfIni.IndexOf("Path=Profiles");
            int endOfPath = contentsOfIni.IndexOf(".default", locOfPath);

            int startOfPath = locOfPath + "Path=Profiles".Length + 1;
            int countofCopy = ((endOfPath + ".default".Length) - startOfPath);
            string path = contentsOfIni.Substring(startOfPath, countofCopy);

            string toReturn = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Mozilla\Firefox\Profiles\" + path + @"\prefs.js";
            return toReturn;
        }

        public static bool isFireFoxOpen()
        {
            foreach (Process proc in Process.GetProcesses())
            {
                if (proc.ProcessName == "firefox")
                {
                    return true;
                }
            }
            return false;
        }
    }
}
