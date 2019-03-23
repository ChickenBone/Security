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
        public static IPAddress ipAd = IPAddress.Parse("192.168.0.45");
        public static int PortNumber = 3456;
        public static List<string> clients = new List<string>();
        
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
            Console.WriteLine("[i] Welcome To Ares");
            Task.Factory.StartNew(() => AsynchronousSocketListener.StartListening());
            Task.Factory.StartNew(() => start());
            Task.Factory.StartNew(() => reset());
            Task.Factory.StartNew(() => UDPHeartBeat.start());
            textBox1.Text = ipAd.ToString();
            PortBox.Text = PortNumber.ToString();
        }
        public static void reset()
        {
            while (true)
            {
                clients = new List<string>();
                Thread.Sleep(30000);
            }
        }
        public static bool CheckConnection(IPAddress IP)
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.Connect(IP, PortNumber);
                    Console.WriteLine("Connecting");
                    return true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Cannot Connect");
                    Thread.Sleep(200);
                    return false;
                }
            }
        }
        void start()
        {
            while (true) {
                try
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        clients = clients.Distinct().ToList();
                        listBox1.DataSource = null;
                        listBox1.DataSource = clients;
                    }));
                }catch(Exception e)
                {
                    Console.WriteLine(e);
                }
                Thread.Sleep(1000);
            }
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

        private void bindingSource1_CurrentChanged(object sender, EventArgs e)
        {

        }

        private void logBox_TextChanged(object sender, EventArgs e)
        {
            logBox.SelectionStart = logBox.TextLength;
            logBox.ScrollToCaret();
        }
    }
    public static class threadsafe
    {
        public static void AddItemThreadSafe(this System.Windows.Forms.ListBox lb, object item)
        {
            if (lb.InvokeRequired)
            {
                lb.Invoke(new MethodInvoker(delegate
                {
                    lb.Items.Add(item);
                    lb.TopIndex = Math.Max(lb.Items.Count - lb.ClientSize.Height / lb.ItemHeight + 1, 0);
                    lb.Refresh();
                }));
            }
            else
            {
                lb.Items.Add(item);
                lb.TopIndex = Math.Max(lb.Items.Count - lb.ClientSize.Height / lb.ItemHeight + 1, 0);
                lb.Refresh();
            }
        }
    }
}
