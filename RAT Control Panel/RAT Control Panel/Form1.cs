using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ares
{
    public partial class Form1 : Form
    {
        static IPAddress ipAd = IPAddress.Parse("192.168.0.45");
        static int PortNumber = 3456;
        TcpListener ServerListener = new TcpListener(ipAd, PortNumber);
        TcpClient clientSocket = default(TcpClient);
        public Form1()
        {
            InitializeComponent();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Console.SetOut(new ControlWriter(logBox));
            listBox1.Items.Add("192.168.0.45");
            Console.WriteLine("[i] Welcome To Ares");
            Task.Factory.StartNew(() => AsynchronousSocketListener.StartListening());
        }
        public class ControlWriter : TextWriter
        {
            private Control textbox;
            public ControlWriter(Control textbox)
            {
                this.textbox = textbox;
            }

            public override void Write(char value)
            {
                textbox.Invoke(new Action(() => textbox.Text += value));
            }

            public override void Write(string value)
            {
                textbox.Invoke(new Action(() => textbox.Text += value));
            }

            public override Encoding Encoding
            {
                get { return Encoding.ASCII; }
            }
        }
    }
}
