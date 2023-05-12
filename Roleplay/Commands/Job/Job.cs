using AltV.Net;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using AltV.Net.Shared.Enums;
using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace Roleplay.Commands.Job
{
    public class Job 
    {
        [Command("sairemprego")]
        public static async Task CMD_sairemprego(MyPlayer player)
        {
            if (player.Character.Job == CharacterJob.None || player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não tem um emprego ou está em serviço.");
                return;
            }

            var emp = Global.Jobs.FirstOrDefault(x => x.CharacterJob == player.Character.Job);
            if (player.Position.Distance(emp.Position) > Global.RP_DISTANCE)
            {
                player.SendMessage(MessageType.Error, "Você não está onde você pegou esse emprego.");
                return;
            }

            player.Character.ExtraPayment = player.ExtraPayment = 0;
            player.Character.Job = CharacterJob.None;
            foreach (var collectSpot in player.CollectSpots)
            {
                collectSpot.Blip.Destroy();
                collectSpot.Marker.Destroy();
            }
            player.CollectSpots = new List<Spot>();
            await player.GravarLog(LogType.Emprego, "/sairemprego", null);
            player.SendMessage(MessageType.Success, "Você saiu do seu emprego.");
        }

        [Command("emprego")]
        public static async Task CMD_emprego(MyPlayer player)
        {
            if (player.Character.Job != CharacterJob.None)
            {
                player.SendMessage(MessageType.Error, "Você já tem um emprego.");
                return;
            }

            if (player.Faction?.Government ?? false)
            {
                player.SendMessage(MessageType.Error, "Você não pode pegar um emprego pois está em uma facção governamental.");
                return;
            }

            var job = Global.Jobs.FirstOrDefault(x => player.Position.Distance(x.Position) <= Global.RP_DISTANCE);
            if (job == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum local de emprego.");
                return;
            }

            player.Character.Job = job.CharacterJob;
            await player.GravarLog(LogType.Emprego, $"/emprego {player.Character.Job}", null);
            player.SendMessage(MessageType.Success, $"Você pegou o emprego {Functions.GetEnumDisplay(player.Character.Job)}.");
        }

        [Command("chamadas")]
        public static void CMD_chamadas(MyPlayer player)
        {
            if ((player.Character.Job != CharacterJob.TaxiDriver && player.Character.Job != CharacterJob.Mechanic) || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não está em serviço como taxista ou mecânico.");
                return;
            }

            if (player.Character.Job == CharacterJob.TaxiDriver)
            {
                if (player.Vehicle?.Model != (uint)VehicleModel.Taxi)
                {
                    player.SendMessage(MessageType.Error, "Você não está em um taxi.");
                    return;
                }
            }

            var chamadas = Global.Players.Where(x => x.AguardandoTipoServico == player.Character.Job).OrderBy(x => x.Character.Id).ToList();
            if (!chamadas.Any())
            {
                player.SendMessage(MessageType.Error, "Não há nenhuma chamada.");
                return;
            }

            player.SendMessage(MessageType.Title, "Chamadas Aguardando");
            foreach (var c in chamadas)
                player.SendMessage(MessageType.None, $"Chamada #{c.SessionId}");
        }

        [Command("atcha", "/atcha (chamada)")]
        public static void CMD_atcha(MyPlayer player, int chamada)
        {
            if ((player.Character.Job != CharacterJob.TaxiDriver && player.Character.Job != CharacterJob.Mechanic) || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não está em serviço como taxista ou mecânico.");
                return;
            }

            if (player.Character.Job == CharacterJob.TaxiDriver)
            {
                if (player.Vehicle?.Model != (uint)VehicleModel.Taxi)
                {
                    player.SendMessage(MessageType.Error, "Você não está em um taxi.");
                    return;
                }
            }

            var target = Global.Players.FirstOrDefault(x => x.SessionId == chamada && x.AguardandoTipoServico == player.Character.Job);
            if (target == null)
            {
                player.SendMessage(MessageType.Error, "Não há nenhuma chamada com esse código.");
                return;
            }

            target.AguardandoTipoServico = 0;
            player.Emit("Server:SetWaypoint", target.Position.X, target.Position.Y);
            player.SendMessage(MessageType.Success, $"Você está atendendo a chamada {chamada} e a localização do solicitante foi marcada em seu GPS.");
            if (player.Character.Job == CharacterJob.TaxiDriver)
                target.SendMessage(MessageType.None, $"[CELULAR] SMS de {target.ObterNomeContato(Global.TAXI_NUMBER)}: Nosso taxista {player.Character.Name} está atendendo sua chamada. Placa: {player.Vehicle.NumberplateText}. Celular: {player.Cellphone}.", Global.CELLPHONE_MAIN_COLOR);
            else if (player.Character.Job == CharacterJob.Mechanic)
                target.SendMessage(MessageType.None, $"[CELULAR] SMS de {target.ObterNomeContato(Global.MECHANIC_NUMBER)}: Nosso mecânico {player.Character.Name} está atendendo sua chamada. Celular: {player.Cellphone}.", Global.CELLPHONE_MAIN_COLOR);
        }

        [Command("duty", Aliases = new string[] { "trabalho" })]
        public async static void CMD_duty(MyPlayer player)
        {
            if ((!player.Character.FactionId.HasValue || player.Faction?.Type == FactionType.Criminal) && player.Character.Job == CharacterJob.None)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção habilitada e não possui um emprego.");
                return;
            }

            if (!player.Character.FactionId.HasValue)
            {
                if (player.Character.Job == CharacterJob.Garbageman)
                {
                    var emp = Global.Jobs.FirstOrDefault(x => x.CharacterJob == CharacterJob.Garbageman);
                    if (emp.Position.Distance(player.Position) > Global.RP_DISTANCE)
                    {
                        player.SendMessage(MessageType.Error, "Você não está próximo do local de emprego de lixeiro.");
                        return;
                    }

                    player.OnDuty = !player.OnDuty;
                    if (player.OnDuty)
                    {
                        foreach (var ponto in Global.Spots.Where(x => x.Type == SpotType.Lixeiro).OrderBy(x => Guid.NewGuid()).Take(20))
                        {
                            var x = new Spot
                            {
                                PosX = ponto.PosX,
                                PosY = ponto.PosY,
                                PosZ = ponto.PosZ,
                            };

                            x.Blip = Alt.CreateBlip(player, 4, new Position(x.PosX, x.PosY, x.PosZ));
                            x.Blip.Sprite = 1;
                            x.Blip.Name = "Ponto de Coleta";
                            x.Blip.Color = 2;
                            x.Blip.ShortRange = false;
                            x.Blip.ScaleXY = new Vector2(0.5f, 0.5f);
                            x.Blip.Display = 2;

                            x.Marker = new AsyncMarker(Alt.Core, player, MarkerType.MarkerHalo, new Position(x.PosX, x.PosY, x.PosZ - 0.95f), Global.MainRgba)
                            {
                                Scale = new Vector3(1, 1, 1)
                            };

                            player.CollectSpots.Add(x);
                        }

                        player.SendMessage(MessageType.Success, $"Você entrou em serviço.");
                        player.SendMessage(MessageType.None, $"Use {{{Global.MAIN_COLOR}}}/pegarlixo {{#FFFFFF}}para pegar um saco de lixo em uma lixeira e {{{Global.MAIN_COLOR}}}/colocarlixo {{#FFFFFF}}para colocá-lo no caminhão.");
                        player.SendMessage(MessageType.None, $"No seu GPS foram marcados {{{Global.MAIN_COLOR}}}{player.CollectSpots.Count} {{#FFFFFF}}pontos de coleta. Você receberá {{{Global.MAIN_COLOR}}}${Global.Parameter.ExtraPaymentGarbagemanValue:N0} {{#FFFFFF}}por cada ponto completado.");
                        player.SendMessage(MessageType.None, $"Após concluir quantos pontos desejar, retorne e saia de serviço para concluir.");
                    }
                    else
                    {
                        var pontosColetados = 20 - player.CollectSpots.Count;
                        foreach (var collectSpot in player.CollectSpots)
                        {
                            collectSpot.Blip.Destroy();
                            collectSpot.Marker.Destroy(); 
                        }
                        player.CollectSpots = new List<Spot>();

                        player.Character.ExtraPayment += player.ExtraPayment;
                        if (player.ExtraPayment > 0)
                            player.SendMessage(MessageType.None, $"Você realizou {{{Global.MAIN_COLOR}}}{pontosColetados} {{#FFFFFF}}coleta{(pontosColetados > 1 ? "s" : string.Empty)} e {{{Global.MAIN_COLOR}}}${player.ExtraPayment:N0} {{#FFFFFF}}foram adicionados no seu próximo pagamento.");

                        player.ExtraPayment = 0;
                        player.SendMessage(MessageType.Success, $"Você saiu de serviço.");
                    }

                    return;
                }
                player.OnDuty = !player.OnDuty;
                player.SendMessage(MessageType.Success, $"Você {(player.OnDuty ? "entrou em" : "saiu de")} serviço.");
            }
            else
            {
                await using var context = new DatabaseContext();
                if (player.OnDuty)
                {
                    player.FactionDutySession.FinalDate = DateTime.Now;
                    context.Sessions.Update(player.FactionDutySession);
                    player.FactionDutySession = new();
                }
                else
                {
                    player.FactionDutySession = new Session
                    {
                        CharacterId = player.Character.Id,
                        Type = SessionType.FactionDuty,
                    };
                    await context.Sessions.AddAsync(player.FactionDutySession);
                }
                await context.SaveChangesAsync();

                player.OnDuty = !player.OnDuty;
                player.SendFactionMessage($"{player.FactionRank.Name} {player.Character.Name} {(player.OnDuty ? "entrou em" : "saiu de")} serviço.");
            }
        }
    }
}