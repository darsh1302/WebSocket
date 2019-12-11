using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Realtime.MetroLine.Entity;

namespace Realtime.MetroLine
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {}

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseDefaultFiles();
            app.UseStaticFiles();


            var webSocketOptions = new WebSocketOptions()
            {
                ReceiveBufferSize = 4 * 1024
            };

            app.UseWebSockets(webSocketOptions);

            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await ProcessRequest(context, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });
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
                Thread.Sleep(1000*60*2);
                //result = await webSocket.ReceiveAsync(bufferResponse, CancellationToken.None);
            }
            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }


    }
}
