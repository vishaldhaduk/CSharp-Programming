using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;

namespace GUISamples
{
    /// <summary>
    /// Interaction logic for RoundProgressBar.xaml
    /// </summary>
    public partial class RoundProgressBar : Window ,INotifyPropertyChanged
    {
        public RoundProgressBar()
        {
            this.DataContext = this;

            InitializeComponent();
        }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        protected virtual void OnPropertyChanged(string prop)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private double pctComplete = 10.0;
        public double PctComplete
        {
            get { return pctComplete; }
            set
            {
                if (pctComplete != value)
                {
                    pctComplete = value;
                    OnPropertyChanged("PctComplete");
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PctComplete = 0.0;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += (s, ea) =>
            {
                PctComplete += 1.0;
                if (PctComplete >= 100.0)
                    timer.Stop();
            };
            timer.Interval = new TimeSpan(0, 0, 0, 0, 30);  // 2/sec
            timer.Start();
        }
    }
}
