using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Net.Sockets;
using System.Threading;
using System.Net.Security;
using System.Security.Authentication;
using System.IO;

namespace PT_MessengerServer
{
    class Client
    {
        private Thread tcpThread;
        private Program program; 
        private TcpClient tcp_client;
        private NetworkStream net_stream;
        private SslStream ssl_stream;
        private BinaryWriter binWrite;
        private BinaryReader binRead;

        public Client(Program _program, TcpClient _tpc_client)
        {
            program = _program; 
            tcp_client = _tpc_client;

            tcpThread = new Thread(new ThreadStart(ConnectInit));
            tcpThread.Start();
        }

        public  void ConnectInit()
        {
            try
            {
                try
                {
                    net_stream = tcp_client.GetStream();
                    ssl_stream = new SslStream(net_stream, false);
                    ssl_stream.AuthenticateAsServer(program.cert, false, SslProtocols.Tls, true);
                    binRead = new BinaryReader(ssl_stream, Encoding.UTF8);
                    binWrite = new BinaryWriter(ssl_stream, Encoding.UTF8);
                    Console.WriteLine("{0} Polaczono z {1}", DateTime.Now, tcp_client.Client.RemoteEndPoint.ToString());
                }
                catch
                {
                    Console.WriteLine("{0} Nie mozna utworzyc polaczenia", DateTime.Now);
                }
                try
                {
                    binWrite.Write("HELLO");
                }
                catch
                {

                } 
            }
            catch
            {
                

            }
            

        }


        public void testList()
        {
            using (var dc = new PTMessengerEntitiesModel())
            {
                var users = (from c in dc.TUsers
                            select c.TUsers_login).ToList();

                foreach(var r in users)
                    Console.WriteLine(r.ToString());
            }
        }
    }
}
