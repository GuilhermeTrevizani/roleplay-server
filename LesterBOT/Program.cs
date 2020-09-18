using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SegundaVidaBOT
{
    class Program
    {
        DiscordSocketClient Client { get; set; }
        ulong GuildId { get; } = 750692435532972063;
         
        static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                Client = services.GetRequiredService<DiscordSocketClient>();

                Client.Log += LogAsync;
                Client.UserJoined += Client_UserJoined;
                Client.UserLeft += Client_UserLeft;
                Client.Ready += Client_Ready;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();
                await Client.LoginAsync(TokenType.Bot, "NzU2MTE0MTU3ODA3NTk5Njc4.X2NILw.VqMigsgJ8hLK_xnoxINniJCICmw");
                await Client.StartAsync();

                await Task.Delay(Timeout.Infinite);
            }
        }

        private async Task Client_Ready()
        {
            await Client.SetGameAsync($"{Client.GetGuild(GuildId)?.Users.Count} jogadores", type: ActivityType.Listening);
        }

        private async Task Client_UserLeft(SocketGuildUser arg)
        {
            var totalJogadores = Client.GetGuild(GuildId)?.Users.Count;
            await (Client.GetChannel(756116408605081621) as SocketTextChannel).SendMessageAsync($"{arg.Mention} saiu do servidor. Total de jogadores: {totalJogadores}");
            await Client.SetGameAsync($"{totalJogadores} jogadores", type: ActivityType.Listening);
        }

        private async Task Client_UserJoined(SocketGuildUser arg)
        {
            var totalJogadores = Client.GetGuild(GuildId)?.Users.Count;
            await (Client.GetChannel(755585494775103548) as SocketTextChannel).SendMessageAsync($"{arg.Mention} entrou no servidor. Total de jogadores: {totalJogadores}");
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