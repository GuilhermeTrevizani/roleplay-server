using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;

namespace TrevizaniRoleplay.Server.DiscordBOT
{
    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;

        public CommandHandlingService(IServiceProvider services)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;

            _commands.CommandExecuted += CommandExecutedAsync;
            _discord.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync()
        {
            await _commands.AddModuleAsync(typeof(PublicModule), _services);
        }

        public async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (rawMessage is not SocketUserMessage message)
                return;

            if (message.Source != MessageSource.User)
                return;

            if (message.Channel is not SocketDMChannel)
                return;

            var argPos = 0;

            if (!message.HasCharPrefix('!', ref argPos))
                return;

            var context = new SocketCommandContext(_discord, message);
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        public static async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified)
                return;

            if (result.IsSuccess)
                return;

            if (result.Error == CommandError.BadArgCount)
            {
                await context.Channel.SendMessageAsync($"Os parâmetros do comando **{((CommandInfo)command).Name}** não foram especificados corretamente.");
                return;
            }

            await context.Channel.SendMessageAsync($"[ERRO] {result}");
        }
    }
}