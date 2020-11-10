#if NETCOREAPP2_2
using Microsoft.AspNetCore;
#elif NETCOREAPP3_1
using Microsoft.Extensions.Hosting;
#endif
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Fireasy.Zero.AspNetCore
{
    public class Program
    {

#if NETCOREAPP2_2
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("hosting.json", optional: true)
                .AddCommandLine(args)
                .Build();

            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .UseStartup<Startup>()
                .Build();
        }
#elif NETCOREAPP3_1
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureHostConfiguration(cfgBuilder =>
                {
                    //貌似没有作用
                    cfgBuilder.SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("hosting.json", optional: true)
                        .AddCommandLine(args);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
#endif
    }
}
