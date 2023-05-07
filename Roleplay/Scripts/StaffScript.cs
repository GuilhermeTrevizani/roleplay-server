using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;

namespace Roleplay.Scripts
{
    public class StaffScript : IScript
    {
        #region Give Item
        [AsyncClientEvent(nameof(StaffGiveItem))]
        public static async Task StaffGiveItem(MyPlayer player, int category, string type, string extra, int quantity, int targetId)
        {
            var target = Global.Players.FirstOrDefault(x => x.SessionId == targetId && x.Character.Id > 0);
            if (target == null)
            {
                player.EmitStaffShowMessage("Jogador inválido.");
                return;
            }

            if (!Enum.IsDefined(typeof(ItemCategory), Convert.ToByte(category)))
            {
                player.EmitStaffShowMessage($"Categoria {category} não existe.");
                return;
            }

            if (quantity <= 0)
            {
                player.EmitStaffShowMessage("Quantidade deve ser maior que 0.");
                return;
            }

            var item = new CharacterItem((ItemCategory)category)
            {
                Quantity = quantity,
                Extra = extra,
            };

            _ = uint.TryParse(type, out uint realType);

            if (item.Category == ItemCategory.Weapon)
            {
                if (!Enum.TryParse(type, true, out WeaponModel wep))
                {
                    player.EmitStaffShowMessage($"Arma {type} não existe.");
                    return;
                }

                if (!Enum.IsDefined(typeof(WeaponModel), wep))
                {
                    player.EmitStaffShowMessage($"Arma {type} não existe.");
                    return;
                }

                realType = (uint)wep;

                try
                {
                    var itemExtra = JsonSerializer.Deserialize<WeaponItem>(extra);
                    if (itemExtra.Ammo <= 0)
                    {
                        player.EmitStaffShowMessage("Ammo deve ser maior que 0.");
                        return;
                    }

                    if (itemExtra.TintIndex < 0 || itemExtra.TintIndex > 7)
                    {
                        player.EmitStaffShowMessage("TintIndex deve ser entre 0 e 7.");
                        return;
                    }

                    foreach (var c in itemExtra.Components)
                    {
                        var comp = Global.WeaponComponents.FirstOrDefault(x => x.Weapon == wep && x.Hash == c);
                        if (comp == null)
                        {
                            player.EmitStaffShowMessage($"Componente {c} não existe.");
                            return;
                        }
                    }
                }
                catch
                {
                    player.EmitStaffShowMessage("Extra do item não foi informado corretamente.");
                    return;
                }
            }
            else if (item.IsCloth)
            {
                try
                {
                    var itemExtra = JsonSerializer.Deserialize<ClotheAccessoryItem>(extra);

                    var cloth = (item.Category switch
                    {
                        ItemCategory.Cloth1 => target.Character.Sex == CharacterSex.Man ? Global.Clothes1Male : Global.Clothes1Female,
                        ItemCategory.Cloth3 => target.Character.Sex == CharacterSex.Man ? Global.Clothes3Male : Global.Clothes3Female,
                        ItemCategory.Cloth4 => target.Character.Sex == CharacterSex.Man ? Global.Clothes4Male : Global.Clothes4Female,
                        ItemCategory.Cloth5 => target.Character.Sex == CharacterSex.Man ? Global.Clothes5Male : Global.Clothes5Female,
                        ItemCategory.Cloth6 => target.Character.Sex == CharacterSex.Man ? Global.Clothes6Male : Global.Clothes6Female,
                        ItemCategory.Cloth7 => target.Character.Sex == CharacterSex.Man ? Global.Clothes7Male : Global.Clothes7Female,
                        ItemCategory.Cloth8 => target.Character.Sex == CharacterSex.Man ? Global.Clothes8Male : Global.Clothes8Female,
                        ItemCategory.Cloth9 => target.Character.Sex == CharacterSex.Man ? Global.Clothes9Male : Global.Clothes9Female,
                        ItemCategory.Cloth10 => target.Character.Sex == CharacterSex.Man ? Global.Clothes10Male : Global.Clothes10Female,
                        ItemCategory.Cloth11 => target.Character.Sex == CharacterSex.Man ? Global.Clothes11Male : Global.Clothes11Female,
                        ItemCategory.Accessory0 => target.Character.Sex == CharacterSex.Man ? Global.Accessories0Male : Global.Accessories0Female,
                        ItemCategory.Accessory1 => target.Character.Sex == CharacterSex.Man ? Global.Accessories1Male : Global.Accessories1Female,
                        ItemCategory.Accessory2 => target.Character.Sex == CharacterSex.Man ? Global.Accessories2Male : Global.Accessories2Female,
                        ItemCategory.Accessory6 => target.Character.Sex == CharacterSex.Man ? Global.Accessories6Male : Global.Accessories6Female,
                        ItemCategory.Accessory7 => target.Character.Sex == CharacterSex.Man ? Global.Accessories7Male : Global.Accessories7Female,
                        _ => new List<ClotheAccessory>(),
                    }).FirstOrDefault(x => x.Drawable == realType && x.DLC == itemExtra.DLC);

                    if (cloth == null)
                    {
                        player.EmitStaffShowMessage($"Tipo {realType} com a DLC {itemExtra.DLC} não existe.");
                        return;
                    }

                    if (cloth.MaxTexture > 0 && itemExtra.Texture > cloth.MaxTexture)
                    {
                        player.EmitStaffShowMessage($"Texture deve ser entre 0 e {cloth.MaxTexture}.");
                        return;
                    }

                    itemExtra.Sexo = target.Character.Sex;
                    extra = JsonSerializer.Serialize(itemExtra);
                }
                catch
                {
                    player.EmitStaffShowMessage("Extra do item não foi informado corretamente.");
                    return;
                }
            }
            else if (item.Category == ItemCategory.WalkieTalkie)
            {
                extra = JsonSerializer.Serialize(new RadioCommunicatorItem());
            }
            else if (item.Category == ItemCategory.Cellphone)
            {
                extra = JsonSerializer.Serialize(new CellphoneItem { Contatos = Functions.GetDefaultsContacts() });
                realType = await Functions.GetNewCellphoneNumber();
            }

            item = new CharacterItem((ItemCategory)category, realType)
            {
                Quantity = quantity,
                Extra = extra,
            };

            var res = await target.GiveItem(item);

            if (!string.IsNullOrWhiteSpace(res))
            {
                player.EmitStaffShowMessage(res);
                return;
            }

            await Functions.SendStaffMessage($"{player.User.Name} deu {item.Quantity}x {item.Name} para {target.Character.Name}.", true);
            player.EmitStaffShowMessage($"Você deu {item.Quantity}x {item.Name} para {target.Character.Name}.");
            target.SendMessage(MessageType.Success, $"{player.User.Name} deu {item.Quantity}x {item.Name} para você.");

            await player.GravarLog(LogType.DarItem, JsonSerializer.Serialize(item), target);
        }

        #endregion Give Item

        #region Doors
        [ClientEvent(nameof(StaffDoorGoto))]
        public static void StaffDoorGoto(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Doors))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var door = Global.Doors.FirstOrDefault(x => x.Id == id);
            if (door == null)
                return;

