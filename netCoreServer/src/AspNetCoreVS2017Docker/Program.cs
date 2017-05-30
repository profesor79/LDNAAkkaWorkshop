using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;

namespace AspNetCoreVS2017Docker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var hostname = System.Net.Dns.GetHostName();
            Console.WriteLine($"hostname: {hostname}");
            var host = new WebHostBuilder()
                .UseKestrel(
                    options =>
                        { options.ThreadCount = 8; })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
