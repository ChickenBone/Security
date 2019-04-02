using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
            if (data.Contains("<SCREENSHOT>"))
            {
                try
                {
                    data = data.Substring(data.IndexOf("<SCREENSHOT>") + 13).Replace("<EOF>", "");
                    byte[] imageb64 = Convert.FromBase64String(data);
                    using (var stream = new MemoryStream(imageb64, 0, imageb64.Length))
                    {
                        Image image = Image.FromStream(stream);
                        image.Save(System.IO.Path.GetTempPath() + "\\Image.png", ImageFormat.Png);
                        Form2 form = new Form2();
                    }

                    Console.WriteLine("Screenshot Recieved and saved to: " + System.IO.Path.GetTempPath() + "\\Image.png");
                }
                catch
                {
                    Console.WriteLine("ERROR IN SCREENSHOT RECIEVE");
                }
            }
            if (data.Contains("<EXEC>"))
            {
                try
                {
                    String command = data.Substring(data.IndexOf("<EXEC>") + 7).Replace("<EOF>", "");
                    Console.WriteLine(command);
                }
                catch
                {
                    Console.WriteLine("Error in command execution!");
                }
            }
        }
    }
}
