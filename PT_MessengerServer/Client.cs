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
using System.Data.Entity.Validation;
using System.Diagnostics;

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
        private string ans;

        private string loginn;

        public Client(Program _program, TcpClient _tpc_client)
        {
            program = _program; 
            tcp_client = _tpc_client;
            this.loginn="";
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
                binWrite.Write("HELLO");
                ans=binRead.ReadString();
                if (ans == "HELLO")
                {
                    while(ans != "GOODBYE")
                    {
                        ans = binRead.ReadString();
                        switch (ans)
                        {
                            case "REG":
                                registration();
                                break;
                            case "LOG":
                                login();
                                break;
                            case "CON_CHECK":
                                conn_check();
                                break;
                            default:
                                break;
                        }
                    }

                }
                    
            }
            catch(Exception ex)
            {
                Console.WriteLine("{0} Kończenie polaczenia z {1}", DateTime.Now, tcp_client.Client.RemoteEndPoint.ToString());
            }
            finally
            {
                ssl_stream.Close();
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
        public void registration()
        {
            String login, passwd, username, surname,email;
            login = binRead.ReadString();
            passwd = binRead.ReadString();
            username = binRead.ReadString();
            surname = binRead.ReadString();
            email = binRead.ReadString();

            using(var dc = new PTMessengerEntitiesModel())
            {
                var log_count = (from l in dc.TUsers
                            where l.TUsers_login == login
                            select l).Count();
                if (log_count > 0)
                {
                    binWrite.Write("LOGIN_EXIST");
                    return;
                }
                    
            }

            var newUser = new TUsers();
            newUser.TUser_imie = username;
            newUser.TUser_nazwisko = surname;
            newUser.TUsers_login = login;
            newUser.TUsers_passwd = passwd;
            newUser.TUsers_email = email;
            newUser.TUser_lock = false;
            try
            {
                using (var dc = new PTMessengerEntitiesModel())
                {
                    dc.TUsers.Add(newUser);
                    dc.SaveChanges();
                }
                binWrite.Write("REG_SUCCESS");
            } 
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Console.WriteLine("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }


        }
        public void login()
        {
            String login, passwd;
            login=binRead.ReadString();
            passwd=binRead.ReadString();
            using (var dc = new PTMessengerEntitiesModel())
            {
                try
                {
                    var users = (from c in dc.TUsers
                                 where c.TUsers_login == login && c.TUsers_passwd == passwd
                                 select c).Count();
                    if (users > 0)
                    {
                        this.loginn = login;
                        Console.WriteLine("{0} Zalogowano {1}",DateTime.Now,login);
                        binWrite.Write("LOGIN_SUCCESS");
                    }
                    else binWrite.Write("LOGIN_UNSUCCESS");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} {1}",DateTime.Now,ex);
                }
                
            }
            

        }
        public void conn_check()
        {
            this.binWrite.Write(true);
        }
    }
}
