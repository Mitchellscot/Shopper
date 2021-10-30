using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Shopper.Models;
using static System.Console;

namespace shopper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseSystemd()
                .ConfigureServices((hostContext, services) =>
                {
                    IConfiguration configuration = hostContext.Configuration;
                    Settings settingOptions = configuration.GetSection("Settings").Get<Settings>();
                    services.AddSingleton(settingOptions);
                    //services.AddTransient<Settings>();
                    //figure this out https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/options?view=aspnetcore-5.0

                    services.Configure<ConsoleLifetimeOptions>(options =>
                        options.SuppressStatusMessages = true);

                    services.AddTransient<Schedule>();
                    services.AddHostedService<Shopper>();
                });
    }
}
