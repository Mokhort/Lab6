using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Lab6_ryd
{
    public class  Msg {
        public static string N;
        public static string M;
    }
    class Program
    {     
        private const int serverPort = 2365;
        private static TcpClient client;


        static async Task Main(string[] args)
        {
            string num;
            bool flag = true;
            Console.WriteLine("Tcp User start");
            Console.WriteLine("Hello! Enter quantity of num, N =");
            Msg.N = Console.ReadLine();
            if (int.Parse(Msg.N) <=0)
            Console.WriteLine("Invalid value");

            Console.WriteLine("Enter max num to delete, M =");
            Msg.M = Console.ReadLine();
            if (int.Parse(Msg.M) <= 0)
                Console.WriteLine("Invalid value");

            string msg = Msg.N + "and" + Msg.M;

            client = new TcpClient();
            await client.ConnectAsync(IPAddress.Loopback, serverPort);
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);
            await writer.WriteLineAsync(msg);
            await writer.FlushAsync();

            msg = await reader.ReadLineAsync();           
            Console.WriteLine($"Message from server: {msg}");
            int serv_left;
            int step;
            while (flag)
            {
                Console.WriteLine("How many you want to delete? ");
                num = Console.ReadLine();
                if (int.Parse(num) > int.Parse(Msg.M))
                    Console.WriteLine("Invalid value");
                else
                {
                    await writer.WriteLineAsync(num);
                    await writer.FlushAsync();
                    msg = await reader.ReadLineAsync();
                    if (msg == "Finish game!") { 
                        Console.WriteLine($"Message from server: {msg}");
                    goto Finish; 
                    }
                    else {
                        string[] StepandN = msg.Split(",");
                        serv_left = int.Parse(StepandN[0]);
                        step = int.Parse(StepandN[1]);
                        Console.WriteLine($"Server delete {serv_left}, current step = {step}.");
                    }
                }
            }
Finish:
            msg = await reader.ReadLineAsync();
            Console.WriteLine($"Message from server: {msg}");
            client.Close();
            Console.ReadLine();
        }
    }
}
