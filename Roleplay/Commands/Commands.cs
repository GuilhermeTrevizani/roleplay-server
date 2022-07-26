using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Roleplay.Commands
{
    public class Commands
    {
        [Command("id", "/id (ID ou nome)", GreedyArg = true)]
        public static void CMD_id(MyPlayer player, string idNome)
        {
            var personagens = Global.Players.Where(x => x.Character.Id > 0 && ((int.TryParse(idNome, out int id) && x.SessionId == id) || x.ICName.ToLower().Contains(idNome.ToLower()))).OrderBy(x => x.SessionId).ToList();
            if (personagens.Count == 0)
            {
                player.SendMessage(MessageType.Error, $"Nenhum jogador foi encontrado com a pesquisa: {idNome}.");
                return;
            }

            player.SendMessage(MessageType.Title, $"Jogadores encontrados com a pesquisa: {idNome}.");
            foreach (var pl in personagens)
                player.SendMessage(MessageType.None, $"{pl.ICName} [{pl.SessionId}]");
        }

        [Command("aceitar", "/aceitar (tipo)", Aliases = new string[] { "ac" })]
        public static async Task CMD_aceitar(MyPlayer player, int tipo)
        {
            if (!Enum.IsDefined(typeof(InviteType), tipo))
            {
                player.SendMessage(MessageType.Error, "Tipo inválido.");
                return;
            }

            var convite = player.Invites.FirstOrDefault(x => x.Type == (InviteType)tipo);
            if (convite == null)
            {
                player.SendMessage(MessageType.Error, $"Você não possui nenhum convite do tipo {tipo}.");
                return;
            }

            var target = Global.Players.FirstOrDefault(x => x.Character.Id == convite.SenderCharacterId);
            if (target == null)
            {
                player.Invites.RemoveAll(x => x.Type == (InviteType)tipo);
                player.SendMessage(MessageType.Error, "Jogador que enviou o convite não está online.");
                return;
            }

            switch ((InviteType)tipo)
            {
                case InviteType.Faccao:
                    if (!int.TryParse(convite.Value[0], out int faccao) || !int.TryParse(convite.Value[1], out int rank))
                        return;

                    player.Character.FactionId = faccao;
                    player.Character.FactionRankId = rank;

                    if (player.Faction?.Type != FactionType.Criminal)
                    {
                        player.Character.Job = CharacterJob.None;
                        player.OnDuty = false;
                    }

                    player.SendFactionMessage($"{player.Character.Name} entrou na facção.");
                    await player.Save();
                    break;
                case InviteType.VendaPropriedade:
                    if (player.Position.Distance(target.Position) > Global.RP_DISTANCE || player.Dimension != target.Dimension)
                    {
                        player.SendMessage(MessageType.Error, "Dono da propriedade não está próximo de você.");
                        return;
                    }

                    if (!int.TryParse(convite.Value[0], out int propriedade) || !int.TryParse(convite.Value[1], out int valor))
                        return;

                    if (player.Money < valor)
                    {
                        player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, valor));
                        break;
                    }

                    var prop = Global.Properties.FirstOrDefault(x => x.Id == propriedade && x.CharacterId == target.Character.Id);
                    if (prop == null)
                    {
                        player.SendMessage(MessageType.Error, "Propriedade inválida.");
                        break;
                    }

                    if (player.Position.Distance(new Position(prop.EntrancePosX, prop.EntrancePosY, prop.EntrancePosZ)) > Global.RP_DISTANCE)
                    {
                        player.SendMessage(MessageType.Error, "Você não está próximo da propriedade.");
                        return;
                    }

                    var res = await target.GiveItem(new CharacterItem(ItemCategory.Money)
                    {
                        Quantity = valor
                    });

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.SendMessage(MessageType.Error, res);
                        return;
                    }

                    await player.RemoveItem(new CharacterItem(ItemCategory.Money)
                    {
                        Quantity = valor
                    });

                    prop.CharacterId = player.Character.Id;

                    await using (var context = new DatabaseContext())
                    {
                        context.Properties.Update(prop);
                        await context.SaveChangesAsync();
                    }

                    await target.GravarLog(LogType.Venda, $"/pvender {prop.Id} {valor}", player);
                    player.SendMessage(MessageType.Success, $"Você comprou a propriedade {prop.Id} de {target.ICName} por ${valor:N0}.");
                    target.SendMessage(MessageType.Success, $"Você vendeu a propriedade {prop.Id} para {player.ICName} por ${valor:N0}.");
                    break;
                case InviteType.Revista:
                    if (player.Position.Distance(target.Position) > Global.RP_DISTANCE || player.Dimension != target.Dimension)
                    {
                        player.SendMessage(MessageType.Error, "Solicitante da revista não está próximo de você.");
                        return;
                    }

                    player.SendMessage(MessageType.Success, $"Você aceitou ser revistado.");
                    player.ShowInventory(target, InventoryShowType.Inspect);
                    break;
                case InviteType.VendaVeiculo:
                    if (player.Position.Distance(target.Position) > Global.RP_DISTANCE || player.Dimension != target.Dimension)
                    {
                        player.SendMessage(MessageType.Error, "Dono do veículo não está próximo de você.");
                        return;
                    }

                    if (!int.TryParse(convite.Value[0], out int veiculo) || !int.TryParse(convite.Value[1], out int valorVeh))
                        return;

                    if (player.Money < valorVeh)
                    {
                        player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, valorVeh));
                        break;
                    }

                    var veh = Global.Vehicles.FirstOrDefault(x => x.Vehicle.Id == veiculo);
                    if (veh == null)
                    {
                        player.SendMessage(MessageType.Error, "Veículo inválido.");
                        break;
                    }

                    if (player.Position.Distance(veh.Position) > Global.RP_DISTANCE)
                    {
                        player.SendMessage(MessageType.Error, "Você não está próximo do veículo.");
                        return;
                    }

                    res = await target.GiveItem(new CharacterItem(ItemCategory.Money)
                    {
                        Quantity = valorVeh
                    });

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.SendMessage(MessageType.Error, res);
                        return;
                    }

                    await player.RemoveItem(new CharacterItem(ItemCategory.Money)
                    {
                        Quantity = valorVeh
                    });

                    veh.Vehicle.CharacterId = player.Character.Id;
                    veh.Vehicle.Parked = false;

                    await using (var context = new DatabaseContext())
                    {
                        context.Vehicles.Update(veh.Vehicle);
                        await context.SaveChangesAsync();
                    }

                    player.SendMessage(MessageType.Success, $"Você comprou o veículo {veh.Vehicle.Id} de {target.ICName} por ${valorVeh:N0}.");
                    target.SendMessage(MessageType.Success, $"Você vendeu o veículo {veh.Vehicle.Id} para {player.ICName} por ${valorVeh:N0}.");
                    await target.GravarLog(LogType.Venda, $"/vvenderpara {veh.Vehicle.Id} {valorVeh}", player);
                    break;
                case InviteType.LocalizacaoCelular:
                    if (!float.TryParse(convite.Value[0], out float posX) || !float.TryParse(convite.Value[1], out float posY))
                        return;

                    player.Emit("Server:SetWaypoint", posX, posY);
                    player.SendMessage(MessageType.None, $"[CELULAR] A posição recebida foi marcada no GPS.", Global.CELLPHONE_SECONDARY_COLOR);
                    break;
                case InviteType.Company:
                    if (!int.TryParse(convite.Value[0], out int companyId))
                        return;

                    var company = Global.Companies.FirstOrDefault(x => x.Id == companyId);
                    if (company == null)
                        return;

                    await using (var context = new DatabaseContext())
                    {
                        var companyCharacter = new CompanyCharacter
                        {
                            CompanyId = companyId,
                            CharacterId = player.Character.Id,
                        };
                        await context.CompaniesCharacters.AddAsync(companyCharacter);
                        await context.SaveChangesAsync();

                        company.Characters.Add(companyCharacter);
                    }

                    player.SendMessage(MessageType.Success, $"Você aceitou o convite para entrar na empresa.");
                    target.SendMessage(MessageType.Success, $"{player.Character.Name} aceitou seu convite para entrar na empresa.");
                    break;
                case InviteType.Mechanic:
                    player.VehicleTuning = JsonSerializer.Deserialize<VehicleTuning>(convite.Value.FirstOrDefault());
                    
                    player.SendMessage(MessageType.Success, $"Você aceitou o convite para receber o catálogo de modificações veiculares.");
                    target.SendMessage(MessageType.Success, $"{player.Character.Name} aceitou seu convite para receber o catálogo de modificações veiculares.");
                    break;
            }

            player.Invites.RemoveAll(x => x.Type == (InviteType)tipo);
        }

        [Command("recusar", "/recusar (tipo)", Aliases = new string[] { "rc" })]
        public static void CMD_recusar(MyPlayer player, int tipo)
        {
            if (!Enum.IsDefined(typeof(InviteType), tipo))
            {
                player.SendMessage(MessageType.Error, "Tipo inválido.");
                return;
            }

            var convite = player.Invites.FirstOrDefault(x => x.Type == (InviteType)tipo);
            if (convite == null)
            {
                player.SendMessage(MessageType.Error, $"Você não possui nenhum convite do tipo {tipo}.");
                return;
            }

            var target = Global.Players.FirstOrDefault(x => x.Character.Id == convite.SenderCharacterId);
            var strPlayer = string.Empty;
            var strTarget = string.Empty;

            switch ((InviteType)tipo)
            {
                case InviteType.Faccao:
                    strPlayer = strTarget = "entrar na facção";
                    break;
                case InviteType.VendaPropriedade:
                    strPlayer = "compra da propriedade";
                    strTarget = "venda da propriedade";
                    break;
                case InviteType.VendaVeiculo:
                    strPlayer = "compra de veículo";
                    strTarget = "venda de veículo";
                    break;
                case InviteType.Revista:
                    strPlayer = strTarget = "revista";
                    break;
                case InviteType.LocalizacaoCelular:
                    strPlayer = strTarget = "envio de localização";
                    break;
                case InviteType.Company:
                    strPlayer = strTarget = "entrar na empresa";
                    break;
                case InviteType.Mechanic:
                    strPlayer = strTarget = "receber o catálogo de modificações veiculares";
                    break;
            }

            player.SendMessage(MessageType.Success, $"Você recusou o convite para {strPlayer}.");

            if (target != null)
                target.SendMessage(MessageType.Success, $"{player.ICName} recusou seu convite para {strTarget}.");

            player.Invites.RemoveAll(x => x.Type == (InviteType)tipo);
        }

        [Command("revistar", "/revistar (ID ou nome)")]
        public static void CMD_revistar(MyPlayer player, string idNome)
        {
            var target = player.ObterPersonagemPorIdNome(idNome, false);
            if (target == null)
                return;

            if (player.Position.Distance(target.Position) > Global.RP_DISTANCE || player.Dimension != target.Dimension)
            {
                player.SendMessage(MessageType.Error, "Jogador não está próximo de você.");
                return;
            }

            if (player.Faction?.Type == FactionType.Police && player.OnDuty)
            {
                target.ShowInventory(player, InventoryShowType.Inspect);
            }
            else
            {
                var invite = new Invite
                {
                    Type = InviteType.Revista,
                    SenderCharacterId = player.Character.Id,
                };
                target.Invites.RemoveAll(x => x.Type == InviteType.Revista);
                target.Invites.Add(invite);

                player.SendMessage(MessageType.Success, $"Você solicitou uma revista para {target.ICName}.");
                target.SendMessage(MessageType.Success, $"{player.ICName} solicitou uma revista em você. (seus itens poderão ser subtraídos) (/ac {(int)invite.Type} para aceitar ou /rc {(int)invite.Type} para recusar)");
            }
        }

        [Command("comprar")]
        public static async Task CMD_comprar(MyPlayer player)
        {
            var ponto = Global.Spots.OrderBy(x => player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ))).FirstOrDefault();
            if (ponto != null && player.Position.Distance(new Position(ponto.PosX, ponto.PosY, ponto.PosZ)) <= Global.RP_DISTANCE)
            {
                if (ponto.Type == SpotType.LojaConveniencia)
                {
                    player.Emit("Server:ComprarConveniencia", JsonSerializer.Serialize(Global.Prices.Where(x => x.Type == PriceType.Conveniencia).OrderBy(x => x.Name).Select(x => new
                    {
                        Nome = x.Name,
                        Preco = $"${x.Value:N0}",
                    }).ToList()));
                    return;
                }

                if (ponto.Type == SpotType.Barbearia)
                {
                    if (player.Money < Global.Parameter.BarberValue)
                    {
                        player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, Global.Parameter.BarberValue));
                        return;
                    }

                    player.Invincible = true;
                    player.Emit("AbrirBarbearia", (int)player.Character.Sex, JsonSerializer.Serialize(player.Personalization));
                    return;
                }

                if (ponto.Type == SpotType.LojaRoupas)
                {
                    if (player.Money < Global.Parameter.ClothesValue)
                    {
                        player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, Global.Parameter.ClothesValue));
                        return;
                    }

                    player.Invincible = true;
                    player.Emit("AbrirLojaRoupas", (int)player.Character.Sex, 1, 0);
                    return;
                }

                if (ponto.Type == SpotType.TattooShop)
                {
                    if (player.Money < Global.Parameter.TattooValue)
                    {
                        player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, Global.Parameter.TattooValue));
                        return;
                    }

                    player.Invincible = true;
                    player.Emit("OpenTattoo", (int)player.Character.Sex, JsonSerializer.Serialize(player.Personalization));
                    return;
                }
            }

            var prox = Global.Properties
                .Where(x => !x.CharacterId.HasValue
                    && player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)) <= Global.RP_DISTANCE)
                .MinBy(x => player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)));
            if (prox != null)
            {
                if (player.Money < prox.Value)
                {
                    player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, prox.Value));
                    return;
                }

                await player.RemoveItem(new CharacterItem(ItemCategory.Money)
                {
                    Quantity = prox.Value
                });

                prox.CharacterId = player.Character.Id;
                prox.CreateIdentifier();

                await using var context = new DatabaseContext();
                context.Properties.Update(prox);
                await context.SaveChangesAsync();

                player.SendMessage(MessageType.Success, $"Você comprou a propriedade por ${prox.Value:N0}.");
                return;
            }

            var conce = Global.Dealerships.OrderBy(x => player.Position.Distance(x.Position))
                .FirstOrDefault();
            if (conce != null && player.Position.Distance(conce.Position) <= Global.RP_DISTANCE)
            {
                var veiculos = Global.Prices.Where(x => x.Type == conce.PriceType).OrderBy(x => x.Name).Select(x => new
                {
                    Nome = x.Name.ToUpper(),
                    Preco = x.Value.ToString("N0"),
                    Exibicao = Alt.GetVehicleModelInfo(x.Name).Title,
                    Restricao = Functions.CheckVIPVehicle(x.Name).Item1,
                }).ToList();

                player.Emit("Server:ComprarVeiculo", conce.Name, (int)conce.PriceType, JsonSerializer.Serialize(veiculos));
                return;
            }

            player.SendMessage(MessageType.Error, "Você não está próximo de nenhum ponto de compra.");
        }

        [Command("sos", "/sos (mensagem)", GreedyArg = true)]
        public static async Task CMD_sos(MyPlayer player, string mensagem)
        {
            if (player?.User?.Staff > 0)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var sos = Global.HelpRequests.FirstOrDefault(x => x.CharacterSessionId == player.SessionId);
            if (sos != null)
            {
                var target = await sos.Check();
                if (target != null)
                {
                    player.SendMessage(MessageType.Error, "Você já possui um SOS pendente de resposta.");
                    return;
                }
            }

            var players = Global.Players.Where(x => x.OnAdminDuty && x.User.Staff != UserStaff.None).ToList();
            if (players.Count == 0)
            {
                player.SendMessage(MessageType.Error, "Não há administradores em serviço.");
                return;
            }

            sos = new HelpRequest()
            {
                CharacterSessionId = player.SessionId,
                Message = mensagem,
                UserId = player.User.Id,
                CharacterName = player.Character.Name,
                UserName = player.User.Name,
            };

            using var context = new DatabaseContext();
            await context.HelpRequests.AddAsync(sos);
            await context.SaveChangesAsync();

            Global.HelpRequests.Add(sos);

            foreach (var x in players)
            {
                x.SendMessage(MessageType.None, $"SOS de {player.Character.Name} [{player.SessionId}] ({player.User.Name}) (use /at {player.SessionId} para atender)");
                x.SendMessage(MessageType.None, mensagem, "#69d4f5");
            }

            player.SendMessage(MessageType.Success, "SOS enviado para os administradores em serviço.");

            var html = Functions.GetSOSJSON();
            foreach (var target in Global.Players.Where(x => x.User.Staff != UserStaff.None))
                target.Emit("ACPUpdateSOS", html);
        }

        [Command("ferimentos", "/ferimentos (ID ou nome)")]
        public static void CMD_ferimentos(MyPlayer player, string idNome)
        {
            var target = player.ObterPersonagemPorIdNome(idNome);
            if (target == null)
                return;

            if (player.Position.Distance(target.Position) > Global.RP_DISTANCE || player.Dimension != target.Dimension || target.Ferimentos.Count == 0)
            {
                player.SendMessage(MessageType.Error, "Jogador não está próximo ou não possui ferimentos.");
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
                    </tr>
                </thead>
                <tbody>";

            foreach (var x in target.Ferimentos)
                html += $@"<tr><td>{x.Data}</td><td>{(WeaponModel)x.Arma}</td><td>{x.Dano}</td><td>{Functions.GetBodyPartName(x.BodyPart)}</td></tr>";

            html += $@"
                </tbody>
            </table>
            </div>";

            player.Emit("Server:BaseHTML", Functions.GetBaseHTML($"Ferimentos de {target.ICName}", html));
        }

        [Command("aceitarhospital")]
        public static async Task CMD_aceitarhospital(MyPlayer player)
        {
            if (player.Character.Wound != CharacterWound.PodeHospitalCK)
            {
                player.SendMessage(MessageType.Error, "Você ainda não pode executar esse comando.");
                return;
            }

            var spot = Global.Spots
                .Where(x => x.Type == SpotType.MeCurar)
                .MinBy(x => player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)));
            if (spot == null)
            {
                player.SendMessage(MessageType.Error, "Nenhum ponto de cura foi configurado. Entre em contato com a administração.");
                return;
            }

            player.Curar();
            await player.RemoveItem(player.Items.Where(x => x.Category == ItemCategory.Weapon));
            player.Character.DrugItemCategory = null;
            player.Character.DrugEndDate = null;
            player.Character.ThresoldDeath = 0;
            player.Character.ThresoldDeathEndDate = null;
            player.DrugTimer?.Stop();
            player.ClearDrugEffect();
            player.Cuffed = false;

            foreach (var x in Global.Players.Where(x => x.Dimension == player.Dimension && player.Position.Distance(x.Position) <= 20))
                x.SendMessage(MessageType.Error, $"(( {player.ICName} [{player.SessionId}] aceitou tratamento no hospital. ))");

            player.SetPosition(new Position(spot.PosX, spot.PosY, spot.PosZ), 0, true);
            player.Character.Bank -= Global.Parameter.HospitalValue;

            await using var context = new DatabaseContext();
            await context.FinancialTransactions.AddAsync(new FinancialTransaction
            {
                Type = FinancialTransactionType.Withdraw,
                CharacterId = player.Character.Id,
                Value = Global.Parameter.HospitalValue,
                Description = "Custos Hospitalares",
            });
            await context.SaveChangesAsync();

            player.SendMessage(MessageType.Success, $"Você aceitou o tratamento e foi levado para o hospital. Os custos hospitalares foram ${Global.Parameter.HospitalValue:N0}.");
        }

        [Command("aceitarck")]
        public static async Task CMD_aceitarck(MyPlayer player)
        {
            if (player.Character.Wound != CharacterWound.PodeHospitalCK)
            {
                player.SendMessage(MessageType.Error, "Você não pode executar esse comando ainda.");
                return;
            }

            player.Character.DeathDate = DateTime.Now;
            player.Character.DeathReason = "Aceitou CK";

            foreach (var x in Global.Players.Where(x => x.Dimension == player.Dimension && player.Position.Distance(x.Position) <= 20))
                x.SendMessage(MessageType.Error, $"(( {player.ICName} [{player.SessionId}] aceitou CK. ))");

            await Functions.SendStaffMessage($"{player.Character.Name} aceitou CK.", true);
            await player.Save();
            await player.GravarLog(LogType.Morte, $"/aceitarck", null);
            await player.ListarPersonagens("CK", "Você aceitou o CK no seu personagem.");
        }

        [Command("trancar")]
        public static async Task CMD_trancar(MyPlayer player) => await Functions.CMDTrancar(player);

        [Command("mostraridentidade", "/mostraridentidade (ID ou nome)", Aliases = new string[] { "mostrarid" })]
        public static void CMD_mostraridentidade(MyPlayer player, string idNome)
        {
            var target = player.ObterPersonagemPorIdNome(idNome);
            if (target == null)
                return;

            if (player.Position.Distance(target.Position) > Global.RP_DISTANCE || player.Dimension != target.Dimension)
            {
                player.SendMessage(MessageType.Error, "Jogador não está próximo de você.");
                return;
            }

            target.SendMessage(MessageType.Title, $"ID de {player.Character.Name}");
            target.SendMessage(MessageType.None, $"Sexo: {Functions.GetEnumDisplay(player.Character.Sex)}");
            target.SendMessage(MessageType.None, $"Nascimento: {player.Character.BirthdayDate.ToShortDateString()} ({Math.Truncate((DateTime.Now.Date - player.Character.BirthdayDate).TotalDays / 365):N0} anos)");
            player.SendMessageToNearbyPlayers(player == target ? "olha sua própria ID." : $"mostra sua ID para {target.ICName}.", MessageCategory.Ame, 10);
        }

        [Command("dmv")]
        public static void CMD_dmv(MyPlayer player)
        {
            if ((player.Character.DriverLicenseValidDate ?? DateTime.MinValue).Date > DateTime.Now.Date
                && !player.Character.PoliceOfficerBlockedDriverLicenseCharacterId.HasValue)
            {
                player.SendMessage(MessageType.Error, "Sua licença de motorista não vence hoje ou não está revogada.");
                return;
            }

            if (!Global.Spots.Any(x => x.Type == SpotType.DMV
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE))
            {
                player.SendMessage(MessageType.Error, "Você não está na DMV.");
                return;
            }

            var valor = player.Character.DriverLicenseValidDate.HasValue ? Global.Parameter.DriverLicenseRenewValue : Global.Parameter.DriverLicenseBuyValue;
            if (player.Money < valor)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, valor));
                return;
            }

            player.SetClothes(1, 0, 0, 0);
            player.ClearProps(0);
            player.ClearProps(1);
            player.ClearProps(2);
            player.ClearProps(6);
            player.ClearProps(7);

            player.Emit("RegistrarImagemDMV", valor);
        }

        [Command("mostrarlicenca", "/mostrarlicenca (ID ou nome)", Aliases = new string[] { "ml" })]
        public static void CMD_mostrarlicenca(MyPlayer player, string idNome)
        {
            if (!player.Character.DriverLicenseValidDate.HasValue)
            {
                player.SendMessage(MessageType.Error, "Você não possui uma licença de motorista.");
                return;
            }

            var target = player.ObterPersonagemPorIdNome(idNome);
            if (target == null)
                return;

            if (player.Position.Distance(target.Position) > Global.RP_DISTANCE || player.Dimension != target.Dimension)
            {
                player.SendMessage(MessageType.Error, "Jogador não está próximo de você.");
                return;
            }

            target.SendMessage(MessageType.Title, $"Licença de Motorista de {player.Character.Name}");
            target.SendMessage(MessageType.None, $"Validade: {player.Character.DriverLicenseValidDate?.ToShortDateString()}");
            target.SendMessage(MessageType.None, $"Status: {(player.Character.PoliceOfficerBlockedDriverLicenseCharacterId.HasValue ? $"{{{Global.ERROR_COLOR}}}REVOGADA" : (player.Character.DriverLicenseValidDate?.Date >= DateTime.Now.Date ? $"{{{Global.SUCCESS_COLOR}}}VÁLIDA" : $"{{{Global.ERROR_COLOR}}}VENCIDA"))}");
            player.SendMessageToNearbyPlayers(player == target ? "olha sua própria licença de motorista." : $"mostra sua licença de motorista para {target.ICName}.", MessageCategory.Ame, 10);
        }

        [Command("horas")]
        public static void CMD_horas(MyPlayer player)
        {
            var horas = Convert.ToInt32(Math.Truncate(player.Character.ConnectedTime / 60M));
            var tempo = 60 - ((horas + 1) * 60 - player.Character.ConnectedTime);
            player.SendMessage(MessageType.None, $"{{{Global.MAIN_COLOR}}}{tempo} {{#FFFFFF}}de {{{Global.MAIN_COLOR}}}60 {{#FFFFFF}}minutos para o próximo pagamento.");
            player.SendMessage(MessageType.None, $"{Global.SERVER_INITIALS} | {DateTime.Now}", notify: true);
        }

        [Command("telapreta")]
        public static void CMD_telapreta(MyPlayer player) => player.Emit("chat:toggleTelaPreta");

        [Command("telacinza")]
        public static void CMD_telacinza(MyPlayer player) => player.Emit("chat:toggleTelaCinza");

        [Command("telalaranja")]
        public static void CMD_telalaranja(MyPlayer player) => player.Emit("chat:toggleTelaLaranja");

        [Command("telaverde")]
        public static void CMD_telaverde(MyPlayer player) => player.Emit("chat:toggleTelaVerde");

        [Command("limparchat")]
        public static void CMD_limparchat(MyPlayer player) => player.LimparChat();

        [Command("mecurar")]
        public static async Task CMD_mecurar(MyPlayer player)
        {
            if (!player.Ferido)
            {
                player.SendMessage(MessageType.Error, "Você não está ferido.");
                return;
            }

            var valor = Global.Parameter.HospitalValue / 2;
            if (player.Money < valor)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, valor));
                return;
            }

            if (!Global.Spots.Any(x => x.Type == SpotType.MeCurar
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE))
            {
                player.SendMessage(MessageType.Error, "Você não está em um hospital.");
                return;
            }

            var onDuty = Global.Players.Count(x => x.Faction?.Type == FactionType.Firefighter && x.OnDuty);
            if (onDuty > Global.Parameter.FirefightersBlockHeal)
            {
                player.SendMessage(MessageType.Error, $"Não é possível curar pois há {onDuty} bombeiro{(onDuty != 1 ? "s" : string.Empty)} em trabalho.");
                return;
            }

            player.ToggleGameControls(false);
            player.SendMessage(MessageType.Success, $"Aguarde 30 segundos. Pressione DELETE para cancelar a ação.");
            player.CancellationTokenSourceAcao?.Cancel();
            player.CancellationTokenSourceAcao = new System.Threading.CancellationTokenSource();
            await Task.Delay(30000, player.CancellationTokenSourceAcao.Token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;

                Task.Run(async () =>
                {
                    player.Curar();
                    await player.RemoveItem(new CharacterItem(ItemCategory.Money)
                    {
                        Quantity = valor
                    });
                    player.ToggleGameControls(true);
                    player.SendMessage(MessageType.Success, $"Você tratou seus ferimentos por ${valor:N0}.");
                    await player.GravarLog(LogType.MeCurar, string.Empty, null);
                    player.CancellationTokenSourceAcao = null;
                });
            });
        }

        [Command("save")]
        public static void CMD_save(MyPlayer player)
        {
            player.Emit("alt:log", $"DIMENSION: {player.Dimension}");
            if (player.IsInVehicle)
            {
                player.Emit("alt:log", $"POS: {player.Vehicle.Position.X.ToString().Replace(",", ".")}f, {player.Vehicle.Position.Y.ToString().Replace(",", ".")}f, {player.Vehicle.Position.Z.ToString().Replace(",", ".")}f");
                player.Emit("alt:log", $"ROT: {player.Vehicle.Rotation.Roll.ToString().Replace(",", ".")}f, {player.Vehicle.Rotation.Pitch.ToString().Replace(",", ".")}f, {player.Vehicle.Rotation.Yaw.ToString().Replace(",", ".")}f");
            }
            else
            {
                player.Emit("alt:log", $"POS: {player.Position.X.ToString().Replace(",", ".")}f, {player.Position.Y.ToString().Replace(",", ".")}f, {player.Position.Z.ToString().Replace(",", ".")}f");
                player.Emit("alt:log", $"ROT: {player.Rotation.Roll.ToString().Replace(",", ".")}f, {player.Rotation.Pitch.ToString().Replace(",", ".")}f, {player.Rotation.Yaw.ToString().Replace(",", ".")}f");
            }
        }

        [Command("dados")]
        public static void CMD_dados(MyPlayer player)
        {
            var numero = new List<int> { 1, 2, 3, 4, 5, 6 }.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            var mensagem = $@"[DADOS] {player.ICName} joga o dado e este fica com o número {numero} voltado para cima.";
            player.SendMessageToNearbyPlayers(mensagem, MessageCategory.DadosMoeda, player.Dimension > 0 ? 7.5f : 20.0f);
        }

        [Command("moeda")]
        public static void CMD_moeda(MyPlayer player)
        {
            var numero = new List<int> { 1, 2 }.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
            var mensagem = $@"[MOEDA] {player.ICName} joga a moeda e esta fica com a {(numero == 1 ? "cara" : "coroa")} voltada para cima.";
            player.SendMessageToNearbyPlayers(mensagem, MessageCategory.DadosMoeda, player.Dimension > 0 ? 7.5f : 20.0f);
        }

        [Command("levantar", "/levantar (ID ou nome)")]
        public static void CMD_levantar(MyPlayer player, string idNome)
        {
            var target = player.ObterPersonagemPorIdNome(idNome, false);
            if (target == null)
                return;

            if (player.Position.Distance(target.Position) > Global.RP_DISTANCE || player.Dimension != target.Dimension)
            {
                player.SendMessage(MessageType.Error, "Jogador não está próximo de você.");
                return;
            }

            if (target.Character.Wound != CharacterWound.GravementeFerido || target.Ferimentos.Any(x => x.Arma != (uint)WeaponModel.Fist))
            {
                player.SendMessage(MessageType.Error, "Jogador não está gravemente ferido ou ferido somente com socos.");
                return;
            }

            target.Curar(true);
            player.SendMessageToNearbyPlayers($"ajuda {target.ICName} a se levantar.", MessageCategory.Me, player.Dimension > 0 ? 7.5f : 20.0f);
        }

        [Command("trocarpersonagem")]
        public static async Task CMD_trocarpersonagem(MyPlayer player) => await player.ListarPersonagens("Troca de Personagem", string.Empty);

        [Command("staff")]
        public static void CMD_staff(MyPlayer player)
        {
            player.SendMessage(MessageType.Title, $"Staff Online {Global.SERVER_NAME}");
            foreach (var x in Global.Players.Where(x => x.Character.Id != 0 && x.User.Staff > UserStaff.None)
                .OrderByDescending(x => x.User.Staff).ThenBy(x => x.Character.Name))
            {
                var status = x.OnAdminDuty ? $"{{{Global.SUCCESS_COLOR}}}EM SERVIÇO" : $"{{{Global.ERROR_COLOR}}}EM ROLEPLAY";
                player.SendMessage(MessageType.None, $"{{{x.StaffColor}}}{Functions.GetEnumDisplay(x.User.Staff)}{{#FFFFFF}} {x.ICName} [{x.SessionId}] ({x.User.Name}) - {status}");
            }
        }

        [Command("dl")]
        public static void CMD_dl(MyPlayer player)
        {
            player.User.VehicleTagToggle = !player.User.VehicleTagToggle;
            player.Emit("dl:Config", player.User.VehicleTagToggle);
            player.SendMessage(MessageType.Success, $"Você {(!player.User.VehicleTagToggle ? "des" : string.Empty)}ativou o DL.");
        }

        [Command("historicocriminal")]
        public static async Task CMD_historicocriminal(MyPlayer player)
        {
            var ponto = Global.Spots.FirstOrDefault(x => x.Type == SpotType.AtendimentoLSPD
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE);
            if (ponto == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de um ponto de atendimento da LSPD.");
                return;
            }

            await using var context = new DatabaseContext();
            var prisoes = await context.Jails.Where(x => x.CharacterId == player.Character.Id).OrderByDescending(x => x.Id).ToListAsync();
            var htmlPrisoes = string.Empty;
            if (prisoes.Any())
            {
                foreach (var prisao in prisoes)
                    htmlPrisoes += $"<h3>Preso em {prisao.Date}. Solto em {prisao.EndDate}</h3>";
            }
            else
            {
                htmlPrisoes = "<h1>Seu histórico criminal está limpo.</h1>";
            }


            player.Emit("Server:BaseHTML", Functions.GetBaseHTML($"Histórico Criminal de {player.Character.Name} [{player.Character.Id}] ({DateTime.Now})", htmlPrisoes));
        }

        [Command("cancelarconvite", "/cancelarconvite (tipo)", Aliases = new string[] { "cc" })]
        public static void CMD_cancelarconvite(MyPlayer player, int tipo)
        {
            if (!Enum.IsDefined(typeof(InviteType), tipo))
            {
                player.SendMessage(MessageType.Error, "Tipo inválido.");
                return;
            }

            var type = (InviteType)tipo;
            var target = Global.Players.FirstOrDefault(x => x.Invites.Any(y => y.SenderCharacterId == player.Character.Id && y.Type == type));
            if (target == null)
            {
                player.SendMessage(MessageType.Error, $"Você não enviou um convite do tipo {Functions.GetEnumDisplay(type)}.");
                return;
            }

            var strPlayer = string.Empty;
            var strTarget = string.Empty;

            switch (type)
            {
                case InviteType.Faccao:
                    strPlayer = strTarget = "entrar na facção";
                    break;
                case InviteType.VendaPropriedade:
                    strPlayer = "compra da propriedade";
                    strTarget = "venda da propriedade";
                    break;
                case InviteType.VendaVeiculo:
                    strPlayer = "compra de veículo";
                    strTarget = "venda de veículo";
                    break;
                case InviteType.Revista:
                    strPlayer = strTarget = "revista";
                    break;
                case InviteType.LocalizacaoCelular:
                    strPlayer = strTarget = "envio de localização";
                    break;
            }

            target.Invites.RemoveAll(x => x.Type == type);
            player.SendMessage(MessageType.Success, $"Você cancelou o convite para {strPlayer}.");
            target.SendMessage(MessageType.Success, $"{player.ICName} cancelou o convite para {strTarget}.");
        }

        [Command("banco")]
        public static async Task CMD_banco(MyPlayer player)
        {
            if (!Global.Spots.Any(x => x.Type == SpotType.Banco
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE))
            {
                player.SendMessage(MessageType.Error, "Você não está em um banco.");
                return;
            }

            await player.ShowBank(false, true, false);
        }

        [Command("atm")]
        public static void CMD_atm(MyPlayer player) => player.Emit("ATMCheck");

        [Command("infos")]
        public static void CMD_infos(MyPlayer player)
        {
            player.Emit("StaffInfos", false, Functions.GetInfosHTML(player.User.Id));
        }

        [Command("bocafumo")]
        public static void CMD_bocafumo(MyPlayer player)
        {
            var crackDen = Global.CrackDens.FirstOrDefault(x =>
                player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE
                && x.Dimension == player.Dimension);
            if (crackDen == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhuma boca de fumo.");
                return;
            }

            var html = Functions.GetCrackDensItemsHTML(crackDen.Id, false);

            player.Emit("ShowCrackDen",
                false,
                html,
                crackDen.Id);
        }

        [Command("usardroga", "/usardroga (nome) (quantidade)")]
        public static async void CMD_usardroga(MyPlayer player, string name, int quantity)
        {
            if (!Enum.TryParse(name, true, out ItemCategory itemCategory))
            {
                player.SendMessage(MessageType.Error, $"Droga {name} não existe.");
                return;
            }

            if (!Functions.CheckIfIsDrug(itemCategory))
            {
                player.SendMessage(MessageType.Error, $"Droga {name} não existe.");
                return;
            }

            if (quantity <= 0)
            {
                player.SendMessage(MessageType.Error, $"Quantidade deve ser maior que 0.");
                return;
            }

            var item = player.Items.FirstOrDefault(x => x.Category == itemCategory && x.Quantity >= quantity);
            if (item == null)
            {
                player.SendMessage(MessageType.Error, $"Você não possui essa quantidade de {Functions.GetEnumDisplay(itemCategory)}.");
                return;
            }

            if (player.Character.DrugItemCategory.HasValue && player.Character.DrugItemCategory != item.Category)
            {
                player.SendMessage(MessageType.Error, $"Você está sob efeito de {Functions.GetEnumDisplay(player.Character.DrugItemCategory)}. Não é possível usar {Functions.GetEnumDisplay(item.Category)}.");
                return;
            }

            var minutesDuration = 0;
            var thresoldDeath = 0;

            switch (item.Category)
            {
                case ItemCategory.Weed:
                    minutesDuration = 10;
                    break;
                case ItemCategory.Cocaine:
                    minutesDuration = 3;
                    thresoldDeath = 10;
                    break;
                case ItemCategory.Crack:
                    minutesDuration = 1;
                    thresoldDeath = 25;
                    break;
                case ItemCategory.Heroin:
                    minutesDuration = 10;
                    thresoldDeath = 25;
                    break;
                case ItemCategory.MDMA:
                    minutesDuration = 10;
                    thresoldDeath = 5;
                    break;
                case ItemCategory.Xanax:
                    minutesDuration = 5;
                    thresoldDeath = 1;
                    break;
                case ItemCategory.Oxycontin:
                    minutesDuration = 3;
                    thresoldDeath = 10;
                    break;
                case ItemCategory.Metanfetamina:
                    minutesDuration = 2;
                    thresoldDeath = 50;
                    break;
            }

            minutesDuration *= quantity;
            thresoldDeath *= quantity;

            if (!player.Character.DrugItemCategory.HasValue)
            {
                switch (item.Category)
                {
                    case ItemCategory.Weed:
                        player.Health = (ushort)(player.Health + 25);
                        break;
                    case ItemCategory.Cocaine:
                        player.Health = (ushort)(player.Health + 50);
                        break;
                    case ItemCategory.Crack:
                        player.Health = (ushort)(player.Health + 50);
                        break;
                    case ItemCategory.Heroin:
                        player.Health = (ushort)(player.Health + 80);
                        break;
                    case ItemCategory.MDMA:
                        player.Health = (ushort)(player.Health + 50);
                        break;
                    case ItemCategory.Xanax:
                        player.Health = (ushort)(player.Health + 50);
                        break;
                    case ItemCategory.Oxycontin:
                        player.Health = (ushort)(player.Health + 50);
                        break;
                }
            }

            await player.RemoveItem(new CharacterItem(item.Category) { Quantity = quantity });

            var newThresoldDeath = player.Character.ThresoldDeath + thresoldDeath;
            player.Character.ThresoldDeath = Convert.ToByte(newThresoldDeath > 100 ? 100 : newThresoldDeath);
            player.Character.DrugItemCategory = item.Category;
            player.Character.DrugEndDate = (player.Character.ThresoldDeath == 100 ? DateTime.Now : player.Character.DrugEndDate ?? DateTime.Now).AddMinutes(minutesDuration);
            player.Character.ThresoldDeathEndDate = null;

            player.SendMessage(MessageType.Success, $"Você usou {quantity}x {Functions.GetEnumDisplay(player.Character.DrugItemCategory)} e seu limiar da morte está em {player.Character.ThresoldDeath}/100.");
            player.SetupDrugTimer(true);

            if (player.Character.ThresoldDeath == 100)
            {
                player.Health = 0;
                player.SendMessage(MessageType.Error, "Você atingiu 100 da limiar de morte e sofreu uma overdose.");
            }
        }

        [Command("boombox")]
        public static void CMD_boombox(MyPlayer player)
        {
            var item = Global.Items.FirstOrDefault(x => x.Category == ItemCategory.Boombox
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE);
            if (item == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de uma boombox.");
                return;
            }

            player.Emit("Boombox", item.Id, item.AudioSpot?.Source ?? string.Empty, item.AudioSpot?.Volume ?? 0.1f);
        }

        [Command("mic", "/mic (mensagem)", GreedyArg = true)]
        public static void CMD_mic(MyPlayer player, string mensagem)
        {
            if (!player.Items.Any(x => x.Category == ItemCategory.Microphone))
            {
                player.SendMessage(MessageType.Error, "Você não possui um microfone.");
                return;
            }

            player.SendMessageToNearbyPlayers(mensagem, MessageCategory.Microphone, 55.0f);
        }

        [Command("colocar", "/colocar (ID ou nome)")]
        public static async Task CMD_colocar(MyPlayer player, string idNome)
        {
            if (player.IsInVehicle)
            {
                player.SendMessage(MessageType.Error, "Você não pode fazer isso dentro de um veículo");
                return;
            }

            var veh = Global.Vehicles.Where(x => player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)) <= Global.RP_DISTANCE
                && x.Dimension == player.Dimension
                && x.LockState == VehicleLockState.Unlocked)
                .OrderBy(x => player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)))
                .FirstOrDefault();

            if (veh == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum veículo destrancado.");
                return;
            }

            var target = player.ObterPersonagemPorIdNome(idNome, false);
            if (target == null)
                return;

            if (player.Position.Distance(target.Position) > Global.RP_DISTANCE
                || player.Dimension != target.Dimension
                || !target.Cuffed
                || target.IsInVehicle)
            {
                player.SendMessage(MessageType.Error, "Jogador não está próximo de você ou não está algemado.");
                return;
            }

            var passageiros = Global.Players.Where(x => x.Vehicle == veh && x != veh.Driver).ToList();

            if (!passageiros.Any(x => x.Seat == 3))
            {
                target.SetIntoVehicle(veh, 3);
            }
            else if (!passageiros.Any(x => x.Seat == 4))
            {
                target.SetIntoVehicle(veh, 4);
            }
            else
            {
                player.SendMessage(MessageType.Error, $"Todos os assentos traseiros do veículo estão ocupados.");
                return;
            }

            player.SendMessage(MessageType.Success, $"Você colocou {target.ICName} dentro do veículo.");
            target.SendMessage(MessageType.Success, $"{player.ICName} colocou você dentro do veículo.");
            await player.GravarLog(LogType.ColocarVeiculo, veh.Vehicle.Id.ToString(), target);
        }

        [Command("retirar", "/retirar (ID ou nome)")]
        public static async Task CMD_retirar(MyPlayer player, string idNome)
        {
            if (player.IsInVehicle)
            {
                player.SendMessage(MessageType.Error, "Você não pode fazer isso dentro de um veículo");
                return;
            }

            var target = player.ObterPersonagemPorIdNome(idNome, false);
            if (target == null)
                return;


            if (player.Position.Distance(target.Position) > Global.RP_DISTANCE
                || player.Dimension != target.Dimension
                || !target.Cuffed
                || !target.IsInVehicle
                || target.Vehicle is not MyVehicle veh
                || veh.LockState != VehicleLockState.Unlocked)
            {
                player.SendMessage(MessageType.Error, "Jogador não está próximo de você ou não está algemado em um veículo destrancado.");
                return;
            }

            var vehId = veh.Vehicle.Id;
            var pos = player.Position;
            pos.Y += 1.5f;
            target.SetPosition(pos, target.Dimension, false);

            player.SendMessage(MessageType.Success, $"Você retirou {target.ICName} do veículo.");
            target.SendMessage(MessageType.Success, $"{player.ICName} retirou você de dentro do veículo.");
            await player.GravarLog(LogType.RetirarVeiculo, vehId.ToString(), target);
        }

        [Command("stopanim", Aliases = new string[] { "sa" })]
        public static void CMD_stopanim(MyPlayer player) => player.CheckAnimations(true);

        [Command("cenario", "/cenario (opção). Use /cenario lista para visualizar as opções disponíveis.")]
        public static void CMD_cenario(MyPlayer player, string option)
        {
            option = option.ToLower();
            if (option == "lista")
            {
                var html = $@"<div class='row'>
                <div class='col-md-12'>
                    <input id='pesquisa' type='text' autofocus class='form-control' placeholder='Pesquise os cenários...' /><br/>
                </div>
                </div>
                <div class='table-responsive'>
                    <div class='table-responsive' style='max-height:50vh;overflow-y:auto;overflow-x:auto;'>
                        <table class='table table-bordered table-striped'>
                            <thead>
                                <tr class='bg-dark'>
                                    <th>Opção</th>
                                </tr>
                            </thead>
                            <tbody>";

                if (!Global.Scenarios.Any())
                {
                    html += "<tr><td class='text-center' colspan='1'>Não há cenários criados.</td></tr>";
                }
                else
                {
                    foreach (var scenario in Global.Scenarios)
                        html += $@"<tr class='pesquisaitem'>
                        <td><strong>{Global.Scenarios.IndexOf(scenario) + 1}</strong> - {scenario.Item2}</td>
                    </tr>";
                }

                html += $@"</tbody>
                </table>
                </div>
                </div>";

                player.Emit("Server:BaseHTML", Functions.GetBaseHTML($"Opções Disponíveis", html));
            }
            else
            {
                if (!int.TryParse(option, out int opcaoInt))
                {
                    player.SendMessage(MessageType.Error, $"Nenhum cenário encontrado com a opção {option}.");
                    return;
                }

                if (opcaoInt <= 0 || opcaoInt > Global.Scenarios.Count)
                {
                    player.SendMessage(MessageType.Error, $"Nenhum cenário encontrado com a opção {option}.");
                    return;
                }

                var scenario = Global.Scenarios[opcaoInt - 1];

                if (!player.CheckAnimations())
                    return;

                player.StopAnimation();
                player.PlayScenario(scenario.Item1);
            }
        }

        [Command("e", "/e (opção). Use /e lista para visualizar as opções disponíveis.")]
        public static void CMD_e(MyPlayer player, string opcao)
        {
            opcao = opcao.ToLower();
            if (opcao == "lista")
            {
                var html = $@"<div class='row'>
                <div class='col-md-12'>
                    <input id='pesquisa' type='text' autofocus class='form-control' placeholder='Pesquise as animações...' /><br/>
                </div>
                </div>
                <div class='table-responsive'>
                    <div class='table-responsive' style='max-height:50vh;overflow-y:auto;overflow-x:auto;'>
                        <table class='table table-bordered table-striped'>
                            <thead>
                                <tr class='bg-dark'>
                                    <th>Opção</th>
                                </tr>
                            </thead>
                            <tbody>";

                if (!Global.Animations.Any())
                {
                    html += "<tr><td class='text-center' colspan='1'>Não há animações criadas.</td></tr>";
                }
                else
                {
                    foreach (var animation in Global.Animations.OrderBy(x => x.Name))
                        html += $@"<tr class='pesquisaitem'>
                        <td>{animation.Display}</td>
                    </tr>";
                }

                html += $@"</tbody>
                </table>
                </div>
                </div>";

                player.Emit("Server:BaseHTML", Functions.GetBaseHTML($"Opções Disponíveis", html));
            }
            else
            {
                var animation = Global.Animations.FirstOrDefault(x => x.Display.ToLower() == opcao);
                if (animation == null)
                {
                    player.SendMessage(MessageType.Error, $"Nenhuma animação encontrada com o nome {opcao}.");
                    return;
                }

                if (!player.CheckAnimations(onlyInVehicle: animation.Vehicle))
                    return;

                player.PlayAnimation(animation.Dictionary, animation.Name, animation.Flag, animation.Duration);
            }
        }

        [Command("porta")]
        public static async Task CMD_porta(MyPlayer player)
        {
            var companies = player.Companies.Select(x => x.Id);

            var door = Global.Doors
                .Where(x => (
                    (x.FactionId.HasValue && x.FactionId == player.Character.FactionId)
                    ||
                    (x.CompanyId.HasValue && companies.Contains(x.CompanyId ?? 0))
                    )
                    && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE * (player.IsInVehicle ? 2 : 1))
                .MinBy(x => player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)));
            if (door == null)
            {
                player.SendMessage(MessageType.Error, "Você não está perto de uma porta que possui acesso.");
                return;
            }

            door.Locked = !door.Locked;
            door.SetupAllClients();
            player.SendMessageToNearbyPlayers($"{(!door.Locked ? "des" : string.Empty)}tranca a porta.", MessageCategory.Ame, 5);

            await using var context = new DatabaseContext();
            context.Doors.Update(door);
            await context.SaveChangesAsync();
        }

        [Command("alugarempresa")]
        public static async Task CMD_alugarempresa(MyPlayer player)
        {
            var company = Global.Companies.Where(x => !x.CharacterId.HasValue
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE)
                .MinBy(x => player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE);
            if (company == null)
            {
                player.SendMessage(MessageType.Error, "Você não está perto de uma empresa disponível para alugar.");
                return;
            }

            if (player.Money < company.WeekRentValue)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, company.WeekRentValue));
                return;
            }

            await player.RemoveItem(new CharacterItem(ItemCategory.Money)
            {
                Quantity = company.WeekRentValue
            });

            company.CharacterId = player.Character.Id;
            company.RentPaymentDate = DateTime.Now.AddDays(7);
            company.RemoveIdentifier();

            await using var context = new DatabaseContext();
            context.Companies.Update(company);
            await context.SaveChangesAsync();

            await player.GravarLog(LogType.Empresa, $"/alugarempresa {company.Id} {company.WeekRentValue}", null);

            player.SendMessage(MessageType.Success, $"Você alugou a empresa {company.Name} [{company.Id}] por 7 dias por ${company.WeekRentValue:N0}.");
            player.SendMessage(MessageType.Success, $"O próximo pagamento será em {company.RentPaymentDate} e será debitado da sua conta bancária. Se você não possuir este valor, a empresa será retirada do seu nome.");
        }

        [Command("empresa")]
        public static void CMD_empresa(MyPlayer player)
        {
            if (!player.Companies.Any())
            {
                player.SendMessage(MessageType.Error, "Você não está em nenhuma empresa.");
                return;
            }

            player.Emit("Companies", false, Functions.GetCompaniesByCharacterHTML(player));
        }

        [Command("tunarver", "/tunarver (ID ou nome)")]
        public static void CMD_tunarver(MyPlayer player, string idNome)
        {
            var target = player.ObterPersonagemPorIdNome(idNome, false);
            if (target == null)
                return;

            if (target.Character.Job != CharacterJob.Mechanic || !target.OnDuty)
            {
                player.SendMessage(MessageType.Error, "O jogador não é mecânico ou não está em serviço.");
                return;
            }

            if (player.Position.Distance(target.Position) > Global.RP_DISTANCE || player.Dimension != target.Dimension)
            {
                player.SendMessage(MessageType.Error, "Jogador não está próximo de você.");
                return;
            }

            Functions.CMDTuning(player, target, false);
        }
    }
}