using LeosSmartBoy.Services;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace LeosSmartBoy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<Program>();
            var configuration = builder.Build();

            BotService service = new BotService(configuration["ApiKey"]);
            service.Run().Wait();
        }

    }
}
