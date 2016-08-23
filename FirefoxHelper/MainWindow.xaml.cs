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
using System.Windows.Shapes;
using System.Management;
using System.Windows.Threading;
using System.Timers;


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
        public MainWindow()
        {
            InitializeComponent();

            InitializeLanguageRelatedData();

            InitializeEventSubscription();
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
    }
}
