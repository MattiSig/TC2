using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TinyChain;

namespace apitest
{
    public class Program
    {
        public static void Main(string[] args)
        {

            test();
            Console.WriteLine("ettaerasinkmaaaarr");
            BuildWebHost(args).Run();
        
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

        public static async void test(){
            await Task.Run(() => BuildWebSocketHost());
        }
        public static void BuildWebSocketHost(){
            for (int i = 0; i < 100; i++)
            {
                    Console.WriteLine("4d3d3d3: " + i);
            }

            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 80);
            server.Start();
            Console.WriteLine("Server has started on 127.0.0.1:80.{0}Waiting for a connection...", Environment.NewLine);

            TcpClient client = server.AcceptTcpClient();

            Console.WriteLine("A client connected.");

            NetworkStream stream = client.GetStream();

            //enter to an infinite cycle to be able to handle every change in stream
            while (true) {
                while (!stream.DataAvailable);
                
                Byte[] bytes = new Byte[client.Available];
               
                stream.Read(bytes, 0, bytes.Length);    
            }
        }
    }
}
