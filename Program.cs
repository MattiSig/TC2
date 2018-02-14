using System;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TinyChain;

namespace apitest
{
    public class Program
    {   
        public static bool listening = true;
        public static void Main(string[] args)
        {

            StartListener();
            Console.WriteLine("ettaerasinkmaaaarr");
            BuildWebHost(args).Run();
        
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

        public static async void StartListener(){
            TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 80);
            server.Start();
            Console.WriteLine("Server has started on 127.0.0.1:80.{0}Waiting for a connection...", Environment.NewLine);
            while(listening){
                TcpClient client = await server.AcceptTcpClientAsync().ConfigureAwait(false);//non blocking waiting                    
                // We are already in a new task to handle this client...
                ThreadPool.QueueUserWorkItem(HandleClient, client);
                //HandleClient(client);
            }
            //await Task.Run(() => BuildWebSocketHost());
        }

        public static async void HandleClient(object obj){
            
            var client = (TcpClient)obj;

            Console.WriteLine("A client connected.");
            
            while(true){
                NetworkStream stream = client.GetStream();

                while (!stream.DataAvailable);

                Byte[] bytes = new Byte[client.Available];

                await stream.ReadAsync(bytes, 0, bytes.Length);

                //translate bytes of request to string

                String data = Encoding.UTF8.GetString(bytes);
                
                if (Regex.IsMatch(data, "^GET")) {
                    Byte[] response = Encoding.UTF8.GetBytes("HTTP/1.1 101 Switching Protocols" + Environment.NewLine
                        + "Connection: Upgrade" + Environment.NewLine
                        + "Upgrade: websocket" + Environment.NewLine
                        + "Sec-WebSocket-Accept: " + Convert.ToBase64String (
                            SHA1.Create().ComputeHash (
                                Encoding.UTF8.GetBytes (
                                    new Regex("Sec-WebSocket-Key: (.*)").Match(data).Groups[1].Value.Trim() + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"
                                )
                            )
                        ) + Environment.NewLine
                        + Environment.NewLine);

                    stream.Write(response, 0, response.Length);
                } else {
                    Console.WriteLine(bytes.Length);
                    string cliData = GetDecodedData(bytes, bytes.Length);
                    Console.WriteLine(cliData);

                    string message = "alvöru pínu success!";

                    List<byte> lb = new List<byte>();
                    lb = new List<byte>();
                    lb.Add(0x81);
                    int size = message.Length;//get the message's size
                    lb.Add((byte)size); //get the size in bytes
                    lb.AddRange(Encoding.UTF8.GetBytes(message));
                    await stream.WriteAsync(lb.ToArray(), 0, size+2); //I do size+2 because we have 2 bytes plus 0x81 and (byte)size

                }
                //stream.Close();
            }

        }

        public static string GetDecodedData(byte[] buffer, int length)
        {
            byte b = buffer[1];
            int dataLength = 0;
            int totalLength = 0;
            int keyIndex = 0;

            if (b - 128 <= 125)
            {
                dataLength = b - 128;
                keyIndex = 2;
                totalLength = dataLength + 6;
            }

            if (b - 128 == 126)
            {
                dataLength = BitConverter.ToInt16(new byte[] { buffer[3], buffer[2] }, 0);
                keyIndex = 4;
                totalLength = dataLength + 8;
            }

            if (b - 128 == 127)
            {
                dataLength = (int)BitConverter.ToInt64(new byte[] { buffer[9], buffer[8], buffer[7], buffer[6], buffer[5], buffer[4], buffer[3], buffer[2] }, 0);
                keyIndex = 10;
                totalLength = dataLength + 14;
            }

            if (totalLength > length)
                throw new Exception("The buffer length is small than the data length");

            byte[] key = new byte[] { buffer[keyIndex], buffer[keyIndex + 1], buffer[keyIndex + 2], buffer[keyIndex + 3] };

            int dataIndex = keyIndex + 4;
            int count = 0;
            for (int i = dataIndex; i < totalLength; i++)
            {
                buffer[i] = (byte)(buffer[i] ^ key[count % 4]);
                count++;
            }

            return Encoding.UTF8.GetString(buffer, dataIndex, dataLength);
        }
    }
}
