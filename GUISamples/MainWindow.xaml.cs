using System;
using System.Windows;
using GUISamples.AppCore;
using GUISamples.AppCore.IEvents;
using GUISamples.AppCore.Shape;
using Shape = GUISamples.AppCore.Shape.Shape;

namespace GUISamples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string myTodoList { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            FavLinksView f = new FavLinksView();
            f.ShowDialog();

            //CustomUserControlSample c = new CustomUserControlSample();
            //c.ShowDialog();

            //GetIEvents();

            //GetShapeEvents();

            //GetBasicEvents();

            // Keep the console window open
            //Console.WriteLine("Press Enter to close this window.");
            //Console.ReadLine();

            //RoundProgressBar r = new RoundProgressBar();
            //r.ShowDialog();

            //TextBoxView.Visibility = System.Windows.Visibility.Visible;

            //var a = AssemblyName.GetAssemblyName(@"C:\Users\vishal.dhaduk.LAVASOFT\Documents\Source\Work\WebCompanion\_build\x86\Debug\BrowserDock.exe");
            //var a1 = AssemblyName.GetAssemblyName(@"C:\Users\vishal.dhaduk.LAVASOFT\Documents\Source\Github\bdStandaloneSetup\bin\BDInstaller.exe");
            ////var a2 = AssemblyName.GetAssemblyName(@"C:\Users\vishal.dhaduk.LAVASOFT\Documents\Source\Github\bdStandaloneSetup\bin\1.0.0.40\AAInstaller\BDInstaller.exe");

            //TextBoxHelper t = new TextBoxHelper();
            //t.ShowDialog();

        }

        public void GetIEvents()
        {
            GUISamples.AppCore.IEvents.Shape shape = new GUISamples.AppCore.IEvents.Shape();
            Subscriber1 sub = new Subscriber1(shape);
            Subscriber2 sub2 = new Subscriber2(shape);
            shape.Draw();
        }

        private void GetShapeEvents()
        {
            //Create the event publishers and subscriber
            Circle c1 = new Circle(54);
            Rectangle r1 = new Rectangle(12, 9);
            ShapeContainer sc = new ShapeContainer();

            // Add the shapes to the container.
            sc.AddShape(c1);
            sc.AddShape(r1);

            // Cause some events to be raised.
            c1.Update(57);
            r1.Update(7, 7);

            // Keep the console window open in debug mode.
            System.Console.WriteLine("Press any key to exit.");
            //System.Console.ReadKey();
        }

        private static void GetBasicEvents()
        {
            Publisher pub = new Publisher();
            Subscriber sub1 = new Subscriber("sub1", pub);
            Subscriber sub2 = new Subscriber("sub2", pub);

            // Call the method that raises the event.
            pub.DoSomething();
        }
    }
}

