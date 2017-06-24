using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Configuration;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace PT_MessengerServer
{
    class Program
    {
        private TcpListener server {get;set;}
        private ushort port { get;set;}
        private bool online { get;set;}
        private IPAddress ipAdd { get;set;}

        public X509Certificate2 cert { get;set;}
        public List<Client> listOfClients { get;set; }

        static void Main(string[] args)
        {
            Program program = new Program();
            Console.Read();
        }

        public Program()
        {
            Console.Title = "Serwer Messengera - Platformy Technologiczne";
            ipAdd = IPAddress.Parse(ConfigurationManager.AppSettings["ip_address"]);
            port = 8510;
            server=new TcpListener(ipAdd, port);
            cert = new X509Certificate2("messenger.pfx", "PT_Messenger");
            listOfClients = new List<Client>();
            try
            {
                server.Start();
                Console.WriteLine("Serwer - start()");
                online=true;
            }
            catch
            {
                Console.WriteLine("ERROR: TCP Listener nie wystartował");
            }
            Listener();
            
        }
        void Listener()
        {
            while (online)
            {
                
                TcpClient tcpClient = server.AcceptTcpClient();
                Client client = new Client(this,tcpClient);
            
            }
        }
    }
}
