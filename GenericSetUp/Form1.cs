using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GenericSetUp
{
    public partial class Form1 : Form
    {
        private Thread newThread;
        public Form1()
        {
            InitializeComponent();
        }

        // Programs the button to throw an exception when clicked.
        private void button1_Click(object sender, System.EventArgs e)
        {
            throw new ArgumentException("The parameter was invalid");
        }

        // Start a new thread, separate from Windows Forms, that will throw an exception.
        private void button2_Click(object sender, System.EventArgs e)
        {
            ThreadStart newThreadStart = new ThreadStart(newThread_Execute);
            newThread = new Thread(newThreadStart);
            newThread.Start();
        }

        // The thread we start up to demonstrate non-UI exception handling. 
        void newThread_Execute()
        {
            throw new Exception("The method or operation is not implemented.");
        }

    }
}
