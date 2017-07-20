using System.IO;

using Autofac;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

using NLog;

namespace MyProject.Api
{
    public class Program
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public static void Main(string[] args)
        {
            ////if (args.Length == 2)
            ////{
            ////    if (args[0] == "/migrate")
            ////    {

            ////        _logger.Info("Will just migrate database.");
            ////        MigrateDatabase();
            ////        _logger.Info("Finished.");

            ////        return;
            ////    }
            ////}

            var builder = new ContainerBuilder();
            var container = builder.Build();

            var config = new ConfigurationBuilder()
              .AddJsonFile("hosting.json", true)
              .AddCommandLine(args)
              .Build();

            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                //.UseApplicationInsights()
                .Build();

#if DEBUG
            MigrateDatabase();
#endif

            host.Run();
        }

        private static void MigrateDatabase(string connectionStringOverride = null)
        {

        }
    }
}
