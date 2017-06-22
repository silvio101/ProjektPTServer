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

        private string login;

        public Client(Program _program, TcpClient _tpc_client)
        {
            program = _program; 
            tcp_client = _tpc_client;
            this.login="";
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
                                logMe();
                                break;
                            case "CON_CHECK":
                                conn_check();
                                break;
                            case "GET_INFO":
                                getUserInfo();
                                break;
                            case "SET_INFO":
                                setUserInfo();
                                break;
                            case "PASS_CHANGE":
                                passChange();
                                break;
                            case "SEARCH_PERSON":
                                searchPerson();
                                break;
                            default:
                                break;
                        }
                    }

                }
                    
            }
            catch
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
                    Console.WriteLine("{0} Nowy uzytkownik chcial uzyc istniejacego loginu {1}",DateTime.Now,login);
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
                Console.WriteLine("{0} Dodano nowego uzytkownika {1}, {2}, {3}, {4}",DateTime.Now,username,surname,login,email);
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
        public void logMe()
        {
            String login, passwd;
            login=binRead.ReadString();
            passwd=binRead.ReadString();
            try
            {
                using (var dc = new PTMessengerEntitiesModel())
                {
                    try
                    {
                        var user = (from c in dc.TUsers
                                     where c.TUsers_login == login
                                     select c).FirstOrDefault();
                        if (user!=null)
                        {
                            if (user.TUsers_passwd==passwd)
                            {
                                saveLoginInfo(user,true);
                                this.login = login;
                                Console.WriteLine("{0} Zalogowano {1}", DateTime.Now, login);
                                binWrite.Write("LOGIN_SUCCESS");
                            }
                            else
                            {
                                saveLoginInfo(user,false);
                                binWrite.Write("LOGIN_UNSUCCESS");
                            }
                        }
                        else 
                        {
                            binWrite.Write("LOGIN_UNSUCCESS");
                        }
                        binWrite.Flush();
                        
                    }
                    catch (Exception ex)
                    {
                        binWrite.Write("500_ERROR");
                        Console.WriteLine("{0} {1}", DateTime.Now, ex);
                        throw new Exception();
                    }
                }
            }
            catch
            {
                Console.WriteLine("{0} Blad SQL",DateTime.Now);
                throw new Exception();
            }
            
        }
        public void saveLoginInfo(TUsers user, bool correct)
        {
            TLastLogin ll = new TLastLogin();
            using (var dcx = new PTMessengerEntitiesModel())
            {
                try
                {
                    TUsers tmpUser = user;
                    DateTime dateTime1H = DateTime.Now.AddHours(-1);
                    var failLoginCount = (from c in dcx.TLastLogin
                                            where c.TLastLogin_TS>dateTime1H && c.TLastLogin_Success == false && c.TLastLogin_TUsers_id == user.TUsers_id
                                            select c).Count();
                    if(failLoginCount>=3)
                    {
                        tmpUser.TUser_lock = true;
                        binWrite.Write("LOGIN_DENY");
                    }
                    else
                        tmpUser.TUser_lock = false;

                    dcx.Entry(tmpUser).State = System.Data.Entity.EntityState.Modified;       
                    
                    ll.TLastLogin_TUsers_id = user.TUsers_id;
                    var ip_add = tcp_client.Client.RemoteEndPoint.ToString().Split(':');
                    ll.TLastLogin_UserIP = ip_add[0];
                    ll.TLastLogin_TS = DateTime.Now;
                    ll.TLastLogin_Success = correct;
                    
                    dcx.TLastLogin.Add(ll);
                    dcx.SaveChanges();
                    
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
        }

        public void getUserInfo()
        {
            using(var dc = new PTMessengerEntitiesModel())
            {
                try
                {
                    var uinfo = (from u in dc.TUsers
                                where u.TUsers_login == this.login
                                select u).First();
                    if(uinfo!=null)
                    {
                        binWrite.Write(uinfo.TUsers_login);
                        binWrite.Write(uinfo.TUser_imie);
                        binWrite.Write(uinfo.TUser_nazwisko);
                        binWrite.Write(uinfo.TUsers_email);
                        binWrite.Flush();
                        Console.WriteLine("{0} Uzytkownik {1} pobral dane {2}, {3}, {4}",DateTime.Now,uinfo.TUsers_login,uinfo.TUser_imie,uinfo.TUser_nazwisko,uinfo.TUsers_email);
                    }
                }
                catch(Exception ex)
                {
                    Console.WriteLine("{0} {1}", DateTime.Now, ex);
                }
            }
        }
        private void setUserInfo()
        {
            TUsers tmpUser;
            try
            {
                using (var dc = new PTMessengerEntitiesModel())
                {
                    tmpUser = dc.TUsers.Where(u => u.TUsers_login == this.login).First();
                }
                if (tmpUser != null)
                {
                    tmpUser.TUser_imie = binRead.ReadString();
                    tmpUser.TUser_nazwisko = binRead.ReadString();
                    tmpUser.TUsers_email = binRead.ReadString();
                }
                using (var dc = new PTMessengerEntitiesModel())
                {
                    dc.Entry(tmpUser).State = System.Data.Entity.EntityState.Modified;
                    dc.SaveChanges();
                }
                binWrite.Write("CHANGE_SUCCESS");
                Console.WriteLine("{0} Uzytkownik {1} zmienil dane na: {2}, {3}, {4}",DateTime.Now, this.login, tmpUser.TUser_imie, tmpUser.TUser_nazwisko, tmpUser.TUsers_email);
            }
            catch(Exception ex)
            {
                Console.WriteLine("{0} {1}",DateTime.Now, ex);
            }
            
        }
        private void passChange()
        {
            TUsers tmpUser;
            try
            {
                using (var dc = new PTMessengerEntitiesModel())
                {
                    tmpUser = dc.TUsers.Where(u => u.TUsers_login == this.login).First();
                }
                if (tmpUser != null)
                {
                    tmpUser.TUsers_passwd = binRead.ReadString();
                }
                using (var dc = new PTMessengerEntitiesModel())
                {
                    dc.Entry(tmpUser).State = System.Data.Entity.EntityState.Modified;
                    dc.SaveChanges();
                }
                binWrite.Write("CHANGE_PASS_SUCCESS");
                Console.WriteLine("{0} Uzytkownik {1} zmienil haslo", DateTime.Now, this.login);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} {1}", DateTime.Now, ex);
            }
        }
        public void searchPerson()
        {
            string login=binRead.ReadString();
            string imie=binRead.ReadString();
            string nazwisko=binRead.ReadString();
            string email=binRead.ReadString();
            
            
            using (var dc = new PTMessengerEntitiesModel())
            {
                try
                {
                    
                    IQueryable<TUsers> query = dc.TUsers;
                    if(!String.IsNullOrEmpty(login)) query = query.Where(row => row.TUsers_login == login);
                    if (!String.IsNullOrEmpty(imie)) query = query.Where(row => row.TUser_imie == imie);
                    if (!String.IsNullOrEmpty(nazwisko)) query = query.Where(row => row.TUser_nazwisko == nazwisko);
                    if (!String.IsNullOrEmpty(email)) query = query.Where(row => row.TUsers_email == email);
                    var searchUsers = query.ToList();
                    
                    /*
                    IEnumerable<TUsers> query = dc.TUsers;
                    var searchUsers = query
                            .Where(x=>x.TUsers_login == login       || String.IsNullOrEmpty(x.TUsers_login))
                            .Where(x=>x.TUser_imie == imie          || String.IsNullOrEmpty(x.TUser_imie))
                            .Where(x=>x.TUser_nazwisko == nazwisko  || String.IsNullOrEmpty(x.TUser_nazwisko))
                            .Where(x=>x.TUsers_email == email       || String.IsNullOrEmpty(x.TUsers_email))
                            .ToList();
                   */
                   binWrite.Write(searchUsers.Count);
                   foreach(var p in searchUsers)
                   {
                       binWrite.Write(p.TUsers_login);
                       binWrite.Write(p.TUser_imie);
                       binWrite.Write(p.TUser_nazwisko);
                       binWrite.Write(p.TUsers_email);
                       binWrite.Flush();
                   }
                   Console.WriteLine("{0} Wysłano dane o wyszukiwaniu do uzytkownika {1}, wyszukana ilosc: {2}", DateTime.Now, this.login, searchUsers.Count);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} {1}", DateTime.Now, ex);
                }
            }
        }
        public void conn_check()
        {
            this.binWrite.Write(true);
        }
    }
}
