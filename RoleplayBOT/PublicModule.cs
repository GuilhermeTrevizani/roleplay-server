using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;
using Roleplay;
using Roleplay.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RoleplayBOT
{
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        [Discord.Commands.Command("reg")]
        public Task RegCommand(int codigo, SocketUser discord)
        {
            try
            {
                if (!((SocketGuildUser)Context.User).Roles.Any(x => x.Id == 755123158591340786))
                    return ReplyAsync($"Só Management pode executar esse comando. :thinking:");

                using var context = new DatabaseContext();
                var user = context.Usuarios.FirstOrDefault(x => x.Codigo == codigo);
                if (user == null)
                    return ReplyAsync($"Não localizei nenhum usuário com o código {codigo}. :thinking:");

                user.Discord = (long)discord.Id;
                context.Update(user);
                context.SaveChanges();

                return ReplyAsync($"ID do Discord do usuário {user.Nome} [{user.Codigo}] setado como {user.Discord}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
                return ReplyAsync("Não consegui recuperar as informações da minha base de dados. A culpa é do sistema!");
            }
        }

        [Discord.Commands.Command("app")]
        public Task AppCommand()
        {
            try
            {
                using var context = new DatabaseContext();
                var user = context.Usuarios.FirstOrDefault(x => x.Discord == (long)Context.User.Id);
                if (user == null)
                    return ReplyAsync($"Seu Discord não foi vinculado com nenhum usuário. Peça a um Manager para configurar. :thinking:");

                if (user.Staff == TipoStaff.Nenhum)
                    return ReplyAsync($"Você não é da staff. :thinking:");
                
                var app = context.Personagens.FirstOrDefault(x => x.UsuarioStaffAvaliador == 0);
                if (app == null)
                    return ReplyAsync("Não temos aplicações aguardando avaliação. :smiling_face_with_3_hearts:");

                var x = new EmbedBuilder
                {
                    Title = $"Aplicação de {app.Nome} [{app.Codigo}]",
                    Description = app.Historia,
                };
                x.AddField("Nome", app.Nome, true);
                x.AddField("Sexo", app.PersonalizacaoDados.sex == 1 ? "Homem" : "Mulher", true);
                x.AddField("Nascimento", app.DataNascimento.ToShortDateString(), true);
                x.WithFooter($"Enviada em {app.DataRegistro}");
                x.Color = Color.DarkRed;

                ReplyAsync(embed: x.Build());
                return ReplyAsync($"Use !aceitarapp {app.Codigo} ou !negarapp {app.Codigo} (motivo)");
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
                return ReplyAsync("Não consegui recuperar as informações da minha base de dados. A culpa é do sistema!");
            }
        }

        [Discord.Commands.Command("aceitarapp")]
        public Task AceitarAppCommand(int codigo)
        {
            try
            {
                using var context = new DatabaseContext();
                var user = context.Usuarios.FirstOrDefault(x => x.Discord == (long)Context.User.Id);
                if (user == null)
                    return ReplyAsync($"Seu Discord não foi vinculado com nenhum usuário. Peça a um Manager para configurar. :thinking:");

                if (user.Staff == TipoStaff.Nenhum)
                    return ReplyAsync($"Você não é da staff. :thinking:");

                var app = context.Personagens.FirstOrDefault(x => x.Codigo == codigo && x.UsuarioStaffAvaliador == 0);
                if (app == null)
                    return ReplyAsync($"Não localizei nenhuma aplicação com o código {codigo}. :thinking:");

                app.UsuarioStaffAvaliador = user.Codigo;
                context.Update(app);
                context.SaveChanges();

                return ReplyAsync($"Você aceitou a aplicação {app.Nome} [{app.Codigo}].");
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
                return ReplyAsync("Não consegui recuperar as informações da minha base de dados. A culpa é do sistema!");
            }
        }

        [Discord.Commands.Command("negarapp")]
        public Task NegarAppCommand(int codigo, [Remainder] string motivo)
        {
            try
            {
                if (motivo.Length > 1000)
                    return ReplyAsync($"Motivo deve ter no máximo 1000 caracteres.");

                using var context = new DatabaseContext();
                var user = context.Usuarios.FirstOrDefault(x => x.Discord == (long)Context.User.Id);
                if (user == null)
                    return ReplyAsync($"Seu Discord não foi vinculado com nenhum usuário. Peça a um Manager para configurar. :thinking:");

                if (user.Staff == TipoStaff.Nenhum)
                    return ReplyAsync($"Você não é da staff. :thinking:");

                var app = context.Personagens.FirstOrDefault(x => x.Codigo == codigo && x.UsuarioStaffAvaliador == 0);
                if (app == null)
                    return ReplyAsync($"Não localizei nenhuma aplicação com o código {codigo}. :thinking:");

                app.UsuarioStaffAvaliador = user.Codigo;
                app.MotivoRejeicao = motivo;
                context.Update(app);
                context.SaveChanges();

                return ReplyAsync($"Você negou a aplicação {app.Nome} [{app.Codigo}].");
            }
            catch (Exception ex)
            {
                Console.WriteLine(JsonConvert.SerializeObject(ex));
                return ReplyAsync("Não consegui recuperar as informações da minha base de dados. A culpa é do sistema!");
            }
        }
    }
}