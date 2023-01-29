using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
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

namespace Roleplay.Scripts
{
    public class VehicleScript : IScript
    {
        [AsyncScriptEvent(ScriptEventType.PlayerLeaveVehicle)]
        public static async Task OnPlayerLeaveVehicle(MyVehicle vehicle, MyPlayer player, byte seat)
        {
            if (player.VehicleAnimation)
                player.StopAnimation();

            if (vehicle.Model == (uint)VehicleModel.Thruster)
            {
                await Task.Delay(2000);
                vehicle.Remove();
            }

            if (vehicle.Vehicle.Id > 0)
            {
                player.Emit("Spotlight:Toggle", false);

                if (vehicle.Vehicle.Model.ToUpper() == VehicleModelMods.LSPDHELI.ToString().ToUpper()
                    || vehicle.Vehicle.Model.ToUpper() == VehicleModel.Polmav.ToString().ToUpper())
                {
                    player.Emit("Helicam:Toggle", true);
                    var spotlight = Global.Spotlights.FirstOrDefault(x => x.Id == vehicle.Id && x.Player == player.SessionId);
                    if (spotlight != null)
                    {
                        Global.Spotlights.Remove(spotlight);
                        Alt.EmitAllClients("Spotlight:Remove", spotlight.Id);
                    }
                }

                if (seat == 1 && vehicle.Vehicle.Job != CharacterJob.None && !vehicle.DataExpiracaoAluguel.HasValue)
                {
                    await Task.Delay(2000);
                    await vehicle.Estacionar(player);
                }
            }
        }

        [ScriptEvent(ScriptEventType.PlayerEnterVehicle)]
        public static void OnPlayerEnterVehicle(MyVehicle vehicle, MyPlayer player, byte seat)
        {
            if (vehicle.Vehicle.Id == 0)
                return;

            if (seat == 1)
                player.Emit("Spotlight:Toggle", vehicle.SpotlightActive);

            player.SetCanDoDriveBy(seat);
            vehicle.SetStreamSyncedMetaData(Constants.VEHICLE_META_DATA_RADIO_ENABLED, !vehicle.Vehicle.FactionId.HasValue);

            if (vehicle.EngineOn)
            {
                if (vehicle.Vehicle.Fuel == 0)
                {
                    vehicle.EngineOn = false;
                }
                else if (vehicle.Vehicle.Job != CharacterJob.None && !vehicle.DataExpiracaoAluguel.HasValue)
                {
                    vehicle.EngineOn = false;
                    if (player.Seat == 1)
                        player.SendMessage(MessageType.Error, "O aluguel do veículo expirou. Use /valugar para alugar novamente por uma hora. Se você sair do veículo, ele será levado para a central.");
                }
            }
            else
            {
                if (vehicle.Info.Type == VehicleModelType.BMX)
                    vehicle.EngineOn = true;
            }

            if (vehicle.Vehicle.Job != CharacterJob.None && vehicle.NomeEncarregado == player.Character.Name && vehicle.DataExpiracaoAluguel.HasValue)
                player.SendMessage(MessageType.Error, $"O aluguel do veículo irá expirar em {vehicle.DataExpiracaoAluguel}.");
        }

        [ScriptEvent(ScriptEventType.VehicleDamage)]
        public static void OnVehicleDamage(MyVehicle veh, IEntity attacker, uint bodyHealthDamage, uint additionalBodyHealthDamage, uint engineHealthDamage, uint petrolTankDamage, uint weaponHash)
        {
            var dano = new MyVehicle.Damage()
            {
                BodyHealthDamage = bodyHealthDamage,
                AdditionalBodyHealthDamage = additionalBodyHealthDamage,
                EngineHealthDamage = engineHealthDamage,
                PetrolTankDamage = petrolTankDamage,
                WeaponHash = weaponHash,
            };

            MyPlayer playerAttacker = null;
            if (attacker is not MyPlayer player)
            {
                if (attacker is IVehicle vehicleAttacker)
                    playerAttacker = (MyPlayer)vehicleAttacker.Driver;
            }
            else
            {
                playerAttacker = player;
            }

            if (playerAttacker != null)
            {
                dano.Attacker = $"{playerAttacker.Character.Id} - {playerAttacker.Character.Name}";
                dano.Distance = veh.Position.Distance(playerAttacker.Position);
            }

            veh.Damages.Add(dano);
        }

