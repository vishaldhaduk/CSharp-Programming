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
using System.Windows.Shapes;
using DevLib;

namespace GUISamples
{
    /// <summary>
    /// Interaction logic for CustomUserControlSample.xaml
    /// </summary>
    public partial class CustomUserControlSample : Window
    {
        public CustomUserControlSample()
        {
            InitializeComponent();
            var control = new DevLib.Controls.UsercontrolSample();
            MainWindowGrid.Children.Add(control);
        }
    }
}
