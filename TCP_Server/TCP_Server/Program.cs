using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Net.NetworkInformation;

namespace TCP_Server
{
    class Program
    {
        private TcpListener server;
        Thread server_process;
        int port = 8082;
        bool isAvailable = true;
        static void Main(string[] args)
        { 
            Program program = new Program();
            program.GetIpAddressList(Dns.GetHostName());
            program.Start();
        }

        void check_port()
        {
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation tcpi in tcpConnInfoArray)
            {
                if (tcpi.LocalEndPoint.Port == port)
                {
                    isAvailable = false;
                    break;
                }
            }

        }

        public void GetIpAddressList(String hostString)
        {
            try
            {
                // Get 'IPHostEntry' object containing information like host name, IP addresses, aliases for a host.
                IPHostEntry hostInfo = Dns.GetHostByName(hostString);
                Console.WriteLine("Host name : " + hostInfo.HostName);
                Console.WriteLine("IP address List : ");
                for (int index = 0; index < hostInfo.AddressList.Length; index++)
                {
                    Console.WriteLine(hostInfo.AddressList[index]);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception caught!!!");
                Console.WriteLine("Source : " + e.Source);
                Console.WriteLine("Message : " + e.Message);
            }
        }

        void Port(int value)
        {
            port = value;
        }

        void Start()
        {
            server_process = new Thread(new ThreadStart(Server));
            server_process.Start();
        }

        void Server()
        {
            try
            {
                server = new TcpListener(IPAddress.Any, port);
                server.Start();

                while (true)
                {
                    Console.WriteLine("Waiting for connection dear..\n");
                    TcpClient client = server.AcceptTcpClient();
                    Thread client_process = new Thread(new ParameterizedThreadStart(client_handler));
                    client_process.Start(client);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.ReadKey();
            }
        }

        private void client_handler(object client)
        {
            TcpClient mClient = (TcpClient)client;
            NetworkStream stream = mClient.GetStream();
            byte[] message = new byte[1024];
            stream.Read(message, 0, message.Length);
            Console.WriteLine("Message:- " + Encoding.ASCII.GetString(message));
        }
    }
}
