using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Enums;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class PropertyScript : IScript
    {
        [Command("pvender", "/pvender (ID ou nome) (valor)")]
        public static void CMD_pvender(MyPlayer player, string idNome, int valor)
        {
            var prop = Global.Properties
                .Where(x => x.CharacterId == player.Character.Id
                    && player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)) <= Global.RP_DISTANCE)
                .MinBy(x => player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)));
            if (prop == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de uma propriedade sua.");
                return;
            }

            if (prop.RobberyValue > 0)
            {
                player.SendMessage(MessageType.Error, Global.ROBBED_PROPERTY_ERROR_MESSAGE);
                return;
            }

            var target = player.ObterPersonagemPorIdNome(idNome, false);
            if (target == null)
                return;

            if (player.Position.Distance(target.Position) > Global.RP_DISTANCE || player.Dimension != target.Dimension)
            {
                player.SendMessage(MessageType.Error, "Jogador não está próximo de você.");
                return;
            }

            if (valor <= 0)
            {
                player.SendMessage(MessageType.Error, "Valor não é válido.");
                return;
            }

            var convite = new Invite()
            {
                Type = InviteType.VendaPropriedade,
                SenderCharacterId = player.Character.Id,
                Value = [prop.Id.ToString(), valor.ToString()],
            };
            target.Invites.RemoveAll(x => x.Type == InviteType.VendaPropriedade);
            target.Invites.Add(convite);

            player.SendMessage(MessageType.Success, $"Você ofereceu sua propriedade {prop.Id} para {target.ICName} por ${valor:N0}.");
            target.SendMessage(MessageType.Success, $"{player.ICName} ofereceu para você a propriedade {prop.Id} por ${valor:N0}. (/ac {(int)convite.Type} para aceitar ou /rc {(int)convite.Type} para recusar)");
        }

        [Command("pvendergoverno")]
        public static async Task CMD_pvendergoverno(MyPlayer player) => await Functions.CMDVenderPropriedadeGoverno(player, false);

        [Command("armazenamento")]
        public static void CMD_armazenamento(MyPlayer player)
        {
            var prop = Global.Properties.FirstOrDefault(x => x.Id == player.Dimension);
            if (prop == null)
            {
                player.SendMessage(MessageType.Error, "Você não está no interior de uma propriedade.");
                return;
            }

            if (!prop.CanAccess(player)
                && !(player.Faction?.Type == FactionType.Police && player.OnDuty))
            {
                player.SendMessage(MessageType.Error, "Você não possui acesso a esta propriedade.");
                return;
            }

            if (prop.RobberyValue > 0)
            {
                player.SendMessage(MessageType.Error, Global.ROBBED_PROPERTY_ERROR_MESSAGE);
                return;
            }

            prop.ShowInventory(player, false);
        }

        [Command("pfechadura")]
        public async static Task CMD_pfechadura(MyPlayer player)
        {
            var prop = Global.Properties
                .Where(x => x.CharacterId == player.Character.Id
                    && player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)) <= Global.RP_DISTANCE)
                .MinBy(x => player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)));
            if (prop == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de uma propriedade sua.");
                return;
            }

            if (prop.RobberyValue > 0)
            {
                player.SendMessage(MessageType.Error, Global.ROBBED_PROPERTY_ERROR_MESSAGE);
                return;
            }

            if (player.Money < Global.Parameter.LockValue)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, Global.Parameter.LockValue));
                return;
            }

            await using var context = new DatabaseContext();
            prop.LockNumber = Global.Properties.Select(x => x.LockNumber).DefaultIfEmpty(0u).Max() + 1;
            context.Properties.Update(prop);
            await context.SaveChangesAsync();

            await player.RemoveStackedItem(ItemCategory.Money, Global.Parameter.LockValue);

            player.SendMessage(MessageType.Success, $"Você trocou a fechadura da propriedade {prop.Id} por ${Global.Parameter.LockValue:N0}.");
        }

        [Command("pchave")]
        public async static Task CMD_pchave(MyPlayer player)
        {
            var prop = Global.Properties
                .Where(x => x.CharacterId == player.Character.Id
                    && player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)) <= Global.RP_DISTANCE)
                .MinBy(x => player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)));
            if (prop == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de uma propriedade sua.");
                return;
            }

            if (prop.RobberyValue > 0)
            {
                player.SendMessage(MessageType.Error, Global.ROBBED_PROPERTY_ERROR_MESSAGE);
                return;
            }

            if (player.Money < Global.Parameter.KeyValue)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, Global.Parameter.KeyValue));
                return;
            }

            var res = await player.GiveItem(new CharacterItem(ItemCategory.PropertyKey, prop.LockNumber));
            if (!string.IsNullOrWhiteSpace(res))
            {
                player.SendMessage(MessageType.Error, res);
                return;
            }

            await player.RemoveStackedItem(ItemCategory.Money, Global.Parameter.KeyValue);

            player.SendMessage(MessageType.Success, $"Você criou uma cópia da chave para a propriedade {prop.Id} por ${Global.Parameter.KeyValue:N0}.");
        }

        [Command("pupgrade")]
        public static void CMD_pupgrade(MyPlayer player)
        {
            var prop = Global.Properties
                .Where(x => x.CharacterId == player.Character.Id
                    && player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)) <= Global.RP_DISTANCE)
                .MinBy(x => player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)));
            if (prop == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de uma propriedade sua.");
                return;
            }

            if (prop.RobberyValue > 0)
            {
                player.SendMessage(MessageType.Error, Global.ROBBED_PROPERTY_ERROR_MESSAGE);
                return;
            }

            var itemsJSON = Functions.Serialize(
                new List<dynamic>
                {
                    new
                    {
                        Name = "Proteção Nível 1",
                        Value = $"${Math.Truncate(prop.Value * 0.05):N0}",
                    },
                    new
                    {
                        Name = "Proteção Nível 2",
                        Value = $"${Math.Truncate(prop.Value * 0.08):N0}",
                    },
                    new
                    {
                        Name = "Proteção Nível 3",
                        Value = $"${Math.Truncate(prop.Value * 0.15):N0}",
                    },
                }
            );

            player.Emit("PropertyUpgrade", $"Upgrades • {prop.Address} [{prop.Id}]", prop.Id, itemsJSON);
        }

        [Command("arrombar")]
        public static async Task CMD_arrombar(MyPlayer player)
        {
            if (player.Faction?.Type != FactionType.Police || !player.OnDuty)
            {
                if (player.Character.ConnectedTime < Global.Parameter.PropertyRobberyConnectedTime)
                {
                    player.SendMessage(MessageType.Error, $"É necessário estar conectado por {Global.Parameter.PropertyRobberyConnectedTime} minutos.");
                    return;
                }
            }

            if (player.IsInVehicle)
            {
                player.SendMessage(MessageType.Error, "Você deve estar fora do veículo.");
                return;
            }

            if (player.CurrentWeapon != (uint)WeaponModel.Crowbar)
            {
                player.SendMessage(MessageType.Error, "Você não está com um pé de cabra em mãos.");
                return;
            }

            var prop = Global.Properties.Where(x =>
                player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)) <= 5
                && x.EntranceDimension == player.Dimension
                && x.Locked
                && x.CharacterId.HasValue)
                .MinBy(x => player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)));

            if (prop == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhuma propriedade trancada.");
                return;
            }

            if (prop.RobberyValue > 0)
            {
                player.SendMessage(MessageType.Error, Global.ROBBED_PROPERTY_ERROR_MESSAGE);
                return;
            }

            await prop.ActivateProtection();

            player.ToggleGameControls(false);
            player.SendMessage(MessageType.Success, $"Aguarde 200 segundos. Pressione DELETE para cancelar a ação.");
            player.CancellationTokenSourceAcao?.Cancel();
            player.CancellationTokenSourceAcao = new System.Threading.CancellationTokenSource();
            await Task.Delay(200000, player.CancellationTokenSourceAcao.Token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;

                Task.Run(async () =>
                {
                    prop.Locked = false;
                    await using var context = new DatabaseContext();
                    context.Properties.Update(prop);
                    await context.SaveChangesAsync();

                    player.ToggleGameControls(true);
                    player.SendMessageToNearbyPlayers("arromba a porta.", MessageCategory.Ame, 5);
                    await player.GravarLog(LogType.BreakIn, prop.Id.ToString(), null);
                    player.CancellationTokenSourceAcao = null;
                });
            });
        }

        [Command("roubarpropriedade")]
        public static async Task CMD_roubarpropriedade(MyPlayer player)
        {
            if ((player.User.PropertyRobberyCooldown ?? DateTime.MinValue) > DateTime.Now)
            {
                player.SendMessage(MessageType.Error, $"Aguarde o cooldown para roubar novamente. Será liberado em {player.User.PropertyRobberyCooldown}.");
                return;
            }

            var prop = Global.Properties.FirstOrDefault(x => x.Id == player.Dimension
                && !x.Locked
                && x.CharacterId != player.Character.Id
                && x.CharacterId.HasValue);

            if (prop == null)
            {
                player.SendMessage(MessageType.Error, "Você não está no interior de uma propriedade destrancada que não seja sua e que possua dono.");
                return;
            }

            if (prop.RobberyValue > 0)
            {
                player.SendMessage(MessageType.Error, Global.ROBBED_PROPERTY_ERROR_MESSAGE);
                return;
            }

            if ((prop.RobberyCooldown ?? DateTime.MinValue) > DateTime.Now)
            {
                player.SendMessage(MessageType.Error, $"Aguarde o cooldown da propriedade para roubar novamente. Será liberado em {prop.RobberyCooldown}.");
                return;
            }

            if (Global.SpawnedPlayers.Count(x => x.Faction?.Type == FactionType.Police && x.OnDuty) < Global.Parameter.PoliceOfficersPropertyRobbery)
            {
                player.SendMessage(MessageType.Error, $"É necessário {Global.Parameter.PoliceOfficersPropertyRobbery} policiais em serviço.");
                return;
            }

            await prop.ActivateProtection();

            var waitSeconds = prop.ProtectionLevel switch
            {
                1 => 400,
                2 => 600,
                3 => 1200,
                _ => 300,
            };

            player.ToggleGameControls(false);
            player.SendMessage(MessageType.Success, $"Aguarde {waitSeconds} segundos. Pressione DELETE para cancelar a ação.");
            player.CancellationTokenSourceAcao?.Cancel();
            player.CancellationTokenSourceAcao = new System.Threading.CancellationTokenSource();
            await Task.Delay(waitSeconds * 1000, player.CancellationTokenSourceAcao.Token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;

                Task.Run(async () =>
                {
                    var value = Convert.ToInt32(Math.Truncate(prop.Value * 0.1));
                    var res = await player.GiveItem(new CharacterItem(ItemCategory.Money) { Quantity = value });
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.SendMessage(MessageType.Error, res);
                        return;
                    }

                    prop.RobberyValue = value;
                    prop.RobberyCooldown = DateTime.Now.AddHours(Global.Parameter.CooldownPropertyRobberyPropertyHours);
                    await using var context = new DatabaseContext();
                    context.Properties.Update(prop);
                    await context.SaveChangesAsync();
                    player.User.PropertyRobberyCooldown = DateTime.Now.AddHours(Global.Parameter.CooldownPropertyRobberyRobberHours);

                    player.ToggleGameControls(true);
                    player.SendMessageToNearbyPlayers("rouba a propriedade.", MessageCategory.Ame, 5);
                    player.SendMessage(MessageType.Success, $"Você roubou a propriedade e recebeu ${value:N0}.");
                    await player.GravarLog(LogType.StealProperty, $"{prop.Id} {value}", null);
                    player.CancellationTokenSourceAcao = null;
                });
            });
        }

        [Command("roubararmazenamento")]
        public static async Task CMD_roubararmazenamento(MyPlayer player)
        {
            if ((player.User.PropertyRobberyCooldown ?? DateTime.MinValue) > DateTime.Now)
            {
                player.SendMessage(MessageType.Error, $"Aguarde o cooldown para roubar novamente. Será liberado em {player.User.PropertyRobberyCooldown}.");
                return;
            }

            var prop = Global.Properties.FirstOrDefault(x => x.Id == player.Dimension
                && !x.Locked
                && x.CharacterId != player.Character.Id
                && x.CharacterId.HasValue);

            if (prop == null)
            {
                player.SendMessage(MessageType.Error, "Você não está no interior de uma propriedade destrancada que não seja sua e que possua dono.");
                return;
            }

            if (prop.RobberyValue > 0)
            {
                player.SendMessage(MessageType.Error, Global.ROBBED_PROPERTY_ERROR_MESSAGE);
                return;
            }

            if ((prop.RobberyCooldown ?? DateTime.MinValue) > DateTime.Now)
            {
                player.SendMessage(MessageType.Error, $"Aguarde o cooldown da propriedade para roubar novamente. Será liberado em {prop.RobberyCooldown}.");
                return;
            }

            if (Global.SpawnedPlayers.Count(x => x.Faction?.Type == FactionType.Police && x.OnDuty) < Global.Parameter.PoliceOfficersPropertyRobbery)
            {
                player.SendMessage(MessageType.Error, $"É necessário {Global.Parameter.PoliceOfficersPropertyRobbery} policiais em serviço.");
                return;
            }

            await prop.ActivateProtection();

            var waitSeconds = prop.ProtectionLevel switch
            {
                1 => 400,
                2 => 600,
                3 => 1200,
                _ => 300,
            };

            player.ToggleGameControls(false);
            player.SendMessage(MessageType.Success, $"Aguarde {waitSeconds} segundos. Pressione DELETE para cancelar a ação.");
            player.CancellationTokenSourceAcao?.Cancel();
            player.CancellationTokenSourceAcao = new CancellationTokenSource();
            await Task.Delay(waitSeconds * 1000, player.CancellationTokenSourceAcao.Token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;

                Task.Run(async () =>
                {
                    prop.RobberyCooldown = DateTime.Now.AddHours(Global.Parameter.CooldownPropertyRobberyPropertyHours);
                    await using var context = new DatabaseContext();
                    context.Properties.Update(prop);
                    await context.SaveChangesAsync();

                    player.User.PropertyRobberyCooldown = DateTime.Now.AddHours(Global.Parameter.CooldownPropertyRobberyRobberHours);

                    player.ToggleGameControls(true);
                    player.SendMessageToNearbyPlayers("encontra o armazenamento da propriedade.", MessageCategory.Ame, 5);
                    await player.GravarLog(LogType.StealStorage, prop.Id.ToString(), null);
                    player.CancellationTokenSourceAcao = null;
                    prop.ShowInventory(player, false);
                });
            });
        }

        [Command("pliberar")]
        public static async Task CMD_pliberar(MyPlayer player)
        {
            var prop = Global.Properties
                .Where(x => x.CharacterId == player.Character.Id
                    && x.RobberyValue > 0
                    && player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)) <= Global.RP_DISTANCE)
                .MinBy(x => player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)));
            if (prop == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de uma propriedade sua que foi roubada.");
                return;
            }

            if (player.Money < prop.RobberyValue)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, prop.RobberyValue));
                return;
            }

            await player.RemoveStackedItem(ItemCategory.Money, prop.RobberyValue);

            player.SendMessage(MessageType.Success, $"Você liberou sua propriedade por ${prop.RobberyValue:N0}.");
            prop.RobberyValue = 0;
            await using var context = new DatabaseContext();
            context.Properties.Update(prop);
            await context.SaveChangesAsync();
        }
    }
}