using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Realtime.MetroLine.Entity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Realtime.Server.MetroLine
{
    public class WebSocketMiddleware : IMiddleware
    {
     
        private static bool ServerIsRunning = true;
        private static CancellationTokenRegistration AppFinalizer;

        public WebSocketMiddleware(IHostApplicationLifetime hostLifetime)
        {
            if (AppFinalizer.Token.Equals(CancellationToken.None))
                AppFinalizer = hostLifetime.ApplicationStopping.Register(ApplicationShutdownHandler);
        }

        public static async void ApplicationShutdownHandler()
        {}
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                if (ServerIsRunning)
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket socket = await context.WebSockets.AcceptWebSocketAsync();
                        await ProcessRequest(context, socket);
                    }
                    else
                    {
                        if (context.Request.Headers["Accept"][0].Contains("text/html"))
                        {
                            Console.WriteLine("Sending HTML to client.");
                            await context.Response.WriteAsync("WebSocket is open for listening new trffic");
                        }
                    }
                }
                else
                {
                    context.Response.StatusCode = 409;
                }
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;
                throw ex;
            }
            finally
            {
                // if this middleware didn't handle the request, pass it on
                if (!context.Response.HasStarted)
                    await next(context);
            }
        }

        private async Task ProcessRequest(HttpContext context, WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            bool sendContinues = true;
            while (!result.CloseStatus.HasValue || sendContinues)
            {
                string userMessage = Encoding.UTF8.GetString(buffer.ToArray(), 0, result.Count);
                ArraySegment<byte> bufferResponse = new ArraySegment<byte>(new byte[1024]);

                MetrolLineScheduler newScheduler = new MetrolLineScheduler();

                int.TryParse(userMessage, out int RouteNumber);
                List<ResponseEntity> lstEntity = newScheduler.getTimeofArrivalbyStopID(RouteNumber);


                bufferResponse = new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(lstEntity)));
                await webSocket.SendAsync(bufferResponse, result.MessageType, result.EndOfMessage, CancellationToken.None);
                Thread.Sleep(1000 * 60 * 2);
                //result = await webSocket.ReceiveAsync(bufferResponse, CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }


    }
}
