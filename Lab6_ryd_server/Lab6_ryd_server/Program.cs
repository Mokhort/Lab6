using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Lab6_ryd_server
{
    class Program
    {
        private static TcpListener listener;
        private const int serverPort = 2365;
        private static bool flag;

        static async Task Main(string[] args)
        {
            Console.WriteLine("Server start");
            listener = new TcpListener(IPAddress.Any, serverPort);
            flag = true;
            await Listen();
        }

        private static int count_res(int M){
            var x = new Random(M);
            int serv_left = x.Next(1, M);
            Console.WriteLine($"x  {serv_left}");
            return serv_left;
        }
        private static async Task Listen()
        {
            listener.Start();
            TcpClient client = await listener.AcceptTcpClientAsync();
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream);

            
            int num, N, M;
            string msg = await reader.ReadLineAsync();
            string[] MandN = msg.Split("and");
            Console.WriteLine($"Message from client: N = {MandN[0]} M = {MandN[1]}");
            N = int.Parse(MandN[0]);
            M = int.Parse(MandN[1]);
            await writer.WriteLineAsync("Start the game");
            await writer.FlushAsync();

            int step=N;
            int new_step;
            int serv_left;

            while (flag)
            {
                msg = await reader.ReadLineAsync();
                num = int.Parse(msg);
                step = step - num;
                Console.WriteLine($"step {step} ");  

                if (step == 0)
                {
                    flag = false;
                    await writer.WriteLineAsync("Finish game!");
                    await writer.FlushAsync();
                    await writer.WriteLineAsync("Congratulates, your win!");
                    await writer.FlushAsync();
                }
                else if (step <= M)
                {
                    flag = false;
                    await writer.WriteLineAsync("Finish game!");
                    await writer.FlushAsync();
                    serv_left = step;
                    step = step - serv_left;
                    await writer.WriteLineAsync($"Oooops, Server win! Server delete {serv_left}, current step = {step}. You have no choice");
                    await writer.FlushAsync();
                }
                else
                {   //num - x = y where y mod M+1 == 0
                    //delete x     x = num-y

                    serv_left = count_res(M);
                    new_step = step - serv_left;
                    if (new_step % (M + 1) == 0)
                    {
                        step = new_step;
                        Console.WriteLine($"step first mod =0 ");
                        await writer.WriteLineAsync($"{serv_left},{step}");
                        await writer.FlushAsync();
                    }
                    else
                    {
                        Console.WriteLine($"step second mod !=0 ");
                        serv_left = count_res(M);
                        step = step - serv_left;
                        await writer.WriteLineAsync($"{serv_left},{step}");
                        await writer.FlushAsync();

                    }
                }
            }
            listener.Stop();
        }
    }
}
