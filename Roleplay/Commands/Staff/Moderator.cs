using AltV.Net.Data;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Roleplay.Commands.Staff
{
    public class Moderator
    {
        [Command("ir", "/ir (ID ou nome)")]
        public static void CMD_ir(MyPlayer player, string idNome)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var target = player.ObterPersonagemPorIdNome(idNome, false);
            if (target == null)
                return;

            player.LimparIPLs();
            player.IPLs = target.IPLs;
            player.SetarIPLs();
            var pos = target.Position;
            pos.X += Global.RP_DISTANCE;
            player.SetPosition(pos, target.Dimension, false);
        }

        [Command("trazer", "/trazer (ID ou nome)")]
        public static void CMD_trazer(MyPlayer player, string idNome)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var target = player.ObterPersonagemPorIdNome(idNome, false);
            if (target == null)
                return;

            target.LimparIPLs();
            target.IPLs = player.IPLs;
            target.SetarIPLs();
            var pos = player.Position;
            pos.X += Global.RP_DISTANCE;
            target.SetPosition(pos, player.Dimension, false);
        }

        [Command("tp", "/tp (ID ou nome) (ID ou nome)")]
        public static void CMD_tp(MyPlayer player, string idNome, string idNomeDestino)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var target = player.ObterPersonagemPorIdNome(idNome, false);
            if (target == null)
                return;

            var targetDest = player.ObterPersonagemPorIdNome(idNomeDestino, false);
            if (targetDest == null)
                return;

            target.LimparIPLs();
            target.IPLs = targetDest.IPLs;
            target.SetarIPLs();
            var pos = targetDest.Position;
            pos.X += Global.RP_DISTANCE;
            target.SetPosition(pos, targetDest.Dimension, false);

            target.SendMessage(MessageType.Success, $"{player.User.Name} teleportou você para {targetDest.Character.Name}.");
            player.SendMessage(MessageType.Success, $"Você teleportou {target.Character.Name} para {targetDest.Character.Name}.");
        }

        [Command("a", "/a (mensagem)", GreedyArg = true)]
        public static async Task CMD_a(MyPlayer player, string mensagem)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (player.User.StaffChatToggle)
            {
                player.SendMessage(MessageType.Error, "Você está com o chat da staff desabilitado.");
                return;
            }

            foreach (var x in Global.Players.Where(x => x.User.Staff != UserStaff.None && !x.User.StaffChatToggle))
                x.SendMessage(MessageType.None, $"(( {Functions.GetEnumDisplay(player.User.Staff)} {player.User.Name} [{player.SessionId}]: {mensagem} ))", "#33EE33");

            await player.GravarLog(LogType.StaffChat, mensagem, null);
        }

        [Command("kick", "/kick (ID ou nome) (motivo)", GreedyArg = true)]
        public static async Task CMD_kick(MyPlayer player, string idNome, string motivo)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var target = player.ObterPersonagemPorIdNome(idNome, false);
            if (target == null)
                return;

            if (target.User.Staff >= player.User.Staff)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            await context.Punishments.AddAsync(new Punishment()
            {
                Date = DateTime.Now,
                Reason = motivo,
                CharacterId = target.Character.Id,
                Type = PunishmentType.Kick,
                StaffUserId = player.User.Id,
            });
            await context.SaveChangesAsync();

            await target.Save();
            await Functions.SendStaffMessage($"{player.User.Name} kickou {target.Character.Name}. Motivo: {motivo}", false);
            target.Kick($"{player.User.Name} kickou você. Motivo: {motivo}");
        }

        [Command("irveh", "/irveh (código)")]
        public static void CMD_irveh(MyPlayer player, int codigo)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var veh = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == codigo);
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, "Veículo não está spawnado.");
                return;
            }

            player.LimparIPLs();
            var pos = veh.Position;
            pos.X += Global.RP_DISTANCE;
            player.SetPosition(pos, veh.Dimension, false);
        }

        [Command("trazerveh", "/trazerveh (código)")]
        public static void CMD_trazerveh(MyPlayer player, int codigo)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (player.Dimension > 0)
            {
                player.SendMessage(MessageType.Error, "Não é possível usar esse comando em um interior.");
                return;
            }

            var veh = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == codigo);
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, "Veículo não está spawnado.");
                return;
            }

            var pos = player.Position;
            pos.X += Global.RP_DISTANCE;
            veh.Position = pos;
            veh.Dimension = player.Dimension;
        }

        [Command("aduty", Aliases = new string[] { "atrabalho" })]
        public async static void CMD_aduty(MyPlayer player)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            if (player.OnAdminDuty)
            {
                player.AdminDutySession.FinalDate = DateTime.Now;
                context.Sessions.Update(player.AdminDutySession);
                player.AdminDutySession = new();
            }
            else
            {
                player.AdminDutySession = new Session
                {
                    CharacterId = player.Character.Id,
                    Type = SessionType.StaffDuty,
                };
                await context.Sessions.AddAsync(player.AdminDutySession);
            }
            await context.SaveChangesAsync();

            player.OnAdminDuty = !player.OnAdminDuty;
            player.Invincible = player.OnAdminDuty;
            player.SetNametag();
            await Functions.SendStaffMessage($"{player.User.Name} {(player.OnAdminDuty ? "entrou em" : "saiu de")} serviço administrativo.", true);
        }

        [Command("at", "/at (código)")]
        public static async Task CMD_at(MyPlayer player, int codigo)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var sos = Global.HelpRequests.FirstOrDefault(x => x.CharacterSessionId == codigo);
            if (sos == null)
            {
                player.SendMessage(MessageType.Error, $"SOS {codigo} não existe.");
                return;
            }

            var target = await sos.Check();
            if (target == null)
            {
                player.SendMessage(MessageType.Error, $"Jogador do SOS não está conectado.");
                return;
            }

            sos.AnswerDate = DateTime.Now;
            sos.StaffUserId = player.User.Id;

            await using var context = new DatabaseContext();
            context.HelpRequests.Update(sos);
            await context.SaveChangesAsync();
            Global.HelpRequests.Remove(sos);

            player.User.HelpRequestsAnswersQuantity++;

            await Functions.SendStaffMessage($"{player.User.Name} atendeu o pedido de ajuda de {sos.CharacterName} [{sos.CharacterSessionId}] ({sos.UserName}).", true);
            target.SendMessage(MessageType.Success, $"{player.User.Name} atendeu o seu pedido de ajuda.");

            var html = Functions.GetSOSJSON();
            foreach (var targetStaff in Global.Players.Where(x => x.User.Staff != UserStaff.None))
                targetStaff.Emit("ACPUpdateSOS", html);
        }

        [Command("spec", "/spec (ID ou nome)")]
        public static async Task CMD_spec(MyPlayer player, string idNome)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (player.Cuffed || player.Character.Wound != CharacterWound.Nenhum)
            {
                player.SendMessage(MessageType.Error, "Você não pode usar esse comando algemado ou ferido.");
                return;
            }

            var target = player.ObterPersonagemPorIdNome(idNome, false);
            if (target == null)
                return;

            if (target.SPECPosition.HasValue)
            {
                player.SendMessage(MessageType.Error, "Jogador está observando outro jogador.");
                return;
            }

            if (!player.SPECPosition.HasValue)
            {
                player.SPECPosition = player.Position;
                player.SPECDimension = player.Dimension;
                player.SPECIPLs = player.IPLs;

                foreach (var x in Global.Players.Where(x => x.SPECId == player.SessionId))
                    await x.Unspectate();
            }

            player.CurrentWeapon = (uint)WeaponModel.Fist;
            player.LimparIPLs();
            player.IPLs = target.IPLs;
            player.SetarIPLs();
            player.SPECId = target.SessionId;
            player.Collision = false;
            player.Invincible = true;
            player.Visible = false;
            player.SetNametag();
            player.SetPosition(new Position(target.Position.X, target.Position.Y, target.Position.Z), target.Dimension, true);
            player.Emit("SpectatePlayer", target);
            await player.GravarLog(LogType.Staff, "/spec", target);
        }

        [Command("specoff")]
        public static async Task CMD_specoff(MyPlayer player)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!player.SPECPosition.HasValue)
            {
                player.SendMessage(MessageType.Error, "Você não está observando um jogador.");
                return;
            }

            await player.Unspectate();
        }

        [Command("aferimentos", "/aferimentos (ID ou nome)")]
        public static void CMD_aferimentos(MyPlayer player, string idNome)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var target = player.ObterPersonagemPorIdNome(idNome);
            if (target == null)
                return;

            if (target.Ferimentos.Count == 0)
            {
                player.SendMessage(MessageType.Error, "Jogador não possui ferimentos.");
                return;
            }

            var html = $@"<div class='table-responsive' style='max-height:50vh;overflow-y:auto;overflow-x:auto;'>
            <table class='table table-bordered table-striped'>
                <thead>
                    <tr>
                        <th>Data</th>
                        <th>Arma</th>
                        <th>Dano</th>
                        <th>Parte do Corpo</th>
                        <th>Autor</th>
                        <th>Distância</th>
                    </tr>
                </thead>
                <tbody>";

            foreach (var x in target.Ferimentos)
                html += $@"<tr><td>{x.Data}</td><td>{(WeaponModel)x.Arma}</td><td>{x.Dano}</td><td>{Functions.GetBodyPartName(x.BodyPart)}</td><td>{x.Attacker}</td><td>{x.Distancia}</td></tr>";

            html += $@"
                </tbody>
            </table>
            </div>";

            player.Emit("Server:BaseHTML", Functions.GetBaseHTML($"Ferimentos de {target.Character.Name}", html));
        }

        [Command("aestacionar", "/aestacionar (veículo)")]
        public static async Task CMD_aestacionar(MyPlayer player, int veiculo)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var veh = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == veiculo);
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, "Veículo não está spawnado.");
                return;
            }

            await veh.Estacionar(player);
            await Functions.SendStaffMessage($"{player.User.Name} estacionou o veículo {veiculo}.", true);
        }

        [Command("acurar", "/acurar (ID ou nome)")]
        public static async Task CMD_acurar(MyPlayer player, string idNome)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var target = player.ObterPersonagemPorIdNome(idNome);
            if (target == null)
                return;

            if (!target.Ferido)
            {
                player.SendMessage(MessageType.Error, "Jogador não está ferido.");
                return;
            }

            target.Curar();
            player.SendMessage(MessageType.Success, $"Você curou {target.Character.Name}.");
            target.SendMessage(MessageType.Success, $"{player.User.Name} curou você.");

            await player.GravarLog(LogType.Staff, "/acurar", target);
        }

        [Command("adanos", "/adanos (veículo)")]
        public static void CMD_adanos(MyPlayer player, int veiculo)
        {
            var veh = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == veiculo);
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, $"Nenhum veículo encontrado com o código {veiculo}.");
                return;
            }

            if (veh.Damages.Count == 0)
            {
                player.SendMessage(MessageType.Error, "Veículo não possui danos.");
                return;
            }

            var html = $@"<div class='table-responsive' style='max-height:50vh;overflow-y:auto;overflow-x:auto;'>
                <table class='table table-bordered table-striped'>
                <thead>
                    <tr>
                        <th>Data</th>
                        <th>Arma</th>
                        <th>Dano na Lataria</th>
                        <th>Dano Adicional na Lataria</th>
                        <th>Dano no Motor</th>
                        <th>Dano no Tanque de Combustível</th>
                        <th>Autor</th>
                        <th>Distância</th>
                    </tr>
                </thead>
                <tbody>";

            foreach (var x in veh.Damages)
                html += $@"<tr><td>{x.Data}</td><td>{(WeaponModel)x.WeaponHash}</td><td>{x.BodyHealthDamage}</td><td>{x.AdditionalBodyHealthDamage}</td><td>{x.EngineHealthDamage}</td><td>{x.PetrolTankDamage}</td><td>{x.Attacker}</td><td>{x.Distance}</td></tr>";

            html += $@"
                </tbody>
            </table>
            </div>";

            player.Emit("Server:BaseHTML", Functions.GetBaseHTML($"Danos do veículo {veh.VehicleDB.Model.ToUpper()} [{veh.VehicleDB.Id}] ({veh.VehicleDB.Plate})", html));
        }

        [Command("checarveh", "/checarveh (código)")]
        public static async Task CMD_checarveh(MyPlayer player, int codigo)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var veh = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == codigo);
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, $"Veículo {codigo} não está spawnado.");
                return;
            }

            var dono = string.Empty;
            if (veh.VehicleDB.FactionId.HasValue)
            {
                dono = $"{Global.Factions.FirstOrDefault(x => x.Id == veh.VehicleDB.FactionId).Name} [{veh.VehicleDB.FactionId}]";
            }
            else if (veh.VehicleDB.CharacterId.HasValue)
            {
                await using var context = new DatabaseContext();
                dono = $"{(await context.Characters.FirstOrDefaultAsync(x => x.Id == veh.VehicleDB.CharacterId)).Name} [{veh.VehicleDB.CharacterId}]";
            }
            else if (veh.VehicleDB.Job != CharacterJob.None)
            {
                dono = $"{Functions.GetEnumDisplay(veh.VehicleDB.Job)} ({veh.NomeEncarregado}){{#FFFFFF}} | Término Aluguel: {{{Global.MAIN_COLOR}}}{veh.DataExpiracaoAluguel}";
            }

            player.SendMessage(MessageType.None, $"Veículo: {{{Global.MAIN_COLOR}}}{codigo}{{#FFFFFF}} | Modelo: {{{Global.MAIN_COLOR}}}{veh.VehicleDB.Model}{{#FFFFFF}} | Proprietário: {{{Global.MAIN_COLOR}}}{dono}");
        }

        [Command("acp")]
        public static async Task CMD_acp(MyPlayer player)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var jsonLogs = JsonSerializer.Serialize(
                Enum.GetValues(typeof(LogType))
                .Cast<LogType>()
                .Select(x => new
                {
                    Id = x,
                    Name = Functions.GetEnumDisplay(x),
                })
            );

            using var context = new DatabaseContext();
            var staffersJson = JsonSerializer.Serialize(
                (await context.Users.Where(x => x.Staff != UserStaff.None)
                    .Include(x => x.Characters)
                    .OrderByDescending(x => x.Staff)
                    .ToListAsync())
                    .Select(x => new
                    {
                        Staff = Functions.GetEnumDisplay(x.Staff),
                        x.Id,
                        x.Name,
                        x.HelpRequestsAnswersQuantity,
                        x.CharacterApplicationsQuantity,
                        x.StaffDutyTime,
                        ConnectedTime = x.Characters.Sum(x => x.ConnectedTime)
                    })
            );

            player.Emit("ACPShow",
                player.User.StaffFlagsJSON,
                await Functions.GetBansJSON(),
                Functions.GetSOSJSON(),
                jsonLogs,
                staffersJson);
        }

        [Command("checar", "/checar (ID ou nome)")]
        public static async Task CMD_checar(MyPlayer player, string idNome)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var target = player.ObterPersonagemPorIdNome(idNome);
            if (target == null)
                return;

            var htmlStats = $"<div style='max-height:80vh;overflow-y:auto;overflow-x:hidden;'>{await target.ObterHTMLStats()}</div>";
            player.Emit("Server:BaseHTML", Functions.GetBaseHTML($"{target.Character.Name} [{target.Character.Id}] ({DateTime.Now})", htmlStats));
        }

        [Command("proximo", Aliases = new string[] { "prox" })]
        public static void CMD_proximo(MyPlayer player)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var isTemAlgoProximo = false;
            var distanceVer = 5f;

            foreach (var x in Global.Blips)
            {
                if (player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= distanceVer)
                {
                    player.SendMessage(MessageType.None, $"Blip {x.Id}");
                    isTemAlgoProximo = true;
                }
            }

            foreach (var x in Global.Spots)
            {
                if (player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= distanceVer)
                {
                    player.SendMessage(MessageType.None, $"Ponto {x.Id} | Tipo: {Functions.GetEnumDisplay(x.Type)} ({(byte)x.Type})");
                    isTemAlgoProximo = true;
                }
            }

            foreach (var x in Global.FactionsArmories)
            {
                if (player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= distanceVer)
                {
                    player.SendMessage(MessageType.None, $"Arsenal {x.Id}");
                    isTemAlgoProximo = true;
                }
            }

            foreach (var x in Global.Vehicles)
            {
                if (player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)) <= distanceVer)
                {
                    player.SendMessage(MessageType.None, $"Veículo {x.VehicleDB.Id} | Modelo: {x.VehicleDB.Model.ToUpper()}");
                    isTemAlgoProximo = true;
                }
            }

            foreach (var x in Global.Doors)
            {
                if (player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= distanceVer)
                {
                    player.SendMessage(MessageType.None, $"Porta {x.Id}");
                    isTemAlgoProximo = true;
                }
            }

            foreach (var x in Global.Infos)
            {
                if (x.Dimension == player.Dimension
                    && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= distanceVer)
                {
                    player.SendMessage(MessageType.None, $"Info {x.Id} | Data: {x.Date} | Expiração: {x.ExpirationDate} | Usuário: {x.User.Name} [{x.UserId}]");
                    isTemAlgoProximo = true;
                }
            }

            foreach (var x in Global.Companies)
            {
                if (player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= distanceVer)
                {
                    player.SendMessage(MessageType.None, $"Empresa {x.Id}");
                    isTemAlgoProximo = true;
                }
            }

            if (!isTemAlgoProximo)
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum item.");
        }

        [Command("ainfos")]
        public static void CMD_ainfos(MyPlayer player)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffInfos", false, Functions.GetInfosHTML(null));
        }

        [Command("debug")]
        public static void CMD_debug(MyPlayer player)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var html = $"<h3>Pontos de Áudio</h3>";
            foreach (var audioSpot in Global.AudioSpots)
                html += $@"Id: {audioSpot.Id} | Position: {audioSpot.Position.X} {audioSpot.Position.Y} {audioSpot.Position.Z} | VehicleId: {audioSpot.VehicleId} <br/><br/>";

            html += $"<h3>Pontos de Luz</h3>";
            foreach (var spotlight in Global.Spotlights)
                html += $@"Id: {spotlight.Id} | Position: {spotlight.Position.X} {spotlight.Position.Y} {spotlight.Position.Z} | Player: {spotlight.Player} <br/><br/>";

            player.Emit("Server:BaseHTML", Functions.GetBaseHTML($"{Global.SERVER_NAME} • Debug", html));
        }

        [Command("app")]
        public static async Task CMD_app(MyPlayer player)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            var app = await context.Characters.Where(x => x.EvaluatingStaffUserId == player.User.Id)
                .Include(x => x.User)
                .FirstOrDefaultAsync();

            void ShowHTML()
            {
                var html = $@"<h4>Nome: {app.Name}<h4/> 
                    <h4>Nascimento: {app.BirthdayDate.ToShortDateString()} ({Math.Truncate((DateTime.Now.Date - app.BirthdayDate).TotalDays / 365):N0} anos)<h4/> 
                    <h4>Caracteres História: {app.History.Length} de 4096<h4/> 
                    <h4>OOC: {app.User.Name} [{app.User.Id}]<h4/> 
                    <h4>Enviada: {app.RegisterDate}<h4/> 
                    {app.History}
                    <h4>Use /aceitarapp ou /negarapp (motivo)</h4>";

                player.Emit("Server:BaseHTML", Functions.GetBaseHTML($"Aplicação de {app.Name} [{app.Id}]", html));
            }

            if (app != null)
            {
                ShowHTML();
                return;
            }

            app = await context.Characters
                .Where(x => !x.EvaluatorStaffUserId.HasValue && !x.EvaluatingStaffUserId.HasValue)
                .Include(x => x.User)
                .OrderByDescending(x => x.User.VIP >= UserVIP.Silver ? 1 : 0)
                .FirstOrDefaultAsync();
            if (app == null)
            {
                player.SendMessage(MessageType.Error, "Nenhuma aplicação está aguardando avaliação.");
                return;
            }

            app.EvaluatingStaffUserId = player.User.Id;
            context.Update(app);
            await context.SaveChangesAsync();

            player.User.CharacterApplicationsQuantity++;

            ShowHTML();
        }

        [Command("aceitarapp")]
        public static async Task CMD_aceitarapp(MyPlayer player)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            var app = await context.Characters.Where(x => x.EvaluatingStaffUserId == player.User.Id)
                    .Include(x => x.User)
                    .FirstOrDefaultAsync();
            if (app == null)
            {
                player.SendMessage(MessageType.Error, "Você não está avaliando uma aplicação.");
                return;
            }

            app.EvaluatorStaffUserId = player.User.Id;
            app.EvaluatingStaffUserId = null;
            context.Update(app);
            await context.SaveChangesAsync();

            await Functions.SendDiscordMessage(app.User.DiscordId, $"A aplicação do seu personagem <strong>{app.Name}</strong> foi aceita.");

            player.SendMessage(MessageType.Success, $"Você aceitou a aplicação de **{app.Name} [{app.Id}]**.");
        }

        [Command("negarapp", "/negarapp (motivo)", GreedyArg = true)]
        public static async Task CMD_negarapp(MyPlayer player, string motivo)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            var app = await context.Characters.Where(x => x.EvaluatingStaffUserId == player.User.Id)
                    .Include(x => x.User)
                    .FirstOrDefaultAsync();
            if (app == null)
            {
                player.SendMessage(MessageType.Error, "Você não está avaliando uma aplicação.");
                return;
            }

            app.EvaluatorStaffUserId = player.User.Id;
            app.EvaluatingStaffUserId = null;
            app.RejectionReason = motivo;
            context.Update(app);
            await context.SaveChangesAsync();

            await Functions.SendDiscordMessage(app.User.DiscordId, $"A aplicação do seu personagem <strong>{app.Name}</strong> foi negada. Motivo: <strong>{motivo}</strong>");

            player.SendMessage(MessageType.Success, $"Você negou a aplicação de **{app.Name} [{app.Id}]**. Motivo: **{motivo}**");
        }

        [Command("apps")]
        public static async Task CMD_apps(MyPlayer player)
        {
            if (player.User.Staff < UserStaff.Moderator)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            await using var context = new DatabaseContext();
            var apps = await context.Characters
                .Where(x => !x.EvaluatorStaffUserId.HasValue)
                .Include(x => x.User)
                .Include(x => x.EvaluatingStaffUser)
                .OrderByDescending(x => x.User.VIP >= UserVIP.Silver ? 1 : 0)
                .ToListAsync();
            if (!apps.Any())
            {
                player.SendMessage(MessageType.Error, "Nenhuma aplicação está aguardando avaliação.");
                return;
            }

            var html = string.Empty;

            foreach (var app in apps)
                html += $"<h4>Aplicação de {app.Name} [{app.Id}] (Responsável: {(!app.EvaluatingStaffUserId.HasValue ? "N/A" : $"{app.EvaluatingStaffUser.Name} [{app.EvaluatingStaffUserId}]")})</h4>";

            player.Emit("Server:BaseHTML", Functions.GetBaseHTML($"Aplicações Aguardando Avaliação", html));
        }
    }
}