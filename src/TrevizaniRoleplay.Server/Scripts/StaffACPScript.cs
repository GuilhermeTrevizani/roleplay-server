﻿using AltV.Net;
using AltV.Net.Async;
using Microsoft.EntityFrameworkCore;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class StaffACPScript : IScript
    {
        [Command("acp")]
        public static async Task CMD_acp(MyPlayer player)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var jsonLogs = Functions.Serialize(
                Enum.GetValues(typeof(LogType))
                .Cast<LogType>()
                .Select(x => new
                {
                    Id = x,
                    Name = x.GetDisplay(),
                })
            );

            using var context = new DatabaseContext();
            var staffersJson = Functions.Serialize(
                (await context.Users.Where(x => x.Staff != UserStaff.None)
                    .Include(x => x.Characters)
                    .OrderByDescending(x => x.Staff)
                    .ToListAsync())
                    .Select(x => new
                    {
                        Staff = x.Staff.GetDisplay(),
                        x.Id,
                        x.Name,
                        x.HelpRequestsAnswersQuantity,
                        x.CharacterApplicationsQuantity,
                        x.StaffDutyTime,
                        ConnectedTime = x.Characters!.Sum(x => x.ConnectedTime)
                    })
            );

            player.Emit("ACPShow",
                player.User.StaffFlagsJSON,
                await GetBansJSON(),
                Functions.GetSOSJSON(),
                jsonLogs,
                staffersJson);
        }

        [AsyncClientEvent(nameof(StaffUnban))]
        public static async Task StaffUnban(MyPlayer player, int id, bool total)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Unban))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            using var context = new DatabaseContext();
            var ban = await context.Banishments.FirstOrDefaultAsync(x => x.Id == id);
            if (ban != null)
            {
                if (total)
                {
                    context.Banishments.Remove(ban);
                }
                else
                {
                    ban.UserId = null;
                    context.Banishments.Update(ban);
                }

                await context.SaveChangesAsync();

                player.EmitStaffShowMessage($"Você removeu o banimento {ban.Id} {(total ? "totalmente" : "do usuário")}.");
                await player.GravarLog(LogType.Staff, $"Unban {ban} {total}", null);
            }
            else
            {
                player.EmitStaffShowMessage($"O banimento {id} não foi encontrado.");
            }

            var html = await GetBansJSON();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.User.Staff != UserStaff.None))
                target.Emit("ACPUpdateBans", html);
        }

        [AsyncClientEvent(nameof(StaffSearchLogs))]
        public static async Task StaffSearchLogs(MyPlayer player,
            string stringInitialDate, string stringFinalDate,
            int type,
            string strOriginCharacter, string strTargetCharacter,
            string description)
        {
            await using var context = new DatabaseContext();
            var query = context.Logs.AsQueryable();

            if (type > 0)
                query = query.Where(x => x.Type == (LogType)type);

            if (DateTime.TryParse(stringInitialDate, out DateTime initialDate))
                query = query.Where(x => x.Date >= initialDate);

            if (DateTime.TryParse(stringFinalDate, out DateTime finalDate))
                query = query.Where(x => x.Date <= finalDate);

            if (int.TryParse(strOriginCharacter, out int originCharacterId))
                query = query.Where(x => x.OriginCharacterId == originCharacterId);

            if (int.TryParse(strTargetCharacter, out int targetCharacterId))
                query = query.Where(x => x.TargetCharacterId == targetCharacterId);

            if (!string.IsNullOrWhiteSpace(description))
                query = query.Where(x => x.Description.Contains(description, StringComparison.CurrentCultureIgnoreCase));

            var logs = await query
                .Include(x => x.OriginCharacter)
                .Include(x => x.TargetCharacter)
                .Include(x => x.OriginCharacter.User)
                .Include(x => x.TargetCharacter.User)
                .OrderByDescending(x => x.Id)
                .Take(50)
                .ToListAsync();

            var json = Functions.Serialize(
                logs.Select(x => new
                {
                    Type = x.Type.GetDisplay(),
                    Date = x.Date.ToString(),
                    x.Description,
                    OriginCharacterName = x.OriginCharacterId.HasValue ? $"{x.OriginCharacter.Name} [{x.OriginCharacterId}] ({x.OriginCharacter.User.Name})" : string.Empty,
                    x.OriginIp,
                    x.OriginHardwareIdHash,
                    x.OriginHardwareIdExHash,
                    TargetCharacterName = x.TargetCharacterId.HasValue ? $"{x.TargetCharacter.Name} [{x.TargetCharacterId}] ({x.TargetCharacter.User.Name})" : string.Empty,
                    x.TargetIp,
                    x.TargetHardwareIdHash,
                    x.TargetHardwareIdExHash,
                })
            );

            player.Emit("ACPUpdateLogs", json);
        }

        [AsyncClientEvent(nameof(StaffSearchUser))]
        public static async Task StaffSearchUser(MyPlayer player, string search)
        {
            await using var context = new DatabaseContext();
            User? user;
            if (int.TryParse(search, out int id))
                user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            else
                user = await context.Users.FirstOrDefaultAsync(x => x.Name.ToLower() == search.ToLower());

            if (user == null)
            {
                player.Emit("ACPUpdateUser", "<div class='alert alert-danger'>Nenhum usuário encontrado com a pesquisa informada.</div>");
                return;
            }

            var staffFlags = Functions.Deserialize<List<StaffFlag>>(user.StaffFlagsJSON);

            var htmlFlags = string.Empty;
            foreach (var flag in Enum.GetValues(typeof(StaffFlag)).Cast<StaffFlag>())
                htmlFlags += $@"<option value='{(int)flag}' {(staffFlags.Contains(flag) ? "selected" : string.Empty)}>{Functions.GetEnumDisplay(flag)}</option>";

            var htmlStaff = string.Empty;
            foreach (var staff in Enum.GetValues(typeof(UserStaff)).Cast<UserStaff>())
                htmlStaff += $@"<option value='{(int)staff}' {(user.Staff == staff ? "selected" : string.Empty)}>{Functions.GetEnumDisplay(staff)}</option>";

            var htmlSave = string.Empty;
            if (player.User.Staff >= UserStaff.HeadAdministrator)
                htmlSave += @"
                <div class='row'>
                    <div class='col-md-offset-11 col-md-1'>
            	        <button onclick='save()' class='btn btn-primary btn-block' type='button'>Salvar</button>
                   </div>
                </div>";

            var html = $@"<h3>{user.Name} [{user.Id}]</h3>
            Tempo Serviço Administrativo (minutos): <strong>{user.StaffDutyTime}</strong> | SOSs Atendidos: <strong>{user.HelpRequestsAnswersQuantity}</strong><br/>
            <input id='input-userId' type='hidden' value='{user.Id}' />
            <div class='row'>
                <div class='col-md-3'>
                    <div class='form-group'>
                        <label class='control-label'>Staff</label>
                        <select id='select-staff' class='form-control' {(player.User.Staff < UserStaff.HeadAdministrator ? "readonly" : string.Empty)}>
                            {htmlStaff}
                        </select>
                    </div>
                </div>
                <div class='col-md-9'>
                    <div class='form-group'>
                        <label class='control-label'>Staff Flags</label>
                        <select id='select-flags' class='form-control' multiple {(player.User.Staff < UserStaff.HeadAdministrator ? "readonly" : string.Empty)}>
                            {htmlFlags}
                        </select>
                    </div>
                </div>
            </div>
            {htmlSave}";

            var personagens = await context.Characters
                .Where(x => x.UserId == user.Id)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
            html += $"<h4>Personagens</h4>";
            if (personagens.Count != 0)
            {
                foreach (var personagem in personagens)
                    html += $"Código: <strong>{personagem.Id}</strong> | Nome: <strong>{personagem.Name}</strong><br/>";
            }
            else
            {
                html += "Nenhum personagem.<br/>";
            }

            var punicoes = await context.Punishments
                .Include(x => x.Character)
                .Include(x => x.StaffUser)
                .Where(x => x.Character.UserId == user.Id)
                .OrderByDescending(x => x.Id)
                .ToListAsync();
            html += $"<h4>Punições Administrativas</h4>";
            if (punicoes.Count != 0)
            {
                foreach (var punicao in punicoes)
                {
                    var duracao = punicao.Type == PunishmentType.Ban ? (punicao.Duration > 0 ? $"{punicao.Duration} dia{(punicao.Duration != 1 ? "s" : string.Empty)}" : "Permanente") : string.Empty;
                    html += $"Personagem: <strong>{punicao.Character.Name} [{punicao.CharacterId}]</strong> |  Data: <strong>{punicao.Date}</strong> | Tipo: <strong>{punicao.Type}</strong> | Duração: <strong>{duracao}</strong> | Staffer: <strong>{punicao.StaffUser.Name}</strong> | Motivo: <strong>{punicao.Reason}</strong><br/>";
                }
            }
            else
            {
                html += "Nenhuma punição administrativa.<br/>";
            }

            if (player.User.Staff >= UserStaff.Manager && user.ForumNameChanges > 0)
                html += $"<button onclick='removeForumNameChange({user.Id})' class='btn btn-dark btn-md' type='button'>Debitar Mudança de Nome do Fórum</button>";

            player.Emit("ACPUpdateUser", html);
        }

        [AsyncClientEvent(nameof(StaffSaveUser))]
        public static async Task StaffSaveUser(MyPlayer player, int userId, int staff, string flagsJSON)
        {
            if (player.User.Staff < UserStaff.HeadAdministrator)
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!Enum.IsDefined(typeof(UserStaff), staff))
            {
                player.EmitStaffShowMessage($"Staff {staff} não existe.");
                return;
            }

            await using var context = new DatabaseContext();
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                player.EmitStaffShowMessage($"Nenhum usuário encontrado com o código {userId}.");
                return;
            }

            if ((user.Id == 1 && player.User.Id != 1)
                ||
                (player.User.Staff <= user.Staff && player.User.Id != user.Id))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var staffFlags = Functions.Deserialize<List<string>>(flagsJSON).Select(x => (StaffFlag)Convert.ToByte(x)).ToList();

            user.Staff = (UserStaff)staff;
            if (user.Staff == UserStaff.None)
                user.StaffFlagsJSON = "[]";
            else
                user.StaffFlagsJSON = Functions.Serialize(staffFlags);

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.User.Id == userId);
            if (target != null)
            {
                target.SendMessage(MessageType.Success, $"{player.User.Name} modificou suas configurações administrativas.");
                target.User.Staff = user.Staff;
                target.StaffFlags = staffFlags;
                target.User.StaffFlagsJSON = user.StaffFlagsJSON;
                await target.Save();
            }
            else
            {
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }

            await player.GravarLog(LogType.Staff, $"Alterar Usuário {userId} {staff} {user.StaffFlagsJSON}", target);
            player.EmitStaffShowMessage($"Você alterou as configurações administrativas de {user.Name}.");
        }

        [AsyncClientEvent(nameof(StaffRemoveForumNameChangeUser))]
        public static async Task StaffRemoveForumNameChangeUser(MyPlayer player, int userId)
        {
            if (player.User.Staff < UserStaff.Manager)
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null)
            {
                player.EmitStaffShowMessage($"Usuário {userId} não existe.");
                return;
            }

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.User.Id == userId);
            if (target != null)
            {
                target.User.ForumNameChanges--;
                target.SendMessage(MessageType.Success, $"{player.User.Name} debitou um namechange do fórum da sua conta.");
                await target.Save();
            }
            else
            {
                user.ForumNameChanges--;
                context.Users.Update(user);
                await context.SaveChangesAsync();
            }

            player.EmitStaffShowMessage($"Você debitou um namechange do fórum de {target.User.Name}.");
            await player.GravarLog(LogType.Staff, $"Debitar Mudança de Nome do Fórum {userId}", null);
            await StaffSearchUser(player, user.Id.ToString());
        }

        [AsyncClientEvent(nameof(StaffSearchCharacter))]
        public static async Task StaffSearchCharacter(MyPlayer player, string search)
        {
            await using var context = new DatabaseContext();
            Character? character;
            if (int.TryParse(search, out int id))
                character = await context.Characters.FirstOrDefaultAsync(x => x.Id == id);
            else
                character = await context.Characters.FirstOrDefaultAsync(x => x.Name.ToLower() == search.ToLower());

            if (character == null)
            {
                player.Emit("ACPUpdateCharacter", "<div class='alert alert-danger'>Nenhum personagem encontrado com a pesquisa informada.</div>");
                return;
            }

            var html = $@"<h3>{character.Name} [{character.Id}]</h3>";

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.Character.Id == character.Id);
            if (target != null)
            {
                html += await target.ObterHTMLStats();
            }
            else
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == character.UserId);

                html += $@"OOC: <strong>{user.Name} [{user.Id}]</strong> | Registro: <strong>{character.RegisterDate}</strong> | VIP: <strong>{Functions.GetEnumDisplay(user.VIP)} {(user.VIPValidDate.HasValue ? $"- {(user.VIPValidDate < DateTime.Now ? "Expirado" : "Expira")} em {user.VIPValidDate}" : string.Empty)}</strong><br/>
                Tempo Conectado (minutos): <strong>{character.ConnectedTime}</strong> | Emprego: <strong>{Functions.GetEnumDisplay(character.Job)}</strong> | Trocas de Nome: <strong>{user.NameChanges} {(character.NameChangeStatus == CharacterNameChangeStatus.Blocked ? "(BLOQUEADO)" : string.Empty)}</strong> | Trocas de Nome Fórum: <strong>{user.ForumNameChanges}</strong> | Trocas de Placa: <strong>{user.PlateChanges}</strong><br/>
                Banco: <strong>${character.Bank:N0}</strong> | Poupança: <strong>${character.Savings:N0}</strong><br/>";

                if (character.FactionId.HasValue)
                {
                    var faccao = Global.Factions.FirstOrDefault(x => x.Id == character.FactionId);
                    var rank = Global.FactionsRanks.FirstOrDefault(x => x.Id == character.FactionRankId);
                    html += $"Facção: <strong>{faccao?.Name} [{character.FactionId}]</strong> | Rank: <strong>{rank?.Name} [{character.FactionRankId}]</strong>";
                }

                html += $"<h4>História (aceito por {character.EvaluatorStaffUserId})</h4> {character.History}";

                var itens = await context.CharactersItems.Where(x => x.CharacterId == character.Id).ToListAsync();
                html += $"<h4>Inventário</h4>";
                if (itens.Count != 0)
                {
                    foreach (var item in itens)
                        html += $"Código: <strong>{item.Id}</strong> | Nome: <strong>{item.Name}</strong> | Quantidade: <strong>{item.Quantity:N0}</strong>{(!string.IsNullOrWhiteSpace(item.Extra) ? $" | Extra: <strong>{Functions.GetItemExtra(item).Replace("<br/>", ", ")}</strong>" : string.Empty)}<br/>";
                }
                else
                {
                    html += "Nenhum item.<br/>";
                }

                var propriedades = Global.Properties.Where(x => x.CharacterId == character.Id);
                html += $"<h4>Propriedades</h4>";
                if (propriedades.Any())
                {
                    foreach (var prop in propriedades)
                        html += $"Código: <strong>{prop.Id}</strong> | Endereço: <strong>{prop.Address}</strong> | Valor: <strong>${prop.Value:N0}</strong> | Nível de Proteção: <strong>{prop.ProtectionLevel}</strong><br/>";
                }
                else
                {
                    html += "Nenhuma propriedade.<br/>";
                }

                var veiculos = await context.Vehicles.Where(x => x.CharacterId == character.Id && !x.Sold).ToListAsync();
                html += $"<h4>Veículos</h4>";
                if (veiculos.Count != 0)
                {
                    foreach (var veh in veiculos)
                        html += $"Código: <strong>{veh.Id}</strong> | Modelo: <strong>{veh.Model.ToUpper()}</strong> | Placa: <strong>{veh.Plate}</strong> | Nível de Proteção: <strong>{veh.ProtectionLevel}</strong> | XMR: <strong>{(veh.XMR ? "SIM" : "NÃO")}</strong><br/>";
                }
                else
                {
                    html += "Nenhum veículo.<br/>";
                }

                html += $"<h4>Empresas</h4>";
                var companies = Global.Companies.Where(x => x.CharacterId == character.Id || x.Characters.Any(y => y.CharacterId == character.Id)).ToList();
                if (companies.Count != 0)
                {
                    foreach (var company in companies)
                        html += $"Código: <strong>{company.Id}</strong> | Nome: <strong>{company.Name}</strong> | Dono: <strong>{(company.CharacterId == character.Id ? "SIM" : "NÃO")}</strong><br/>";
                }
                else
                {
                    html += "Nenhuma empresa.<br/>";
                }

                var ban = await context.Banishments.Where(x => x.CharacterId == character.Id)
                    .Include(x => x.StaffUser)
                    .FirstOrDefaultAsync();
                if (ban != null)
                    html += $"<h4 style='color:red;'>ESTE USUÁRIO ESTÁ BANIDO.</h4>Data: <strong>{ban.Date}</strong><br/>Motivo: <strong>{ban.Reason}</strong><br/>Expiração: <strong>{(ban.ExpirationDate.HasValue ? ban.ExpirationDate?.ToString() : "Permanente")}</strong><br/>Staffer: <strong>{ban.StaffUser.Name}</strong>";
                else if (player.User.Staff >= UserStaff.GameAdministrator)
                    html += $"<br/><br/><button onClick='banirPersonagem({id})' class='btn btn-danger btn-md'>Banir</button>";
            }

            if (character.DeathDate.HasValue)
                html += $"<h4 style='color:red;'>ESTE PERSONAGEM ESTÁ MORTO.</h4>Data: <strong>{character.DeathDate}</strong><br/>Motivo: <strong>{character.DeathReason}</strong>";
            else if (character.CKAvaliation)
                html += $"<h4 style='color:red;'>ESTE PERSONAGEM ESTÁ COM O CK EM AVALIAÇÃO.</h4>";
            else if ((character.JailFinalDate ?? DateTime.MinValue) > DateTime.Now)
                html += $"<h4 style='color:red;'>PRESO ATÉ {character.JailFinalDate}.</h4>";

            if (player.StaffFlags.Contains(StaffFlag.CK))
            {
                if (character.CKAvaliation || character.DeathDate.HasValue)
                    html += $"<br/><br/><button onClick='ckAvalationRemoveCharacter({id})' class='btn btn-dark btn-md'>Remover do CK ou Avaliação de CK</button>";

                if (!character.DeathDate.HasValue)
                {
                    html += $"<br/><br/><button onClick='ckCharacter({id})' class='btn btn-dark btn-md'>Aplicar CK</button>";

                    if (!character.CKAvaliation)
                        html += $"<br/><br/><button onClick='ckAvalationCharacter({id})' class='btn btn-dark btn-md'>Aplicar Avaliação de CK</button>";
                }

                if (character.NameChangeStatus != CharacterNameChangeStatus.Done)
                    html += $"<br/><br/><button onClick='nameChangeStatusCharacter({id})' class='btn btn-dark btn-md'>{(character.NameChangeStatus == CharacterNameChangeStatus.Allowed ? "Bloquear" : "Liberar")} Mudança de Nome</button>";
            }

            if ((character.JailFinalDate ?? DateTime.MinValue) > DateTime.Now && player.User.Staff >= UserStaff.LeadAdministrator)
                html += $"<br/><br/><button onClick='removeJailCharacter({id})' class='btn btn-dark btn-md'>Soltar da Prisão</button>";

            player.Emit("ACPUpdateCharacter", html);
        }

        [AsyncClientEvent(nameof(StaffBanCharacter))]
        public static async Task StaffBanCharacter(MyPlayer player, int id, int days, string reason)
        {
            if (player.User.Staff < UserStaff.GameAdministrator)
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.Character.Id == id);
            if (target != null)
            {
                player.EmitStaffShowMessage($"O personagem está online! Use /ban {target.SessionId}.");
                return;
            }

            await using var context = new DatabaseContext();
            var character = await context.Characters
                .Include(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (character == null)
            {
                player.EmitStaffShowMessage($"Personagem {id} não existe.");
                return;
            }

            target = Global.SpawnedPlayers.FirstOrDefault(x => x.User.Id == character.UserId);
            if (target != null)
            {
                player.EmitStaffShowMessage($"O usuário do personagem está online! Use /ban {target.SessionId}.");
                return;
            }

            if (character.User.Staff >= player.User.Staff)
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var ban = await context.Banishments.FirstOrDefaultAsync(x => x.CharacterId == character.Id);
            if (ban != null)
            {
                player.EmitStaffShowMessage($"Personagem {id} já está banido.");
                return;
            }

            ban = new Banishment
            {
                ExpirationDate = days > 0 ? DateTime.Now.AddDays(days) : null,
                Reason = reason,
                CharacterId = character.Id,
                UserId = character.User.Id,
                StaffUserId = player.User.Id,
            };

            await context.Banishments.AddAsync(ban);

            await context.Punishments.AddAsync(new Punishment
            {
                Duration = days,
                Reason = reason,
                CharacterId = character.Id,
                Type = PunishmentType.Ban,
                StaffUserId = player.User.Id,
            });
            await context.SaveChangesAsync();

            var strBan = days == 0 ? "permanentemente" : $"por {days} dia{(days > 1 ? "s" : string.Empty)}";
            player.EmitStaffShowMessage($"Você baniu {character.Name} ({character.User.Name}) {strBan}. Motivo: {reason}");

            var html = await GetBansJSON();
            foreach (var targetStaff in Global.SpawnedPlayers.Where(x => x.User.Staff != UserStaff.None))
                targetStaff.Emit("ACPUpdateBans", html);

            await StaffSearchCharacter(player, character.Id.ToString());
        }

        [AsyncClientEvent(nameof(StaffCKAvalationRemoveCharacter))]
        public static async Task StaffCKAvalationRemoveCharacter(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CK))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            var character = await context.Characters.FirstOrDefaultAsync(x => x.Id == id);
            if (character == null)
            {
                player.EmitStaffShowMessage($"Personagem {id} não existe.");
                return;
            }

            if (!character.DeathDate.HasValue && !character.CKAvaliation)
            {
                player.EmitStaffShowMessage($"Personagem {id} não está morto ou em avaliação de CK.");
                return;
            }

            character.DeathDate = null;
            character.DeathReason = string.Empty;
            character.CKAvaliation = false;
            context.Characters.Update(character);
            await context.SaveChangesAsync();
            await Functions.SendStaffMessage($"{player.User.Name} removeu o CK / avaliação de CK do personagem {character.Name}.", true);
            await player.GravarLog(LogType.Staff, $"Remover Avaliação de CK / CK {character.Id}", null);
            await StaffSearchCharacter(player, character.Id.ToString());
        }

        [AsyncClientEvent(nameof(StaffCKCharacter))]
        public static async Task StaffCKCharacter(MyPlayer player, int id, string reason)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CK))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            var character = await context.Characters.FirstOrDefaultAsync(x => x.Id == id);
            if (character == null)
            {
                player.EmitStaffShowMessage($"Personagem {id} não existe.");
                return;
            }

            if (character.DeathDate.HasValue)
            {
                player.EmitStaffShowMessage($"Personagem {id} já está morto.");
                return;
            }

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.Character.Id == id);
            if (target != null)
            {
                target.Character.DeathDate = DateTime.Now;
                target.Character.DeathReason = reason;
                target.Character.CKAvaliation = false;
                await target.Save();
                await target.ListarPersonagens("CK", $"{player.User.Name} aplicou CK no seu personagem. Motivo: {reason}");
            }
            else
            {
                character.DeathDate = DateTime.Now;
                character.DeathReason = reason;
                character.CKAvaliation = false;
                context.Characters.Update(character);
                await context.SaveChangesAsync();
            }

            await Functions.SendStaffMessage($"{player.User.Name} aplicou CK no personagem {character.Name}. Motivo: {reason}", false);
            await player.GravarLog(LogType.Staff, $"CK {character.Id} {reason}", target);
            await StaffSearchCharacter(player, character.Id.ToString());
        }

        [AsyncClientEvent(nameof(StaffCKAvalationCharacter))]
        public static async Task StaffCKAvalationCharacter(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CK))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            var character = await context.Characters.FirstOrDefaultAsync(x => x.Id == id);
            if (character == null)
            {
                player.EmitStaffShowMessage($"Personagem {id} não existe.");
                return;
            }

            if (character.DeathDate.HasValue)
            {
                player.EmitStaffShowMessage($"Personagem {id} já está morto.");
                return;
            }

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.Character.Id == id);
            if (target != null)
            {
                target.Character.CKAvaliation = true;
                await target.Save();
                await target.ListarPersonagens("Avaliação de CK", $"{player.User.Name} colocou seu personagem na avaliação de CK.");
            }
            else
            {
                character.CKAvaliation = true;
                context.Characters.Update(character);
                await context.SaveChangesAsync();
            }

            await Functions.SendStaffMessage($"{player.User.Name} colocou o personagem {character.Name} na avaliação de CK.", false);
            await player.GravarLog(LogType.Staff, $"Avaliação de CK {character.Id}", target);
            await StaffSearchCharacter(player, character.Id.ToString());
        }

        [AsyncClientEvent(nameof(StaffNameChangeStatusCharacter))]
        public static async Task StaffNameChangeStatusCharacter(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CK))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            var character = await context.Characters.FirstOrDefaultAsync(x => x.Id == id);
            if (character == null)
            {
                player.EmitStaffShowMessage($"Personagem {id} não existe.");
                return;
            }

            if (character.NameChangeStatus == CharacterNameChangeStatus.Done)
            {
                player.EmitStaffShowMessage($"Personagem {id} realizou a mudança de nome.");
                return;
            }

            character.NameChangeStatus = character.NameChangeStatus == CharacterNameChangeStatus.Allowed
                ?
                CharacterNameChangeStatus.Blocked
                :
                CharacterNameChangeStatus.Allowed;

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.Character.Id == id);
            if (target != null)
            {
                target.Character.NameChangeStatus = target.Character.NameChangeStatus == CharacterNameChangeStatus.Allowed
                ?
                CharacterNameChangeStatus.Blocked
                :
                CharacterNameChangeStatus.Allowed;
                await target.Save();
                target.SendMessage(MessageType.Success, $"{player.User.Name}{(target.Character.NameChangeStatus == CharacterNameChangeStatus.Allowed ? "des" : string.Empty)}bloqueou a troca de nome do seu personagem.");
            }
            else
            {
                context.Characters.Update(character);
                await context.SaveChangesAsync();
            }

            player.EmitStaffShowMessage($"Você {(character.NameChangeStatus == CharacterNameChangeStatus.Allowed ? "des" : string.Empty)}bloqueou a troca de nome de {character.Name}.");
            await player.GravarLog(LogType.Staff, $"{(character.NameChangeStatus == CharacterNameChangeStatus.Allowed ? "Desbloquear" : "Bloquear")} Mudança de Nome {character.Id}", target);
            await StaffSearchCharacter(player, character.Id.ToString());
        }

        [AsyncClientEvent(nameof(StaffRemoveJailCharacter))]
        public static async Task StaffRemoveJailCharacter(MyPlayer player, int id)
        {
            if (player.User.Staff < UserStaff.LeadAdministrator)
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            var character = await context.Characters.FirstOrDefaultAsync(x => x.Id == id);
            if (character == null)
            {
                player.EmitStaffShowMessage($"Personagem {id} não existe.");
                return;
            }

            if ((character.JailFinalDate ?? DateTime.MinValue) <= DateTime.Now)
            {
                player.EmitStaffShowMessage($"Personagem {id} não está preso.");
                return;
            }

            var jail = await context.Jails.OrderByDescending(x => x.Id).LastOrDefaultAsync(x => x.CharacterId == character.Id);
            if (jail == null)
            {
                player.EmitStaffShowMessage($"Não foi possível encontrar o registro de prisão do personagem {id}.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(jail.Description))
            {
                player.EmitStaffShowMessage($"O relatório da prisão já foi preenchido. Não é possível remover.");
                return;
            }

            context.Jails.Remove(jail);
            await context.SaveChangesAsync();

            character.JailFinalDate = null;
            context.Characters.Update(character);
            await context.SaveChangesAsync();

            player.EmitStaffShowMessage($"Você removeu {character.Name} da prisão.");
            await player.GravarLog(LogType.Staff, $"Remover da Prisão {character.Id}", null);
            await StaffSearchCharacter(player, character.Id.ToString());
        }

        private static async Task<string> GetBansJSON()
        {
            await using var context = new DatabaseContext();
            var banishments = (await context.Banishments
                .Include(x => x.Character)
                .Include(x => x.User)
                .Include(x => x.StaffUser)
                .ToListAsync())
                .OrderByDescending(x => x.ExpirationDate)
                .Select(x => new
                {
                    x.Id,
                    Date = x.Date.ToString(),
                    ExpirationDate = x.ExpirationDate.HasValue ? x.ExpirationDate?.ToString() : "Permanente",
                    x.Reason,
                    Character = $"{x.Character.Name} [{x.CharacterId}]",
                    User = x.UserId.HasValue ? $"{x.User.Name} [{x.UserId}]" : string.Empty,
                    UserStaff = $"{x.StaffUser.Name} [{x.StaffUserId}]",
                });

            return Functions.Serialize(banishments);
        }
    }
}