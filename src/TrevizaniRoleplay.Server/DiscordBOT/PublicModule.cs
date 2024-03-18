using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;

namespace TrevizaniRoleplay.Server.DiscordBOT
{
    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        private void TratarException(Exception ex)
        {
            ex.HelpLink += $"{Context.User.Username}#{Context.User.Discriminator} ({Context.User.Id})";
            Functions.GetException(ex);
            ReplyAsync(Global.MENSAGEM_ERRO_DISCORD);
        }

        [Discord.Commands.Command("ajuda")]
        public async Task AjudaCommand()
        {
            try
            {
                await using var context = new DatabaseContext();
                var user = await context.Users.FirstOrDefaultAsync(x => x.DiscordId == Context.User.Id.ToString());
                if (user == null)
                {
                    await ReplyAsync(Global.MENSAGEM_DISCORD_NAO_VINCULADO);
                    return;
                }

                if (user.Staff < UserStaff.Moderator)
                {
                    await ReplyAsync(Global.MENSAGEM_SEM_AUTORIZACAO);
                    return;
                }

                var x = new EmbedBuilder
                {
                    Title = $"Comandos Disponíveis",
                    Color = new Color(Global.MainRgba.R, Global.MainRgba.G, Global.MainRgba.B),
                };

                x.AddField("!on", "Exibe a quantidade de jogadores online", true);
                x.AddField("!apps", "Lista as aplicações para avaliação", true);
                x.AddField("!app", "Pega uma aplicação para avaliação", true);
                x.AddField("!aceitarapp", "Aceita a aplicação que você está avaliando", true);
                x.AddField("!negarapp", "Nega a aplicação que você está avaliando", true);

                if (user.Staff >= UserStaff.Manager)
                    x.AddField("!vip", "Adiciona VIP para um usuário", true);

                await ReplyAsync(embed: x.Build());
            }
            catch (Exception ex)
            {
                TratarException(ex);
            }
        }

        [Discord.Commands.Command("on")]
        public async Task OnCommand()
        {
            try
            {
                await using var context = new DatabaseContext();
                var user = await context.Users.FirstOrDefaultAsync(x => x.DiscordId == Context.User.Id.ToString());
                if (user == null)
                {
                    await ReplyAsync(Global.MENSAGEM_DISCORD_NAO_VINCULADO);
                    return;
                }

                if (user.Staff < UserStaff.Moderator)
                {
                    await ReplyAsync(Global.MENSAGEM_SEM_AUTORIZACAO);
                    return;
                }

                await ReplyAsync($"Quantidade de jogadores online: **{Global.SpawnedPlayers.Count()}**");
            }
            catch (Exception ex)
            {
                TratarException(ex);
            }
        }

        [Discord.Commands.Command("app")]
        public async Task AppCommand()
        {
            try
            {
                await using var context = new DatabaseContext();
                var user = await context.Users.FirstOrDefaultAsync(x => x.DiscordId == Context.User.Id.ToString());
                if (user == null)
                {
                    await ReplyAsync(Global.MENSAGEM_DISCORD_NAO_VINCULADO);
                    return;
                }

                if (user.Staff < UserStaff.Moderator)
                {
                    await ReplyAsync(Global.MENSAGEM_SEM_AUTORIZACAO);
                    return;
                }

                if (await context.Characters.AnyAsync(x => x.EvaluatingStaffUserId == user.Id))
                {
                    await ReplyAsync("Você já está avaliando uma aplicação.");
                    return;
                }

                var app = await context.Characters
                    .Where(x => !x.EvaluatorStaffUserId.HasValue && !x.EvaluatingStaffUserId.HasValue)
                    .Include(x => x.User)
                    .OrderByDescending(x => x.User.VIP >= UserVIP.Silver ? 1 : 0)
                    .FirstOrDefaultAsync();
                if (app == null)
                {
                    await ReplyAsync("Nenhuma aplicação está aguardando avaliação.");
                    return;
                }

                app.EvaluatingStaffUserId = user.Id;
                context.Update(app);

                var target = Global.SpawnedPlayers.FirstOrDefault(x => x.User.Id == user.Id);
                if (target != null)
                {
                    target.User.CharacterApplicationsQuantity++;
                }
                else
                {
                    user.CharacterApplicationsQuantity++;
                    context.Users.Update(user);
                    await context.SaveChangesAsync();
                }

                await context.SaveChangesAsync();

                var x = new EmbedBuilder
                {
                    Title = $"Aplicação de {app.Name} [{app.Id}]",
                    Description = app.History,
                    Color = new Color(Global.MainRgba.R, Global.MainRgba.G, Global.MainRgba.B),
                };
                x.AddField("Nome", app.Name, true);
                x.AddField("Sexo", app.Sex.GetDisplay(), true);
                x.AddField("Nascimento", $"{app.BirthdayDate.ToShortDateString()} ({Math.Truncate((DateTime.Now.Date - app.BirthdayDate).TotalDays / 365):N0} anos)", true);
                x.AddField("Caracteres História", $"{app.History.Length} de 4096", false);
                x.AddField("OOC", $"{app.User.Name} [{app.User.Id}]", true);
                x.WithFooter($"Enviada em {app.RegisterDate}.");

                await ReplyAsync(embed: x.Build());
                await ReplyAsync($"Use **!aceitarapp** ou **!negarapp (motivo)**");
            }
            catch (Exception ex)
            {
                TratarException(ex);
            }
        }