        [AsyncClientEvent(nameof(ComprarVeiculo))]
        public async Task ComprarVeiculo(MyPlayer player, int tipo, string veiculo, byte r1, byte g1, byte b1, byte r2, byte g2, byte b2)
        {
            if (string.IsNullOrWhiteSpace(veiculo))
            {
                player.SendMessage(MessageType.Error, "Verifique se todos os campos foram preenchidos corretamente.", notify: true);
                return;
            }

            var preco = Global.Prices.FirstOrDefault(x => x.Type == (PriceType)tipo && x.Name.ToLower() == veiculo.ToLower());
            if (preco == null)
            {
                player.SendMessage(MessageType.Error, "Veículo não está disponível para compra.", notify: true);
                return;
            }

            var value = Convert.ToInt32(Math.Abs(preco.Value));
            if (player.Money < value)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, value), notify: true);
                return;
            }

            var restricao = Functions.CheckVIPVehicle(veiculo);
            if (restricao.Item2 != UserVIP.None && (restricao.Item2 > player.User.VIP || (player.User.VIPValidDate ?? DateTime.MinValue) < DateTime.Now))
            {
                player.SendMessage(MessageType.Error, $"O veículo é restrito para VIP {restricao.Item2}.", notify: true);
                return;
            }

            var concessionaria = Global.Dealerships.FirstOrDefault(x => x.PriceType == (PriceType)tipo);

            var veh = new Entities.Vehicle()
            {
                CharacterId = player.Character.Id,
                Color1R = r1,
                Color1G = g1,
                Color1B = b1,
                Color2R = r2,
                Color2G = g2,
                Color2B = b2,
                Model = veiculo,
                Plate = await Functions.GenerateVehiclePlate(false),
                PosX = concessionaria.VehiclePosition.X,
                PosY = concessionaria.VehiclePosition.Y,
                PosZ = concessionaria.VehiclePosition.Z,
                RotR = concessionaria.VehicleRotation.X,
                RotP = concessionaria.VehicleRotation.Y,
                RotY = concessionaria.VehicleRotation.Z,
            };

            veh.Fuel = veh.MaxFuel;

            await using var context = new DatabaseContext();
            await context.Vehicles.AddAsync(veh);
            await context.SaveChangesAsync();
            await player.RemoveItem(new CharacterItem(ItemCategory.Money)
            {
                Quantity = value
            });

            player.SendMessage(MessageType.Success, $"Você comprou {veh.Model.ToUpper()} por ${preco.Value:N0}. Use /vlista para spawnar.");
            player.Emit("Server:CloseView");
        }

        [ClientEvent(nameof(SetVehicleMeta))]
        public void SetVehicleMeta(MyPlayer player, IVehicle vehicle, string meta, object value) => vehicle.SetStreamSyncedMetaData(meta, value);

        [AsyncClientEvent(nameof(SpawnarVeiculoFaccao))]
        public async Task SpawnarVeiculoFaccao(MyPlayer player, int spotId, int vehicleId)
        {
            try
            {
                if (Global.Vehicles.Any(x => x.Vehicle.Id == vehicleId))
                {
                    player.SendMessage(MessageType.Error, "Veículo já está spawnado.", notify: true);
                    return;
                }

                await using var context = new DatabaseContext();
                var veh = await context.Vehicles.FirstOrDefaultAsync(x => x.Id == vehicleId);
                veh.PosX = player.Position.X;
                veh.PosY = player.Position.Y;
                veh.PosZ = player.Position.Z;

                var spot = Global.Spots.FirstOrDefault(x => x.Id == spotId);
                if (spot != null)
                {
                    veh.RotR = spot.AuxiliarPosX;
                    veh.RotP = spot.AuxiliarPosY;
                    veh.RotY = spot.AuxiliarPosZ;
                }

                var vehicle = await veh.Spawnar(player);
                vehicle.NomeEncarregado = player.Character.Name;
                vehicle.LockState = VehicleLockState.Unlocked;
                player.SetIntoVehicle(vehicle, 1);
                player.Emit("Server:CloseView");
                player.SendMessage(MessageType.Success, $"Você spawnou o veículo {vehicleId}.", notify: true);
            }
            catch (Exception ex)
            {
                ex.HelpLink = $"Vehicle Id: {vehicleId}";
                Functions.GetException(ex);
            }
        }

        [AsyncClientEvent(nameof(AbastecerVeiculo))]
        public async Task AbastecerVeiculo(MyPlayer player, int veiculo)
        {
            var veh = Global.Vehicles.FirstOrDefault(x => x.Vehicle.Id == veiculo);

            var combustivelNecessario = veh.Vehicle.MaxFuel - veh.Vehicle.Fuel;
            var valor = combustivelNecessario * Global.Parameter.FuelValue;
            if (valor > player.Money && !veh.Vehicle.FactionId.HasValue)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, valor));
                return;
            }

            var segundos = Convert.ToInt32(Math.Ceiling(combustivelNecessario / 10f));
            if (segundos == 0)
                segundos = 1;

            player.ToggleGameControls(false);
            player.SendMessage(MessageType.Success, $"Aguarde {segundos} segundo{(segundos != 1 ? "s" : string.Empty)}. Pressione DELETE para cancelar a ação.");
            player.CancellationTokenSourceAcao?.Cancel();
            player.CancellationTokenSourceAcao = new System.Threading.CancellationTokenSource();
            await Task.Delay(segundos * 1000, player.CancellationTokenSourceAcao.Token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;

                Task.Run(async () =>
                {
                    veh.Vehicle.Fuel = veh.Vehicle.MaxFuel;
                    veh.SetStreamSyncedMetaData(Constants.VEHICLE_META_DATA_FUEL, veh.CombustivelHUD);
                    if (!veh.Vehicle.FactionId.HasValue)
                    {
                        await player.RemoveItem(new CharacterItem(ItemCategory.Money)
                        {
                            Quantity = valor
                        });
                        player.SendMessage(MessageType.Success, $"Você abasteceu seu veículo com {combustivelNecessario} litro{(combustivelNecessario > 1 ? "s" : string.Empty)} de combustível por ${valor:N0}.");
                    }
                    else
                    {
                        player.SendMessage(MessageType.Success, $"Você abasteceu seu veículo com {combustivelNecessario} litro{(combustivelNecessario > 1 ? "s" : string.Empty)} de combustível e a conta foi paga pelo estado.");
                    }
                    player.SendMessageToNearbyPlayers("abastece o veículo.", MessageCategory.Ame, 10);
                    player.ToggleGameControls(true);
                    player.CancellationTokenSourceAcao = null;
                });
            });
        }

        [AsyncClientEvent(nameof(SpawnarVeiculo))]
        public async Task SpawnarVeiculo(MyPlayer player, int vehicleId)
        {
            try
            {
                if (Global.Vehicles.Any(x => x.Vehicle.Id == vehicleId))
                {
                    player.SendMessage(MessageType.Error, "Veículo já está spawnado.", notify: true);
                    return;
                }

                await using var context = new DatabaseContext();
                var veh = await context.Vehicles.FirstOrDefaultAsync(x => x.Id == vehicleId);
                if (veh.SeizedValue > 0)
                {
                    player.SendMessage(MessageType.Error, "Veículo está apreendido. Vá até o pátio de apreensão para realizar a liberação.", notify: true);
                    return;
                }

                if (veh.DismantledValue > 0)
                {
                    player.SendMessage(MessageType.Error, "Veículo está na seguradora. Vá até o pátio de apreensão para realizar a liberação.", notify: true);
                    return;
                }

                await veh.Spawnar(player);
                player.Emit("Server:SetWaypoint", veh.PosX, veh.PosY);
                player.SendMessage(MessageType.Success, $"Você spawnou o veículo {vehicleId}.", notify: true);
                player.Emit("Server:CloseView");
            }
            catch (Exception ex)
            {
                ex.HelpLink = $"Vehicle Id: {vehicleId}";
                Functions.GetException(ex);
            }
        }

        [AsyncClientEvent(nameof(VenderVeiculo))]
        public async Task VenderVeiculo(MyPlayer player) => await Functions.CMDVenderVeiculoConcessionaria(player, player.TargetConfirmation.FirstOrDefault(), true);

        [AsyncClientEvent(nameof(Trancar))]
        public async Task Trancar(MyPlayer player) => await Functions.CMDTrancar(player);

        [ClientEvent(nameof(Motor))]
        public static void Motor(MyPlayer player) => Functions.CMDMotor(player);

        [AsyncScriptEvent(ScriptEventType.VehicleDestroy)]
        public static async Task OnVehicleDestroy(MyVehicle vehicle)
        {
            if (vehicle.Vehicle.Id == 0)
                return;

            await Functions.WriteLog(LogType.DestruicaoVeiculo, $"{vehicle.Vehicle.Id} | {JsonSerializer.Serialize(vehicle.Damages)}");
        }

        [AsyncClientEvent(nameof(BuyVehicleUpgrade))]
        public static async Task BuyVehicleUpgrade(MyPlayer player, int vehicleId, string name)
        {
            if (Global.Vehicles.Any(x => x.Vehicle.Id == vehicleId))
            {
                player.SendMessage(MessageType.Error, $"Veículo {vehicleId} está spawnado.");
                return;
            }

            await using var context = new DatabaseContext();
            var veh = await context.Vehicles.FirstOrDefaultAsync(x => x.CharacterId == player.Character.Id && x.Id == vehicleId && !x.Sold);
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_OWNER_ERROR_MESSAGE, notify: true);
                return;
            }

            var preco = Global.Prices.FirstOrDefault(x => x.Vehicle && x.Name.ToLower() == veh.Model.ToLower());
            if (preco == null)
            {
                player.SendMessage(MessageType.Error, "Preço não encontrado.", notify: true);
                return;
            }

            var value = Convert.ToInt32(Math.Abs(preco.Value));

            switch (name)
            {
                case "Proteção Nível 1":
                    value = Convert.ToInt32(Math.Truncate(value * 0.05));
                    break;
                case "Proteção Nível 2":
                    value = Convert.ToInt32(Math.Truncate(value * 0.08));
                    break;
                case "Proteção Nível 3":
                    value = Convert.ToInt32(Math.Truncate(value * 0.2));
                    break;
                case "XMR":
                    value = Convert.ToInt32(Math.Truncate(value * 0.01));
                    break;
            }

            if (player.Money < value)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, value), notify: true);
                return;
            }

            switch (name)
            {
                case "Proteção Nível 1":
                    if (veh.ProtectionLevel >= 1)
                    {
                        player.SendMessage(MessageType.Error, $"O veículo já possui um nível de proteção igual ou maior que 1.", notify: true);
                        return;
                    }

                    veh.ProtectionLevel = 1;
                    break;
                case "Proteção Nível 2":
                    if (veh.ProtectionLevel >= 2)
                    {
                        player.SendMessage(MessageType.Error, $"O veículo já possui um nível de proteção igual ou maior que 2.", notify: true);
                        return;
                    }

                    veh.ProtectionLevel = 2;
                    break;
                case "Proteção Nível 3":
                    if (veh.ProtectionLevel >= 3)
                    {
                        player.SendMessage(MessageType.Error, $"O veículo já possui um nível de proteção igual ou maior que 3.", notify: true);
                        return;
                    }

                    veh.ProtectionLevel = 3;
                    break;
                case "XMR":
                    if (veh.XMR)
                    {
                        player.SendMessage(MessageType.Error, $"O veículo já possui XMR.", notify: true);
                        return;
                    }

                    veh.XMR = true;
                    break;
            }

            context.Vehicles.Update(veh);
            await context.SaveChangesAsync();

            await player.RemoveItem(new CharacterItem(ItemCategory.Money)
            {
                Quantity = value
            });
            player.SendMessage(MessageType.Success, $"Você comprou {name}.", notify: true);
        }

        [ClientEvent(nameof(ConfirmXMR))]
        public static void ConfirmXMR(MyPlayer player, int vehicleId, string url, float volume)
        {
            if (!(Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)))
            {
                player.EmitStaffShowMessage("URL está em um formato inválido.");
                return;
            }

            var vehicle = Global.Vehicles.FirstOrDefault(x => x.Vehicle.Id == vehicleId);
            if (vehicle == null)
                return;

            vehicle.AudioSpot ??= new AudioSpot
            {
                MaxRange = 10,
                Dimension = vehicle.Dimension,
                VehicleId = vehicle.Id,
            };

            if (vehicle.AudioSpot.Source != url)
                vehicle.AudioSpot.RemoveAllClients();

            vehicle.AudioSpot.Source = url;
            vehicle.AudioSpot.Volume = volume;

            vehicle.AudioSpot.SetupAllClients();

            player.SendMessageToNearbyPlayers($"configura o XMR.", MessageCategory.Ame, 5);
        }

        [ClientEvent(nameof(TurnOffXMR))]
        public static void TurnOffXMR(MyPlayer player, int vehicleId)
        {
            var vehicle = Global.Vehicles.FirstOrDefault(x => x.Vehicle.Id == vehicleId);
            if (vehicle == null)
                return;

            if (vehicle.AudioSpot != null)
            {
                vehicle.AudioSpot.RemoveAllClients();
                player.SendMessageToNearbyPlayers($"desliga o XMR.", MessageCategory.Ame, 5);
                vehicle.AudioSpot = null;
            }
        }

        [ClientEvent(nameof(TuningSyncMod))]
        public static void TuningSyncMod(MyPlayer player, int mod, int type)
        {
            if (player.IsInVehicle)
                player.Vehicle.SetMod(Convert.ToByte(mod), Convert.ToByte(type));
        }

        [ClientEvent(nameof(TuningSyncColor))]
        public static void TuningSyncColor(MyPlayer player, string color1, string color2)
        {
            if (player.IsInVehicle)
            {
                var drawingColor1 = System.Drawing.ColorTranslator.FromHtml(color1);
                var drawingColor2 = System.Drawing.ColorTranslator.FromHtml(color2);

                player.Vehicle.PrimaryColorRgb = new AltV.Net.Data.Rgba(drawingColor1.R, drawingColor1.G, drawingColor1.B, 255);
                player.Vehicle.SecondaryColorRgb = new AltV.Net.Data.Rgba(drawingColor2.R, drawingColor2.G, drawingColor2.B, 255);
            }
        }

        [ClientEvent(nameof(TuningSyncWheel))]
        public static void TuningSyncWheel(MyPlayer player, int wheelType, int wheelVariation, int wheelColor)
        {
            if (player.IsInVehicle)
            {
                player.Vehicle.SetWheels(Convert.ToByte(wheelType), Convert.ToByte(wheelVariation));
                player.Vehicle.WheelColor = Convert.ToByte(wheelColor);
            }
        }

        [ClientEvent(nameof(TuningSyncNeon))]
        public static void TuningSyncNeon(MyPlayer player, string neonColor, int neonLeft, int neonRight, int neonFront, int neonBack)
        {
            if (player.IsInVehicle)
            {
                var drawingNeonColor = System.Drawing.ColorTranslator.FromHtml(neonColor);
                player.Vehicle.NeonColor = new AltV.Net.Data.Rgba(drawingNeonColor.R, drawingNeonColor.G, drawingNeonColor.B, 255);
                player.Vehicle.SetNeonActive(Convert.ToBoolean(neonLeft), Convert.ToBoolean(neonRight), Convert.ToBoolean(neonFront), Convert.ToBoolean(neonBack));
            }
        }

        [ClientEvent(nameof(TuningSyncXenonColor))]
        public static void TuningSyncXenonColor(MyPlayer player, int headlightColor, int lightsMultiplier)
        {
            if (player.IsInVehicle)
            {
                player.Vehicle.HeadlightColor = Convert.ToByte(headlightColor);
                player.Vehicle.LightsMultiplier = Convert.ToByte(lightsMultiplier);
            }
        }

        [ClientEvent(nameof(TuningSyncWindowTint))]
        public static void TuningSyncWindowTint(MyPlayer player, int windowTint)
        {
            if (player.IsInVehicle)
                player.Vehicle.WindowTint = Convert.ToByte(windowTint);
        }

        [ClientEvent(nameof(TuningSyncTireSmokeColor))]
        public static void TuningSyncTireSmokeColor(MyPlayer player, string tireSmokeColor)
        {
            if (player.IsInVehicle)
            {
                var drawingTireSmokeColor = System.Drawing.ColorTranslator.FromHtml(tireSmokeColor);
                player.Vehicle.TireSmokeColor = new AltV.Net.Data.Rgba(drawingTireSmokeColor.R, drawingTireSmokeColor.G, drawingTireSmokeColor.B, 255);
            }
        }

        [AsyncClientEvent(nameof(ConfirmTuning))]
        public async Task ConfirmTuning(MyPlayer player, bool confirm, string vehicleTuningJSON)
        {
            if (player.Vehicle is not MyVehicle veh || veh == null || veh.Driver != player)
            {
                player.Emit("Tuning:ShowMessage", "Você não está dirigindo um veículo.");
                return;
            }

            var vehicleTuning = JsonSerializer.Deserialize<VehicleTuning>(vehicleTuningJSON);

            async Task SetMods()
            {
                var realMods = JsonSerializer.Deserialize<List<VehicleMod>>(veh.Vehicle.ModsJSON);
                foreach (var mod in vehicleTuning.Mods.Where(x => x.Type < 249))
                {
                    var realMod = realMods.FirstOrDefault(x => x.Type == mod.Type);
                    if (realMod == null)
                    {
                        realMods.Add(new VehicleMod
                        {
                            Type = mod.Type,
                            Id = mod.Selected,
                        });
                    }
                    else
                    {
                        realMod.Id = mod.Selected;
                        if (realMod.Id == 0)
                            realMods.Remove(realMod);
                    }
                }

                var drawingColor1 = System.Drawing.ColorTranslator.FromHtml(vehicleTuning.Color1);
                veh.Vehicle.Color1R = drawingColor1.R;
                veh.Vehicle.Color1G = drawingColor1.G;
                veh.Vehicle.Color1B = drawingColor1.B;

                var drawingColor2 = System.Drawing.ColorTranslator.FromHtml(vehicleTuning.Color2);
                veh.Vehicle.Color2R = drawingColor2.R;
                veh.Vehicle.Color2G = drawingColor2.G;
                veh.Vehicle.Color2B = drawingColor2.B;

                veh.Vehicle.WheelType = vehicleTuning.WheelType;
                veh.Vehicle.WheelVariation = vehicleTuning.WheelVariation;
                veh.Vehicle.WheelColor = vehicleTuning.WheelColor;

                var drawingNeonColor = System.Drawing.ColorTranslator.FromHtml(vehicleTuning.NeonColor);
                veh.Vehicle.NeonColorR = drawingNeonColor.R;
                veh.Vehicle.NeonColorG = drawingNeonColor.G;
                veh.Vehicle.NeonColorB = drawingNeonColor.B;
                veh.Vehicle.NeonLeft = Convert.ToBoolean(vehicleTuning.NeonLeft);
                veh.Vehicle.NeonRight = Convert.ToBoolean(vehicleTuning.NeonRight);
                veh.Vehicle.NeonFront = Convert.ToBoolean(vehicleTuning.NeonFront);
                veh.Vehicle.NeonBack = Convert.ToBoolean(vehicleTuning.NeonBack);

                veh.Vehicle.HeadlightColor = vehicleTuning.HeadlightColor;
                veh.Vehicle.LightsMultiplier = vehicleTuning.LightsMultiplier;

                veh.Vehicle.WindowTint = vehicleTuning.WindowTint;

                var drawingTireSmokeColor = System.Drawing.ColorTranslator.FromHtml(vehicleTuning.TireSmokeColor);
                veh.Vehicle.TireSmokeColorR = drawingTireSmokeColor.R;
                veh.Vehicle.TireSmokeColorG = drawingTireSmokeColor.G;
                veh.Vehicle.TireSmokeColorB = drawingTireSmokeColor.B;

                veh.Vehicle.ModsJSON = JsonSerializer.Serialize(realMods);

                await using var context = new DatabaseContext();
                context.Vehicles.Update(veh.Vehicle);
                await context.SaveChangesAsync();

                if (vehicleTuning.Repair == 1)
                    veh = await veh.Reparar();
            }

            if (confirm)
            {
                if (vehicleTuning.Staff)
                {
                    await SetMods();
                    await player.GravarLog(LogType.Staff, $"Tunar Veículo | {veh.Vehicle.Id} | {vehicleTuningJSON}", null);
                    player.SendMessage(MessageType.Success, $"Você aplicou as modificações no veículo {veh.Vehicle.Id}.");
                }
                else
                {
                    if (vehicleTuning.TargetId.HasValue)
                    {
                        var target = Global.Players.FirstOrDefault(x => x.Character.Id == vehicleTuning.TargetId);
                        if (target == null)
                            player.SendMessage(MessageType.Error, $"O jogador não está mais conectado.");

                        vehicleTuning.VehicleId = veh.Vehicle.Id;
                        vehicleTuning.TargetId = null;
                        vehicleTuning.Mods = new List<VehicleTuning.Mod>();

                        var invite = new Invite()
                        {
                            Type = InviteType.Mechanic,
                            SenderCharacterId = player.Character.Id,
                            Value = new string[] { JsonSerializer.Serialize(vehicleTuning) },
                        };
                        target.Invites.RemoveAll(x => x.Type == InviteType.Mechanic);
                        target.Invites.Add(invite);

                        player.SendMessage(MessageType.Success, $"Você solicitou enviar o catálogo de modificações veiculares para {target.ICName}.");
                        target.SendMessage(MessageType.Success, $"{player.ICName} solicitou enviar o catálogo de modificações veiculares para você. (/ac {(int)invite.Type} para aceitar ou /rc {(int)invite.Type} para recusar)");
                    }
                    else
                    {
                        var totalValue = vehicleTuning.Mods.Sum(x => x.Value);
                        if (player.Money < totalValue)
                        {
                            player.Emit("Tuning:ShowMessage", string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, totalValue));
                            return;
                        }

                        await SetMods();
                        await player.RemoveItem(new CharacterItem(ItemCategory.Money) { Quantity = totalValue });
                        player.SendMessage(MessageType.Success, $"Você aplicou as modificações no veículo {veh.Vehicle.Id} por ${totalValue:N0}.");
                        await player.GravarLog(LogType.TunarVeiculoMecanico, $"{veh.Vehicle.Id} | {vehicleTuningJSON}", null);
                    }
                }
            }

            veh.ClearMods();
            veh.SetDefaultMods();

            player.Emit("VehicleTuning:Close");
        }
    }
}