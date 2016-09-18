using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

namespace Client
{
    public class ThreadParamter
    {
        public string Id { get; set; }
    }

    class Program
    {
        static string url = "";
        public static bool isRunning = true;

        static void Main(string[] args)
        {
            var threadCount = 100;
            var host = "localhost";
            var port = 15000;
            if (args.Length >= 1)
            {
                var c = 0;
                if (int.TryParse(args[0], out c)) threadCount = c;
            }
            if (args.Length >= 2) host = args[1];
            if (args.Length >= 3)
            {
                var p = 0;
                if (int.TryParse(args[2], out p)) port = p;
            }

            url = $"ws://{host}:{port}";
            for (var i = 0; i < threadCount; i++)
            {
                var username = Guid.NewGuid().ToString().Substring(0, 8);
                var password = username;
                var t = new Thread(new ParameterizedThreadStart(StartClient)) { IsBackground = true };
                var id = i;
                t.Start(id);
                while (!t.IsAlive) Thread.Sleep(10);
                Console.WriteLine($"The {id} client is ready.");
            }

            Console.ReadLine();
            isRunning = false;
            Console.ReadLine();
        }

        public static void StartClient(object id)
        {
            var ws = new ClientWebSocket();
            try { ws.ConnectAsync(new Uri(url), CancellationToken.None).Wait(); }
            catch (Exception)
            {
                Console.WriteLine("connect to websocket server failure.");
            }

            while (isRunning)
            {
                try
                {
                    ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes($"I am {id}")), WebSocketMessageType.Text, true, CancellationToken.None).Wait();
                    Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Send message error : {e.Message}");
                    break;
                }
            }

            ws.CloseAsync(WebSocketCloseStatus.Empty, "", CancellationToken.None);
            Console.WriteLine($"The {id} client is closed.");
        }
    }
}
