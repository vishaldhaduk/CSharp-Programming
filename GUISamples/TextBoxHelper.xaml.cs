using System.Windows;
using System.Windows.Controls;

namespace GUISamples
{
    /// <summary>
    /// Interaction logic for TextBoxHelper.xaml
    /// </summary>
    public partial class TextBoxHelper : Window
    {
        public TextBoxHelper()
        {
            InitializeComponent();
            //LabelTest.Content = "Content from code behind";
        }

        private void TextSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            var value = (TextBox)sender;
            LabelTest.Content = value.Text;
        }

    }
}
