using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Roleplay;
using Roleplay.Models;
using System.IO;

namespace RoleplayUCP
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var config = JsonConvert.DeserializeObject<Configuracao>(File.ReadAllText("settings.json"));
            Global.ConnectionString = $"Server={config.DBHost};Database={config.DBName};Uid={config.DBUser};Password={config.DBPassword}";
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}