        [Discord.Commands.Command("aceitarapp")]
        public async Task AceitarAppCommand()
        {
            try
            {
                await using var context = new DatabaseContext();
                var user = await context.Users.FirstOrDefaultAsync(x => x.DiscordId == Context.User.Id.ToString());
                if (user == null)
                {
                    await ReplyAsync(Global.MENSAGEM_DISCORD_NAO_VINCULADO);
                    return;
                }

                if (user.Staff < UserStaff.Moderator)
                {
                    await ReplyAsync(Global.MENSAGEM_SEM_AUTORIZACAO);
                    return;
                }

                var app = await context.Characters.Where(x => x.EvaluatingStaffUserId == user.Id)
                    .Include(x => x.User)
                    .FirstOrDefaultAsync();
                if (app == null)
                {
                    await ReplyAsync("Você não está avaliando uma aplicação.");
                    return;
                }

                app.EvaluatorStaffUserId = user.Id;
                app.EvaluatingStaffUserId = null;
                context.Update(app);
                await context.SaveChangesAsync();

                await Functions.SendDiscordMessage(app.User.DiscordId, $"A aplicação do seu personagem <strong>{app.Name}</strong> foi aceita.");

                await ReplyAsync($"Você aceitou a aplicação de **{app.Name} [{app.Id}]**.");
            }
            catch (Exception ex)
            {
                TratarException(ex);
            }
        }

        [Discord.Commands.Command("negarapp")]
        public async Task NegarAppCommand([Remainder] string motivo)
        {
            try
            {
                if (motivo.Length > 1000)
                {
                    await ReplyAsync($"Motivo deve ter no máximo 1000 caracteres.");
                    return;
                }

                await using var context = new DatabaseContext();
                var user = await context.Users.FirstOrDefaultAsync(x => x.DiscordId == Context.User.Id.ToString());
                if (user == null)
                {
                    await ReplyAsync(Global.MENSAGEM_DISCORD_NAO_VINCULADO);
                    return;
                }

                if (user.Staff < UserStaff.Moderator)
                {
                    await ReplyAsync(Global.MENSAGEM_SEM_AUTORIZACAO);
                    return;
                }

                var app = await context.Characters.Where(x => x.EvaluatingStaffUserId == user.Id)
                    .Include(x => x.User)
                    .FirstOrDefaultAsync();
                if (app == null)
                {
                    await ReplyAsync("Você não está avaliando uma aplicação.");
                    return;
                }

                app.EvaluatorStaffUserId = user.Id;
                app.EvaluatingStaffUserId = null;
                app.RejectionReason = motivo;
                context.Update(app);
                await context.SaveChangesAsync();

                await Functions.SendDiscordMessage(app.User.DiscordId, $"A aplicação do seu personagem <strong>{app.Name}</strong> foi negada. Motivo: <strong>{motivo}</strong>");

                await ReplyAsync($"Você negou a aplicação de **{app.Name} [{app.Id}]**. Motivo: **{motivo}**");
            }
            catch (Exception ex)
            {
                TratarException(ex);
            }
        }

