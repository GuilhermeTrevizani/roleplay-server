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
using System.Numerics;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace Roleplay.Scripts
{
    public class PlayerScript : IScript
    {
        [ScriptEvent(ScriptEventType.PlayerConnect)]
        public static void OnPlayerConnect(MyPlayer player, string reason)
        {
            try
            {
                player.SessionId = Convert.ToInt16(Enumerable.Range(0, 1000).FirstOrDefault(i => !Global.Players.Any(x => x.SessionId == i)));
                player.Dimension = player.SessionId;
                Alt.Log($"OnPlayerConnect | {player.Id} | {player.Name} | {player.Ip}");
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        [AsyncScriptEvent(ScriptEventType.PlayerDisconnect)]
        public static async Task OnPlayerDisconnect(MyPlayer player, string reason)
        {
            Alt.Log($"OnPlayerDisconnect | {player.Id} | {player.Name} | {player.Ip} | {reason}");
            await player.Disconnect(reason, true);
        }

        [AsyncScriptEvent(ScriptEventType.PlayerDead)]
        public static async Task OnPlayerDead(MyPlayer player, IEntity killer, uint weapon)
        {
            try
            {
                if (player.Character.Id == 0)
                    return;

                await player.GravarLog(LogType.Morte, JsonSerializer.Serialize(player.Ferimentos), killer is MyPlayer playerKiller ? playerKiller : null);
                player.SetarFerido();
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        [ScriptEvent(ScriptEventType.WeaponDamage)]
        public static bool OnWeaponDamage(MyPlayer player, IEntity target, uint weapon, ushort damage, Position shotOffset, BodyPart bodyPart)
        {
            var hasDamage = true;
            try
            {
                if (weapon == 0 || target is not MyPlayer playerTarget || playerTarget.Character.Id == 0)
                    return false;

                if (playerTarget.CancellationTokenSourceAcao != null)
                {
                    playerTarget.SendMessage(MessageType.Error, "Você sofreu um dano e sua ação foi cancelada.");
                    playerTarget.CancellationTokenSourceAcao?.Cancel();
                    playerTarget.CancellationTokenSourceAcao = null;
                    playerTarget.ToggleGameControls(true);
                }

                if (playerTarget.Character.Wound != CharacterWound.Nenhum)
                {
                    if (playerTarget.Character.Wound == CharacterWound.GravementeFerido)
                    {
                        playerTarget.SendMessage(MessageType.Error, Global.MENSAGEM_PK);
                        playerTarget.Character.Wound = CharacterWound.PK;
                        playerTarget.SetStreamSyncedMetaData(Constants.PLAYER_META_DATA_INJURED, (int)playerTarget.Character.Wound);
                    }

                    hasDamage = false;
                }

                playerTarget.Ferimentos.Add(new MyPlayer.Ferimento
                {
                    Arma = weapon,
                    Dano = damage,
                    BodyPart = bodyPart,
                    Attacker = $"{player.Character.Id} - {player.Character.Name}",
                    Distancia = playerTarget.Position.Distance(player.Position),
                    ShotOffset = shotOffset,
                });
                playerTarget.SetNametagDamaged();

                if (hasDamage && playerTarget.Armor > 0)
                {
                    if (bodyPart != BodyPart.LowerTorso && bodyPart != BodyPart.UpperTorso && bodyPart != BodyPart.Chest)
                    {
                        playerTarget.Health -= damage;
                        hasDamage = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
            return hasDamage;
        }

        [ScriptEvent(ScriptEventType.PlayerDamage)]
        public static void OnPlayerDamage(MyPlayer player, IEntity attacker, uint weapon, ushort healthDamage, ushort armorDamage)
        {
            try
            {
                if (weapon == 0 || Enum.IsDefined(typeof(WeaponModel), weapon) || player.Character.Id == 0)
                    return;

                if (player.CancellationTokenSourceAcao != null)
                {
                    player.SendMessage(MessageType.Error, "Você sofreu um dano e sua ação foi cancelada.");
                    player.CancellationTokenSourceAcao?.Cancel();
                    player.CancellationTokenSourceAcao = null;
                    player.ToggleGameControls(true);
                }

                if (player.Character.Wound == CharacterWound.GravementeFerido)
                {
                    player.SendMessage(MessageType.Error, Global.MENSAGEM_PK);
                    player.Character.Wound = CharacterWound.PK;
                    player.SetStreamSyncedMetaData(Constants.PLAYER_META_DATA_INJURED, (int)player.Character.Wound);
                }

                var ferimento = new MyPlayer.Ferimento
                {
                    Arma = weapon,
                    Dano = armorDamage > 0 ? armorDamage : healthDamage,
                };

                MyPlayer pAttacker = null;
                if (attacker is MyPlayer playerAttacker)
                    pAttacker = playerAttacker;
                else if (attacker is IVehicle vehicleAttacker)
                    pAttacker = (MyPlayer)vehicleAttacker.Driver;

                if (pAttacker != null)
                {
                    ferimento.Attacker = $"{pAttacker.Character.Id} - {pAttacker.Character.Name}";
                    ferimento.Distancia = pAttacker.Position.Distance(player.Position);
                }

                player.Ferimentos.Add(ferimento);
                player.SetNametagDamaged();
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        [ClientEvent(nameof(OnPlayerChat))]
        public void OnPlayerChat(MyPlayer player, string message)
        {
            try
            {
                if (message.Contains("<script>") || message.Contains("{#"))
                    return;

                if (message[0] != '/')
                {
                    if (string.IsNullOrWhiteSpace(message))
                        return;

                    if (!player.OnAdminDuty)
                        message = Functions.CheckFinalDot(message);

                    if (player.Character.Wound != CharacterWound.Nenhum)
                    {
                        player.SendMessage(MessageType.Error, Global.MENSAGEM_GRAVEMENTE_FERIDO);
                        return;
                    }

                    if (player.CellphoneCall.Tipo == CellphoneCallType.Atendida)
                    {
                        player.SendMessageToNearbyPlayers(message, MessageCategory.Celular, player.Dimension > 0 ? 7.5f : 10.0f, false);

                        if (player.CellphoneCall.Numero == Global.EMERGENCY_NUMBER)
                        {
                            if (!player.EmergencyCallType.HasValue)
                            {
                                if (message.ToUpper().Contains("AMBOS"))
                                    player.EmergencyCallType = EmergencyCallType.Both;
                                else if (message.ToUpper().Contains("POLÍCIA") || message.ToUpper().Contains("POLICIA"))
                                    player.EmergencyCallType = EmergencyCallType.Police;
                                else if (message.ToUpper().Contains("BOMBEIRO"))
                                    player.EmergencyCallType = EmergencyCallType.Medic;

                                if (!player.EmergencyCallType.HasValue)
                                {
                                    player.SendMessage(MessageType.None, $"[CELULAR] {player.ObterNomeContato(Global.EMERGENCY_NUMBER)} diz: Não entendi sua mensagem. Deseja falar com polícia, bombeiros ou ambos?", Global.CELLPHONE_MAIN_COLOR);
                                    return;
                                }

                                player.SendMessage(MessageType.None, $"[CELULAR] {player.ObterNomeContato(Global.EMERGENCY_NUMBER)} diz: Qual sua emergência?", Global.CELLPHONE_MAIN_COLOR);
                            }
                            else
                            {
                                player.SendMessage(MessageType.None, $"[CELULAR] {player.ObterNomeContato(Global.EMERGENCY_NUMBER)} diz: Nossas unidades foram alertadas.", Global.CELLPHONE_MAIN_COLOR);

                                var emergencyCall = new EmergencyCall
                                {
                                    Type = player.EmergencyCallType.Value,
                                    Number = player.Cellphone,
                                    PosX = player.ICPosition.X,
                                    PosY = player.ICPosition.Y,
                                    Message = message,
                                };

                                player.AreaNameType = 2;
                                player.AreaNameJSON = JsonSerializer.Serialize(emergencyCall);
                                player.Emit("SetAreaName");
                            }
                        }
                        else if (player.CellphoneCall.Numero == Global.TAXI_NUMBER)
                        {
                            player.SendMessage(MessageType.None, $"[CELULAR] {player.ObterNomeContato(Global.TAXI_NUMBER)} diz: Nossos taxistas em serviço foram avisados e você receberá um SMS de confirmação.", Global.CELLPHONE_MAIN_COLOR);
                            player.AguardandoTipoServico = CharacterJob.TaxiDriver;
                            player.AreaNameType = 3;
                            player.AreaNameJSON = message;
                            player.Emit("SetAreaName");
                        }
                        else if (player.CellphoneCall.Numero == Global.MECHANIC_NUMBER)
                        {
                            player.SendMessage(MessageType.None, $"[CELULAR] {player.ObterNomeContato(Global.MECHANIC_NUMBER)} diz: Nossos mecânicos em serviço foram avisados e você receberá um SMS de confirmação.", Global.CELLPHONE_MAIN_COLOR);
                            player.AguardandoTipoServico = CharacterJob.Mechanic;
                            player.AreaNameType = 4;
                            player.AreaNameJSON = message;
                            player.Emit("SetAreaName");
                        }
                        else
                        {
                            var target = Global.Players.FirstOrDefault(x => x.CellphoneCall.Numero == player.Cellphone);
                            target?.SendMessage(MessageType.None, $"[CELULAR] {target.ObterNomeContato(player.Cellphone)} diz: {message}", Global.CELLPHONE_MAIN_COLOR);
                        }

                        return;
                    }

                    if (player.IsInVehicle && player.Seat <= 4 && !player.OnAdminDuty)
                    {
                        var veh = (MyVehicle)player.Vehicle;
                        if (veh.TemJanelas)
                        {
                            if (!veh.IsWindowOpened(0) && !veh.IsWindowOpened(1) && !veh.IsWindowOpened(2) && !veh.IsWindowOpened(3))
                            {
                                foreach (var target in Global.Players.Where(x => x.Vehicle == veh))
                                    target.SendMessage(MessageType.None, $"{player.ICName} diz [veículo]: {message}");
                                return;
                            }
                        }
                    }

                    player.SendMessageToNearbyPlayers(message,
                    player.OnAdminDuty ? MessageCategory.ChatOOC : MessageCategory.ChatICNormal,
                    player.Dimension > 0 ? 7.5f : 10.0f);

                    return;
                }

                var split = message.Split(" ");
                var cmd = split[0].Replace("/", string.Empty).Trim().ToLower();
                var method = Global.Commands.FirstOrDefault(x => x.GetCustomAttribute<CommandAttribute>().Command.ToLower() == cmd
                || (x.GetCustomAttribute<CommandAttribute>().Aliases?.Any(y => y.ToLower() == cmd) ?? false));
                if (method == null)
                {
                    player.SendMessage(MessageType.None, $"O comando {{{Global.MAIN_COLOR}}}{message}{{#FFFFFF}} não existe. Pressione {{{Global.MAIN_COLOR}}}F1{{#FFFFFF}} para visualizar os comandos disponíveis.");
                    return;
                }

                var methodParams = method.GetParameters();
                var obj = Activator.CreateInstance(method.DeclaringType);
                var command = method.GetCustomAttribute<CommandAttribute>();

                var arr = new List<object>();

                var list = methodParams.ToList();
                foreach (var x in list)
                {
                    var index = list.IndexOf(x);
                    if (index == 0)
                    {
                        arr.Add(player);
                    }
                    else
                    {
                        if (split.Length <= index)
                            break;

                        var p = split[index];

                        if (x.ParameterType == typeof(int))
                        {
                            if (!int.TryParse(p, out int val))
                                break;

                            if (val == 0 && p != "0")
                                break;

                            arr.Add(val);
                        }
                        else if (x.ParameterType == typeof(string))
                        {
                            if (string.IsNullOrWhiteSpace(p))
                                break;

                            if (command.GreedyArg && index + 1 == list.Count)
                                p = string.Join(" ", split.Skip(index).Take(split.Length - index));

                            arr.Add(p);
                        }
                        else if (x.ParameterType == typeof(float))
                        {
                            if (!float.TryParse(p, out float val))
                                break;

                            if (val == 0 && p != "0")
                                break;

                            arr.Add(val);
                        }
                        else if (x.ParameterType == typeof(byte))
                        {
                            if (!byte.TryParse(p, out byte val))
                                break;

                            if (val == 0 && p != "0")
                                break;

                            arr.Add(val);
                        }
                        else if (x.ParameterType == typeof(uint))
                        {
                            if (!uint.TryParse(p, out uint val))
                                break;

                            if (val == 0 && p != "0")
                                break;

                            arr.Add(val);
                        }
                    }
                }

                if (methodParams.Length != arr.Count)
                {
                    player.SendMessage(MessageType.None, $"Use: {{{Global.MAIN_COLOR}}}{command.HelpText}");
                    return;
                }

                method.Invoke(obj, arr.ToArray());
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
                player.SendMessage(MessageType.Error, "Não foi possível interpretar o comando.");
            }
        }

        [AsyncClientEvent(nameof(ComprarConveniencia))]
        public async Task ComprarConveniencia(MyPlayer player, string nome)
        {
            var preco = Global.Prices.FirstOrDefault(x => x.Name == nome && x.Type == PriceType.Conveniencia);
            if (preco == null)
            {
                player.SendMessage(MessageType.Error, "Preço não foi configurado.");
                return;
            }

            var value = Convert.ToInt32(Math.Abs(preco.Value));
            if (player.Money < value)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, value), notify: true);
                return;
            }

            var strMensagem = string.Empty;
            var res = string.Empty;
            switch (nome)
            {
                case "Celular":
                    var celular = await Functions.GetNewCellphoneNumber();

                    res = await player.GiveItem(new CharacterItem(ItemCategory.Cellphone, celular)
                    {
                        Extra = JsonSerializer.Serialize(new CellphoneItem { Contatos = Functions.GetDefaultsContacts() }),
                    });

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.SendMessage(MessageType.Error, res, notify: true);
                        return;
                    }

                    strMensagem = $"Você comprou um celular. Seu número é: {celular}.";
                    break;
                case "Rádio Comunicador":
                    res = await player.GiveItem(new CharacterItem(ItemCategory.WalkieTalkie)
                    {
                        Extra = JsonSerializer.Serialize(new RadioCommunicatorItem()),
                    });

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.SendMessage(MessageType.Error, res, notify: true);
                        return;
                    }

                    strMensagem = $"Você comprou um rádio comunicador.";
                    break;
                case "Galão de Combustível":
                    res = await player.GiveItem(new CharacterItem(ItemCategory.Weapon, (uint)WeaponModel.JerryCan)
                    {
                        Extra = JsonSerializer.Serialize(new WeaponItem { Ammo = 5000 }),
                    });

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.SendMessage(MessageType.Error, res, notify: true);
                        return;
                    }

                    strMensagem = $"Você comprou um galão de combustível.";
                    break;
                case "Soco Inglês":
                    res = await player.GiveItem(new CharacterItem(ItemCategory.Weapon, (uint)WeaponModel.BrassKnuckles)
                    {
                        Extra = JsonSerializer.Serialize(new WeaponItem()),
                    });

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.SendMessage(MessageType.Error, res, notify: true);
                        return;
                    }

                    strMensagem = $"Você comprou um soco inglês.";
                    break;
                case "Garrafa":
                    res = await player.GiveItem(new CharacterItem(ItemCategory.Weapon, (uint)WeaponModel.BrokenBottle)
                    {
                        Extra = JsonSerializer.Serialize(new WeaponItem()),
                    });

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.SendMessage(MessageType.Error, res, notify: true);
                        return;
                    }

                    strMensagem = $"Você comprou uma garrafa.";
                    break;
                case "Pé de Cabra":
                    res = await player.GiveItem(new CharacterItem(ItemCategory.Weapon, (uint)WeaponModel.Crowbar)
                    {
                        Extra = JsonSerializer.Serialize(new WeaponItem()),
                    });

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.SendMessage(MessageType.Error, res, notify: true);
                        return;
                    }

                    strMensagem = $"Você comprou um pé de cabra.";
                    break;
                case "Taco de Golfe":
                    res = await player.GiveItem(new CharacterItem(ItemCategory.Weapon, (uint)WeaponModel.GolfClub)
                    {
                        Extra = JsonSerializer.Serialize(new WeaponItem()),
                    });

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.SendMessage(MessageType.Error, res, notify: true);
                        return;
                    }

                    strMensagem = $"Você comprou um taco de golfe.";
                    break;
                case "Martelo":
                    res = await player.GiveItem(new CharacterItem(ItemCategory.Weapon, (uint)WeaponModel.Hammer)
                    {
                        Extra = JsonSerializer.Serialize(new WeaponItem()),
                    });

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.SendMessage(MessageType.Error, res, notify: true);
                        return;
                    }

                    strMensagem = $"Você comprou um martelo.";
                    break;
                case "Chave de Grifo":
                    res = await player.GiveItem(new CharacterItem(ItemCategory.Weapon, (uint)WeaponModel.PipeWrench)
                    {
                        Extra = JsonSerializer.Serialize(new WeaponItem()),
                    });

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.SendMessage(MessageType.Error, res, notify: true);
                        return;
                    }

                    strMensagem = $"Você comprou uma chave de grifo.";
                    break;
                case "Taco de Baseball":
                    res = await player.GiveItem(new CharacterItem(ItemCategory.Weapon, (uint)WeaponModel.BaseballBat)
                    {
                        Extra = JsonSerializer.Serialize(new WeaponItem()),
                    });

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.SendMessage(MessageType.Error, res, notify: true);
                        return;
                    }

                    strMensagem = $"Você comprou um taco de baseball.";
                    break;
                case "Bola de Baseball":
                    res = await player.GiveItem(new CharacterItem(ItemCategory.Weapon, (uint)WeaponModel.Baseball)
                    {
                        Extra = JsonSerializer.Serialize(new WeaponItem()),
                    });

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.SendMessage(MessageType.Error, res, notify: true);
                        return;
                    }

                    strMensagem = $"Você comprou uma bola de baseball.";
                    break;
                case "Boombox":
                    res = await player.GiveItem(new CharacterItem(ItemCategory.Boombox));

                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.SendMessage(MessageType.Error, res, notify: true);
                        return;
                    }

                    strMensagem = $"Você comprou uma boombox.";
                    break;
            }

            await player.RemoveItem(new CharacterItem(ItemCategory.Money)
            {
                Quantity = value
            });
            player.SendMessage(MessageType.Success, strMensagem, notify: true);
        }

        [ClientEvent(nameof(AtualizarInformacoes))]
        public void AtualizarInformacoes(MyPlayer player, bool isGameFocused)
        {
            var hasData = player.HasStreamSyncedMetaData(Constants.PLAYER_META_DATA_GAME_UNFOCUSED);
            if (isGameFocused)
            {
                if (hasData)
                    player.DeleteStreamSyncedMetaData(Constants.PLAYER_META_DATA_GAME_UNFOCUSED);
            }
            else
            {
                if (!hasData)
                    player.SetStreamSyncedMetaData(Constants.PLAYER_META_DATA_GAME_UNFOCUSED, DateTime.Now.ToString());
            }
        }

        [AsyncClientEvent(nameof(ConfirmarLojaRoupas))]
        public async Task ConfirmarLojaRoupas(MyPlayer player, string strRoupas, string strAcessorios, int tipo, bool sucesso)
        {
            if (sucesso)
            {
                var items = new List<CharacterItem>();

                void AddItems(ItemCategory tipoCategoriaItem, List<ClotheAccessory> clothes)
                {
                    short slot = 0;
                    if (tipo == 0)
                        slot = tipoCategoriaItem switch
                        {
                            ItemCategory.Cloth3 => -101,
                            ItemCategory.Cloth11 => -102,
                            ItemCategory.Cloth8 => -103,
                            ItemCategory.Cloth4 => -104,
                            ItemCategory.Cloth6 => -105,
                            ItemCategory.Cloth7 => -106,
                            ItemCategory.Cloth10 => -107,
                            ItemCategory.Cloth9 => -108,
                            ItemCategory.Cloth1 => -109,
                            ItemCategory.Accessory0 => -110,
                            ItemCategory.Accessory1 => -111,
                            ItemCategory.Accessory2 => -112,
                            ItemCategory.Accessory6 => -113,
                            ItemCategory.Accessory7 => -114,
                            ItemCategory.Cloth5 => -115,
                            _ => 0,
                        };

                    items.AddRange(clothes.Select(x => new CharacterItem(tipoCategoriaItem, (uint)x.Drawable)
                    {
                        Extra = JsonSerializer.Serialize(new ClotheAccessoryItem
                        {
                            DLC = x.DLC,
                            Texture = x.Texture,
                            Sexo = player.Character.Sex,
                        }),
                        Slot = slot,
                    }).ToList());
                }

                var roupas = JsonSerializer.Deserialize<List<ClotheAccessory>>(strRoupas);
                if (roupas.Count > 0)
                {
                    AddItems(ItemCategory.Cloth1, roupas.Where(x => x.Component == 1).ToList());
                    AddItems(ItemCategory.Cloth3, roupas.Where(x => x.Component == 3).ToList());
                    AddItems(ItemCategory.Cloth4, roupas.Where(x => x.Component == 4).ToList());
                    AddItems(ItemCategory.Cloth5, roupas.Where(x => x.Component == 5).ToList());
                    AddItems(ItemCategory.Cloth6, roupas.Where(x => x.Component == 6).ToList());
                    AddItems(ItemCategory.Cloth7, roupas.Where(x => x.Component == 7).ToList());
                    AddItems(ItemCategory.Cloth8, roupas.Where(x => x.Component == 8).ToList());
                    AddItems(ItemCategory.Cloth9, roupas.Where(x => x.Component == 9).ToList());
                    AddItems(ItemCategory.Cloth10, roupas.Where(x => x.Component == 10).ToList());
                    AddItems(ItemCategory.Cloth11, roupas.Where(x => x.Component == 11).ToList());
                }

                var acessorios = JsonSerializer.Deserialize<List<ClotheAccessory>>(strAcessorios);
                if (acessorios.Count > 0)
                {
                    AddItems(ItemCategory.Accessory0, acessorios.Where(x => x.Component == 0).ToList());
                    AddItems(ItemCategory.Accessory1, acessorios.Where(x => x.Component == 1).ToList());
                    AddItems(ItemCategory.Accessory2, acessorios.Where(x => x.Component == 2).ToList());
                    AddItems(ItemCategory.Accessory6, acessorios.Where(x => x.Component == 6).ToList());
                    AddItems(ItemCategory.Accessory7, acessorios.Where(x => x.Component == 7).ToList());
                }

                if (!items.Any())
                {
                    player.Emit("Character:ShowMessage", "Nenhum item foi selecionado.");
                    return;
                }

                var res = await player.GiveItem(items);
                if (!string.IsNullOrWhiteSpace(res))
                {
                    player.Emit("Character:ShowMessage", res);
                    return;
                }
            }

            await player.SetarRoupas();

            if (tipo == 0)
            {
                player.Character.PersonalizationStep = CharacterPersonalizationStep.Ready;
                await using var context = new DatabaseContext();
                context.Characters.Update(player.Character);
                await context.SaveChangesAsync();
                await player.Spawnar();
            }
            else
            {
                player.Invincible = false;
                player.Emit("Character:CloseClothes");

                if (tipo == 1 && sucesso)
                {
                    await player.RemoveItem(new CharacterItem(ItemCategory.Money)
                    {
                        Quantity = Global.Parameter.ClothesValue
                    });
                    player.SendMessage(MessageType.Success, $"Você pagou ${Global.Parameter.ClothesValue:N0} na loja de roupas.");
                }
            }

            player.Emit("Server:SelecionarPersonagem",
                (int)player.Character.PersonalizationStep,
                (int)player.Character.Sex,
                JsonSerializer.Serialize(player.Personalization));
        }

        [AsyncClientEvent(nameof(ConfirmarPersonalizacao))]
        public static async Task ConfirmarPersonalizacao(MyPlayer player, string strPersonalizacao, int tipo, bool sucesso)
        {
            if (sucesso)
                player.Personalization = JsonSerializer.Deserialize<MyPlayer.Personalizacao>(strPersonalizacao);

            if (tipo == 0)
            {
                player.Character.PersonalizationJSON = strPersonalizacao;
                player.Character.PersonalizationStep = CharacterPersonalizationStep.Tattoos;
                await using var context = new DatabaseContext();
                context.Characters.Update(player.Character);
                await context.SaveChangesAsync();
            }
            else if (sucesso)
            {
                player.Invincible = false;
                await player.RemoveItem(new CharacterItem(ItemCategory.Money)
                {
                    Quantity = Global.Parameter.BarberValue
                });
                player.SendMessage(MessageType.Success, $"Você pagou ${Global.Parameter.BarberValue:N0} na barbearia.");
            }
            else
            {
                player.SetarPersonalizacao(player.Personalization);
            }

            player.Emit("Server:SelecionarPersonagem",
                (int)player.Character.PersonalizationStep,
                (int)player.Character.Sex,
                JsonSerializer.Serialize(player.Personalization));
        }

        [ClientEvent(nameof(Chatting))]
        public void Chatting(MyPlayer player, bool chatting) => player.SetStreamSyncedMetaData(Constants.PLAYER_META_DATA_CHATTING, chatting);

        [ClientEvent(nameof(SoltarSacoLixo))]
        public void SoltarSacoLixo(MyPlayer player, float x, float y, float z)
        {
            if (player.Position.Distance(new Position(x, y, z)) > 2)
            {
                player.SendMessage(MessageType.Error, "Você não está na parte de trás de um caminhão de lixo.");
                return;
            }

            player.GarbageBagObject.Destroy();
            player.GarbageBagObject = null;
            player.CollectingSpot.Blip.Destroy();
            player.CollectingSpot.Marker.Destroy();
            player.CollectSpots.Remove(player.CollectingSpot);
            player.CollectingSpot = new Spot();
            player.SendMessageToNearbyPlayers($"coloca um saco de lixo no caminhão.", MessageCategory.Ame, 5);

            var multiplier = player.Character.DrugItemCategory switch
            {
                ItemCategory.Cocaine or ItemCategory.Crack => 2,
                ItemCategory.Metanfetamina => 2.7,
                _ => 1,
            };

            var extraPayment = Convert.ToInt32(Math.Abs(Global.Parameter.ExtraPaymentGarbagemanValue * multiplier));
            player.ExtraPayment += extraPayment;

            if (player.CollectSpots.Count == 0)
            {
                player.SendMessage(MessageType.Success, "Você realizou todas as coletas. Retorne ao centro de reciclagem e saia de serviço.");
                return;
            }
        }

        [AsyncClientEvent(nameof(AbrirPainelControleUsuario))]
        public async Task AbrirPainelControleUsuario(MyPlayer player)
        {
            var listaComandos = new List<Comando>()
            {
                new Comando("Teclas", "F2", "Painel de Controle do Usuário"),
                new Comando("Teclas", "F7", "Habilita/desabilita a HUD"),
                new Comando("Teclas", "B", "Aponta/para de apontar o dedo estando fora de um veículo"),
                new Comando("Teclas", "T", "Abrir caixa de texto para digitação no chat"),
                new Comando("Teclas", "F", "Entra em veículo como motorista"),
                new Comando("Teclas", "G", "Entra em veículo como passageiro"),
                new Comando("Teclas", "L", "Tranca/destranca propriedades e veículos"),
                new Comando("Teclas", "J", "Ativa/desativa o controle de velocidade (cruise control)"),
                new Comando("Teclas", "Z", "Ativa/desativa o modo de andar agachado"),
                new Comando("Teclas", "Z", "Liga/desliga o motor do veículo"),
                new Comando("Teclas", "Y", "Entra/sai de uma propriedade"),
                new Comando("Teclas", "Shift + G", "Entra em veículo dando prioridade como passageiro externo"),
                new Comando("Teclas", "TAB", "Abre/fecha lista de jogadores online"),
                new Comando("Teclas", "I", "Abre o inventário"),
                new Comando("Teclas", "'", "Retira a arma que estiver em mãos"),
                new Comando("Teclas", "1", "Pega a arma do slot 1"),
                new Comando("Teclas", "2", "Pega a arma do slot 2"),
                new Comando("Teclas", "3", "Pega a arma do slot 3"),
                new Comando("Geral", "/id", "Procura o ID de um personagem"),
                new Comando("Geral", "/aceitar /ac", "Aceita um convite"),
                new Comando("Geral", "/recusar /rc", "Recusa um convite"),
                new Comando("Geral", "/revistar", "Solicita uma revista em um personagem"),
                new Comando("Geral", "/comprar", "Compra itens em um ponto de compra"),
                new Comando("Geral", "/emprego", "Pega um emprego"),
                new Comando("Geral", "/sos", "Envia solicitação de ajuda aos administradores em serviço"),
                new Comando("Geral", "/ferimentos", "Visualiza os ferimentos de um personagem"),
                new Comando("Geral", "/aceitarhospital", "Aceita o tratamento médico após estar ferido e é levado ao hospital"),
                new Comando("Geral", "/aceitarck", "Aceita o CK no personagem"),
                new Comando("Geral", "/trancar", "Tranca/destranca propriedades e veículos"),
                new Comando("Geral", "/mostraridentidade /mostrarid", "Mostra a identidade para um personagem"),
                new Comando("Geral", "/dmv", "Compra/renova a licença de motorista"),
                new Comando("Geral", "/mostrarlicenca /ml", "Mostra a licença de motorista para um personagem"),
                new Comando("Geral", "/horas", "Exibe o horário"),
                new Comando("Geral", "/telapreta", "Exibe um fundo preto na tela"),
                new Comando("Geral", "/telacinza", "Exibe um fundo cinza na tela"),
                new Comando("Geral", "/telaverde", "Exibe um fundo verde na tela"),
                new Comando("Geral", "/telalaranja", "Exibe um fundo laranja na tela"),
                new Comando("Geral", "/limparchat", "Limpa o seu chat"),
                new Comando("Geral", "/mecurar", "Trata os ferimentos em um hospital"),
                new Comando("Geral", "/save", "Exibe sua posição e rotação ou do seu veículo no console"),
                new Comando("Geral", "/dados", "Joga um dado cujo o resultado será um número de 1 a 6"),
                new Comando("Geral", "/moeda", "Joga uma moeda cujo o resultado será cara ou coroa"),
                new Comando("Geral", "/levantar", "Levanta um jogador gravemente ferido somente de socos"),
                new Comando("Geral", "/trocarpersonagem", "Desconecta do personagem atual e abre a seleção de personagens"),
                new Comando("Geral", "/staff", "Exibe os membros da staff que estão online"),
                new Comando("Geral", "/dl", "Ativa/desativa o DL"),
                new Comando("Geral", "/historicocriminal", "Visualiza o histórico criminal do seu personagem"),
                new Comando("Geral", "/cancelarconvite /cc", "Cancela um convite"),
                new Comando("Geral", "/banco", "Gerencia sua conta bancária em um banco"),
                new Comando("Geral", "/atm", "Gerencia sua conta bancária em uma ATM"),
                new Comando("Geral", "/infos", "Gerencia suas marcas de informações"),
                new Comando("Geral", "/bocafumo", "Usa uma boca de fumo"),
                new Comando("Geral", "/usardroga", "Usa droga"),
                new Comando("Geral", "/boombox", "Altera as configurações de uma boombox"),
                new Comando("Geral", "/mic", "Fala em um microfone"),
                new Comando("Geral", "/stopanim /sa", "Para as animações"),
                new Comando("Geral", "/e", "Faz uma animação"),
                new Comando("Geral", "/cenario", "Reproduz um cenário no jogo com seu personagem"),
                new Comando("Geral", "/porta", "Abre/fecha uma porta que você possui acesso"),
                new Comando("Geral", "/alugarempresa", "Aluga uma empresa"),
                new Comando("Geral", "/empresa", "Gerencia suas empresas"),
                new Comando("Propriedades", "/pvender", "Vende uma propriedade para um personagem"),
                new Comando("Propriedades", "/pvendergoverno", "Venda uma propriedade para o governo"),
                new Comando("Propriedades", "/armazenamento", "Visualiza o armazenamento de uma propriedade"),
                new Comando("Propriedades", "/pchave", "Cria uma chave cópia de uma propriedade"),
                new Comando("Propriedades", "/pfechadura", "Altera a fechadura de uma propriedade"),
                new Comando("Propriedades", "/pupgrade", "Realiza atualições na propriedade"),
                new Comando("Propriedades", "/arrombar", "Arromba a porta de uma propriedade"),
                new Comando("Propriedades", "/roubarpropriedade", "Rouba uma propriedade"),
                new Comando("Propriedades", "/roubararmazenamento", "Rouba o armazenamento de uma propriedade"),
                new Comando("Propriedades", "/pliberar", "Libera uma propriedade roubada"),
                new Comando("Propriedades", "/mobilias", "Gerencia as mobílias de uma propriedade"),
                new Comando("Chat IC", "/me", "Interpretação de ações de um personagem"),
                new Comando("Chat IC", "/do", "Interpretação do ambiente"),
                new Comando("Chat IC", "/g", "Grita"),
                new Comando("Chat IC", "/baixo", "Fala baixo"),
                new Comando("Chat IC", "/s", "Sussura"),
                new Comando("Chat IC", "/ame", "Interpretação de ações de um personagem"),
                new Comando("Chat IC", "/ado", "Interpretação do ambiente"),
                new Comando("Chat OOC", "/b", "Chat OOC local"),
                new Comando("Chat OOC", "/pm", "Envia uma mensagem privada"),
                new Comando("Celular", "/celular /cel", "Abre o celular"),
                new Comando("Celular", "/sms", "Envia um SMS"),
                new Comando("Celular", "/ligar", "Liga para um número"),
                new Comando("Celular", "/atender", "Atende a ligação"),
                new Comando("Celular", "/desligar /des", "Desliga a ligação"),
                new Comando("Celular", "/an", "Envia um anúncio"),
                new Comando("Veículos", "/motor", "Liga/desliga o motor de um veículo"),
                new Comando("Veículos", "/vcomprarvaga", "Compra uma vaga para estacionar um veículo"),
                new Comando("Veículos", "/vestacionar", "Estaciona um veículo"),
                new Comando("Veículos", "/vlista", "Mostra seus veículos"),
                new Comando("Veículos", "/vvender", "Vende um veículo para outro personagem"),
                new Comando("Veículos", "/vliberar", "Libera um veículo apreendido"),
                new Comando("Veículos", "/portamalas", "Gerencia o porta-malas de um veículo"),
                new Comando("Veículos", "/capo", "Abre/fecha o capô de um veículo"),
                new Comando("Veículos", "/vporta", "Abre/fecha a porta de um veículo"),
                new Comando("Veículos", "/abastecer", "Abastece um veículo em um posto de combustível"),
                new Comando("Veículos", "/vplaca", "Altera a placa de um veículo"),
                new Comando("Veículos", "/vvenderconce", "Vende um veículo para a concessionária"),
                new Comando("Veículos", "/valugar", "Aluga um veículo"),
                new Comando("Veículos", "/danos", "Visualiza os danos de um veículo"),
                new Comando("Veículos", "/velmax", "Define a velocidade máxima de um veículo"),
                new Comando("Veículos", "/janela /janelas /ja", "Abre/fecha a janela de um veículo"),
                new Comando("Veículos", "/vchave", "Cria uma chave cópia de um veículo"),
                new Comando("Veículos", "/vfechadura", "Altera a fechadura de um veículo"),
                new Comando("Veículos", "/usargalao", "Abastece um veículo usando um galão de combustível"),
                new Comando("Veículos", "/drift", "Ativa/desativa o modo drift de um veículo"),
                new Comando("Veículos", "/vupgrade", "Realiza atualições no veículo"),
                new Comando("Veículos", "/vrastrear", "Rastreia um veículo"),
                new Comando("Veículos", "/xmr", "Altera as configurações do XMR"),
                new Comando("Veículos", "/quebrartrava", "Quebra a trava de um veículo"),
                new Comando("Veículos", "/colocar", "Coloca um personagem algemado no banco traseiro de um veículo"),
                new Comando("Veículos", "/retirar", "Retira um personagem algemado de dentro de um veículo"),
                new Comando("Veículos", "/reparar", "Conserta um veículo na central de mecânicos quando não há mecânicos em serviço"),
                new Comando("Veículos", "/tunarver", "Visualiza as modificações disponíveis para um veículo"),
                new Comando("Veículos", "/ligacaodireta /hotwire", "Faz ligação direta em um veículo"),
                new Comando("Veículos", "/desmanchar", "Desmancha um veículo"),
                new Comando("Veículos", "/rebocar", "Rebocar um veículo"),
                new Comando("Veículos", "/rebocaroff", "Solta um veículo"),
                new Comando("Rádio", "/canal", "Troca os canais de rádio"),
                new Comando("Rádio", "/r", "Fala no canal de rádio 1"),
                new Comando("Rádio", "/r2", "Fala no canal de rádio 2"),
                new Comando("Rádio", "/r3", "Fala no canal de rádio 3"),
                new Comando("Rádio", "/r4", "Fala no canal de rádio 4"),
                new Comando("Rádio", "/r5", "Fala no canal de rádio 5"),
            };

            if (player.Character.Job > 0)
            {
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Emprego", "/sairemprego", "Sai do emprego"),
                    new Comando("Emprego", "/duty /trabalho", "Entra/sai de serviço"),
                });

                if (player.Character.Job == CharacterJob.TaxiDriver || player.Character.Job == CharacterJob.Mechanic)
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Emprego", "/chamadas", "Exibe as chamadas aguardando resposta"),
                        new Comando("Emprego", "/atcha", "Atende uma chamada"),
                    });

                if (player.Character.Job == CharacterJob.Mechanic)
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Emprego", "/tunarcomprar", "Realiza modificações em um veículo"),
                    });
                else if (player.Character.Job == CharacterJob.Garbageman)
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Emprego", "/pegarlixo", "Pega um saco de lixo em um ponto de coleta"),
                        new Comando("Emprego", "/colocarlixo", "Coloca um saco de lixo em um caminhão de lixo"),
                    });
                else if (player.Character.Job == CharacterJob.Trucker)
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Emprego", "/rotas", "Exibe as rotas disponíveis para trabalho"),
                        new Comando("Emprego", "/carregarcaixas", "Carrega o seu veículo com as caixas da rota"),
                        new Comando("Emprego", "/cancelarcaixas", "Devolve as caixas da rota"),
                        new Comando("Emprego", "/entregarcaixas", "Entrega as caixas da rota em um ponto de entrega"),
                    });
            }

            if (player.Character.FactionId.HasValue)
            {
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Facção", "/f", "Chat OOC da facção"),
                    new Comando("Facção", "/faccao", "Abre o painel de gerenciamento da facção"),
                    new Comando("Facção", "/sairfaccao", "Sai da facção"),
                });

                if (player.Faction.Government)
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Facção", "/fspawn", "Spawna veículos da facção"),
                        new Comando("Facção", "/ex", "Ativa/desativa extras de veículos da facção"),
                        new Comando("Facção", "/mostrardistintivo", "Mostra seu distintivo para um personagem"),
                        new Comando("Facção", "/freparar", "Conserta veículos da facção"),
                        new Comando("Facção", "/duty /trabalho", "Entra/sai de trabalho"),
                        new Comando("Facção", "/mdc", "Abre o MDC"),
                        new Comando("Facção", "/uniforme", "Coloca/retira o uniforme de serviço"),
                        new Comando("Facção", "/m", "Fala no megafone"),
                        new Comando("Teclas", "Q", "Desliga/liga som da sirene de um veículo"),
                        new Comando("Facção", "/rapel", "Desce de rapel dos assentos traseiros de um helicóptero apropriado"),
                        new Comando("Facção", "/br", "Cria uma barreira"),
                        new Comando("Facção", "/rb", "Remove uma barreira"),
                        new Comando("Facção", "/rballme", "Remove todas barreiras criadas por você"),
                        new Comando("Facção", "/rball", "Remove todas barreiras"),
                    });

                if (player.Faction.Type == FactionType.Police)
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Teclas", "F3", "Ativa/desativa a câmera do helicóptero"),
                        new Comando("Teclas", "Botão Esquerdo do Mouse", "Ativa/desativa a luz do helicóptero enquanto a câmera estiver ativa"),
                        new Comando("Teclas", "Botão Direito do Mouse", "Altera o modo da visão do helicóptero enquanto a câmera estiver ativa"),
                        new Comando("Facção", "/prender", "Prende um personagem"),
                        new Comando("Facção", "/algemar", "Algema/desalgema um personagem"),
                        new Comando("Facção", "/apreender", "Apreende um veículo"),
                        new Comando("Facção", "/radar", "Coloca um radar de velocidade"),
                        new Comando("Facção", "/radaroff", "Remove um radar de velocidade"),
                        new Comando("Facção", "/spotlight /holofote", "Ativa/desativa o holofote de um veículo"),
                        new Comando("Facção", "/confisco", "Cria um registro de confisco"),
                    });
                else if (player.Faction.Type == FactionType.Firefighter)
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Facção", "/curar", "Cura um personagem ferido"),
                    });

                if (player.FactionFlags.Any())
                {
                    if (player.FactionFlags.Contains(FactionFlag.BlockChat))
                        listaComandos.AddRange(new List<Comando>()
                        {
                            new Comando("Flag Facção Bloquear Chat", "/blockf", "Bloqueia/desbloqueia o chat OOC da facção"),
                        });

                    if (player.FactionFlags.Contains(FactionFlag.GovernmentAnnouncement))
                        listaComandos.AddRange(new List<Comando>()
                        {
                            new Comando("Flag Facção Anúncio do Governo", "/gov", "Envia um anúncio governamental da facção"),
                        });

                    if (player.FactionFlags.Contains(FactionFlag.HQ))
                        listaComandos.AddRange(new List<Comando>()
                        {
                            new Comando("Flag Facção HQ", "/hq", "Envia uma mensagem no rádio da facção como dispatcher"),
                        });

                    if (player.FactionFlags.Contains(FactionFlag.Armory))
                        listaComandos.AddRange(new List<Comando>()
                        {
                            new Comando("Flag Facção Arsenal", "/arsenal", "Usa o arsenal da facção"),
                        });

                    if (player.FactionFlags.Contains(FactionFlag.DrugHouse))
                        listaComandos.AddRange(new List<Comando>()
                        {
                            new Comando("Flag Facção Drogas", "/drughouse", "Usa a drug house da facção"),
                        });
                }
            }

            if (player.StaffFlags.Any())
            {
                if (player.StaffFlags.Contains(StaffFlag.Doors))
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Flag Staff Portas", "/portas", "Abre o painel de gerenciamento de portas"),
                    });

                if (player.StaffFlags.Contains(StaffFlag.Prices))
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Flag Staff Preços", "/precos", "Abre o painel de gerenciamento de preços"),
                    });

                if (player.StaffFlags.Contains(StaffFlag.Factions))
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Flag Staff Facções", "/faccoes", "Abre o painel de gerenciamento de facções"),
                    });

                if (player.StaffFlags.Contains(StaffFlag.FactionsArmories))
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Flag Staff Arsenais", "/arsenais", "Abre o painel de gerenciamento de arsenais"),
                    });

                if (player.StaffFlags.Contains(StaffFlag.Properties))
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Flag Staff Propriedades", "/propriedades", "Abre o painel de gerenciamento de propriedades"),
                        new Comando("Flag Staff Propriedades", "/int", "Visualiza um tipo de interior"),
                    });

                if (player.StaffFlags.Contains(StaffFlag.GiveItem))
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Flag Staff Dar Item", "/daritem", "Dá um item para um personagem"),
                    });

                if (player.StaffFlags.Contains(StaffFlag.FactionsDrugsHouses))
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Flag Staff Drug Houses", "/drughouses", "Abre o painel de gerenciamento de drug houses"),
                    });

                if (player.StaffFlags.Contains(StaffFlag.CrackDens))
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Flag Staff Bocas de Fumo", "/bocasfumo", "Abre o painel de gerenciamento de bocas de fumo"),
                    });

                if (player.StaffFlags.Contains(StaffFlag.TruckerLocations))
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Flag Staff Localizações de Caminhoneiros", "/acaminhoneiro", "Abre o painel de gerenciamento de localizações de caminhoneiros"),
                    });

                if (player.StaffFlags.Contains(StaffFlag.Furnitures))
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Flag Staff Mobílias", "/amobilias", "Abre o painel de gerenciamento de mobílias"),
                    });

                if (player.StaffFlags.Contains(StaffFlag.Animations))
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Flag Staff Animações", "/animacoes", "Abre o painel de gerenciamento de animações"),
                    });

                if (player.StaffFlags.Contains(StaffFlag.Companies))
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Flag Staff Empresas", "/empresas", "Abre o painel de gerenciamento de empresas"),
                    });

                if (player.StaffFlags.Contains(StaffFlag.Vehicles))
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Flag Staff Veículos", "/veiculos", "Abre o painel de gerenciamento de veículos"),
                        new Comando("Flag Staff Veículos", "/atunar", "Realiza modificações em um veículo"),
                    });
            }

            if (player.User.Staff >= UserStaff.Moderator)
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Moderator", "/ir", "Vai a um personagem"),
                    new Comando("Moderator", "/trazer", "Traz um personagem"),
                    new Comando("Moderator", "/tp", "Teleporta um personagem para outro"),
                    new Comando("Moderator", "/a", "Chat administrativo"),
                    new Comando("Moderator", "/kick", "Expulsa um personagem"),
                    new Comando("Moderator", "/irveh", "Vai a um veículo"),
                    new Comando("Moderator", "/trazerveh", "Traz um veículo"),
                    new Comando("Moderator", "/aduty /atrabalho", "Entra/sai de serviço administrativo"),
                    new Comando("Moderator", "/at", "Atende um pedido de ajuda"),
                    new Comando("Moderator", "/spec", "Observa um personagem"),
                    new Comando("Moderator", "/specoff", "Para de observar um personagem"),
                    new Comando("Moderator", "/aferimentos", "Visualiza os ferimentos de um personagem"),
                    new Comando("Moderator", "/aestacionar", "Estaciona um veículo"),
                    new Comando("Moderator", "/acurar", "Cura um personagem ferido"),
                    new Comando("Moderator", "/adanos", "Visualiza os danos de um veículo"),
                    new Comando("Moderator", "/checarveh", "Visualiza o proprietário de um veículo"),
                    new Comando("Moderator", "/acp", "Abre o painel de controle administrativo"),
                    new Comando("Moderator", "/checar", "Checa as informações de um personagem"),
                    new Comando("Moderator", "/proximo /prox", "Lista os itens que estão próximos"),
                    new Comando("Moderator", "/ainfos", "Gerencia todas marcas de informações"),
                    new Comando("Moderator", "/debug", "Lista os pontos de áudio e de luz ativos"),
                    new Comando("Moderator", "/app", "Pega uma aplicação para avaliação"),
                    new Comando("Moderator", "/aceitarapp", "Aceita a aplicação que você está avaliando"),
                    new Comando("Moderator", "/negarapp", "Nega a aplicação que você está avaliando"),
                    new Comando("Moderator", "/apps", "Lista as aplicações para avaliação"),
                });

            if (player.User.Staff >= UserStaff.GameAdministrator)
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Game Administrator", "/ban", "Bane um jogador"),
                    new Comando("Game Administrator", "/pos", "Vai até a posição"),
                    new Comando("Game Administrator", "/o", "Chat OOC Global"),
                    new Comando("Game Administrator", "/waypoint", "Teleporta até o waypoint marcado no mapa"),
                });

            if (player.User.Staff >= UserStaff.LeadAdministrator)
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Lead Administrator", "/tempo", "Altera o tempo"),
                    new Comando("Lead Administrator", "/limparchatgeral", "Limpa o chat de todos os personagens"),
                    new Comando("Lead Administrator", "/areparar", "Conserta um veículo"),
                    new Comando("Lead Administrator", "/jetpack", "Cria um jetpack"),
                });

            if (player.User.Staff >= UserStaff.HeadAdministrator)
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Head Administrator", "/parametros", "Edita os parâmetros do servidor"),
                });

            if (player.User.Staff >= UserStaff.Manager)
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Manager", "/gmx", "Salva todas as informações do servidor"),
                    new Comando("Manager", "/vip", "Adiciona VIP para um usuário"),
                    new Comando("Manager", "/password", "Altera a senha do servidor"),
                });

            var html = $@"<div class='row'><div class='col-md-3'><select class='form-control' id='sel-categoria'><option value='Todas' selected>Todas</option>{string.Join("", listaComandos.GroupBy(x => x.Categoria).Select(x => $"<option value='{x.Key}'>{x.Key}</option>").ToList())}</select></div><div class='col-md-9'><input id='pesquisa' type='text' autofocus class='form-control' placeholder='Pesquise os comandos...' /></div></div><br/>
            <div class='table-responsive' style='max-height:50vh;overflow-y:auto;overflow-x:hidden;'>
                <table class='table table-bordered table-striped'>
                <thead>
                    <tr class='bg-dark'>
                        <th>Categoria</th>
                        <th>Comando</th>
                        <th>Descrição</th>
                    </tr>
                </thead>
                <tbody>";

            listaComandos = listaComandos.OrderBy(x => x.Categoria).ThenBy(x => x.Nome).ToList();
            foreach (var x in listaComandos)
                html += $@"<tr class='pesquisaitem' data-categoria='{x.Categoria}'><td>{x.Categoria}</td><td>{x.Nome}</td><td>{x.Descricao}</td></tr>";

            html += $@"
                </tbody>
            </table>
            </div>";

            var htmlStats = await player.ObterHTMLStats();
            htmlStats = $"<h3>{player.Character.Name} [{player.Character.Id}] ({DateTime.Now})</h3><div style='max-height:50vh;overflow-y:auto;overflow-x:hidden;'>{htmlStats}</div>";

            var htmlConfiguracoes = $@"<div class='row'>
                            <div class='col-md-12'>
                                <div class='form-group'>
                                    <label class='control-label'>Tipo da Fonte do Chat</label>
                                    <select class='form-control' id='sel-tipofontechat'>
                                        <option value='0' {(player.User.ChatFontType == 0 ? "selected" : string.Empty)}>Arial, Helvetica, sans-serif</option>
                                        <option value='1' {(player.User.ChatFontType == 1 ? "selected" : string.Empty)}>Georgia, 'Times New Roman', Times, serif</option>
                                        <option value='2' {(player.User.ChatFontType == 2 ? "selected" : string.Empty)}>'Times New Roman', Times, serif</option>
                                        <option value='3' {(player.User.ChatFontType == 3 ? "selected" : string.Empty)}>'Lucida Sans', 'Lucida Sans Regular', 'Lucida Grande', 'Lucida Sans Unicode', Geneva, Verdana, sans-serif</option>
                                        <option value='4' {(player.User.ChatFontType == 4 ? "selected" : string.Empty)}>'Segoe UI', Tahoma, Geneva, Verdana, sans-serif</option>
                                        <option value='5' {(player.User.ChatFontType == 5 ? "selected" : string.Empty)}>'Trebuchet MS', 'Lucida Sans Unicode', 'Lucida Grande', 'Lucida Sans', Arial, sans-serif</option>
                                        <option value='5' {(player.User.ChatFontType == 6 ? "selected" : string.Empty)}>'Monospaced', sans-serif</option>
                                    </select>
                                </div>
                            </div>
                            <div class='col-md-12'>
                                <div class='form-group'>
                                    <label class='control-label'>Tamanho da Fonte do Chat (12-18)</label>
                                    <input type='text' class='form-control' id='tamanhofontechat' value='{player.User.ChatFontSize}'/>
                                </div>
                            </div>
                            <div class='col-md-12'>
                                <div class='form-group'>
                                    <label class='control-label'>Quantidade de Linhas do Chat (10-20)</label>
                                    <input type='text' class='form-control' id='linhaschat' value='{player.User.ChatLines}'/>
                                </div>
                            </div>
                            <div class='col-md-12'>
                                <div class='checkbox'>
                                    <input id='chk-timestamp' class='magic-checkbox' type='checkbox' {(player.User.TimeStampToggle ? "checked" : string.Empty)}>
                                    <label for='chk-timestamp'>Exibir horário no chat (TimeStamp)</label>
                                </div>
                            </div>
                            <div class='col-md-12'>
                                <div class='checkbox'>
                                    <input id='chk-dl' class='magic-checkbox' type='checkbox' {(player.User.VehicleTagToggle ? "checked" : string.Empty)}>
                                    <label for='chk-dl'>Exibir tags de veículos (DL)</label>
                                </div>
                            </div>
                            <div class='col-md-12'>
                                <div class='checkbox'>
                                    <input id='chk-anuncios' class='magic-checkbox' type='checkbox' {(player.User.AnnouncementToggle ? "checked" : string.Empty)}>
                                    <label for='chk-anuncios'>Ocultar anúncios</label>
                                </div>
                            </div>";

            if ((player.User.VIPValidDate ?? DateTime.MinValue) > DateTime.Now)
                htmlConfiguracoes += $@"<div class='col-md-12'>
                                <div class='checkbox'>
                                    <input id='chk-pm' class='magic-checkbox' type='checkbox' {(player.User.PMToggle ? "checked" : string.Empty)}>
                                    <label for='chk-pm'>Bloquear mensagens privadas (PM)</label>
                                </div>
                            </div>";

            if (player.Character.FactionId.HasValue)
                htmlConfiguracoes += $@"<div class='col-md-12'>
                                <div class='checkbox'>
                                    <input id='chk-chatfaccao' class='magic-checkbox' type='checkbox' {(player.User.FactionChatToggle ? "checked" : string.Empty)}>
                                    <label for='chk-chatfaccao'>Ocultar mensagem do chat da faccção</label>
                                </div>
                            </div>
                            <div class='col-md-12'>
                                <div class='checkbox'>
                                    <input id='chk-faccao' class='magic-checkbox' type='checkbox' {(player.User.FactionToggle ? "checked" : string.Empty)}>
                                    <label for='chk-faccao'>Ocultar mensagem da faccção</label>
                                </div>
                            </div>";

            if (player.User.Staff != UserStaff.None)
                htmlConfiguracoes += $@"
                            <div class='col-md-12'>
                                <div class='checkbox'>
                                    <input id='chk-chatstaff' class='magic-checkbox' type='checkbox' {(player.User.StaffChatToggle ? "checked" : string.Empty)}>
                                    <label for='chk-chatstaff'>Ocultar mensagem do chat da staff</label>
                                </div>
                            </div>";

            htmlConfiguracoes += $@"<div class='col-md-12 text-right'><button id='btn-gravar' onclick='gravar()' class='btn btn-dark' type='button'>Gravar</button></div></div>";

            player.Emit("Server:AbrirPainelControleUsuario",
                html,
                htmlStats,
                htmlConfiguracoes);
        }

        [ClientEvent(nameof(UCPGravar))]
        public static void UCPGravar(MyPlayer player, bool timeStamp, bool dl, bool anuncios, bool pm, bool chatFaccao, bool chatStaff,
            int tipoFonteChat, int tamanhoFonteChat, int linhasChat, bool faction)
        {
            if (tamanhoFonteChat < 12 || tamanhoFonteChat > 18)
            {
                player.SendMessage(MessageType.Error, "Tamanho da fonte do chat deve ser entre 12 e 18.", notify: true);
                return;
            }

            if (linhasChat < 10 || linhasChat > 20)
            {
                player.SendMessage(MessageType.Error, "Quantidade de linhas do chat deve ser entre 10 e 20.", notify: true);
                return;
            }

            player.User.TimeStampToggle = timeStamp;
            player.User.VehicleTagToggle = dl;
            player.User.AnnouncementToggle = anuncios;
            player.User.PMToggle = pm;
            player.User.FactionChatToggle = chatFaccao;
            player.User.StaffChatToggle = chatStaff;
            player.User.ChatFontType = tipoFonteChat;
            player.User.ChatLines = linhasChat;
            player.User.ChatFontSize = tamanhoFonteChat;
            player.User.FactionToggle = faction;
            player.ConfigurarChat();
            player.Emit("dl:Config", player.User.VehicleTagToggle);
            player.SendMessage(MessageType.Success, "Configurações gravadas com sucesso.", notify: true);
        }

        [ClientEvent(nameof(StopAnimation))]
        public static void StopAnimation(MyPlayer player) => player.StopAnimation();

        [ClientEvent(nameof(Waypoint))]
        public static void Waypoint(MyPlayer player, float x, float y, float z) => player.Position = new Position(x, y, z);

        [ClientEvent(nameof(SetarPersonalizacao))]
        public static void SetarPersonalizacao(MyPlayer player, string personalizacao)
        {
            var personalizacaoDados = JsonSerializer.Deserialize<MyPlayer.Personalizacao>(personalizacao);
            player.SetarPersonalizacao(personalizacaoDados);
        }

        [ClientEvent(nameof(SetClothes))]
        public static void SetClothes(MyPlayer player, byte component, ushort drawable, byte texture, string dlc)
        {
            if (!string.IsNullOrWhiteSpace(dlc))
                player.SetDlcClothes(component, drawable, texture, 0, Alt.Hash(dlc));
            else
                player.SetClothes(component, drawable, texture, 0);
        }

        [ClientEvent(nameof(SetProps))]
        public static void SetProps(MyPlayer player, byte component, int drawable, byte texture, string dlc)
        {
            if (drawable == -1)
            {
                player.ClearProps(component);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(dlc))
                    player.SetDlcProps(component, (ushort)drawable, texture, Alt.Hash(dlc));
                else
                    player.SetProps(component, (ushort)drawable, texture);
            }
        }

        [ClientEvent(nameof(ShowPlayerList))]
        public static void ShowPlayerList(MyPlayer player)
        {
            var personagens = Global.Players.Where(x => x.Character.PersonalizationStep == CharacterPersonalizationStep.Ready).ToList();
            var duty = personagens.Where(x => x.OnDuty);
            var rodape = $"Policiais: {duty.Count(x => x.Faction?.Type == FactionType.Police)} | Bombeiros: {duty.Count(x => x.Faction?.Type == FactionType.Firefighter)} | Taxistas: {duty.Count(x => x.Character.Job == CharacterJob.TaxiDriver)} | Mecânicos: {duty.Count(x => x.Character.Job == CharacterJob.Mechanic)}";

            player.Emit("Server:ListarPlayers", Global.SERVER_NAME,
                JsonSerializer.Serialize(personagens.OrderBy(x => x.SessionId == player.SessionId ? 0 : 1).ThenBy(x => x.SessionId)
                .Select(x => new { x.SessionId, x.ICName, x.Ping }).ToList()),
                rodape);
        }

        [AsyncClientEvent(nameof(RegistrarImagemDMV))]
        public async static Task RegistrarImagemDMV(MyPlayer player, string headShot, int valor)
        {
            if (!string.IsNullOrWhiteSpace(headShot))
            {
                player.Character.Image = headShot;
                player.Character.DriverLicenseValidDate = DateTime.Now.AddMonths(3);
                player.Character.PoliceOfficerBlockedDriverLicenseCharacterId = null;

                await player.Save();

                await player.RemoveItem(new CharacterItem(ItemCategory.Money)
                {
                    Quantity = valor
                });

                player.SendMessage(MessageType.Success, $"Você comprou/renovou sua licença de motorista por ${valor:N0}. A validade é {player.Character.DriverLicenseValidDate?.ToShortDateString()}.");
            }
            else
            {
                player.SendMessage(MessageType.Error, "Não foi possível registrar a imagem do seu personagem.");
            }

            await player.SetarRoupas();
        }

        [AsyncClientEvent(nameof(UpdateWeaponAmmo))]
        public static async Task UpdateWeaponAmmo(MyPlayer player, uint weapon)
        {
            try
            {
                var weap = (WeaponModel)weapon;
                var ammo = player.GetWeaponAmmo(weapon);

                var item = player.Items.FirstOrDefault(x => x.Slot < 0 && x.Category == ItemCategory.Weapon && x.Type == weapon);
                if (item == null)
                {
                    await Functions.SendStaffMessage($"{player.Character.Name} tem a arma {weap} para {ammo} sem um item no inventário.", true, true);
                    await player.GravarLog(LogType.Hack, $"Arma sem item no inventário | {weap} | {ammo}", null);
                    return;
                }

                if (ammo == 0)
                {
                    if (weap == WeaponModel.Grenade || weap == WeaponModel.BZGas
                        || weap == WeaponModel.MolotovCocktail || weap == WeaponModel.StickyBomb
                        || weap == WeaponModel.ProximityMines || weap == WeaponModel.Snowballs
                        || weap == WeaponModel.PipeBombs || weap == WeaponModel.Baseball
                        || weap == WeaponModel.TearGas || weap == WeaponModel.Flare
                        || weap == WeaponModel.JerryCan)
                    {
                        await player.RemoveItem(item);
                        return;
                    }
                }

                var extra = JsonSerializer.Deserialize<WeaponItem>(item.Extra);
                extra.Ammo = ammo;
                item.Extra = JsonSerializer.Serialize(extra);
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        [AsyncClientEvent(nameof(VenderPropriedade))]
        public static async Task VenderPropriedade(MyPlayer player) => await Functions.CMDVenderPropriedadeGoverno(player, true);

        [ClientEvent(nameof(KeyY))]
        public static void KeyY(MyPlayer player)
        {
            if (player.Cuffed || player.Character.Wound != CharacterWound.Nenhum)
            {
                player.SendMessage(MessageType.Error, "Você não pode fazer isso algemado ou ferido.");
                return;
            }

            var prox = Global.Properties
                .Where(x => player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)) <= Global.RP_DISTANCE)
                .MinBy(x => player.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)));

            if (prox != null)
            {
                if (prox.Locked)
                {
                    player.SendMessage(MessageType.Error, "A porta está trancada.");
                    return;
                }

                if (player.IsInVehicle)
                {
                    player.SendMessage(MessageType.Error, "Você está dentro de um veículo.");
                    return;
                }

                if (prox.RobberyValue > 0)
                {
                    player.SendMessage(MessageType.Error, Global.ROBBED_PROPERTY_ERROR_MESSAGE);
                    return;
                }

                player.IPLs = Functions.GetIPLsByInterior(prox.Interior);
                player.SetarIPLs();
                player.SetPosition(new Position(prox.ExitPosX, prox.ExitPosY, prox.ExitPosZ), prox.Id, false);
                return;
            }

            var proxEntrada = Global.Spots
                .Where(x => x.Type == SpotType.Entrada && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE)
                .MinBy(x => player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)));
            if (proxEntrada != null)
            {
                player.SetPosition(new Position(proxEntrada.AuxiliarPosX, proxEntrada.AuxiliarPosY, proxEntrada.AuxiliarPosZ), 0, false);
                return;
            }

            prox = Global.Properties
                .Where(x => player.Dimension == x.Id
                    && player.Position.Distance(new Position(x.ExitPosX, x.ExitPosY, x.ExitPosZ)) <= Global.RP_DISTANCE)
                .MinBy(x => player.Position.Distance(new Position(x.ExitPosX, x.ExitPosY, x.ExitPosZ)));

            if (prox != null)
            {
                if (prox.Locked)
                {
                    player.SendMessage(MessageType.Error, "A porta está trancada.");
                    return;
                }

                if (player.IsInVehicle)
                {
                    player.SendMessage(MessageType.Error, "Você está dentro de um veículo.");
                    return;
                }

                player.LimparIPLs();
                player.SetPosition(new Position(prox.EntrancePosX, prox.EntrancePosY, prox.EntrancePosZ), 0, false);
                return;
            }

            player.SendMessage(MessageType.Error, "Você não está próximo de uma entrada ou uma saída.");
        }

        [ClientEvent(nameof(KeyDelete))]
        public static void KeyDelete(MyPlayer player)
        {
            if (player.CancellationTokenSourceAcao == null)
                return;

            player.CancellationTokenSourceAcao?.Cancel();
            player.CancellationTokenSourceAcao = null;
            player.ToggleGameControls(true);
            player.SendMessage(MessageType.Success, "Você cancelou sua ação.");
        }

        [AsyncClientEvent(nameof(ConfiscationSave))]
        public static async Task ConfiscationSave(MyPlayer player, string characterName, string itemsJSON)
        {
            await using var context = new DatabaseContext();
            var character = await context.Characters.FirstOrDefaultAsync(x => x.Name == characterName
                && !x.DeathDate.HasValue && !x.DeletedDate.HasValue);
            if (character == null)
            {
                player.EmitStaffShowMessage($"Nenhum personagem encontrado com o nome {characterName}.");
                return;
            }

            var items = JsonSerializer.Deserialize<List<BaseItem>>(itemsJSON);
            if (items.Count == 0)
            {
                player.EmitStaffShowMessage($"Nenhum item informado.");
                return;
            }

            var confiscationItems = new List<ConfiscationItem>();

            foreach (var item in items)
            {
                var it = player.Items.FirstOrDefault(x => x.Id == item.Id);
                if (item.Quantity > (it?.Quantity ?? 0))
                {
                    player.EmitStaffShowMessage($"Quantidade do item {item.Id} é superior a que você possui no inventário.");
                    return;
                }

                confiscationItems.Add(new ConfiscationItem
                {
                    Id = it.Id,
                    Category = it.Category,
                    Type = it.Type,
                    Quantity = item.Quantity,
                    Extra = it.Extra,
                });
            }

            var confiscation = new Confiscation
            {
                CharacterId = character.Id,
                PoliceOfficerCharacterId = player.Character.Id,
                FactionId = player.Faction.Id,
            };

            await context.Confiscations.AddAsync(confiscation);
            await context.SaveChangesAsync();

            foreach (var confiscationItem in confiscationItems)
                confiscationItem.ConfiscationId = confiscation.Id;

            await context.ConfiscationsItems.AddRangeAsync(confiscationItems);

            await context.SaveChangesAsync();

            foreach (var item in items)
            {
                var it = player.Items.FirstOrDefault(x => x.Id == item.Id);
                if (it != null)
                {
                    await player.RemoveItem(it.IsStack ?
                        new CharacterItem(it.Category)
                        {
                            Quantity = item.Quantity,
                        }
                        :
                        it);
                }
            }

            player.SendMessage(MessageType.Success, $"Registro de confisco {confiscation.Id} criado.");
            player.Emit("Server:CloseView");
        }

        [AsyncClientEvent(nameof(ConfirmTattoos))]
        public static async Task ConfirmTattoos(MyPlayer player, string strTattoos, bool estudio, bool sucesso)
        {
            if (sucesso)
            {
                var tattoos = JsonSerializer.Deserialize<List<MyPlayer.Personalizacao.Tattoo>>(strTattoos);

                if (estudio && !tattoos.Any())
                {
                    player.Emit("Character:ShowMessage", "Nenhum item foi selecionado.");
                    return;
                }

                if (player.Personalization.Tattoos.Count + tattoos.Count > 30)
                {
                    player.Emit("Character:ShowMessage", "O limite de é de 30 tatuagens.");
                    return;
                }

                foreach (var tattoo in tattoos)
                {
                    if (!player.Personalization.Tattoos.Any(x => x.Collection == tattoo.Collection && x.Overlay == tattoo.Overlay))
                        player.Personalization.Tattoos.Add(tattoo);
                }
            }

            if (!estudio)
            {
                player.Character.PersonalizationJSON = JsonSerializer.Serialize(player.Personalization);
                player.Character.PersonalizationStep = CharacterPersonalizationStep.Clothes;
                await using var context = new DatabaseContext();
                context.Characters.Update(player.Character);
                await context.SaveChangesAsync();
            }
            else if (sucesso)
            {
                player.Invincible = false;
                await player.RemoveItem(new CharacterItem(ItemCategory.Money)
                {
                    Quantity = Global.Parameter.TattooValue
                });
                player.SendMessage(MessageType.Success, $"Você pagou ${Global.Parameter.TattooValue:N0} no estúdio de tatuagens.");
            }

            player.SetarPersonalizacao(player.Personalization);
            player.Emit("Character:CloseTattoos");
            player.Emit("Server:SelecionarPersonagem",
                (int)player.Character.PersonalizationStep,
                (int)player.Character.Sex,
                JsonSerializer.Serialize(player.Personalization));
        }

        [ClientEvent(nameof(ConfirmBoombox))]
        public static void ConfirmBoombox(MyPlayer player, ulong itemId, string url, float volume)
        {
            if (!(Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps)))
            {
                player.EmitStaffShowMessage("URL está em um formato inválido.");
                return;
            }

            var item = Global.Items.FirstOrDefault(x => x.Id == itemId);
            if (item == null)
                return;

            item.AudioSpot ??= new AudioSpot
            {
                Position = new Vector3(item.PosX, item.PosY, item.PosZ),
                Dimension = item.Dimension,
            };

            if (item.AudioSpot.Source != url)
                item.AudioSpot.RemoveAllClients();

            item.AudioSpot.Source = url;
            item.AudioSpot.Volume = volume;

            item.AudioSpot.SetupAllClients();

            player.SendMessageToNearbyPlayers($"configura a boombox.", MessageCategory.Ame, 5);
        }

        [ClientEvent(nameof(TurnOffBoombox))]
        public static void TurnOffBoombox(MyPlayer player, ulong itemId)
        {
            var item = Global.Items.FirstOrDefault(x => x.Id == itemId);
            if (item == null)
                return;

            if (item.AudioSpot != null)
            {
                item.AudioSpot.RemoveAllClients();
                player.SendMessageToNearbyPlayers($"desliga a boombox.", MessageCategory.Ame, 5);
                item.AudioSpot = null;
            }
        }

        [AsyncClientEvent(nameof(BuyPropertyUpgrade))]
        public static async Task BuyPropertyUpgrade(MyPlayer player, int propertyId, string name)
        {
            var property = Global.Properties.FirstOrDefault(x => x.Id == propertyId);
            if (property == null)
                return;

            var value = property.Value;

            switch (name)
            {
                case "Proteção Nível 1":
                    value = Convert.ToInt32(Math.Truncate(value * 0.05));
                    break;
                case "Proteção Nível 2":
                    value = Convert.ToInt32(Math.Truncate(value * 0.08));
                    break;
                case "Proteção Nível 3":
                    value = Convert.ToInt32(Math.Truncate(value * 0.15));
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
                    if (property.ProtectionLevel >= 1)
                    {
                        player.SendMessage(MessageType.Error, $"A propriedade já possui um nível de proteção igual ou maior que 1.", notify: true);
                        return;
                    }

                    property.ProtectionLevel = 1;
                    break;
                case "Proteção Nível 2":
                    if (property.ProtectionLevel >= 2)
                    {
                        player.SendMessage(MessageType.Error, $"A propriedade já possui um nível de proteção igual ou maior que 2.", notify: true);
                        return;
                    }

                    property.ProtectionLevel = 2;
                    break;
                case "Proteção Nível 3":
                    if (property.ProtectionLevel >= 3)
                    {
                        player.SendMessage(MessageType.Error, $"A propriedade já possui um nível de proteção igual ou maior que 3.", notify: true);
                        return;
                    }

                    property.ProtectionLevel = 3;
                    break;
            }

            await using var context = new DatabaseContext();
            context.Properties.Update(property);
            await context.SaveChangesAsync();

            await player.RemoveItem(new CharacterItem(ItemCategory.Money)
            {
                Quantity = value
            });
            player.SendMessage(MessageType.Success, $"Você comprou {name}.", notify: true);
        }

        [AsyncScriptEvent(ScriptEventType.ConsoleCommand)]
        public static async Task OnConsoleCommand(string name, string[] args)
        {
            if (name.ToLower() == "gmx")
            {
                foreach (var x in Global.Vehicles)
                    await x.Estacionar(null);

                foreach (var x in Global.Players.Where(x => x.Character.PersonalizationStep == CharacterPersonalizationStep.Ready))
                {
                    x.SendMessage(MessageType.None, $"[{Global.SERVER_INITIALS}] As informações do servidor foram salvas através do console de gerenciamento.", Global.STAFF_COLOR);
                    await x.Save();
                }

                Alt.Log("As informações do servidor foram salvas.");
                await Functions.WriteLog(LogType.Staff, $"GMX pelo console");
            }
        }

        [ScriptEvent(ScriptEventType.ServerEvent)]
        private static void OnServerEvent(string eventName, object[] args)
        {
            Alt.Log($"OnServerEvent | {eventName} | {string.Join(" ; ", args)}");
        }

        [ScriptEvent(ScriptEventType.PlayerEvent)]
        private static void OnPlayerEvent(MyPlayer player, string eventName, object[] args)
        {
            Alt.Log($"OnPlayerEvent | {player.Character.Id} | {eventName} | {string.Join(" ; ", args)}");
        }

        [ScriptEvent(ScriptEventType.ServerCustomEvent)]
        public static void OnServerCustomEvent(string eventName, AltV.Net.Elements.Args.MValueConst[] mValueArray)
        {
            Alt.Log($"OnServerCustomEvent | {eventName} | {string.Join(" ; ", mValueArray)}");
        }

        [ScriptEvent(ScriptEventType.PlayerCustomEvent)]
        public static void OnPlayerCustomEvent(MyPlayer player, string eventName, AltV.Net.Elements.Args.MValueConst[] mValueArray)
        {
            Alt.Log($"OnPlayerCustomEvent | {player.Character.Id} | {eventName} | {string.Join(" ; ", mValueArray)}");
        }

        [AsyncClientEvent(nameof(SetAreaName))]
        public async Task SetAreaName(MyPlayer player, string areaName)
        {
            try
            {
                if (player.Dimension != 0)
                {
                    var prop = Global.Properties.FirstOrDefault(x => x.Id == player.Dimension);
                    if (prop != null)
                        areaName = prop.Address;
                }

                await using var context = new DatabaseContext();
                switch (player.AreaNameType)
                {
                    case 1:
                        var emergencyCall = JsonSerializer.Deserialize<EmergencyCall>(player.AreaNameJSON);
                        emergencyCall.Location = areaName;

                        await context.EmergencyCalls.AddAsync(emergencyCall);
                        await context.SaveChangesAsync();
                        Global.EmergencyCalls.Add(emergencyCall);
                        emergencyCall.SendMessage();
                        break;
                    case 2:
                        var emergencyCall2 = JsonSerializer.Deserialize<EmergencyCall>(player.AreaNameJSON);
                        emergencyCall2.Location = areaName;

                        await context.EmergencyCalls.AddAsync(emergencyCall2);
                        await context.SaveChangesAsync();
                        Global.EmergencyCalls.Add(emergencyCall2);
                        emergencyCall2.SendMessage();

                        await player.EndCellphoneCall();
                        break;
                    case 3:
                        Functions.SendJobMessage(CharacterJob.TaxiDriver, $"Downtown Cab Company | Solicitação de Táxi {{#FFFFFF}}#{player.SessionId}", Global.CELLPHONE_SECONDARY_COLOR);
                        Functions.SendJobMessage(CharacterJob.TaxiDriver, $"De: {{#FFFFFF}}{player.Cellphone}", Global.CELLPHONE_SECONDARY_COLOR);
                        Functions.SendJobMessage(CharacterJob.TaxiDriver, $"Localização: {{#FFFFFF}}{areaName}", Global.CELLPHONE_SECONDARY_COLOR);
                        Functions.SendJobMessage(CharacterJob.TaxiDriver, $"Destino: {{#FFFFFF}}{player.AreaNameJSON}", Global.CELLPHONE_SECONDARY_COLOR);

                        await player.EndCellphoneCall();
                        break;
                    case 4:
                        Functions.SendJobMessage(CharacterJob.Mechanic, $"Central de Mecânicos | Solicitação de Mecânico {{#FFFFFF}}#{player.SessionId}", Global.CELLPHONE_SECONDARY_COLOR);
                        Functions.SendJobMessage(CharacterJob.Mechanic, $"De: {{#FFFFFF}}{player.Cellphone}", Global.CELLPHONE_SECONDARY_COLOR);
                        Functions.SendJobMessage(CharacterJob.Mechanic, $"Localização: {{#FFFFFF}}{areaName}", Global.CELLPHONE_SECONDARY_COLOR);
                        Functions.SendJobMessage(CharacterJob.Mechanic, $"Mensagem: {{#FFFFFF}}{player.AreaNameJSON}", Global.CELLPHONE_SECONDARY_COLOR);

                        await player.EndCellphoneCall();
                        break;
                }
            }
            catch (Exception ex)
            {
                Functions.GetException(ex);
            }
        }

        [ClientEvent(nameof(ClearDecorations))]
        public void ClearDecorations(MyPlayer player)
        {
            player.ClearDecorations();
        }

        [ClientEvent(nameof(AddDecoration))]
        public void AddDecoration(MyPlayer player, string collection, string overlay)
        {
            player.AddDecoration(Alt.Hash(collection), Alt.Hash(overlay));
        }
    }
}