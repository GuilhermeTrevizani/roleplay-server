using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Roleplay;
using Roleplay.Models;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace RoleplayBOT
{
    class Program
    {
        DiscordSocketClient Client { get; set; }

        static void Main(string[] args)
        {
            var config = JsonConvert.DeserializeObject<Configuracao>(File.ReadAllText("settings.json"));
            Global.ConnectionString = $"Server={config.DBHost};Database={config.DBName};Uid={config.DBUser};Password={config.DBPassword}";
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public async Task MainAsync()
        {
            using var services = ConfigureServices();
            Client = services.GetRequiredService<DiscordSocketClient>();

            Client.Log += LogAsync;
            services.GetRequiredService<CommandService>().Log += LogAsync;

            await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
            await Client.LoginAsync(TokenType.Bot, GlobalConfig.TokenBot);
            await Client.StartAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                .AddSingleton<HttpClient>()
                .BuildServiceProvider();
        }
    }
}