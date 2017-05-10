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

namespace GUISamples
{
    /// <summary>
    /// Interaction logic for FavLinksView.xaml
    /// </summary>
    public partial class FavLinksView : Window
    {
        public FavLinksView()
        {
            InitializeComponent();
        }
        CollectionView myView;
        private void AddGrouping(object sender, RoutedEventArgs e)
        {
            myView = (CollectionView)CollectionViewSource.GetDefaultView(myItemsControl.ItemsSource);
            if (myView.CanGroup == true)
            {
                PropertyGroupDescription groupDescription
                    = new PropertyGroupDescription("@Type");
                myView.GroupDescriptions.Add(groupDescription);
            }
            else
                return;
        }

        private void RemoveGrouping(object sender, RoutedEventArgs e)
        {
            myView = (CollectionView)CollectionViewSource.GetDefaultView(myItemsControl.ItemsSource);
            myView.GroupDescriptions.Clear();
        }
    }
}
