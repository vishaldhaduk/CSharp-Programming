using System.Windows;

namespace GUISamples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            RoundProgressBar r = new RoundProgressBar();
            r.ShowDialog();

            //TextBoxView.Visibility = System.Windows.Visibility.Visible;

            //var a = AssemblyName.GetAssemblyName(@"C:\Users\vishal.dhaduk.LAVASOFT\Documents\Source\Work\WebCompanion\_build\x86\Debug\BrowserDock.exe");
            //var a1 = AssemblyName.GetAssemblyName(@"C:\Users\vishal.dhaduk.LAVASOFT\Documents\Source\Github\bdStandaloneSetup\bin\BDInstaller.exe");
            ////var a2 = AssemblyName.GetAssemblyName(@"C:\Users\vishal.dhaduk.LAVASOFT\Documents\Source\Github\bdStandaloneSetup\bin\1.0.0.40\AAInstaller\BDInstaller.exe");

            //TextBoxHelper t = new TextBoxHelper();
            //t.ShowDialog();

        }
    }
}

