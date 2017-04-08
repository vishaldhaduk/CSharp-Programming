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
using Microsoft.Win32;
using System.Runtime.InteropServices;


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
        private bool _alreadyRunning = false;
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

            //PerformInjectDll();

            InitializeEventSubscription();

        }

        private void PerformInjectDll()
        {
            string tgtProc = "firefox";
            string tgtDll = @"C:\test.dll";

            System.Console.WriteLine("Injecting dll...");
            DllInjector.Inject(tgtProc, tgtDll);
            System.Console.WriteLine("done.");
            return;
        }


        private static List<string> GetWCVersionFromIS()
        {

            string offer = Path.GetTempPath();

            var exeFiles = Directory.GetFiles(Path.GetTempPath(),
                                        "*", SearchOption.AllDirectories)
               .Where(s => s.EndsWith(".exe"))
               .ToList();

            foreach (var item in exeFiles)
            {
                FileInfo f = new FileInfo(item);
                int count = f.Name.Count(d => d == '.');

                if (count == 2)
                {
                    string version = FileVersionInfo.GetVersionInfo(f.FullName.ToString()).FileVersion;

                    if (version == "2.3.1479.2868")
                    {
                        Crc32 crc32 = new Crc32();
                        String hash = String.Empty;

                        using (FileStream fs = File.Open(f.FullName, FileMode.Open))
                            foreach (byte b in crc32.ComputeHash(fs)) hash += b.ToString("x2").ToLower();

                        Console.WriteLine("CRC-32 is {0}", hash);
                    }
                }
            }
            return exeFiles;
        }

        public static void CheckInstallRegistry()
        {
            try
            {
                Debug.WriteLine("Getting installed browser...");
                RegistryKey regKeys;
                //on 64bit the browsers are in a different location
                regKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Mozilla\Mozilla Firefox\");

                if (regKeys == null)
                {
                    string value64 = RegistryWOW6432.GetRegKey64(RegHive.HKEY_LOCAL_MACHINE, @"SOFTWARE\WOW6432Node\WOW6432NodeMozilla\Mozilla Firefox\", "RegisteredOrganization");
                    string value32 = RegistryWOW6432.GetRegKey32(RegHive.HKEY_LOCAL_MACHINE, @"SOFTWARE\WOW6432Node\Mozilla\Mozilla Firefox\", "RegisteredOrganization");
                }

                regKeys = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Mozilla\Mozilla Firefox\");

                string version = regKeys.GetValue("CurrentVersion").ToString();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Could not read the value from registry " + ex.Message);
            }
        }

        public static void GetCultureCode()
        {
            string s1 = "50.0.2 (x86 bs)";
            string s2 = "50.0.2 (x86 en-US)";

            Debug.WriteLine("Index of (:" + s1.IndexOf("("));
            Debug.WriteLine("Index of (:" + s2.IndexOf("("));

            int startIndex1 = s1.IndexOf("(") + 5;
            int startIndex2 = s2.IndexOf("(") + 5;

            int endIndex1 = s1.IndexOf(")");
            int endIndex2 = s2.IndexOf(")");


            int length1 = endIndex1 - startIndex1;
            int length2 = endIndex2 - startIndex2;

            string sub1 = s1.Substring(startIndex1, length1);
            string sub2 = s2.Substring(startIndex2, length2);

            sub2 = sub2.Substring(0, 2);
            sub1 = sub1.Substring(0, 2);

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
            myGif.Position = new TimeSpan(0, 0, 1);
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
                if (!_alreadyRunning)
                {
                    _alreadyRunning = true;
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
                            //PerformInjectDll();

                        }));
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message, ex);
            }
        }

        private void StartTimerToPressKey()
        {
            Process[] p = Process.GetProcessesByName("firefox.exe");

            timer = new Timer(PressKeyTimer.TotalMilliseconds);
            timer.Elapsed += t_Elapsed;
            timer.Start();
        }

        void t_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool resetTimer = KeyPressHelper.PressKeyForFF(Shortcut, WindowName);

            if (resetTimer)
            {
                timer.Stop();
                _alreadyRunning = false;
                Console.WriteLine("reseting the timer...");
                timer = new Timer(PressKeyTimer.TotalMilliseconds);
                timer.Start();
            }

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
                WindowName = "Restaurar la csnonfiguración de búsqueda - Mozilla Firefox";
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


        #region firefox
        public enum RegSAM
        {
            QueryValue = 0x0001,
            SetValue = 0x0002,
            CreateSubKey = 0x0004,
            EnumerateSubKeys = 0x0008,
            Notify = 0x0010,
            CreateLink = 0x0020,
            WOW64_32Key = 0x0200,
            WOW64_64Key = 0x0100,
            WOW64_Res = 0x0300,
            Read = 0x00020019,
            Write = 0x00020006,
            Execute = 0x00020019,
            AllAccess = 0x000f003f
        }

        public static UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(0x80000002u);
        public static UIntPtr HKEY_CURRENT_USER = new UIntPtr(0x80000001u);

        #region Member Variables
        #region Read 64bit Reg from 32bit app
        [DllImport("Advapi32.dll")]
        static extern uint RegOpenKeyEx(
            UIntPtr hKey,
            string lpSubKey,
            uint ulOptions,
            int samDesired,
            out int phkResult);

        [DllImport("Advapi32.dll")]
        static extern uint RegCloseKey(int hKey);

        [DllImport("advapi32.dll", EntryPoint = "RegQueryValueEx")]
        public static extern int RegQueryValueEx(
            int hKey, string lpValueName,
            int lpReserved,
            ref uint lpType,
            System.Text.StringBuilder lpData,
            ref uint lpcbData);
        #endregion
        #endregion

        #region Functions
        static public string GetRegKey64(UIntPtr inHive, String inKeyName, String inPropertyName)
        {
            return GetRegKey64(inHive, inKeyName, RegSAM.WOW64_64Key, inPropertyName);
        }

        static public string GetRegKey32(UIntPtr inHive, String inKeyName, String inPropertyName)
        {
            return GetRegKey64(inHive, inKeyName, RegSAM.WOW64_32Key, inPropertyName);
        }

        static public string GetRegKey64(UIntPtr inHive, String inKeyName, RegSAM in32or64key, String inPropertyName)
        {
            //UIntPtr HKEY_LOCAL_MACHINE = (UIntPtr)0x80000002;
            int hkey = 0;

            try
            {
                uint lResult = RegOpenKeyEx(HKEY_LOCAL_MACHINE, inKeyName, 0, (int)RegSAM.QueryValue | (int)in32or64key, out hkey);
                if (0 != lResult) return null;
                uint lpType = 0;
                uint lpcbData = 1024;
                StringBuilder AgeBuffer = new StringBuilder(1024);
                RegQueryValueEx(hkey, inPropertyName, 0, ref lpType, AgeBuffer, ref lpcbData);
                string Age = AgeBuffer.ToString();
                return Age;
            }
            finally
            {
                if (0 != hkey) RegCloseKey(hkey);
            }
        }
        #endregion
        #endregion

        public static void MethodsCalled()
        {
            //var exeFiles = GetWCVersionFromIS();

            //&& s.Count(c => c == '.') == 2

            //string value64 = GetRegKey64(HKEY_LOCAL_MACHINE, @"SOFTWARE\Mozilla\Mozilla Firefox\", "CurrentVersion");
            //Console.WriteLine(value64);
            //string value32 = GetRegKey32(HKEY_LOCAL_MACHINE, @"SOFTWARE\Mozilla\Mozilla Firefox\", "CurrentVersion");
            //Console.WriteLine(value32);


            //string returnStatus = RegOpenKeyEx(HKEY_LOCAL_MACHINE, @"SOFTWARE\Mozilla\Mozilla Firefox\",0, KEY_ALL_ACCESS, &hKey);

            //GetCultureCode();

            //CheckInstallRegistry();

            //InitializeLanguageRelatedData();


            //GetFirefoxVersion();

            //System.Drawing.Image img;

            //img = System.Drawing.Image.FromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            //                @".Resources\\progress-animation-final.gif"));

            //img = System.Drawing.Image.FromFile(@"D:\Personal\Files\CSharp-Programming\FirefoxHelper\progress-animation-final.gif");
            //aimg.AnimatedBitmap = (System.Drawing.Bitmap)img;

            //BitmapImage image = new BitmapImage(new Uri(base.BaseUri, @"/Assets/favorited.png"));
            //string wanted_path = Path.GetDirectoryName(Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()));

            //DisableAutoUpdate();
        }
    }
}
