using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace SegundaVidaBOT
{
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        [Command("online")]
        [Alias("on")]
        public Task OnlineAsync()
        {
            try
            {
                var players =  0;
                return ReplyAsync($"O servidor está com {players} jogador{(players != 1 ? "es" : string.Empty)} online!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
                return ReplyAsync("Não consegui recuperar as informações da minha base de dados. A culpa é do sistema!");
            }
        }
    }
}