            player.LimparIPLs();
            player.SetPosition(new Position(door.PosX, door.PosY, door.PosZ), 0, false);
        }

        [AsyncClientEvent(nameof(StaffDoorRemove))]
        public static async Task StaffDoorRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Doors))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var door = Global.Doors.FirstOrDefault(x => x.Id == id);
            if (door == null)
                return;

            await using var context = new DatabaseContext();
            context.Doors.Remove(door);
            await context.SaveChangesAsync();
            Global.Doors.Remove(door);
            door.Locked = false;
            door.SetupAllClients();

            player.EmitStaffShowMessage($"Porta {door.Id} excluída.");

            var html = Functions.GetDoorsHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Doors)))
                target.Emit("StaffDoors", true, html);

            await player.GravarLog(LogType.Staff, $"Remover Porta | {JsonSerializer.Serialize(door)}", null);
        }

        [AsyncClientEvent(nameof(StaffDoorSave))]
        public static async Task StaffDoorSave(MyPlayer player, int id, string name, long hash, Vector3 pos,
            int factionId, int companyId, bool locked)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Doors))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (factionId != 0 && !Global.Factions.Any(x => x.Id == factionId))
            {
                player.EmitStaffShowMessage($"Facção {factionId} não existe.");
                return;
            }

            if (companyId != 0 && !Global.Companies.Any(x => x.Id == companyId))
            {
                player.EmitStaffShowMessage($"Empresa {factionId} não existe.");
                return;
            }

            var door = new Door();
            if (id > 0)
                door = Global.Doors.FirstOrDefault(x => x.Id == id);

            door.Name = name;
            door.Hash = hash;
            door.PosX = pos.X;
            door.PosY = pos.Y;
            door.PosZ = pos.Z;
            door.FactionId = factionId == 0 ? null : factionId;
            door.CompanyId = companyId == 0 ? null : companyId;
            door.Locked = locked;

            await using var context = new DatabaseContext();

            if (door.Id == 0)
                await context.Doors.AddAsync(door);
            else
                context.Doors.Update(door);

            await context.SaveChangesAsync();

            if (id == 0)
                Global.Doors.Add(door);

            door.SetupAllClients();

            player.EmitStaffShowMessage($"Porta {(id == 0 ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Porta | {JsonSerializer.Serialize(door)}", null);

            var html = Functions.GetDoorsHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Doors)))
                target.Emit("StaffDoors", true, html);
        }
        #endregion Doors

        #region ACP
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

            var html = await Functions.GetBansJSON();
            foreach (var target in Global.Players.Where(x => x.User.Staff != UserStaff.None))
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
                query = query.Where(x => x.Description.ToLower().Contains(description.ToLower()));

            var logs = await query
                .Include(x => x.OriginCharacter)
                .Include(x => x.TargetCharacter)
                .Include(x => x.OriginCharacter.User)
                .Include(x => x.TargetCharacter.User)
                .OrderByDescending(x => x.Id)
                .Take(50)
                .ToListAsync();

            var json = JsonSerializer.Serialize(
                logs.Select(x => new
                {
                    Type = Functions.GetEnumDisplay(x.Type),
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
            User user;
            if (int.TryParse(search, out int id))
                user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            else
                user = await context.Users.FirstOrDefaultAsync(x => x.Name.ToLower() == search.ToLower());

            if (user == null)
            {
                player.Emit("ACPUpdateUser", "<div class='alert alert-danger'>Nenhum usuário encontrado com a pesquisa informada.</div>");
                return;
            }

            var staffFlags = JsonSerializer.Deserialize<List<StaffFlag>>(user.StaffFlagsJSON);

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
            if (personagens.Any())
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
            if (punicoes.Any())
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

            var staffFlags = JsonSerializer.Deserialize<List<string>>(flagsJSON).Select(x => (StaffFlag)Convert.ToByte(x)).ToList();

            user.Staff = (UserStaff)staff;
            if (user.Staff == UserStaff.None)
                user.StaffFlagsJSON = "[]";
            else
                user.StaffFlagsJSON = JsonSerializer.Serialize(staffFlags);

            var target = Global.Players.FirstOrDefault(x => x.User.Id == userId);
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

            var target = Global.Players.FirstOrDefault(x => x.User.Id == userId);
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
            Character character;
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

            var target = Global.Players.FirstOrDefault(x => x.Character.Id == character.Id);
            if (target != null)
            {
                html += await target.ObterHTMLStats();
            }
            else
            {
                var user = await context.Users.FirstOrDefaultAsync(x => x.Id == character.UserId);

                html += $@"OOC: <strong>{user.Name} [{user.Id}]</strong> | Registro: <strong>{character.RegisterDate}</strong> | VIP: <strong>{Functions.GetEnumDisplay(user.VIP)} {(user.VIPValidDate.HasValue ? $"- {(user.VIPValidDate < DateTime.Now ? "Expirado" : "Expira")} em {user.VIPValidDate}" : string.Empty)}</strong><br/>
                Tempo Conectado (minutos): <strong>{character.ConnectedTime}</strong> | Emprego: <strong>{Functions.GetEnumDisplay(character.Job)}</strong> | Trocas de Nome: <strong>{user.NameChanges} {(character.NameChangeStatus == CharacterNameChangeStatus.Bloqueado ? "(BLOQUEADO)" : string.Empty)}</strong> | Trocas de Nome Fórum: <strong>{user.ForumNameChanges}</strong> | Trocas de Placa: <strong>{user.PlateChanges}</strong><br/>
                Banco: <strong>${character.Bank:N0}</strong> | Poupança: <strong>${character.Savings:N0}</strong><br/>";

                if (character.FactionId > 0)
                {
                    var faccao = Global.Factions.FirstOrDefault(x => x.Id == character.FactionId);
                    var rank = Global.FactionsRanks.FirstOrDefault(x => x.Id == character.FactionRankId);
                    html += $"Facção: <strong>{faccao?.Name} [{character.FactionId}]</strong> | Rank: <strong>{rank?.Name} [{character.FactionRankId}]</strong>";
                }

                html += $"<h4>História (aceito por {character.EvaluatorStaffUserId})</h4> {character.History}";

                var itens = (await context.CharactersItems.Where(x => x.CharacterId == character.Id).ToListAsync()).Select(x => new CharacterItem(x)).ToList();
                html += $"<h4>Inventário</h4>";
                if (itens.Any())
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
                if (veiculos.Any())
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
                if (companies.Any())
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

                if (character.NameChangeStatus != CharacterNameChangeStatus.Realizado)
                    html += $"<br/><br/><button onClick='nameChangeStatusCharacter({id})' class='btn btn-dark btn-md'>{(character.NameChangeStatus == CharacterNameChangeStatus.Liberado ? "Bloquear" : "Liberar")} Mudança de Nome</button>";
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

            var target = Global.Players.FirstOrDefault(x => x.Character.Id == id);
            if (target != null)
            {
                player.EmitStaffShowMessage($"O personagem está online! Use /ban {target.SessionId}.");
                return;
            }

            await using var context = new DatabaseContext();
            var character = await context.Characters.FirstOrDefaultAsync(x => x.Id == id);
            if (character == null)
            {
                player.EmitStaffShowMessage($"Personagem {id} não existe.");
                return;
            }

            target = Global.Players.FirstOrDefault(x => x.User.Id == character.UserId);
            if (target != null)
            {
                player.EmitStaffShowMessage($"O usuário do personagem está online! Use /ban {target.SessionId}.");
                return;
            }

            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == character.UserId);
            if (user.Staff >= player.User.Staff)
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
                UserId = user.Id,
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
            player.EmitStaffShowMessage($"Você baniu {character.Name} ({user.Name}) {strBan}. Motivo: {reason}");

            var html = await Functions.GetBansJSON();
            foreach (var targetStaff in Global.Players.Where(x => x.User.Staff != UserStaff.None))
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

            var target = Global.Players.FirstOrDefault(x => x.Character.Id == id);
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

            var target = Global.Players.FirstOrDefault(x => x.Character.Id == id);
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

            if (character.NameChangeStatus == CharacterNameChangeStatus.Realizado)
            {
                player.EmitStaffShowMessage($"Personagem {id} realizou a mudança de nome.");
                return;
            }

            character.NameChangeStatus = character.NameChangeStatus == CharacterNameChangeStatus.Liberado
                ?
                CharacterNameChangeStatus.Bloqueado
                :
                CharacterNameChangeStatus.Liberado;

            var target = Global.Players.FirstOrDefault(x => x.Character.Id == id);
            if (target != null)
            {
                target.Character.NameChangeStatus = target.Character.NameChangeStatus == CharacterNameChangeStatus.Liberado
                ?
                CharacterNameChangeStatus.Bloqueado
                :
                CharacterNameChangeStatus.Liberado;
                await target.Save();
                target.SendMessage(MessageType.Success, $"{player.User.Name}{(target.Character.NameChangeStatus == CharacterNameChangeStatus.Liberado ? "des" : string.Empty)}bloqueou a troca de nome do seu personagem.");
            }
            else
            {
                context.Characters.Update(character);
                await context.SaveChangesAsync();
            }

            player.EmitStaffShowMessage($"Você {(character.NameChangeStatus == CharacterNameChangeStatus.Liberado ? "des" : string.Empty)}bloqueou a troca de nome de {character.Name}.");
            await player.GravarLog(LogType.Staff, $"{(character.NameChangeStatus == CharacterNameChangeStatus.Liberado ? "Desbloquear" : "Bloquear")} Mudança de Nome {character.Id}", target);
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
        #endregion ACP

        #region Parameters
        [AsyncClientEvent(nameof(StaffParametersSave))]
        public static async Task StaffParametersSave(MyPlayer player, string jsonParameters)
        {
            if (player.User.Staff < UserStaff.HeadAdministrator)
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var parametrosAntigo = JsonSerializer.Serialize(Global.Parameter);

            var parameter = JsonSerializer.Deserialize<Parameter>(jsonParameters);

            if (parameter.InitialTimeCrackDen < 0 || parameter.InitialTimeCrackDen > 23)
            {
                player.EmitStaffShowMessage("Hora Inicial para Uso da Boca de Fumo não foi preenchida corretamente.");
                return;
            }

            if (parameter.EndTimeCrackDen < 0 || parameter.EndTimeCrackDen > 23)
            {
                player.EmitStaffShowMessage("Hora Final para Uso da Boca de Fumo não foi preenchida corretamente.");
                return;
            }

            Global.Parameter.VehicleParkValue = parameter.VehicleParkValue;
            Global.Parameter.HospitalValue = parameter.HospitalValue;
            Global.Parameter.BarberValue = parameter.BarberValue;
            Global.Parameter.ClothesValue = parameter.ClothesValue;
            Global.Parameter.DriverLicenseBuyValue = parameter.DriverLicenseBuyValue;
            Global.Parameter.Paycheck = parameter.Paycheck;
            Global.Parameter.DriverLicenseRenewValue = parameter.DriverLicenseRenewValue;
            Global.Parameter.AnnouncementValue = parameter.AnnouncementValue;
            Global.Parameter.ExtraPaymentGarbagemanValue = parameter.ExtraPaymentGarbagemanValue;
            Global.Parameter.Blackout = parameter.Blackout;
            Global.Parameter.KeyValue = parameter.KeyValue;
            Global.Parameter.LockValue = parameter.LockValue;
            Global.Parameter.IPLsJSON = parameter.IPLsJSON ?? "[]";
            Global.Parameter.TattooValue = parameter.TattooValue;
            Global.Parameter.CooldownDismantleHours = parameter.CooldownDismantleHours;
            Global.Parameter.PropertyRobberyConnectedTime = parameter.PropertyRobberyConnectedTime;
            Global.Parameter.CooldownPropertyRobberyRobberHours = parameter.CooldownPropertyRobberyRobberHours;
            Global.Parameter.CooldownPropertyRobberyPropertyHours = parameter.CooldownPropertyRobberyPropertyHours;
            Global.Parameter.PoliceOfficersPropertyRobbery = parameter.PoliceOfficersPropertyRobbery;
            Global.Parameter.InitialTimeCrackDen = parameter.InitialTimeCrackDen;
            Global.Parameter.EndTimeCrackDen = parameter.EndTimeCrackDen;
            Global.Parameter.FirefightersBlockHeal = parameter.FirefightersBlockHeal;

            Global.IPLs.ForEach(ipl => Alt.EmitAllClients("Server:RemoveIpl", ipl));
            Global.IPLs = JsonSerializer.Deserialize<List<string>>(Global.Parameter.IPLsJSON);
            Global.IPLs.ForEach(ipl => Alt.EmitAllClients("Server:RequestIpl", ipl));

            Alt.EmitAllClients("Server:setArtificialLightsState", Global.Parameter.Blackout);

            await using var context = new DatabaseContext();
            context.Parameters.Update(Global.Parameter);
            await context.SaveChangesAsync();

            await player.GravarLog(LogType.Staff, $"Parâmetros | {parametrosAntigo} | {JsonSerializer.Serialize(Global.Parameter)}", null);

            player.EmitStaffShowMessage("Parâmetros do servidor alterados.");
        }
        #endregion Parameters

        #region Prices
        [AsyncClientEvent(nameof(StaffPriceSave))]
        public static async Task StaffPriceSave(MyPlayer player, int id, int type, string name, float value)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Prices))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!Enum.IsDefined(typeof(PriceType), Convert.ToByte(type)))
            {
                player.EmitStaffShowMessage("Tipo inválido.");
                return;
            }

            if (value <= 0)
            {
                player.EmitStaffShowMessage("Valor inválido.");
                return;
            }

            var priceType = (PriceType)type;
            if (priceType == PriceType.Armas)
            {
                if (!Enum.TryParse(name, true, out WeaponModel wep))
                {
                    player.EmitStaffShowMessage($"Arma {name} não existe.");
                    return;
                }
            }
            else if (priceType == PriceType.Empregos || priceType == PriceType.AluguelEmpregos)
            {
                if (!Global.Jobs.Any(x => x.CharacterJob.ToString().ToLower() == name.ToLower()))
                {
                    player.EmitStaffShowMessage($"Emprego {name} não existe.");
                    return;
                }
            }
            else if (priceType == PriceType.Drogas)
            {
                if (!Enum.TryParse(name, true, out ItemCategory itemCategory))
                {
                    player.EmitStaffShowMessage($"Droga {name} não existe.");
                    return;
                }

                if (itemCategory != ItemCategory.Weed && itemCategory != ItemCategory.Cocaine
                    && itemCategory != ItemCategory.Crack && itemCategory != ItemCategory.Heroin
                    && itemCategory != ItemCategory.MDMA && itemCategory != ItemCategory.Xanax
                    && itemCategory != ItemCategory.Oxycontin && itemCategory != ItemCategory.Metanfetamina)
                {
                    player.EmitStaffShowMessage($"Droga {itemCategory} não existe.");
                    return;
                }
            }
            else if (priceType != PriceType.Conveniencia && priceType != PriceType.Tuning)
            {
                if (!Enum.TryParse(name, true, out VehicleModel v1) && !Enum.TryParse(name, true, out VehicleModelMods v2))
                {
                    player.EmitStaffShowMessage($"Veículo {name} não existe.");
                    return;
                }
            }

            if (Global.Prices.Any(x => x.Id != id && x.Type == priceType && x.Name.ToLower() == name.ToLower()))
            {
                player.EmitStaffShowMessage($"Já existe um preço do tipo {Functions.GetEnumDisplay(priceType)} com o nome {name}.");
                return;
            }

            var price = new Price();
            if (id > 0)
                price = Global.Prices.FirstOrDefault(x => x.Id == id);

            price.Type = priceType;
            price.Name = name;
            price.Value = value;

            await using var context = new DatabaseContext();

            if (price.Id == 0)
                await context.Prices.AddAsync(price);
            else
                context.Prices.Update(price);

            await context.SaveChangesAsync();

            if (id == 0)
                Global.Prices.Add(price);

            player.EmitStaffShowMessage($"Preço {(id == 0 ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Preço | {JsonSerializer.Serialize(price)}", null);

            var html = Functions.GetPricesHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Prices)))
                target.Emit("StaffPrices", true, html);
        }

        [AsyncClientEvent(nameof(StaffPriceRemove))]
        public static async Task StaffPriceRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Prices))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var price = Global.Prices.FirstOrDefault(x => x.Id == id);
            if (price != null)
            {
                await using var context = new DatabaseContext();
                context.Prices.Remove(price);
                await context.SaveChangesAsync();
                Global.Prices.Remove(price);
                await player.GravarLog(LogType.Staff, $"Remover Preço | {JsonSerializer.Serialize(price)}", null);
            }

            player.EmitStaffShowMessage($"Preço {id} excluído.");

            var html = Functions.GetPricesHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Prices)))
                target.Emit("StaffPrices", true, html);
        }
        #endregion Prices

        #region Factions
        [AsyncClientEvent(nameof(StaffFactionSave))]
        public static async Task StaffFactionSave(MyPlayer player, int id, string name, int type, string color,
            int slots, string chatColor)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Factions))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!Enum.IsDefined(typeof(FactionType), Convert.ToByte(type)))
            {
                player.EmitStaffShowMessage("Tipo inválido.");
                return;
            }

            if (slots < 0)
            {
                player.EmitStaffShowMessage("Slots deve ser maior ou igual a zero.");
                return;
            }

            var faction = new Faction();
            if (id > 0)
                faction = Global.Factions.FirstOrDefault(x => x.Id == id);

            faction.Name = name;
            faction.Type = (FactionType)Convert.ToByte(type);
            faction.Color = color;
            faction.Slots = slots;
            faction.ChatColor = chatColor;

            await using var context = new DatabaseContext();

            if (faction.Id == 0)
                await context.Factions.AddAsync(faction);
            else
                context.Factions.Update(faction);

            await context.SaveChangesAsync();

            if (id == 0)
                Global.Factions.Add(faction);

            player.EmitStaffShowMessage($"Facção {(id == 0 ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Facção | {JsonSerializer.Serialize(faction)}", null);

            var html = Functions.GetFactionsHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Factions)))
                target.Emit("StaffFactions", true, html);
        }

        [ClientEvent(nameof(StaffFactionShowRanks))]
        public static void StaffFactionShowRanks(MyPlayer player, int factionId)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Factions))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("Server:CloseView");
            var htmlFactionRanks = Functions.GetFactionRanksHTML(factionId);
            player.Emit("StaffShowFactionRanks",
                false,
                htmlFactionRanks,
                factionId,
                Global.Factions.FirstOrDefault(x => x.Id == factionId)?.Name);
        }

        [AsyncClientEvent(nameof(StaffFactionRankSave))]
        public static async Task StaffFactionRankSave(MyPlayer player, int factionId, int factionRankId,
            string name, int salary)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Factions))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (salary < 0)
            {
                player.EmitStaffShowMessage("Salário deve ser maior que 0.");
                return;
            }

            var factionRank = new FactionRank();
            if (factionRankId > 0)
                factionRank = Global.FactionsRanks.FirstOrDefault(x => x.Id == factionRankId);
            else
                factionRank.Position = Global.FactionsRanks.Select(x => x.Position).DefaultIfEmpty(0).Max() + 1;

            factionRank.FactionId = factionId;
            factionRank.Name = name;
            factionRank.Salary = salary;

            await using var context = new DatabaseContext();

            if (factionRank.Id == 0)
                await context.FactionsRanks.AddAsync(factionRank);
            else
                context.FactionsRanks.Update(factionRank);

            await context.SaveChangesAsync();

            if (factionRankId == 0)
                Global.FactionsRanks.Add(factionRank);

            player.EmitStaffShowMessage($"Rank {(factionRankId == 0 ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Rank | {JsonSerializer.Serialize(factionRank)}", null);

            var html = Functions.GetFactionRanksHTML(factionId);
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Factions)))
                target.Emit("StaffShowFactionRanks", true, html);
        }

        [AsyncClientEvent(nameof(StaffFactionRankRemove))]
        public static async Task StaffFactionRankRemove(MyPlayer player, int factionRankId)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Factions))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var factionRank = Global.FactionsRanks.FirstOrDefault(x => x.Id == factionRankId);
            if (factionRank == null)
                return;

            await using var context = new DatabaseContext();

            if (await context.Characters.AnyAsync(x => x.FactionRankId == factionRankId))
            {
                player.EmitStaffShowMessage($"Não é possível remover o rank {factionRankId} pois existem personagens nele.");
                return;
            }

            context.FactionsRanks.Remove(factionRank);
            await context.SaveChangesAsync();
            Global.FactionsRanks.Remove(factionRank);

            player.EmitStaffShowMessage($"Rank {factionRank.Id} excluído.");

            await player.GravarLog(LogType.Staff, $"Remover Rank | {JsonSerializer.Serialize(factionRank)}", null);

            var html = Functions.GetFactionRanksHTML(factionRank.FactionId);
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Factions)))
                target.Emit("StaffShowFactionRanks", true, html);
        }

        [AsyncClientEvent(nameof(StaffFactionRankOrder))]
        public static async Task StaffFactionRankOrder(MyPlayer player, int factionId, string ranksJSON)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Factions))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();

            var ranks = JsonSerializer.Deserialize<List<FactionRank>>(ranksJSON);
            foreach (var rank in ranks)
            {
                var factionRank = Global.FactionsRanks.FirstOrDefault(x => x.Id == rank.Id);
                factionRank.Position = rank.Position;
                context.FactionsRanks.Update(factionRank);
            }

            await context.SaveChangesAsync();
            player.EmitStaffShowMessage("Ranks ordenados.");

            await player.GravarLog(LogType.Faction, $"Ordenar Ranks | {ranksJSON}", null);

            var html = Functions.GetFactionRanksHTML(factionId);
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Factions)))
                target.Emit("StaffShowFactionRanks", true, html);
        }

        [AsyncClientEvent(nameof(StaffFactionShowMembers))]
        public static async Task StaffFactionShowMembers(MyPlayer player, int factionId)
        {
            player.Emit("Server:CloseView");
            var ranksJson = JsonSerializer.Serialize(Global.FactionsRanks.Where(x => x.FactionId == factionId).OrderBy(x => x.Position));

            var faction = Global.Factions.FirstOrDefault(x => x.Id == factionId);

            var flagsJson = JsonSerializer.Serialize(
                faction.GetFlags()
                .Select(x => new
                {
                    Id = x,
                    Name = Functions.GetEnumDisplay(x),
                })
            );

            var htmlFactionMembers = await Functions.GetFactionMembersHTML(factionId);
            player.Emit("StaffShowFactionMembers",
                false,
                htmlFactionMembers,
                factionId,
                faction.Name,
                faction.Government,
                ranksJson,
                flagsJson);
        }

        [AsyncClientEvent(nameof(StaffFactionMemberInvite))]
        public static async Task StaffFactionMemberInvite(MyPlayer player, int factionId, int characterSessionId)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Factions))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var faction = Global.Factions.FirstOrDefault(x => x.Id == factionId);
            if (faction == null)
                return;

            await using var context = new DatabaseContext();
            if (faction.Slots > 0)
            {
                var members = await context.Characters.CountAsync(x => x.FactionId == faction.Id && !x.DeathDate.HasValue && !x.DeletedDate.HasValue);
                if (members >= faction.Slots)
                {
                    player.EmitStaffShowMessage($"Facção atingiu o máximo de slots ({faction.Slots}).");
                    return;
                }
            }

            var rank = Global.FactionsRanks.Where(x => x.FactionId == faction.Id).MinBy(x => x.Position);
            if (rank == null)
            {
                player.EmitStaffShowMessage("Facção não possui ranks.");
                return;
            }

            var target = Global.Players.FirstOrDefault(x => x.SessionId == characterSessionId);
            if (target == null)
            {
                player.EmitStaffShowMessage($"Nenhum personagem online com o ID {characterSessionId}.");
                return;
            }

            if (target.Character.FactionId.HasValue)
            {
                player.EmitStaffShowMessage("Personagem já está em uma facção.");
                return;
            }

            var convite = new Invite()
            {
                Type = InviteType.Faccao,
                SenderCharacterId = player.Character.Id,
                Value = new string[] { faction.Id.ToString(), rank.Id.ToString() },
            };
            target.Invites.RemoveAll(x => x.Type == InviteType.Faccao);
            target.Invites.Add(convite);

            player.EmitStaffShowMessage($"Você convidou {target.Character.Name} para a facção.", true);
            target.SendMessage(MessageType.Success, $"{player.User.Name} convidou você para a facção {faction.Name}. (/ac {(int)convite.Type} para aceitar ou /rc {(int)convite.Type} para recusar)");

            await player.GravarLog(LogType.Staff, $"Convidar Facção {factionId}", target);
        }

        [AsyncClientEvent(nameof(StaffFactionMemberSave))]
        public static async Task StaffFactionMemberSave(MyPlayer player, int factionId, int characterId, int factionRankId,
            int badge, string flagsJSON)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Factions))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            var character = await context.Characters.FirstOrDefaultAsync(x => x.Id == characterId);
            if (character == null)
            {
                player.EmitStaffShowMessage($"Nenhum jogador encontrado com o ID {characterId}.");
                return;
            }

            if (character.FactionId != factionId)
            {
                player.EmitStaffShowMessage($"Jogador não pertence a esta facção.");
                return;
            }

            var rank = Global.FactionsRanks.FirstOrDefault(x => x.Id == factionRankId);
            if (rank?.FactionId != factionId)
            {
                player.EmitStaffShowMessage($"Rank {factionRankId} não existe na facção {factionId}.");
                return;
            }

            if (badge < 0)
            {
                player.EmitStaffShowMessage($"Distintivo deve ser maior que zero.");
                return;
            }

            if (badge > 0)
            {
                var characterTarget = await context.Characters.FirstOrDefaultAsync(x => x.FactionId == factionId
                    && x.Badge == badge
                    && !x.DeathDate.HasValue
                    && !x.DeletedDate.HasValue);
                if (characterTarget != null && characterTarget.Id != character.Id)
                {
                    player.EmitStaffShowMessage($"Distintivo {badge} está sendo usado por {characterTarget.Name}.");
                    return;
                }
            }

            var factionFlags = JsonSerializer.Deserialize<List<string>>(flagsJSON).Select(x => (FactionFlag)Convert.ToByte(x)).ToList();

            var target = Global.Players.FirstOrDefault(x => x.Character.Id == character.Id);
            if (target != null)
            {
                target.Character.FactionRankId = factionRankId;
                target.Character.Badge = badge;
                target.FactionFlags = factionFlags;
                target.Character.FactionFlagsJSON = JsonSerializer.Serialize(target.FactionFlags);
                target.SendMessage(MessageType.Success, $"{player.User.Name} alterou suas informações na facção.");
                await target.Save();
            }
            else
            {
                character.FactionRankId = factionRankId;
                character.Badge = badge;
                character.FactionFlagsJSON = JsonSerializer.Serialize(factionFlags);
                context.Characters.Update(character);
                await context.SaveChangesAsync();
            }

            player.EmitStaffShowMessage($"Você alterou as informações de {character.Name} na facção.", true);
            await player.GravarLog(LogType.Staff, $"Salvar Membro Facção {factionId} {characterId} {factionRankId} {badge} {flagsJSON}", target);

            var html = await Functions.GetFactionMembersHTML(factionId);
            foreach (var targetStaff in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Factions)))
                target.Emit("StaffShowFactionMembers", true, html);
        }

        [AsyncClientEvent(nameof(StaffFactionMemberRemove))]
        public static async Task StaffFactionMemberRemove(MyPlayer player, int factionId, int characterId)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Factions))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            var character = await context.Characters.FirstOrDefaultAsync(x => x.Id == characterId);
            if (character == null)
            {
                player.EmitStaffShowMessage($"Nenhum jogador encontrado com o ID {characterId}.");
                return;
            }

            if (character.FactionId != factionId)
            {
                player.EmitStaffShowMessage($"Jogador não pertence a esta facção.");
                return;
            }

            var target = Global.Players.FirstOrDefault(x => x.Character.Id == character.Id);
            if (target != null)
            {
                await target.RemoveFromFaction();
                await target.Save();
                target.SendMessage(MessageType.Success, $"{player.User.Name} expulsou você da facção.");
            }
            else
            {
                var faction = Global.Factions.FirstOrDefault(x => x.Id == factionId);

                if (faction?.Government ?? false)
                {
                    character.Badge = 0;
                    character.Armor = 0;

                    var items = (await context.CharactersItems.Where(x => x.CharacterId == character.Id).ToListAsync()).Select(x => new CharacterItem(x)).ToList();
                    items = items.Where(x => !Functions.CanDropItem(character.Sex, faction, x)).ToList();
                    context.CharactersItems.RemoveRange(items);
                }

                character.FactionFlagsJSON = "[]";
                character.FactionId = character.FactionRankId = null;
                context.Characters.Update(character);
                await context.SaveChangesAsync();
            }

            player.EmitStaffShowMessage($"Você expulsou {character.Name} da facção.", true);
            await player.GravarLog(LogType.Staff, $"Expulsar Facção {factionId} {characterId}", target);

            var html = await Functions.GetFactionMembersHTML(factionId);
            foreach (var targetStaff in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Factions)))
                target.Emit("StaffShowFactionMembers", true, html);
        }
        #endregion Factions

        #region Factions Armories
        [ClientEvent(nameof(StaffFactionArmoryGoto))]
        public static void StaffFactionArmoryGoto(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsArmories))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var factionArmory = Global.FactionsArmories.FirstOrDefault(x => x.Id == id);
            if (factionArmory == null)
                return;

            player.LimparIPLs();

            if (factionArmory.Dimension > 0)
            {
                player.IPLs = Functions.GetIPLsByInterior(Global.Properties.FirstOrDefault(x => x.Id == factionArmory.Dimension).Interior);
                player.SetarIPLs();
            }

            player.SetPosition(new Position(factionArmory.PosX, factionArmory.PosY, factionArmory.PosZ), factionArmory.Dimension, false);
        }

        [AsyncClientEvent(nameof(StaffFactionArmoryRemove))]
        public static async Task StaffFactionArmoryRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsArmories))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var factionArmory = Global.FactionsArmories.FirstOrDefault(x => x.Id == id);
            if (factionArmory == null)
                return;

            await using var context = new DatabaseContext();
            context.FactionsArmories.Remove(factionArmory);
            context.FactionsArmoriesWeapons.RemoveRange(Global.FactionsArmoriesWeapons.Where(x => x.FactionArmoryId == id));
            await context.SaveChangesAsync();
            Global.FactionsArmories.Remove(factionArmory);
            Global.FactionsArmoriesWeapons.RemoveAll(x => x.FactionArmoryId == factionArmory.Id);
            factionArmory.RemoveIdentifier();

            player.EmitStaffShowMessage($"Arsenal {factionArmory.Id} excluído.");

            var html = Functions.GetFactionsArmoriesHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsArmories)))
                target.Emit("StaffFactionsArmories", true, html);

            await player.GravarLog(LogType.Staff, $"Remover Arsenal | {JsonSerializer.Serialize(factionArmory)}", null);
        }

        [AsyncClientEvent(nameof(StaffFactionArmorySave))]
        public static async Task StaffFactionArmorySave(MyPlayer player, int id, int factionId, Vector3 pos, int dimension)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsArmories))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (factionId != 0 && !Global.Factions.Any(x => x.Id == factionId))
            {
                player.EmitStaffShowMessage($"Facção {factionId} não existe.");
                return;
            }

            var factionArmory = new FactionArmory();
            if (id > 0)
                factionArmory = Global.FactionsArmories.FirstOrDefault(x => x.Id == id);

            factionArmory.FactionId = factionId;
            factionArmory.PosX = pos.X;
            factionArmory.PosY = pos.Y;
            factionArmory.PosZ = pos.Z;
            factionArmory.Dimension = dimension;

            await using var context = new DatabaseContext();

            if (factionArmory.Id == 0)
                await context.FactionsArmories.AddAsync(factionArmory);
            else
                context.FactionsArmories.Update(factionArmory);

            await context.SaveChangesAsync();

            if (id == 0)
                Global.FactionsArmories.Add(factionArmory);

            factionArmory.CreateIdentifier();

            player.EmitStaffShowMessage($"Arsenal {(id == 0 ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Arsenal | {JsonSerializer.Serialize(factionArmory)}", null);

            var html = Functions.GetFactionsArmoriesHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsArmories)))
                target.Emit("StaffFactionsArmories", true, html);
        }

        [ClientEvent(nameof(StaffFactionsArmorysWeaponsShow))]
        public static void StaffFactionsArmorysWeaponsShow(MyPlayer player, int factionArmoryId)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsArmories))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("Server:CloseView");

            player.Emit("StaffFactionsArmoriesWeapons",
                false,
                Functions.GetFactionArmoriesWeaponsHTML(factionArmoryId, true),
                factionArmoryId);
        }

        [AsyncClientEvent(nameof(StaffFactionArmoryWeaponSave))]
        public static async Task StaffFactionArmoryWeaponSave(MyPlayer player, int factionArmoryWeaponId, int factionArmoryId, string weapon,
            int ammo, int quantity, int tintIndex, string componentsJSON)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsArmories))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!Enum.TryParse(weapon, true, out WeaponModel wep) || wep == 0)
            {
                player.EmitStaffShowMessage($"Arma {weapon} não existe.");
                return;
            }

            if (ammo <= 0)
            {
                player.EmitStaffShowMessage($"Munição deve ser maior que 0.");
                return;
            }

            if (quantity < 0)
            {
                player.EmitStaffShowMessage($"Estoque deve ser maior ou igual a 0.");
                return;
            }

            if (tintIndex < 0 || tintIndex > 7)
            {
                player.EmitStaffShowMessage($"Pintura deve ser entre 0 e 7.");
                return;
            }

            var realComponents = new List<uint>();
            var components = JsonSerializer.Deserialize<List<string>>(componentsJSON);
            foreach (var component in components)
            {
                var comp = Global.WeaponComponents.FirstOrDefault(x => x.Name.ToLower() == component.ToLower() && x.Weapon == wep);
                if (comp == null)
                {
                    player.EmitStaffShowMessage($"Componente {component} não existe para a arma {wep}.");
                    return;
                }

                if (realComponents.Contains(comp.Hash))
                {
                    player.EmitStaffShowMessage($"Componente {component} foi inserido na lista mais de uma vez.");
                    return;
                }

                realComponents.Add(comp.Hash);
            }

            var factionArmoryWeapon = new FactionArmoryWeapon();
            if (factionArmoryWeaponId > 0)
                factionArmoryWeapon = Global.FactionsArmoriesWeapons.FirstOrDefault(x => x.Id == factionArmoryWeaponId);

            factionArmoryWeapon.FactionArmoryId = factionArmoryId;
            factionArmoryWeapon.Weapon = wep;
            factionArmoryWeapon.Ammo = ammo;
            factionArmoryWeapon.Quantity = quantity;
            factionArmoryWeapon.TintIndex = Convert.ToByte(tintIndex);
            factionArmoryWeapon.ComponentsJSON = JsonSerializer.Serialize(realComponents);

            await using var context = new DatabaseContext();

            if (factionArmoryWeapon.Id == 0)
                await context.FactionsArmoriesWeapons.AddAsync(factionArmoryWeapon);
            else
                context.FactionsArmoriesWeapons.Update(factionArmoryWeapon);

            await context.SaveChangesAsync();

            if (factionArmoryWeaponId == 0)
                Global.FactionsArmoriesWeapons.Add(factionArmoryWeapon);

            player.EmitStaffShowMessage($"Arma {(factionArmoryWeaponId == 0 ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Arma Arsenal | {JsonSerializer.Serialize(factionArmoryWeapon)}", null);

            var html = Functions.GetFactionArmoriesWeaponsHTML(factionArmoryId, true);
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsArmories)))
                target.Emit("StaffFactionsArmoriesWeapons", true, html);
        }

        [AsyncClientEvent(nameof(StaffFactionArmoryWeaponRemove))]
        public static async Task StaffFactionArmoryWeaponRemove(MyPlayer player, int factionArmoryWeaponId)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsArmories))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var factionArmoryWeapon = Global.FactionsArmoriesWeapons.FirstOrDefault(x => x.Id == factionArmoryWeaponId);
            if (factionArmoryWeapon == null)
                return;

            await using var context = new DatabaseContext();
            context.FactionsArmoriesWeapons.Remove(factionArmoryWeapon);
            await context.SaveChangesAsync();
            Global.FactionsArmoriesWeapons.Remove(factionArmoryWeapon);

            player.EmitStaffShowMessage($"Arma do Arsenal {factionArmoryWeapon.Id} excluída.");

            await player.GravarLog(LogType.Staff, $"Remover Arma Arsenal | {JsonSerializer.Serialize(factionArmoryWeapon)}", null);

            var html = Functions.GetFactionArmoriesWeaponsHTML(factionArmoryWeapon.FactionArmoryId, true);
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsArmories)))
                target.Emit("StaffFactionsArmoriesWeapons", true, html);
        }
        #endregion Factions Armories

        #region Properties

        [ClientEvent(nameof(StaffPropertyGoto))]
        public static void StaffPropertyGoto(MyPlayer player, int id)
        {
            var property = Global.Properties.FirstOrDefault(x => x.Id == id);
            if (property == null)
                return;

            player.LimparIPLs();
            player.SetPosition(new Position(property.EntrancePosX, property.EntrancePosY, property.EntrancePosZ), property.Dimension, false);
        }

        [AsyncClientEvent(nameof(StaffPropertySave))]
        public static async Task StaffPropertySave(MyPlayer player, int id, int interior, int value, int dimension, Vector3 pos, string address)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Properties))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!Enum.IsDefined(typeof(PropertyInterior), Convert.ToByte(interior)))
            {
                player.EmitStaffShowMessage("Interior inválido.");
                return;
            }

            if (value <= 0)
            {
                player.EmitStaffShowMessage("Valor deve ser maior que 0.");
                return;
            }

            var property = new Property();
            if (id > 0)
                property = Global.Properties.FirstOrDefault(x => x.Id == id);
            else
                property.LockNumber = Global.Properties.Select(x => x.LockNumber).DefaultIfEmpty(0u).Max() + 1;

            var propertyInterior = (PropertyInterior)Convert.ToByte(interior);
            var exit = Functions.GetExitPositionByInterior(propertyInterior);

            property.Interior = propertyInterior;
            property.EntrancePosX = pos.X;
            property.EntrancePosY = pos.Y;
            property.EntrancePosZ = pos.Z;
            property.Value = value;
            property.ExitPosX = exit.X;
            property.ExitPosY = exit.Y;
            property.ExitPosZ = exit.Z;
            property.Dimension = dimension;
            property.Address = address;

            await using var context = new DatabaseContext();

            if (property.Id == 0)
                await context.Properties.AddAsync(property);
            else
                context.Properties.Update(property);

            await context.SaveChangesAsync();

            property.CreateIdentifier();

            if (id == 0)
            {
                property.Items = new List<PropertyItem>();
                Global.Properties.Add(property);
            }

            player.EmitStaffShowMessage($"Propriedade {(id == 0 ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Propriedade | {JsonSerializer.Serialize(property)}", null);

            var html = Functions.GetPropertiesHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Properties)))
                target.Emit("StaffProperties", true, html);
        }

        [AsyncClientEvent(nameof(StaffPropertyRemove))]
        public static async Task StaffPropertyRemove(MyPlayer player, int id)
        {
            try
            {
                if (!player.StaffFlags.Contains(StaffFlag.Properties))
                {
                    player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                    return;
                }

                var property = Global.Properties.FirstOrDefault(x => x.Id == id);
                if (property != null)
                {
                    if (property.CharacterId > 0)
                    {
                        player.EmitStaffShowMessage($"Propriedade {id} possui um dono.");
                        return;
                    }

                    await using var context = new DatabaseContext();
                    context.Properties.Remove(property);
                    await context.SaveChangesAsync();

                    if (property.Items.Any())
                    {
                        foreach (var propertyItem in property.Items)
                            context.PropertiesItems.Remove(propertyItem);
                        await context.SaveChangesAsync();
                    }

                    Global.Properties.Remove(property);
                    property.RemoveIdentifier();
                    await player.GravarLog(LogType.Staff, $"Remover Propriedade | {JsonSerializer.Serialize(property)}", null);
                }

                player.EmitStaffShowMessage($"Propriedade {id} excluída.");

                var html = Functions.GetPropertiesHTML();
                foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Properties)))
                    target.Emit("StaffProperties", true, html);
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }
        #endregion Properties

        #region Spots
        [ClientEvent(nameof(StaffSpotGoto))]
        public static void StaffSpotGoto(MyPlayer player, int id)
        {
            var spot = Global.Spots.FirstOrDefault(x => x.Id == id);
            if (spot == null)
                return;

            player.LimparIPLs();
            player.SetPosition(new Position(spot.PosX, spot.PosY, spot.PosZ), 0, false);
        }

        [AsyncClientEvent(nameof(StaffSpotSave))]
        public static async Task StaffSpotSave(MyPlayer player, int id, int type, Vector3 pos, Vector3 auxiliarPos)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Spots))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!Enum.IsDefined(typeof(SpotType), Convert.ToByte(type)))
            {
                player.EmitStaffShowMessage("Tipo inválido.");
                return;
            }

            var spot = new Spot();
            if (id > 0)
                spot = Global.Spots.FirstOrDefault(x => x.Id == id);

            spot.Type = (SpotType)type;
            spot.PosX = pos.X;
            spot.PosY = pos.Y;
            spot.PosZ = pos.Z;
            spot.AuxiliarPosX = auxiliarPos.X;
            spot.AuxiliarPosY = auxiliarPos.Y;
            spot.AuxiliarPosZ = auxiliarPos.Z;

            await using var context = new DatabaseContext();

            if (spot.Id == 0)
                await context.Spots.AddAsync(spot);
            else
                context.Spots.Update(spot);

            await context.SaveChangesAsync();

            spot.CreateIdentifier();

            if (id == 0)
                Global.Spots.Add(spot);

            player.EmitStaffShowMessage($"Ponto {(id == 0 ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Ponto | {JsonSerializer.Serialize(spot)}", null);

            var html = Functions.GetSpotsHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Spots)))
                target.Emit("StaffSpots", true, html);
        }

        [AsyncClientEvent(nameof(StaffSpotRemove))]
        public static async Task StaffSpotRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Spots))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var spot = Global.Spots.FirstOrDefault(x => x.Id == id);
            if (spot != null)
            {
                await using var context = new DatabaseContext();
                context.Spots.Remove(spot);
                await context.SaveChangesAsync();
                Global.Spots.Remove(spot);
                spot.RemoveIdentifier();
                await player.GravarLog(LogType.Staff, $"Remover Ponto | {JsonSerializer.Serialize(spot)}", null);
            }

            player.EmitStaffShowMessage($"Ponto {id} excluído.");

            var html = Functions.GetSpotsHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Spots)))
                target.Emit("StaffSpots", true, html);
        }
        #endregion Spots

        #region Blips
        [ClientEvent(nameof(StaffBlipGoto))]
        public static void StaffBlipGoto(MyPlayer player, int id)
        {
            var blip = Global.Blips.FirstOrDefault(x => x.Id == id);
            if (blip == null)
                return;

            player.LimparIPLs();
            player.SetPosition(new Position(blip.PosX, blip.PosY, blip.PosZ), 0, false);
        }

        [AsyncClientEvent(nameof(StaffBlipSave))]
        public static async Task StaffBlipSave(MyPlayer player, int id, string name, Vector3 pos, int type, int color)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Blips))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (type < 1 || type > 744)
            {
                player.EmitStaffShowMessage("Tipo deve ser entre 1 e 744.");
                return;
            }

            if (color < 1 || color > 85)
            {
                player.EmitStaffShowMessage("Cor deve ser entre 1 e 85.");
                return;
            }

            var blip = new Blip();
            if (id > 0)
                blip = Global.Blips.FirstOrDefault(x => x.Id == id);

            blip.Name = name;
            blip.PosX = pos.X;
            blip.PosY = pos.Y;
            blip.PosZ = pos.Z;
            blip.Type = Convert.ToUInt16(type);
            blip.Color = Convert.ToByte(color);

            await using var context = new DatabaseContext();

            if (blip.Id == 0)
                await context.Blips.AddAsync(blip);
            else
                context.Blips.Update(blip);

            await context.SaveChangesAsync();

            blip.CreateIdentifier();

            if (id == 0)
                Global.Blips.Add(blip);

            player.EmitStaffShowMessage($"Blip {(id == 0 ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Blip | {JsonSerializer.Serialize(blip)}", null);

            var html = Functions.GetBlipsHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Blips)))
                target.Emit("StaffBlips", true, html);
        }

        [AsyncClientEvent(nameof(StaffBlipRemove))]
        public static async Task StaffBlipRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Blips))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var blip = Global.Blips.FirstOrDefault(x => x.Id == id);
            if (blip != null)
            {
                await using var context = new DatabaseContext();
                context.Blips.Remove(blip);
                await context.SaveChangesAsync();
                Global.Blips.Remove(blip);
                blip.RemoveIdentifier();
                await player.GravarLog(LogType.Staff, $"Remover Blip | {JsonSerializer.Serialize(blip)}", null);
            }

            player.EmitStaffShowMessage($"Blip {id} excluído.");

            var html = Functions.GetBlipsHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Blips)))
                target.Emit("StaffBlips", true, html);
        }
        #endregion Blips

        #region Vehicles
        [AsyncClientEvent(nameof(StaffVehicleSave))]
        public static async Task StaffVehicleSave(MyPlayer player, int id, string model, int type, int typeId,
            int livery, int color1R, int color1G, int color1B, int color2R, int color2G, int color2B)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Vehicles))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (id > 0 && Global.Vehicles.Any(x => x.VehicleDB.Id == id))
            {
                player.EmitStaffShowMessage($"Veículo {id} está spawnado.");
                return;
            }

            if (id == 0 && !Enum.TryParse(model, true, out VehicleModel v1) && !Enum.TryParse(model, true, out VehicleModelMods v2))
            {
                player.EmitStaffShowMessage($"Modelo {model} não existe.");
                return;
            }

            if (!byte.TryParse(livery.ToString(), out byte bLivery) || bLivery == 0)
            {
                player.EmitStaffShowMessage($"Livery deve ser entre 1 e 255.");
                return;
            }

            await using var context = new DatabaseContext();

            var vehicle = new Vehicle();
            if (id > 0)
                vehicle = await context.Vehicles.FirstOrDefaultAsync(x => x.Id == id);

            var government = false;
            if (type == 1) // Faction
            {
                var faction = Global.Factions.FirstOrDefault(x => x.Id == typeId);
                if (!(faction?.Government ?? false))
                {
                    player.EmitStaffShowMessage($"Facção {typeId} não é do tipo governamental.");
                    return;
                }

                government = true;
                vehicle.FactionId = typeId;
            }
            else if (type == 2) // Job
            {
                var job = Global.Jobs.FirstOrDefault(x => x.CharacterJob == (CharacterJob)typeId);
                if (job == null)
                {
                    player.EmitStaffShowMessage($"Emprego {typeId} não existe.");
                    return;
                }

                vehicle.Job = job.CharacterJob;
                vehicle.Color1R = job.VehicleColor.R;
                vehicle.Color1G = job.VehicleColor.G;
                vehicle.Color1B = job.VehicleColor.B;
                vehicle.Color2R = job.VehicleColor.R;
                vehicle.Color2G = job.VehicleColor.G;
                vehicle.Color2B = job.VehicleColor.B;
            }
            else if (type == 3) // Faction Gift
            {
                var character = await context.Characters.FirstOrDefaultAsync(x => x.Id == typeId);
                if (character == null)
                {
                    player.EmitStaffShowMessage($"Personagem {typeId} não existe.");
                    return;
                }

                vehicle.CharacterId = typeId;
                vehicle.FactionGift = true;
            }

            vehicle.Livery = bLivery;

            if (type != 2)
            {
                vehicle.Color1R = Convert.ToByte(color1R);
                vehicle.Color1G = Convert.ToByte(color1G);
                vehicle.Color1B = Convert.ToByte(color1B);
                vehicle.Color2R = Convert.ToByte(color2R);
                vehicle.Color2G = Convert.ToByte(color2G);
                vehicle.Color2B = Convert.ToByte(color2B);
            }

            if (vehicle.Id == 0)
            {
                vehicle.Model = model;
                vehicle.PosX = player.Position.X;
                vehicle.PosY = player.Position.Y;
                vehicle.PosZ = player.Position.Z;
                vehicle.RotR = player.Rotation.Roll;
                vehicle.RotP = player.Rotation.Pitch;
                vehicle.RotY = player.Rotation.Yaw;
                vehicle.Fuel = vehicle.MaxFuel;
                vehicle.Plate = await Functions.GenerateVehiclePlate(government);
                vehicle.LockNumber = await context.Vehicles.OrderByDescending(x => x.LockNumber).Select(x => x.LockNumber).FirstOrDefaultAsync() + 1;
                await context.Vehicles.AddAsync(vehicle);
            }
            else
            {
                context.Vehicles.Update(vehicle);
            }

            await context.SaveChangesAsync();

            player.EmitStaffShowMessage($"Veículo {(id == 0 ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Veículo | {JsonSerializer.Serialize(vehicle)}", null);

            var html = await Functions.GetVehiclesHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Vehicles)))
                target.Emit("StaffVehicles", true, html);
        }

        [AsyncClientEvent(nameof(StaffVehicleRemove))]
        public static async Task StaffVehicleRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Vehicles))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (Global.Vehicles.Any(x => x.VehicleDB.Id == id))
            {
                player.EmitStaffShowMessage($"Veículo {id} está spawnado.");
                return;
            }

            await using var context = new DatabaseContext();
            var vehicle = await context.Vehicles.FirstOrDefaultAsync(x => x.Id == id);
            if (vehicle != null)
            {
                vehicle.Sold = true;
                context.Vehicles.Update(vehicle);
                await context.SaveChangesAsync();
                await player.GravarLog(LogType.Staff, $"Remover Veículo | {JsonSerializer.Serialize(vehicle)}", null);
            }

            player.EmitStaffShowMessage($"Veículo {id} excluído.");

            var html = await Functions.GetVehiclesHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Vehicles)))
                target.Emit("StaffVehicles", true, html);
        }
        #endregion Vehicles

        #region Infos
        [AsyncClientEvent(nameof(StaffInfoSave))]
        public static async Task StaffInfoSave(MyPlayer player, int days, string message)
        {
            var maxDays = player.User.VIP switch
            {
                UserVIP.Gold => int.MaxValue,
                UserVIP.Silver => 15,
                UserVIP.Bronze => 5,
                _ => 3,
            };

            if (days > maxDays)
            {
                player.EmitStaffShowMessage($"O máximo de dias permitido para seu nível de VIP é de {maxDays}.");
                return;
            }

            var info = new Info
            {
                PosX = player.Position.X,
                PosY = player.Position.Y,
                PosZ = player.Position.Z - 0.7f,
                Dimension = player.Dimension,
                ExpirationDate = DateTime.Now.AddDays(days),
                UserId = player.User.Id,
                Message = message,
            };

            await using var context = new DatabaseContext();
            await context.Infos.AddAsync(info);
            await context.SaveChangesAsync();

            info.User = player.User;

            Global.Infos.Add(info);
            info.CreateIdentifier();

            player.EmitStaffShowMessage($"Info {info.Id} criada.", true);
            player.Emit("StaffInfos", true, Functions.GetInfosHTML(player.User.Id));
        }

        [AsyncClientEvent(nameof(StaffInfoGoto))]
        public static void StaffInfoGoto(MyPlayer player, int id)
        {
            var info = Global.Infos.FirstOrDefault(x => x.Id == id);
            if (info == null)
                return;

            if (player.User.Staff < UserStaff.Moderator)
            {
                player.EmitShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.SetPosition(new Position(info.PosX, info.PosY, info.PosZ), info.Dimension, false);
            player.Emit("StaffInfos", true, Functions.GetInfosHTML(player.User.Id));
        }

        [AsyncClientEvent(nameof(StaffInfoRemove))]
        public static async Task StaffInfoRemove(MyPlayer player, int id)
        {
            var info = Global.Infos.FirstOrDefault(x => x.Id == id);
            if (info == null)
                return;

            if (player.User.Id != info.UserId
                && player.User.Staff < UserStaff.Moderator)
            {
                player.EmitShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            info.RemoveIdentifier();
            Global.Infos.Remove(info);
            context.Infos.Remove(info);
            await context.SaveChangesAsync();

            player.EmitStaffShowMessage($"Info {id} removida.");
            player.Emit("StaffInfos", true, Functions.GetInfosHTML(info.UserId == player.User.Id ? player.User.Id : null));
        }
        #endregion Infos

        #region Factions Drugs Houses
        [ClientEvent(nameof(StaffFactionDrugHouseGoto))]
        public static void StaffFactionDrugHouseGoto(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsDrugsHouses))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var factionDrugHouse = Global.FactionsDrugsHouses.FirstOrDefault(x => x.Id == id);
            if (factionDrugHouse == null)
                return;

            player.LimparIPLs();

            if (factionDrugHouse.Dimension > 0)
            {
                player.IPLs = Functions.GetIPLsByInterior(Global.Properties.FirstOrDefault(x => x.Id == factionDrugHouse.Dimension).Interior);
                player.SetarIPLs();
            }

            player.SetPosition(new Position(factionDrugHouse.PosX, factionDrugHouse.PosY, factionDrugHouse.PosZ), factionDrugHouse.Dimension, false);
        }

        [AsyncClientEvent(nameof(StaffFactionDrugHouseRemove))]
        public static async Task StaffFactionDrugHouseRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsDrugsHouses))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var factionDrugHouse = Global.FactionsDrugsHouses.FirstOrDefault(x => x.Id == id);
            if (factionDrugHouse == null)
                return;

            await using var context = new DatabaseContext();
            context.FactionsDrugsHouses.Remove(factionDrugHouse);
            context.FactionsDrugsHousesItems.RemoveRange(Global.FactionsDrugsHousesItems.Where(x => x.FactionDrugHouseId == id));
            await context.SaveChangesAsync();
            Global.FactionsDrugsHouses.Remove(factionDrugHouse);
            Global.FactionsDrugsHousesItems.RemoveAll(x => x.FactionDrugHouseId == factionDrugHouse.Id);
            factionDrugHouse.RemoveIdentifier();

            player.EmitStaffShowMessage($"Drug house {factionDrugHouse.Id} excluída.");

            var html = Functions.GetFactionsDrugsHousesHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsArmories)))
                target.Emit("StaffFactionsDrugsHouses", true, html);

            await player.GravarLog(LogType.Staff, $"Remover Drug House | {JsonSerializer.Serialize(factionDrugHouse)}", null);
        }

        [AsyncClientEvent(nameof(StaffFactionDrugHouseSave))]
        public static async Task StaffFactionDrugHouseSave(MyPlayer player, int id, int factionId, Vector3 pos, int dimension)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsDrugsHouses))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (factionId != 0 && !Global.Factions.Any(x => x.Id == factionId))
            {
                player.EmitStaffShowMessage($"Facção {factionId} não existe.");
                return;
            }

            var factionDrugHouse = new FactionDrugHouse();
            if (id > 0)
                factionDrugHouse = Global.FactionsDrugsHouses.FirstOrDefault(x => x.Id == id);

            factionDrugHouse.FactionId = factionId;
            factionDrugHouse.PosX = pos.X;
            factionDrugHouse.PosY = pos.Y;
            factionDrugHouse.PosZ = pos.Z;
            factionDrugHouse.Dimension = dimension;

            await using var context = new DatabaseContext();

            if (factionDrugHouse.Id == 0)
                await context.FactionsDrugsHouses.AddAsync(factionDrugHouse);
            else
                context.FactionsDrugsHouses.Update(factionDrugHouse);

            await context.SaveChangesAsync();

            if (id == 0)
                Global.FactionsDrugsHouses.Add(factionDrugHouse);

            factionDrugHouse.CreateIdentifier();

            player.EmitStaffShowMessage($"Drug house {(id == 0 ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Drug House | {JsonSerializer.Serialize(factionDrugHouse)}", null);

            var html = Functions.GetFactionsDrugsHousesHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsDrugsHouses)))
                target.Emit("StaffFactionsDrugsHouses", true, html);
        }

        [ClientEvent(nameof(StaffFactionsDrugsHousesItemsShow))]
        public static void StaffFactionsDrugsHousesItemsShow(MyPlayer player, int factionDrugHouseId)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsDrugsHouses))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("Server:CloseView");

            player.Emit("StaffFactionsDrugsHousesItems",
                false,
                Functions.GetFactionsDrugsHousesItemsHTML(factionDrugHouseId, true),
                factionDrugHouseId);
        }

        [AsyncClientEvent(nameof(StaffFactionDrugHouseItemSave))]
        public static async Task StaffFactionDrugHouseItemSave(MyPlayer player,
            int factionDrugHouseItemId,
            int factionDrugHouseId,
            string strItemCategory,
            int quantity)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsDrugsHouses))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!Enum.TryParse(strItemCategory, true, out ItemCategory itemCategory))
            {
                player.EmitStaffShowMessage($"Droga {strItemCategory} não existe.");
                return;
            }

            if (!Functions.CheckIfIsDrug(itemCategory))
            {
                player.EmitStaffShowMessage($"Droga {strItemCategory} não existe.");
                return;
            }

            if (quantity < 0)
            {
                player.EmitStaffShowMessage($"Estoque deve ser maior ou igual a 0.");
                return;
            }

            var factionDrugHouseItem = new FactionDrugHouseItem();
            if (factionDrugHouseItemId > 0)
                factionDrugHouseItem = Global.FactionsDrugsHousesItems.FirstOrDefault(x => x.Id == factionDrugHouseItemId);

            factionDrugHouseItem.FactionDrugHouseId = factionDrugHouseId;
            factionDrugHouseItem.ItemCategory = itemCategory;
            factionDrugHouseItem.Quantity = quantity;

            await using var context = new DatabaseContext();

            if (factionDrugHouseItem.Id == 0)
                await context.FactionsDrugsHousesItems.AddAsync(factionDrugHouseItem);
            else
                context.FactionsDrugsHousesItems.Update(factionDrugHouseItem);

            await context.SaveChangesAsync();

            if (factionDrugHouseItemId == 0)
                Global.FactionsDrugsHousesItems.Add(factionDrugHouseItem);

            player.EmitStaffShowMessage($"Item {(factionDrugHouseItemId == 0 ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Item Drug House | {JsonSerializer.Serialize(factionDrugHouseItem)}", null);

            var html = Functions.GetFactionsDrugsHousesItemsHTML(factionDrugHouseId, true);
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsDrugsHouses)))
                target.Emit("StaffFactionsDrugsHousesItems", true, html);
        }

        [AsyncClientEvent(nameof(StaffFactionDrugHouseItemRemove))]
        public static async Task StaffFactionDrugHouseItemRemove(MyPlayer player, int factionDrugHouseItemId)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsDrugsHouses))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var factionDrugHouseItem = Global.FactionsDrugsHousesItems.FirstOrDefault(x => x.Id == factionDrugHouseItemId);
            if (factionDrugHouseItem == null)
                return;

            await using var context = new DatabaseContext();
            context.FactionsDrugsHousesItems.Remove(factionDrugHouseItem);
            await context.SaveChangesAsync();
            Global.FactionsDrugsHousesItems.Remove(factionDrugHouseItem);

            player.EmitStaffShowMessage($"Item da Drug House {factionDrugHouseItem.Id} excluído.");

            await player.GravarLog(LogType.Staff, $"Remover Item Drug House | {JsonSerializer.Serialize(factionDrugHouseItem)}", null);

            var html = Functions.GetFactionsDrugsHousesItemsHTML(factionDrugHouseItem.FactionDrugHouseId, true);
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.FactionsDrugsHouses)))
                target.Emit("StaffFactionsDrugsHousesItems", true, html);
        }
        #endregion Factions Drugs Houses

        #region Crack Dens
        [ClientEvent(nameof(StaffCrackDenGoto))]
        public static void StaffCrackDenGoto(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var crackDen = Global.CrackDens.FirstOrDefault(x => x.Id == id);
            if (crackDen == null)
                return;

            player.LimparIPLs();

            if (crackDen.Dimension > 0)
            {
                player.IPLs = Functions.GetIPLsByInterior(Global.Properties.FirstOrDefault(x => x.Id == crackDen.Dimension).Interior);
                player.SetarIPLs();
            }

            player.SetPosition(new Position(crackDen.PosX, crackDen.PosY, crackDen.PosZ), crackDen.Dimension, false);
        }

        [AsyncClientEvent(nameof(StaffCrackDenRemove))]
        public static async Task StaffCrackDenRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var crackDen = Global.CrackDens.FirstOrDefault(x => x.Id == id);
            if (crackDen == null)
                return;

            await using var context = new DatabaseContext();
            context.CrackDens.Remove(crackDen);
            context.CrackDensItems.RemoveRange(Global.CrackDensItems.Where(x => x.CrackDenId == id));
            await context.SaveChangesAsync();
            await context.Database.ExecuteSqlRawAsync($"DELETE FROM {nameof(context.CrackDensSells)} WHERE {nameof(CrackDenSell.CrackDenId)} = {id}");
            Global.CrackDens.Remove(crackDen);
            Global.CrackDensItems.RemoveAll(x => x.CrackDenId == crackDen.Id);
            crackDen.RemoveIdentifier();

            player.EmitStaffShowMessage($"Boca de fumo {crackDen.Id} excluída.");

            var html = Functions.GetCrackDensHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.CrackDens)))
                target.Emit("StaffCrackDens", true, html);

            await player.GravarLog(LogType.Staff, $"Remover Boca de Fumo | {JsonSerializer.Serialize(crackDen)}", null);
        }

        [AsyncClientEvent(nameof(StaffCrackDenSave))]
        public static async Task StaffCrackDenSave(MyPlayer player, int id, Vector3 pos, int dimension,
            int onlinePoliceOfficers, int cooldownQuantityLimit, int cooldownHours)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (onlinePoliceOfficers < 0)
            {
                player.EmitStaffShowMessage($"Policiais Online deve ser maior ou igual a 0.");
                return;
            }

            if (cooldownQuantityLimit < 0)
            {
                player.EmitStaffShowMessage($"Quantidade Limite Cooldown deve ser maior ou igual a 0.");
                return;
            }

            if (cooldownHours < 0)
            {
                player.EmitStaffShowMessage($"Horas Cooldown deve ser maior ou igual a 0.");
                return;
            }

            var crackDen = new CrackDen();
            if (id > 0)
                crackDen = Global.CrackDens.FirstOrDefault(x => x.Id == id);

            crackDen.PosX = pos.X;
            crackDen.PosY = pos.Y;
            crackDen.PosZ = pos.Z;
            crackDen.Dimension = dimension;
            crackDen.OnlinePoliceOfficers = onlinePoliceOfficers;
            crackDen.CooldownQuantityLimit = cooldownQuantityLimit;
            crackDen.CooldownHours = cooldownHours;

            await using var context = new DatabaseContext();

            if (crackDen.Id == 0)
                await context.CrackDens.AddAsync(crackDen);
            else
                context.CrackDens.Update(crackDen);

            await context.SaveChangesAsync();

            if (id == 0)
                Global.CrackDens.Add(crackDen);

            crackDen.CreateIdentifier();

            player.EmitStaffShowMessage($"Boca de fumo {(id == 0 ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Boca de Fumo | {JsonSerializer.Serialize(crackDen)}", null);

            var html = Functions.GetCrackDensHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.CrackDens)))
                target.Emit("StaffCrackDens", true, html);
        }

        [ClientEvent(nameof(StaffCrackDensItemsShow))]
        public static void StaffCrackDensItemsShow(MyPlayer player, int crackDenId)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("Server:CloseView");

            player.Emit("StaffCrackDensItems",
                false,
                Functions.GetCrackDensItemsHTML(crackDenId, true),
                crackDenId);
        }

        [AsyncClientEvent(nameof(StaffCrackDenItemSave))]
        public static async Task StaffCrackDenItemSave(MyPlayer player,
            int crackDenItemId,
            int crackDenId,
            string strItemCategory,
            int value)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!Enum.TryParse(strItemCategory, true, out ItemCategory itemCategory))
            {
                player.EmitStaffShowMessage($"Droga {strItemCategory} não existe.");
                return;
            }

            if (!Functions.CheckIfIsDrug(itemCategory))
            {
                player.EmitStaffShowMessage($"Droga {strItemCategory} não existe.");
                return;
            }

            if (value <= 0)
            {
                player.EmitStaffShowMessage($"Valor deve ser maior que 0.");
                return;
            }

            var crackDenItem = new CrackDenItem();
            if (crackDenItemId > 0)
                crackDenItem = Global.CrackDensItems.FirstOrDefault(x => x.Id == crackDenItemId);

            crackDenItem.CrackDenId = crackDenId;
            crackDenItem.ItemCategory = itemCategory;
            crackDenItem.Value = value;

            await using var context = new DatabaseContext();

            if (crackDenItem.Id == 0)
                await context.CrackDensItems.AddAsync(crackDenItem);
            else
                context.CrackDensItems.Update(crackDenItem);

            await context.SaveChangesAsync();

            if (crackDenItemId == 0)
                Global.CrackDensItems.Add(crackDenItem);

            player.EmitStaffShowMessage($"Item {(crackDenItemId == 0 ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Item Boca de Fumo | {JsonSerializer.Serialize(crackDenItem)}", null);

            var html = Functions.GetCrackDensItemsHTML(crackDenId, true);
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.CrackDens)))
                target.Emit("StaffCrackDensItems", true, html);
        }

        [AsyncClientEvent(nameof(StaffCrackDenItemRemove))]
        public static async Task StaffCrackDenItemRemove(MyPlayer player, int crackDenItemId)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var crackDenItem = Global.CrackDensItems.FirstOrDefault(x => x.Id == crackDenItemId);
            if (crackDenItem == null)
                return;

            await using var context = new DatabaseContext();
            context.CrackDensItems.Remove(crackDenItem);
            await context.SaveChangesAsync();
            Global.CrackDensItems.Remove(crackDenItem);

            player.EmitStaffShowMessage($"Item da Boca de Fumo {crackDenItem.Id} excluído.");

            await player.GravarLog(LogType.Staff, $"Remover Item Boca de Fumo | {JsonSerializer.Serialize(crackDenItem)}", null);

            var html = Functions.GetCrackDensItemsHTML(crackDenItem.CrackDenId, true);
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.CrackDens)))
                target.Emit("StaffCrackDensItems", true, html);
        }

        [AsyncClientEvent(nameof(StaffCrackDenRevokeCooldown))]
        public static async Task StaffCrackDenRevokeCooldown(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var crackDen = Global.CrackDens.FirstOrDefault(x => x.Id == id);
            if (crackDen == null)
                return;

            crackDen.CooldownDate = DateTime.Now;

            await using var context = new DatabaseContext();
            context.CrackDens.Update(crackDen);
            await context.SaveChangesAsync();

            player.EmitStaffShowMessage($"Cooldown da boca de fumo revogado.", true);

            await player.GravarLog(LogType.Staff, $"Revogar Cool Down Boca de Fumo | {id}", null);

            var html = Functions.GetCrackDensHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.CrackDens)))
                target.Emit("StaffCrackDens", true, html);
        }
        #endregion Crack Dens

        #region Trucker Locations
        [ClientEvent(nameof(StaffTruckerLocationGoto))]
        public static void StaffTruckerLocationGoto(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var truckerLocation = Global.TruckerLocations.FirstOrDefault(x => x.Id == id);
            if (truckerLocation == null)
                return;

            player.LimparIPLs();

            player.SetPosition(new Position(truckerLocation.PosX, truckerLocation.PosY, truckerLocation.PosZ), 0, false);
        }

        [AsyncClientEvent(nameof(StaffTruckerLocationRemove))]
        public static async Task StaffTruckerLocationRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var truckerLocation = Global.TruckerLocations.FirstOrDefault(x => x.Id == id);
            if (truckerLocation == null)
                return;

            await using var context = new DatabaseContext();
            context.TruckerLocations.Remove(truckerLocation);
            context.TruckerLocationsDeliveries.RemoveRange(Global.TruckerLocationsDeliveries.Where(x => x.TruckerLocationId == id));
            await context.SaveChangesAsync();
            Global.TruckerLocations.Remove(truckerLocation);
            Global.TruckerLocationsDeliveries.RemoveAll(x => x.TruckerLocationId == truckerLocation.Id);
            truckerLocation.RemoveIdentifier();

            player.EmitStaffShowMessage($"Localização de caminhoneiro {truckerLocation.Id} excluída.");

            var html = Functions.GetTruckerLocationsHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.TruckerLocations)))
                target.Emit("StaffTruckerLocations", true, html);

            await player.GravarLog(LogType.Staff, $"Remover Localização de Caminhoneiro | {JsonSerializer.Serialize(truckerLocation)}", null);
        }

        [AsyncClientEvent(nameof(StaffTruckerLocationSave))]
        public static async Task StaffTruckerLocationSave(MyPlayer player, int id, string name, Vector3 pos,
            int deliveryValue, int loadWaitTime, int unloadWaitTime, string allowedVehiclesJSON)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (deliveryValue <= 0)
            {
                player.EmitStaffShowMessage($"Valor por Entrega deve ser maior que 0.");
                return;
            }

            if (loadWaitTime <= 0)
            {
                player.EmitStaffShowMessage($"Valor por Entrega deve ser maior que 0.");
                return;
            }

            if (unloadWaitTime <= 0)
            {
                player.EmitStaffShowMessage($"Tempo de Espera por Entrega deve ser maior que 0.");
                return;
            }

            var allowedVehicles = JsonSerializer.Deserialize<List<string>>(allowedVehiclesJSON.ToUpper());
            if (allowedVehicles.Count == 0)
            {
                player.EmitStaffShowMessage($"Veículos Permitidos devem ser informados.");
                return;
            }

            foreach (var allowedVehicle in allowedVehicles)
            {
                if (!Enum.TryParse(allowedVehicle, true, out VehicleModel v1) && !Enum.TryParse(allowedVehicle, true, out VehicleModelMods v2))
                {
                    player.EmitStaffShowMessage($"Veículo {allowedVehicle} não existe.");
                    return;
                }
            }

            var truckerLocation = new TruckerLocation();
            if (id > 0)
                truckerLocation = Global.TruckerLocations.FirstOrDefault(x => x.Id == id);

            truckerLocation.Name = name;
            truckerLocation.PosX = pos.X;
            truckerLocation.PosY = pos.Y;
            truckerLocation.PosZ = pos.Z;
            truckerLocation.DeliveryValue = deliveryValue;
            truckerLocation.LoadWaitTime = loadWaitTime;
            truckerLocation.UnloadWaitTime = unloadWaitTime;
            truckerLocation.AllowedVehiclesJSON = JsonSerializer.Serialize(allowedVehicles);

            await using var context = new DatabaseContext();

            if (truckerLocation.Id == 0)
                await context.TruckerLocations.AddAsync(truckerLocation);
            else
                context.TruckerLocations.Update(truckerLocation);

            await context.SaveChangesAsync();

            if (id == 0)
                Global.TruckerLocations.Add(truckerLocation);

            truckerLocation.CreateIdentifier();

            player.EmitStaffShowMessage($"Localização de caminhoneiro {(id == 0 ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Localização de Caminhoneiro | {JsonSerializer.Serialize(truckerLocation)}", null);

            var html = Functions.GetTruckerLocationsHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.TruckerLocations)))
                target.Emit("StaffTruckerLocations", true, html);
        }

        [ClientEvent(nameof(StaffTruckerLocationsDeliveriesShow))]
        public static void StaffTruckerLocationsDeliveriesShow(MyPlayer player, int truckerLocationId)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("Server:CloseView");

            player.Emit("StaffTruckerLocationsDeliveries",
                false,
                Functions.GetTruckerLocationsDeliverysHTML(truckerLocationId),
                truckerLocationId);
        }

        [ClientEvent(nameof(StaffTruckerLocationDeliveryGoto))]
        public static void StaffTruckerLocationDeliveryGoto(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var truckerLocationDelivery = Global.TruckerLocationsDeliveries.FirstOrDefault(x => x.Id == id);
            if (truckerLocationDelivery == null)
                return;

            player.LimparIPLs();
            player.SetPosition(new Position(truckerLocationDelivery.PosX, truckerLocationDelivery.PosY, truckerLocationDelivery.PosZ), 0, false);
        }


        [AsyncClientEvent(nameof(StaffTruckerLocationDeliverySave))]
        public static async Task StaffTruckerLocationDeliverySave(MyPlayer player,
            int truckerLocationDeliveryId,
            int truckerLocationId,
            Vector3 pos)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var truckerLocationDelivery = new TruckerLocationDelivery();
            if (truckerLocationDeliveryId > 0)
                truckerLocationDelivery = Global.TruckerLocationsDeliveries.FirstOrDefault(x => x.Id == truckerLocationDeliveryId);

            truckerLocationDelivery.TruckerLocationId = truckerLocationId;
            truckerLocationDelivery.PosX = pos.X;
            truckerLocationDelivery.PosY = pos.Y;
            truckerLocationDelivery.PosZ = pos.Z;

            await using var context = new DatabaseContext();

            if (truckerLocationDelivery.Id == 0)
                await context.TruckerLocationsDeliveries.AddAsync(truckerLocationDelivery);
            else
                context.TruckerLocationsDeliveries.Update(truckerLocationDelivery);

            await context.SaveChangesAsync();

            if (truckerLocationDeliveryId == 0)
                Global.TruckerLocationsDeliveries.Add(truckerLocationDelivery);

            player.EmitStaffShowMessage($"Entrega {(truckerLocationDeliveryId == 0 ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Entrega Localização de Caminhoneiro | {JsonSerializer.Serialize(truckerLocationDelivery)}", null);

            var html = Functions.GetTruckerLocationsDeliverysHTML(truckerLocationId);
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.TruckerLocations)))
                target.Emit("StaffTruckerLocationsDeliveries", true, html);
        }

        [AsyncClientEvent(nameof(StaffTruckerLocationDeliveryRemove))]
        public static async Task StaffTruckerLocationDeliveryRemove(MyPlayer player, int truckerLocationDeliveryId)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var truckerLocationDelivery = Global.TruckerLocationsDeliveries.FirstOrDefault(x => x.Id == truckerLocationDeliveryId);
            if (truckerLocationDelivery == null)
                return;

            await using var context = new DatabaseContext();
            context.TruckerLocationsDeliveries.Remove(truckerLocationDelivery);
            await context.SaveChangesAsync();
            Global.TruckerLocationsDeliveries.Remove(truckerLocationDelivery);

            player.EmitStaffShowMessage($"Entrega da Localização de Caminhoneiro {truckerLocationDelivery.Id} excluída.");

            await player.GravarLog(LogType.Staff, $"Remover Entrega da Localização de Caminhoneiro | {JsonSerializer.Serialize(truckerLocationDelivery)}", null);

            var html = Functions.GetTruckerLocationsDeliverysHTML(truckerLocationDelivery.TruckerLocationId);
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.TruckerLocations)))
                target.Emit("StaffTruckerLocationsDeliveries", true, html);
        }
        #endregion Trucker Locations

        #region Furnitures
        [AsyncClientEvent(nameof(StaffFurnitureSave))]
        public static async Task StaffFurnitureSave(MyPlayer player, int id, string category, string name, string model, int value)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Furnitures))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (category.ToLower() != "barreiras" && value <= 0)
            {
                player.EmitStaffShowMessage("Valor inválido.");
                return;
            }

            var furniture = new Furniture();
            if (id > 0)
                furniture = Global.Furnitures.FirstOrDefault(x => x.Id == id);

            furniture.Category = category;
            furniture.Name = name;
            furniture.Model = model;
            furniture.Value = value;

            await using var context = new DatabaseContext();

            if (furniture.Id == 0)
                await context.Furnitures.AddAsync(furniture);
            else
                context.Furnitures.Update(furniture);

            await context.SaveChangesAsync();

            if (id == 0)
                Global.Furnitures.Add(furniture);

            player.EmitStaffShowMessage($"Mobília {(id == 0 ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Mobília | {JsonSerializer.Serialize(furniture)}", null);

            var html = Functions.GetFurnituresHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Furnitures)))
                target.Emit("StaffFurnitures", true, html);
        }

        [AsyncClientEvent(nameof(StaffFurnitureRemove))]
        public static async Task StaffFurnitureRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Furnitures))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var furniture = Global.Furnitures.FirstOrDefault(x => x.Id == id);
            if (furniture != null)
            {
                await using var context = new DatabaseContext();
                context.Furnitures.Remove(furniture);
                await context.SaveChangesAsync();
                Global.Furnitures.Remove(furniture);
                await player.GravarLog(LogType.Staff, $"Remover Mobília | {JsonSerializer.Serialize(furniture)}", null);
            }

            player.EmitStaffShowMessage($"Mobília {id} excluída.");

            var html = Functions.GetFurnituresHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Furnitures)))
                target.Emit("StaffFurnitures", true, html);
        }
        #endregion Furnitures

        #region Animations
        [AsyncClientEvent(nameof(StaffAnimationSave))]
        public static async Task StaffAnimationSave(MyPlayer player, int id, string display, string dictionary, string name,
            int flag, int duration, bool vehicle)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Animations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var animation = new Animation();
            if (id > 0)
                animation = Global.Animations.FirstOrDefault(x => x.Id == id);

            animation.Display = display;
            animation.Dictionary = dictionary;
            animation.Name = name;
            animation.Flag = flag;
            animation.Duration = duration;
            animation.Vehicle = vehicle;

            await using var context = new DatabaseContext();

            if (animation.Id == 0)
                await context.Animations.AddAsync(animation);
            else
                context.Animations.Update(animation);

            await context.SaveChangesAsync();

            if (id == 0)
                Global.Animations.Add(animation);

            player.EmitStaffShowMessage($"Animação {(id == 0 ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Animação | {JsonSerializer.Serialize(animation)}", null);

            var html = Functions.GetAnimationsHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Animations)))
                target.Emit("StaffAnimations", true, html);
        }

        [AsyncClientEvent(nameof(StaffAnimationRemove))]
        public static async Task StaffAnimationRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Animations))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var animation = Global.Animations.FirstOrDefault(x => x.Id == id);
            if (animation != null)
            {
                await using var context = new DatabaseContext();
                context.Animations.Remove(animation);
                await context.SaveChangesAsync();
                Global.Animations.Remove(animation);
                await player.GravarLog(LogType.Staff, $"Remover Animação | {JsonSerializer.Serialize(animation)}", null);
            }

            player.EmitStaffShowMessage($"Animação {id} excluída.");

            var html = Functions.GetAnimationsHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Animations)))
                target.Emit("StaffAnimations", true, html);
        }
        #endregion Animations

        #region Companies
        [ClientEvent(nameof(StaffCompanyGoto))]
        public static void StaffCompanyGoto(MyPlayer player, int id)
        {
            var company = Global.Companies.FirstOrDefault(x => x.Id == id);
            if (company == null)
                return;

            player.LimparIPLs();
            player.SetPosition(new Position(company.PosX, company.PosY, company.PosZ), 0, false);
        }

        [AsyncClientEvent(nameof(StaffCompanySave))]
        public static async Task StaffCompanySave(MyPlayer player, int id, string name, Vector3 pos, int weekRentValue)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Companies))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (weekRentValue < 0)
            {
                player.EmitStaffShowMessage("Aluguel Semanal deve ser igual ou maior que 0.");
                return;
            }

            var company = new Company();
            if (id > 0)
                company = Global.Companies.FirstOrDefault(x => x.Id == id);

            company.Name = name;
            company.PosX = pos.X;
            company.PosY = pos.Y;
            company.PosZ = pos.Z;
            company.WeekRentValue = weekRentValue;

            await using var context = new DatabaseContext();

            if (company.Id == 0)
                await context.Companies.AddAsync(company);
            else
                context.Companies.Update(company);

            await context.SaveChangesAsync();

            company.CreateIdentifier();

            if (id == 0)
            {
                company.Characters = new List<CompanyCharacter>();
                Global.Companies.Add(company);
            }

            player.EmitStaffShowMessage($"Empresa {(id == 0 ? "criada" : "editada")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Empresa | {JsonSerializer.Serialize(company)}", null);

            var html = Functions.GetCompaniesHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Companies)))
                target.Emit("StaffCompanies", true, html);
        }

        [AsyncClientEvent(nameof(StaffCompanyRemove))]
        public static async Task StaffCompanyRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Companies))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var company = Global.Companies.FirstOrDefault(x => x.Id == id);
            if (company != null)
            {
                await using var context = new DatabaseContext();
                context.Companies.Remove(company);
                await context.SaveChangesAsync();
                Global.Companies.Remove(company);
                company.RemoveIdentifier();
                await player.GravarLog(LogType.Staff, $"Remover Empresa | {JsonSerializer.Serialize(company)}", null);
            }

            player.EmitStaffShowMessage($"Empresa {id} excluída.");

            var html = Functions.GetCompaniesHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Companies)))
                target.Emit("StaffCompanies", true, html);
        }

        [AsyncClientEvent(nameof(StaffCompanyRemoveOwner))]
        public static async Task StaffCompanyRemoveOwner(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Companies))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var company = Global.Companies.FirstOrDefault(x => x.Id == id);
            if (company != null)
            {
                await company.RemoveOwner();
                await player.GravarLog(LogType.Staff, $"Remover Dono Empresa {id}", null);
            }

            player.EmitStaffShowMessage($"Dono da empresa {id} removido.");

            var html = Functions.GetCompaniesHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Companies)))
                target.Emit("StaffCompanies", true, html);
        }
        #endregion Companies
    }
}