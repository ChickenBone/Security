using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class StateObject
{
    public Socket workSocket = null;
    public const int BufferSize = 200000000;
    public byte[] buffer = new byte[BufferSize];
    public StringBuilder sb = new StringBuilder();
}

public class AsynchronousSocketListener
{
    public static ManualResetEvent allDone = new ManualResetEvent(false);
    public AsynchronousSocketListener()
    {
    }

    public static void  StartListening()
    {
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddress = Ares.Form1.ipAd;
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, Ares.Form1.PortNumber);
        Socket listener = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);
            while (true)
            {
                allDone.Reset();
                listener.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    listener);
                allDone.WaitOne();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
        Console.Read();
    }
    public static void AcceptCallback(IAsyncResult ar)
    {
        allDone.Set();
        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);
        StateObject state = new StateObject();
        state.workSocket = handler;
        handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
            new AsyncCallback(ReadCallback), state);
    }

    public static void ReadCallback(IAsyncResult ar)
    {
        try
        {
            String content = String.Empty;
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));
                content = Ares.UDPHeartBeat.Crypto.Decrypt(state.sb.ToString());
                if (content.IndexOf("<EOF>") > -1)
                {
                    Ares.Form1 form = new Ares.Form1();
                    Ares.TCPInterpreter.main(content);
                    Send(handler, content);
                }
                else
                {
                    handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReadCallback), state);
                }

            }
        }
        catch
        {
            
        }
    }

    private static void Send(Socket handler, String data)
    {
        byte[] byteData = Encoding.ASCII.GetBytes(Ares.UDPHeartBeat.Crypto.Encrypt(data));
        handler.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), handler);
    }

    private static void SendCallback(IAsyncResult ar)
    {
        try
        {
            Socket handler = (Socket)ar.AsyncState;
            int bytesSent = handler.EndSend(ar);
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }
}