using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Dining
{
    public class DiningServer: BackgroundService
    {
        public static HttpListener listener;
        public static string url = "http://localhost:8000/";
        public static int pageViews = 0;
        public static int requestCount = 0;
        public static string pageData = "Ok, you got it";


        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {
                Console.WriteLine("Listening...");
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                // Print out some info about the request
                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(req.Url.ToString());
                Console.WriteLine(req.HttpMethod);
                Console.WriteLine(req.UserHostName);
                Console.WriteLine(req.UserAgent);
                Console.WriteLine();

                // If `shutdown` url requested w/ POST, then shutdown the server after serving the page
                if ((req.HttpMethod == "POST") && (req.Url.AbsolutePath == "/shutdown"))
                {
                    Console.WriteLine("Shutdown requested");
                    runServer = false;
                }

                // Write the response info
                string disableSubmit = !runServer ? "disabled" : "";
                byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, pageViews, disableSubmit));
                resp.ContentType = "text";
                resp.ContentEncoding = Encoding.UTF8;
                resp.ContentLength64 = data.LongLength;

                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }

        public async Task SendOrder(Waiter waiter, Order order, Table table)
        {
            using (var client = new HttpClient())
            {
                /*var message = JsonSerializer.Serialize(order);
                var response = client.PostAsync("http://localhost:4000", new StringContent(message, Encoding.UTF8, "application/json")).GetAwaiter().GetResult();
                if (response.StatusCode == HttpStatusCode.OK)
                    Logger.Log($"Order {order.Id} was sent.");
                else
                    Logger.Log($"Order {order.Id} failed to send.");*/
                await Task.Run(() =>
                {
                    Logger.Log($"Waiter {waiter.Id} start sending Order {order.Id}");
                    waiter.State = WaiterState.SendingOrderToKitchen;
                    Thread.Sleep(1000);
                    Logger.Log($"Order {order.Id} was sent.");
                    waiter.State = WaiterState.WaitingForOrder;
                    table.GetClients();
                });
            }

           
            
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests
            await HandleIncomingConnections();

            // Close the listener
            listener.Close();
        }
    }
}
