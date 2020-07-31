using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;

namespace TCP_Client
{
    class Program
    {
        private TcpClient client;
        Thread client_process;
        NetworkStream stream;
        int port = 5005;
        IPAddress ip;

        static void Main(string[] args)
        {
            Program program = new Program();
            string ipstr = "192.168.43.43";
            program.Start();
            program.ip = IPAddress.Parse(ipstr);
            program.IP(program.ip);
            Console.WriteLine(ipstr);
            Console.ReadKey();
        }

        private IPAddress GetExternalIPAddress()
        {
            IPHostEntry myIPHostEntry = Dns.GetHostEntry(Dns.GetHostName());

            foreach (IPAddress myIPAddress in myIPHostEntry.AddressList)
            {
                byte[] ipBytes = myIPAddress.GetAddressBytes();

                if (myIPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    if (!IsPrivateIP(myIPAddress))
                    {
                        return myIPAddress;
                    }
                }
            }

            return null;
        }

        private bool IsPrivateIP(IPAddress myIPAddress)
        {
            if (myIPAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                byte[] ipBytes = myIPAddress.GetAddressBytes();

                // 10.0.0.0/24 
                if (ipBytes[0] == 10)
                {
                    return true;
                }
                // 172.16.0.0/16
                else if (ipBytes[0] == 172 && ipBytes[1] == 16)
                {
                    return true;
                }
                // 192.168.0.0/16
                else if (ipBytes[0] == 192 && ipBytes[1] == 168)
                {
                    return true;
                }
                // 169.254.0.0/16
                else if (ipBytes[0] == 169 && ipBytes[1] == 254)
                {
                    return true;
                }
            }

            return false;
        }


        private bool CompareIpAddress(IPAddress IPAddress1, IPAddress IPAddress2)
        {
            byte[] b1 = IPAddress1.GetAddressBytes();
            byte[] b2 = IPAddress2.GetAddressBytes();

            if (b1.Length == b2.Length)
            {
                for (int i = 0; i < b1.Length; ++i)
                {
                    if (b1[i] != b2[i])
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        void Port(int value)
        {
            port = value;
        }
        
        void IP(IPAddress value)
        {
            ip = value;
        }

        void Start()
        {
            client_process = new Thread(new ThreadStart(Client));
            client_process.Start();
        }

        void Client()
        {
            client = new TcpClient();
            client.Connect(ip, port);
            String info = "Hello Server this is client " + ip;
            Send(info);
        }

        void Send(String info)
        {
            stream = client.GetStream();
            byte[] message = Encoding.ASCII.GetBytes(info);
            stream.Write(message, 0, message.Length);
            stream.Close();
            Console.WriteLine("Message send to " + ip.MapToIPv4().ToString() + " sucessfully.");
        }

        void Close()
        {
            client.Close();
        }
    }
}
