using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ares
{
    public class TCPInterpreter
    {
        public static void main(String data)
        {
            if (data.Contains("<HEARTBEAT>"))
            {
                String command = data.Substring(data.IndexOf("<HEARTBEAT>") + 12).Replace("<EOF>","");
                Console.WriteLine("Heartbeat From "+ command);
                Form1 form = new Form1();
                Form1.clients.Add(command);
            }
        }
    }
}
