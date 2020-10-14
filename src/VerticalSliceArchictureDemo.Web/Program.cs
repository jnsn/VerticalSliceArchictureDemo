using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;

namespace VerticalSliceArchictureDemo.Web
{
    public class Program
    {
        public static void Main(string[] args)
            => CreateHostBuilder(args).Build().Run();

        [SuppressMessage("ReSharper", "RedundantAssignment")]
        public static IHostBuilder CreateHostBuilder(string[] args)
            => Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseSerilog((webHostBuilderContext, loggerConfiguration) =>
                    {
                        loggerConfiguration = loggerConfiguration
                            .ReadFrom.Configuration(webHostBuilderContext.Configuration)
                            .Enrich.WithProperty("Application", Startup.ApplicationName)
                            .Enrich.FromLogContext()
                            .Enrich.WithExceptionDetails()
                            .Enrich.WithMachineName()
                            .Enrich.WithEnvironmentUserName()
                            .Filter.ByExcluding(x => x.Properties.Any(y => y.Key == "RequestPath" && y.Value.ToString().Contains("/up.html")))
                            .Filter.ByExcluding(x => x.Properties.Any(y => y.Key == "RequestPath" && y.Value.ToString().Contains("/health")));

#if DEBUG
                        loggerConfiguration = loggerConfiguration.WriteTo.Debug(
                            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}");
#endif
                    });
                });
    }
}