        [Discord.Commands.Command("apps")]
        public async Task AppsCommand()
        {
            try
            {
                await using var context = new DatabaseContext();
                var user = await context.Users.FirstOrDefaultAsync(x => x.DiscordId == Context.User.Id.ToString());
                if (user == null)
                {
                    await ReplyAsync(Global.MENSAGEM_DISCORD_NAO_VINCULADO);
                    return;
                }

                if (user.Staff < UserStaff.Moderator)
                {
                    await ReplyAsync(Global.MENSAGEM_SEM_AUTORIZACAO);
                    return;
                }

                var apps = await context.Characters
                    .Where(x => !x.EvaluatorStaffUserId.HasValue)
                    .Include(x => x.User)
                    .Include(x => x.EvaluatingStaffUser)
                    .OrderByDescending(x => x.User.VIP >= UserVIP.Silver ? 1 : 0)
                    .ToListAsync();
                if (apps.Count == 0)
                {
                    await ReplyAsync("Nenhuma aplicação está aguardando avaliação.");
                    return;
                }

                var aplicacoes = string.Empty;

                foreach (var app in apps)
                    aplicacoes += $"Aplicação de {app.Name} [{app.Id}] (Responsável: {(!app.EvaluatingStaffUserId.HasValue ? "N/A" : $"{app.EvaluatingStaffUser.Name} [{app.EvaluatingStaffUserId}]")}){Environment.NewLine}";

                var x = new EmbedBuilder
                {
                    Title = $"Aplicações Aguardando Avaliação",
                    Color = new Color(Global.MainRgba.R, Global.MainRgba.G, Global.MainRgba.B),
                    Description = aplicacoes,
                };
                x.WithFooter($"Status em {DateTime.Now}.");

                await ReplyAsync(embed: x.Build());
            }
            catch (Exception ex)
            {
                TratarException(ex);
            }
        }

        [Discord.Commands.Command("vip")]
        public async Task VipCommand(string usuario, byte nivelVip, int meses)
        {
            try
            {
                await using var context = new DatabaseContext();
                var userStaff = await context.Users.FirstOrDefaultAsync(x => x.DiscordId == Context.User.Id.ToString());
                if (userStaff == null)
                {
                    await ReplyAsync(Global.MENSAGEM_DISCORD_NAO_VINCULADO);
                    return;
                }

                if (userStaff.Staff < UserStaff.Manager)
                {
                    await ReplyAsync(Global.MENSAGEM_SEM_AUTORIZACAO);
                    return;
                }

                if (!Enum.IsDefined(typeof(UserVIP), nivelVip))
                {
                    await ReplyAsync($"VIP {nivelVip} não existe.");
                    return;
                }

                var user = await context.Users.FirstOrDefaultAsync(x => x.Name.ToLower() == usuario.ToLower());
                if (user == null)
                {
                    await ReplyAsync($"Usuário {usuario} não existe.");
                    return;
                }

                var vip = (UserVIP)nivelVip;
                user.VIP = vip;
                user.VIPValidDate = (user.VIPValidDate > DateTime.Now && user.VIP == vip ? user.VIPValidDate.Value : DateTime.Now).AddMonths(meses);
                user.NameChanges += vip switch
                {
                    UserVIP.Gold => 4,
                    UserVIP.Silver => 3,
                    _ => 2,
                };

                user.ForumNameChanges += vip switch
                {
                    UserVIP.Gold => 2,
                    _ => 1,
                };

                user.PlateChanges += vip switch
                {
                    UserVIP.Gold => 2,
                    UserVIP.Silver => 1,
                    _ => 0,
                };

                var target = Global.SpawnedPlayers.FirstOrDefault(x => x.User.Id == user.Id);
                if (target != null)
                {
                    target.User.VIP = user.VIP;
                    target.User.VIPValidDate = user.VIPValidDate;
                    target.User.NameChanges = user.NameChanges;
                    target.User.ForumNameChanges = user.ForumNameChanges;
                    target.User.PlateChanges = user.PlateChanges;
                    target.SendMessage(Models.MessageType.Success, $"{userStaff.Name} alterou seu nível VIP para {vip.GetDisplay()} expirando em {user.VIPValidDate}.");
                }
                else
                {
                    context.Users.Update(user);
                    await context.SaveChangesAsync();
                }

                await Functions.WriteLog(LogType.Staff, $"!vip {user.Id} {vip} {meses} (por {userStaff.Id})");
                await ReplyAsync($"Você alterou o nível VIP de {user.Name} para {vip.GetDisplay()} expirando em {user.VIPValidDate}.");
            }
            catch (Exception ex)
            {
                TratarException(ex);
            }
        }
    }
}