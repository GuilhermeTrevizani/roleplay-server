using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Enums;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Commands
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

            var emp = Global.Jobs.FirstOrDefault(x => x.CharacterJob == player.Character.Job)!;
            if (player.Position.Distance(emp.Position) > Global.RP_DISTANCE)
            {
                player.SendMessage(MessageType.Error, "Você não está onde você pegou esse emprego.");
                return;
            }

            player.Character.QuitJob();
            player.ExtraPayment = 0;
            foreach (var collectSpot in player.CollectSpots)
                collectSpot.RemoveIdentifier();
            player.CollectSpots = [];
            await player.GravarLog(LogType.Job, "/sairemprego", null);
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

            player.Character.SetJob(job.CharacterJob);
            await player.GravarLog(LogType.Job, $"/emprego {player.Character.Job}", null);
            player.SendMessage(MessageType.Success, $"Você pegou o emprego {player.Character.Job.GetDisplay()}.");
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

            var chamadas = Global.SpawnedPlayers.Where(x => x.AguardandoTipoServico == player.Character.Job).OrderBy(x => x.Character.Id).ToList();
            if (chamadas.Count == 0)
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

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.SessionId == chamada && x.AguardandoTipoServico == player.Character.Job);
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

        [Command("duty", Aliases = ["trabalho"])]
        public async static void CMD_duty(MyPlayer player)
        {
            if ((!player.Character.FactionId.HasValue || player.Faction?.Type == FactionType.Criminal) && player.Character.Job == CharacterJob.None)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção habilitada e não possui um emprego.");
                return;
            }

            if (!player.Character.FactionId.HasValue)
            {
                if (player.Character.Job == CharacterJob.GarbageCollector)
                {
                    var emp = Global.Jobs.FirstOrDefault(x => x.CharacterJob == CharacterJob.GarbageCollector)!;
                    if (emp.Position.Distance(player.Position) > Global.RP_DISTANCE)
                    {
                        player.SendMessage(MessageType.Error, "Você não está próximo do local de emprego de lixeiro.");
                        return;
                    }

                    player.OnDuty = !player.OnDuty;
                    if (player.OnDuty)
                    {
                        foreach (var spot in Global.Spots
                            .Where(x => x.Type == SpotType.GarbageCollector)
                            .OrderBy(x => Guid.NewGuid())
                            .Take(20))
                        {
                            var newSpot = new Spot();
                            newSpot.Create(spot.Type, spot.PosX, spot.PosY, spot.PosZ, 0, 0, 0);

                            var blip = (MyBlip)Alt.CreateBlip(false, 4, new Position(newSpot.PosX, newSpot.PosY, newSpot.PosZ), new MyPlayer[] { player });
                            blip.Sprite = 1;
                            blip.Name = "Ponto de Coleta";
                            blip.Color = 2;
                            blip.ShortRange = false;
                            blip.ScaleXY = new Vector2(0.5f, 0.5f);
                            blip.Display = 2;
                            blip.SpotId = newSpot.Id;

                            // TODO: Rollback commentary when alt:V implements
                            //var marker = (MyMarker)Alt.CreateMarker(player, MarkerType.MarkerHalo, new Position(newSpot.PosX, newSpot.PosY, newSpot.PosZ - 0.95f), Global.MainRgba);
                            //marker.Scale = new Vector3(1, 1, 1);
                            //marker.SpotId = newSpot.Id;

                            player.CollectSpots.Add(newSpot);
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
                            collectSpot.RemoveIdentifier();
                        player.CollectSpots = [];

                        player.Character.AddExtraPayment(player.ExtraPayment);
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
                    player.FactionDutySession!.End();
                    context.Sessions.Update(player.FactionDutySession);
                    player.FactionDutySession = null;
                }
                else
                {
                    player.FactionDutySession = new Session();
                    player.FactionDutySession.Create(player.Character.Id, SessionType.FactionDuty);
                    await context.Sessions.AddAsync(player.FactionDutySession);
                }
                await context.SaveChangesAsync();

                player.OnDuty = !player.OnDuty;
                player.SendFactionMessage($"{player.FactionRank.Name} {player.Character.Name} {(player.OnDuty ? "entrou em" : "saiu de")} serviço.");
            }
        }
    }
}