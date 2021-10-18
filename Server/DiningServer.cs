using Dining.Infrastructure.Utils;
using Dining.Models;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Dining.Server
{
    public class DiningServer
    {
        private static HttpListener listener;
        private static string receiveUrl = "http://localhost:8001/";
        private static string sendUrl = "http://localhost:8000/";
        
        private Dining dining;


        public async Task HandleIncomingConnections()
        {
            bool runServer = true;

            while (runServer)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    Console.WriteLine("Shutdown requested");
                    runServer = false;
                }

                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/ready"))
                {
                    using (var reader = new StreamReader(req.InputStream, req.ContentEncoding))
                    {
                        Order order = JsonSerializer.Deserialize<Order>(reader.ReadToEnd());
                        dining.ServeOrder(order);
                    }
                }

                resp.StatusCode = 200;
                resp.Close();
            }
        }

        public void SendOrder(Waiter waiter, Order order, Table table)
        {
            using (var client = new HttpClient())
            {
                var message = JsonSerializer.Serialize(order);
                var response = client.PostAsync(sendUrl + "order", new StringContent(message, Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Logger.Log($"Order {order.Id} was sent.");
                    waiter.State = WaiterState.WaitingForOrder;
                }
            }
        }


        public async Task StartAsync(Dining dining)
        {
            this.dining = dining;

            listener = new HttpListener();
            listener.Prefixes.Add(receiveUrl);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", receiveUrl);

            // Handle requests
            await HandleIncomingConnections();

            // Close the listener
            listener.Close();
        }
    }
}
