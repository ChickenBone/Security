using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ares
{
    class UDPHeartBeat
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
        public static void start()
        {
            while (true)
            {
                Ares.UDPHeartBeat c = new UDPHeartBeat();
                c.Send("<SERVERIP> " + Form1.ipAd.ToString());
                c.Send("<SERVERPORT> " + Form1.PortNumber.ToString());

                Thread.Sleep(10000);
            }
        }
        public void Send(String text)
        {
            UdpClient client = new UdpClient();
            IPEndPoint ip = new IPEndPoint(IPAddress.Broadcast, 9732);
            byte[] bytes = Encoding.ASCII.GetBytes(Crypto.Encrypt(text));
            client.Send(bytes, bytes.Length, ip);
            client.Close();
        }
        }
    }

