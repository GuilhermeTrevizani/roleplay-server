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
            using (var services = ConfigureServices())
            {
                Client = services.GetRequiredService<DiscordSocketClient>();

                Client.Log += LogAsync;
                /*Client.Ready += Client_Ready;
                Client.UserJoined += Client_UserJoined;*/
                Client.UserLeft += Client_UserLeft;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
                await Client.LoginAsync(TokenType.Bot, GlobalConfig.TokenBot);
                await Client.StartAsync();

                await Task.Delay(Timeout.Infinite);
            }
        }

        /*private async Task Client_Ready()
        {
            await Client.SetGameAsync($"{Client.GetGuild(GlobalConfig.GuildId)?.Users.Count} jogadores", type: ActivityType.Listening);
        }

        private async Task Client_UserJoined(SocketGuildUser arg)
        {
            var totalJogadores = Client.GetGuild(GlobalConfig.GuildId)?.Users.Count;
            await (Client.GetChannel(GlobalConfig.UserJoinedChanelId) as SocketTextChannel).SendMessageAsync($"{arg.Mention} entrou no servidor. Total de jogadores: {totalJogadores}");
            await Client.SetGameAsync($"{totalJogadores} jogadores", type: ActivityType.Listening);
        }*/

        private async Task Client_UserLeft(SocketGuildUser arg)
        {
            var totalJogadores = Client.GetGuild(GlobalConfig.GuildId)?.Users.Count;
            await (Client.GetChannel(GlobalConfig.UserLeftChanelId) as SocketTextChannel).SendMessageAsync($"{arg.Mention} saiu do servidor. Total de jogadores: {totalJogadores}");
            await Client.SetGameAsync($"{totalJogadores} jogadores", type: ActivityType.Listening);
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