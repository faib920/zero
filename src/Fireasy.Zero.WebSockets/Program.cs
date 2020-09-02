using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Fireasy.Zero.WebSockets
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("hosting.json", optional: true)
                .Build();

            var builder = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseConfiguration(config)
                .UseStartup<Start>()
                .UseKestrel();

            var host = builder.Build();
            host.Run();
        }
    }

    public class Start
    {
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseWebSockets(options =>
            {
                options.KeepAliveInterval = TimeSpan.MaxValue;
                options.ReceiveBufferSize = 4 * 1024;
                options.MapHandler<ChatHandler>("/wsChat");
            });
        }
    }
}
