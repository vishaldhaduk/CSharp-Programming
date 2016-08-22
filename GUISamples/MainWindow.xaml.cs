using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            //TextBoxView.Visibility = System.Windows.Visibility.Visible;

            var a = AssemblyName.GetAssemblyName(@"C:\Users\vishal.dhaduk.LAVASOFT\Documents\Source\Work\WebCompanion\_build\x86\Debug\BrowserDock.exe");
            var a1 = AssemblyName.GetAssemblyName(@"C:\Users\vishal.dhaduk.LAVASOFT\Documents\Source\Github\bdStandaloneSetup\bin\BDInstaller.exe");
            //var a2 = AssemblyName.GetAssemblyName(@"C:\Users\vishal.dhaduk.LAVASOFT\Documents\Source\Github\bdStandaloneSetup\bin\1.0.0.40\AAInstaller\BDInstaller.exe");
            
            TextBoxHelper t = new TextBoxHelper();
            t.ShowDialog();

        }
    }
}

