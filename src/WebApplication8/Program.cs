using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Globalization;

namespace WebApplication8
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Setter korrekt lokalisering for decimal/Price!
            // ("nb-NO")
            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            

            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
