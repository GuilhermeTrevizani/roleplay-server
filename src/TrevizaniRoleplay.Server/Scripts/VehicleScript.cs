using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class VehicleScript : IScript
    {
        [Command("tunarcomprar")]
        public static void CMD_tunarcomprar(MyPlayer player)
        {
            if (player.Character.Job != CharacterJob.Mechanic)
            {
                player.SendMessage(MessageType.Error, "Você não é um mecânico.");
                return;
            }

            if (!player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não está em serviço.");
                return;
            }

            Functions.CMDTuning(player, null, false);
        }

        [Command("motor")]
        public static void CMD_motor(MyPlayer player) => CMDMotor(player);

        [Command("vcomprarvaga")]
        public static async Task CMD_vcomprarvaga(MyPlayer player)
        {
            if (player.Vehicle is not MyVehicle veh || veh.Driver != player)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_DRIVER_ERROR_MESSAGE);
                return;
            }

            if (veh.VehicleDB.CharacterId != player.Character.Id)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_OWNER_ERROR_MESSAGE);
                return;
            }

            var valor = Global.Parameter.VehicleParkValue;
            if (!veh.VehicleDB.Parked
                || Global.Properties.Any(x => x.CharacterId == player.Character.Id
                    && player.Vehicle.Position.Distance(new Position(x.EntrancePosX, x.EntrancePosY, x.EntrancePosZ)) <= 25))
                valor = 0;

            if (player.Money < valor)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, valor));
                return;
            }

            veh.VehicleDB.ChangePosition(player.Vehicle.Position.X, player.Vehicle.Position.Y, player.Vehicle.Position.Z,
                player.Vehicle.Rotation.Roll, player.Vehicle.Rotation.Pitch, player.Vehicle.Rotation.Yaw);
            veh.VehicleDB.SetParked();

            await using var context = new DatabaseContext();
            context.Vehicles.Update(veh.VehicleDB);
            await context.SaveChangesAsync();

            if (valor > 0)
            {
                await player.RemoveStackedItem(ItemCategory.Money, valor);
                player.SendMessage(MessageType.Success, $"Você alterou a posição de estacionar seu veículo por ${valor:N0}.", notify: true);
            }
            else
            {
                player.SendMessage(MessageType.Success, $"Você alterou a posição de estacionar seu veículo.", notify: true);
            }
        }

        [Command("vestacionar")]
        public static async Task CMD_vestacionar(MyPlayer player)
        {
            if (player.Vehicle is not MyVehicle veh || veh.Driver != player)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_DRIVER_ERROR_MESSAGE);
                return;
            }

            if (veh.VehicleDB.CharacterId == player.Character.Id
                || player.Items.Any(x => x.Category == ItemCategory.VehicleKey && x.Type == veh.VehicleDB.LockNumber))
            {
                if (player.Vehicle.Position.Distance(new Position(veh.VehicleDB.PosX, veh.VehicleDB.PosY, veh.VehicleDB.PosZ)) > Global.RP_DISTANCE)
                {
                    player.Emit("Server:SetWaypoint", veh.VehicleDB.PosX, veh.VehicleDB.PosY);
                    player.SendMessage(MessageType.Error, "Você não está próximo de sua vaga.");
                    return;
                }

                await veh.Estacionar(player);
                player.SendMessage(MessageType.Success, $"Você estacionou o veículo.", notify: true);
                return;
            }

            if (veh.VehicleDB.FactionId == player.Character.FactionId && veh.VehicleDB.FactionId.HasValue)
            {
                if (!Global.Spots.Any(x => x.Type == SpotType.FactionVehicleSpawn
                    && player.Vehicle.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE))
                {
                    player.SendMessage(MessageType.Error, "Você não está próximo de nenhum ponto de spawn de veículos da facção.");
                    return;
                }

                await veh.Estacionar(player);
                player.SendMessage(MessageType.Success, $"Você estacionou o veículo.", notify: true);
                return;
            }

            if (veh.NameInCharge == player.Character.Name)
            {
                var emp = Global.Jobs.FirstOrDefault(x => x.CharacterJob == player.Character.Job)!;
                if (player.Vehicle.Position.Distance(emp.VehicleRentPosition) > Global.RP_DISTANCE)
                {
                    player.SendMessage(MessageType.Error, "Você não está no aluguel de veículos para esse emprego.");
                    return;
                }

                veh.VehicleDB.SetFuel(veh.VehicleDB.GetMaxFuel());
                await veh.Estacionar(player);
                player.SendMessage(MessageType.Success, $"Você estacionou o veículo.", notify: true);
                return;
            }

            player.SendMessage(MessageType.Error, Global.VEHICLE_ACCESS_ERROR_MESSAGE);
        }

        [Command("vlista")]
        public static async Task CMD_vlista(MyPlayer player)
        {
            await using var context = new DatabaseContext();
            var veiculos = (await context.Vehicles.Where(x => x.CharacterId == player.Character.Id && !x.Sold).ToListAsync())
                .Select(x => new
                {
                    x.Id,
                    Model = x.Model.ToUpper(),
                    x.Plate,
                    Spawn = Global.Vehicles.Any(y => y.VehicleDB.Id == x.Id) ? $"<span class='label' style='background-color:{Global.SUCCESS_COLOR}'>SIM</span>" : $"<span class='label' style='background-color:{Global.ERROR_COLOR}'>NÃO</span>",
                    Seized = x.SeizedValue > 0 ? $"<span class='label' style='background-color:{Global.SUCCESS_COLOR}'>SIM (${x.SeizedValue:N0})</span>" : $"<span class='label' style='background-color:{Global.ERROR_COLOR}'>NÃO</span>",
                    Dismantled = x.DismantledValue > 0 ? $"<span class='label' style='background-color:{Global.SUCCESS_COLOR}'>SIM (${x.DismantledValue:N0})</span>" : $"<span class='label' style='background-color:{Global.ERROR_COLOR}'>NÃO</span>",
                }).OrderByDescending(x => Convert.ToInt32(Global.Vehicles.Any(y => y.VehicleDB.Id == x.Id))).ToList();
            if (veiculos.Count == 0)
            {
                player.SendMessage(MessageType.Error, "Você não possui veículos.");
                return;
            }

            player.Emit("Server:SpawnarVeiculos", $"Veículos de {player.Character.Name} [{player.Character.Id}] ({DateTime.Now})", Functions.Serialize(veiculos));
        }

        [Command("vvender", "/vvender (ID ou nome) (valor)")]
        public static void CMD_vvender(MyPlayer player, string idOrName, int valor)
        {
            var vehicle = Global.Vehicles
                .Where(x => x.VehicleDB.CharacterId == player.Character.Id
                    && player.Position.Distance(x.Position) <= Global.RP_DISTANCE
                    && x.Dimension == player.Dimension)
                .MinBy(x => player.Position.Distance(x.Position));

            if (vehicle == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum veículo seu.");
                return;
            }

            if (vehicle.VehicleDB.StaffGift)
            {
                player.SendMessage(MessageType.Error, "Você não pode vender este veículo pois é um benefício da staff.");
                return;
            }

            var target = player.GetCharacterByIdOrName(idOrName, false);
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

            var restricao = Functions.CheckVIPVehicle(vehicle.VehicleDB.Model);
            if (restricao.Item2 != UserVIP.None && (restricao.Item2 > target.User.VIP || (target.User.VIPValidDate ?? DateTime.MinValue) < DateTime.Now))
            {
                player.SendMessage(MessageType.Error, $"O veículo é restrito para VIP {restricao.Item2}.");
                return;
            }

            var convite = new Invite
            {
                Type = InviteType.VendaVeiculo,
                SenderCharacterId = player.Character.Id,
                Value = [vehicle.VehicleDB.Id.ToString(), valor.ToString()],
            };
            target.Invites.RemoveAll(x => x.Type == InviteType.VendaVeiculo);
            target.Invites.Add(convite);

            player.SendMessage(MessageType.Success, $"Você ofereceu seu veículo {vehicle.VehicleDB.Id} para {target.ICName} por ${valor:N0}.");
            target.SendMessage(MessageType.Success, $"{player.ICName} ofereceu para você o veículo {vehicle.VehicleDB.Id} por ${valor:N0}. (/ac {(int)convite.Type} para aceitar ou /rc {(int)convite.Type} para recusar)");
        }

        [Command("vliberar", "/vliberar (código do veículo)")]
        public static async Task CMD_vliberar(MyPlayer player, string idString)
        {
            if (!Global.Spots.Any(x => x.Type == SpotType.VehicleRelease
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE))
            {
                player.SendMessage(MessageType.Error, "Você não está em ponto de liberação de veículos apreendidos.");
                return;
            }

            var id = idString.ToGuid();
            if (Global.Vehicles.Any(x => x.VehicleDB.Id == id))
            {
                player.SendMessage(MessageType.Error, "Veículo está spawnado.");
                return;
            }

            await using var context = new DatabaseContext();
            var veh = await context.Vehicles.FirstOrDefaultAsync(x => x.CharacterId == player.Character.Id && x.Id == id);
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_OWNER_ERROR_MESSAGE);
                return;
            }

            if (veh.SeizedValue == 0 && veh.DismantledValue == 0)
            {
                player.SendMessage(MessageType.Error, "Veículo não está apreendido ou na seguradora.");
                return;
            }

            var value = veh.SeizedValue > 0 ? veh.SeizedValue : veh.DismantledValue;

            if (player.Money < value)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, value));
                return;
            }

            await player.RemoveStackedItem(ItemCategory.Money, value);

            if (veh.SeizedValue > 0)
            {
                var seizedVehicle = await context.SeizedVehicles.Where(x => x.VehicleId == veh.Id).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                seizedVehicle!.Pay();
                context.SeizedVehicles.Update(seizedVehicle);
                await context.SaveChangesAsync();
            }

            player.SendMessage(MessageType.Success, $"Você liberou seu veículo por ${value:N0}.");
            veh.ResetSeizedAndDismantled();
            context.Vehicles.Update(veh);
            await context.SaveChangesAsync();
        }

        [Command("vporta", "/vporta (porta [1-4])", Aliases = ["vp"])]
        public static void CMD_vporta(MyPlayer player, int porta)
        {
            if (porta < 1 || porta > 4)
            {
                player.SendMessage(MessageType.Error, "Porta deve ser entre 1 e 4.");
                return;
            }

            var veh = Global.Vehicles.Where(x => player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)) <= Global.RP_DISTANCE
                && x.Dimension == player.Dimension
                && x.LockState == VehicleLockState.Unlocked)
                .MinBy(x => player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)));

            if (veh == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum veículo destrancado.");
                return;
            }

            porta--;
            veh.DoorsStates[porta] = !veh.DoorsStates[porta];
            player.Emit("SetVehicleDoorState", veh, porta, veh.DoorsStates[porta]);
        }

        [Command("capo")]
        public static void CMD_capo(MyPlayer player)
        {
            var veh = Global.Vehicles.Where(x => player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)) <= Global.RP_DISTANCE
                && x.Dimension == player.Dimension
                && x.LockState == VehicleLockState.Unlocked)
                .MinBy(x => player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)));

            if (veh == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum veículo destrancado.");
                return;
            }

            veh.DoorsStates[4] = !veh.DoorsStates[4];
            player.SendMessageToNearbyPlayers($"{(veh.DoorsStates[4] ? "fecha" : "abre")} o capô do veículo.", MessageCategory.Ame, 5);
            player.Emit("SetVehicleDoorState", veh, 4, veh.DoorsStates[4]);
        }

        [Command("portamalas", "/portamalas (opção [abrir, fechar, ver])")]
        public static void CMD_portamalas(MyPlayer player, string option)
        {
            var veh = Global.Vehicles.Where(x => x.Dimension == player.Dimension
                && player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)) <= Global.RP_DISTANCE)
                .MinBy(x => player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)));

            if (veh == null || !(veh?.HasStorage ?? false) || veh?.VehicleDB?.Job != CharacterJob.None)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de um veículo que possui armazenamento.");
                return;
            }

            option = option.ToLower();
            if (option == "abrir" || option == "fechar")
            {
                if (veh.LockState != VehicleLockState.Unlocked
                    && !(player.Faction?.Type == FactionType.Police && player.OnDuty))
                {
                    player.SendMessage(MessageType.Error, "O veículo está trancado.");
                    return;
                }

                veh.DoorsStates[5] = option == "fechar";
                player.SendMessageToNearbyPlayers($"{(veh.DoorsStates[5] ? "fecha" : "abre")} o porta-malas do veículo.", MessageCategory.Ame, 5);
                player.Emit("SetVehicleDoorState", veh, 5, veh.DoorsStates[5]);
            }
            else if (option == "ver")
            {
                if (player.IsInVehicle)
                {
                    player.SendMessage(MessageType.Error, "Você não pode fazer isso estando dentro do veículo.");
                    return;
                }

                if (veh.DoorsStates[5])
                {
                    player.SendMessage(MessageType.Error, "O porta-malas está fechado.");
                    return;
                }

                if (veh.VehicleDB.FactionId.HasValue && veh.VehicleDB.FactionId != player.Character.FactionId)
                {
                    player.SendMessage(MessageType.Error, Global.VEHICLE_ACCESS_ERROR_MESSAGE);
                    return;
                }

                veh.ShowInventory(player, false);
            }
            else
            {
                player.SendMessage(MessageType.Error, "Opção inválida. Opções disponíveis: abrir, fechar, ver");
            }
        }

        [Command("abastecer")]
        public static void CMD_abastecer(MyPlayer player)
        {
            if (player.IsInVehicle)
            {
                player.SendMessage(MessageType.Error, "Você deve estar fora do veículo.");
                return;
            }

            var veh = Global.Vehicles.Where(x => player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)) <= 5
                && x.Dimension == player.Dimension
                && x.LockState == VehicleLockState.Unlocked)
                .OrderBy(x => player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)))
                .FirstOrDefault();

            if (veh == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum veículo destrancado.");
                return;
            }

            if (veh.VehicleDB.Fuel == veh.VehicleDB.GetMaxFuel())
            {
                player.SendMessage(MessageType.Error, "Veículo está com tanque cheio.");
                return;
            }

            player.Emit("Server:Abastecer", veh.VehicleDB.Id.ToString());
        }

        [Command("vplaca", "/vplaca (código do veículo) (placa)")]
        public static async Task CMD_vplaca(MyPlayer player, string idString, string plate)
        {
            if (plate.Length > 8)
            {
                player.SendMessage(MessageType.Error, "A placa deve ter até 8 caracteres.");
                return;
            }

            if (player.User.PlateChanges == 0)
            {
                player.SendMessage(MessageType.Error, "Você não possui uma mudança de placa.");
                return;
            }

            var id = idString.ToGuid();
            if (Global.Vehicles.Any(x => x.VehicleDB.Id == id))
            {
                player.SendMessage(MessageType.Error, $"Veículo {id} está spawnado.");
                return;
            }

            await using var context = new DatabaseContext();
            var vehicle = await context.Vehicles.FirstOrDefaultAsync(x => x.CharacterId == player.Character.Id && x.Id == id);
            if (vehicle == null)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_OWNER_ERROR_MESSAGE);
                return;
            }

            if (await context.Vehicles.AnyAsync(x => x.Plate.Equals(plate, StringComparison.CurrentCultureIgnoreCase)))
            {
                player.SendMessage(MessageType.Error, $"Já existe um veículo com a placa {plate.ToUpper()}.");
                return;
            }

            player.User.RemovePlateChanges();
            vehicle.SetPlate(plate.ToUpper());
            context.Vehicles.Update(vehicle);
            await context.SaveChangesAsync();
            await player.Save();

            player.SendMessage(MessageType.Success, $"Você alterou a placa do veículo {vehicle.Id} para {vehicle.Plate}.");
            await player.GravarLog(LogType.PlateChange, $"{vehicle.Id} {vehicle.Plate}", null);
        }

        [Command("vvenderconce", "/vvenderconce (código do veículo)")]
        public static async Task CMD_vvenderconce(MyPlayer player, string id) => await CMDVenderVeiculoConcessionaria(player, new Guid(id), false);

        [Command("valugar")]
        public static async Task CMD_valugar(MyPlayer player)
        {
            if (player.Character.Job == CharacterJob.None || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não tem um emprego ou não está em serviço.");
                return;
            }

            var emp = Global.Jobs.FirstOrDefault(x => x.CharacterJob == player.Character.Job)!;
            if (!player.IsInVehicle)
            {
                if (player.Position.Distance(emp.VehicleRentPosition) > Global.RP_DISTANCE)
                {
                    player.SendMessage(MessageType.Error, "Você não está no aluguel de veículos para seu emprego.");
                    return;
                }
            }

            if (Global.Vehicles.Any(x => x.NameInCharge == player.Character.Name))
            {
                player.SendMessage(MessageType.Error, "Você já possui um veículo alugado.");
                return;
            }

            var preco = Convert.ToInt32(Math.Abs(Global.Prices.FirstOrDefault(x => x.Type == PriceType.JobVehicleRental && x.Name.Equals(player.Character.Job.ToString(), StringComparison.CurrentCultureIgnoreCase))?.Value ?? 0));
            if (player.Money < preco)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, preco));
                return;
            }

            MyVehicle? veh = null;
            if (player.IsInVehicle)
            {
                veh = Global.Vehicles.FirstOrDefault(x => x == player.Vehicle && x.VehicleDB.Job == player.Character.Job && !x.RentExpirationDate.HasValue);
                if (veh == null)
                {
                    player.SendMessage(MessageType.Error, "Você não está dentro de um veículo disponível para aluguel.");
                    return;
                }
            }
            else
            {
                await using var context = new DatabaseContext();
                var vehicles = await context.Vehicles
                    .Where(x => x.Job == player.Character.Job && !x.Sold)
                        .Include(x => x.Items)
                    .ToListAsync();
                var vehicle = vehicles.FirstOrDefault(x => !Global.Vehicles.Any(y => y.VehicleDB.Id == x.Id));
                if (vehicle == null)
                {
                    player.SendMessage(MessageType.Error, "Não há nenhum veículo disponível para aluguel.");
                    return;
                }

                vehicle.ChangePosition(emp.VehicleRentPosition.X, emp.VehicleRentPosition.Y, emp.VehicleRentPosition.Z,
                    emp.VehicleRentRotation.Roll, emp.VehicleRentRotation.Pitch, emp.VehicleRentRotation.Yaw);
                vehicle.SetFuel(vehicle.GetMaxFuel());
                veh = await vehicle.Spawnar(player);
                veh.LockState = VehicleLockState.Unlocked;
                player.SetIntoVehicle(veh, 1);
            }

            veh.NameInCharge = player.Character.Name;
            veh.RentExpirationDate = DateTime.Now.AddHours(1);

            await player.RemoveStackedItem(ItemCategory.Money, preco);
            player.SendMessage(MessageType.Success, $"Você alugou um {emp.VehicleModel.ToString().ToUpper()} por ${preco:N0} com expiração em {veh.RentExpirationDate}.");
        }

        [Command("danos", "/danos (veículo)")]
        public static void CMD_danos(MyPlayer player, int id)
        {
            var veh = Global.Vehicles.FirstOrDefault(x => x.Id == id);
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, $"Nenhum veículo encontrado com o código {id}.");
                return;
            }

            if (player.Position.Distance(veh.Position) > Global.RP_DISTANCE || player.Dimension != veh.Dimension || veh.Damages.Count == 0)
            {
                player.SendMessage(MessageType.Error, "Veículo não está próximo ou não possui danos.");
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
                    </tr>
                </thead>
                <tbody>";

            foreach (var x in veh.Damages)
                html += $@"<tr><td>{x.Date}</td><td>{(WeaponModel)x.WeaponHash}</td><td>{x.BodyHealthDamage}</td><td>{x.AdditionalBodyHealthDamage}</td><td>{x.EngineHealthDamage}</td><td>{x.PetrolTankDamage}</td></tr>";

            html += $@"
                </tbody>
            </table>
            </div>";

            player.Emit("Server:BaseHTML", Functions.GetBaseHTML($"Danos do veículo {veh.VehicleDB.Model.ToUpper()} [{veh.VehicleDB.Id}] ({veh.VehicleDB.Plate})", html));
        }

        [Command("velmax", "/velmax (velocidade)")]
        public static void CMD_velmax(MyPlayer player, int velocidade)
        {
            if (velocidade < 5 && velocidade != 0)
            {
                player.SendMessage(MessageType.Error, "Velocidade precisa ser maior que 5 ou igual a 0.");
                return;
            }

            if (player.Vehicle is not MyVehicle veh || veh.Driver != player)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_DRIVER_ERROR_MESSAGE);
                return;
            }

            if (!veh.CanAccess(player))
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_ACCESS_ERROR_MESSAGE);
                return;
            }

            if (veh.Speed > velocidade && velocidade != 0)
            {
                player.SendMessage(MessageType.Error, "A velocidade do veículo está acima da velocidade máxima pretendida.");
                return;
            }

            player.Emit("SetVehicleMaxSpeed", velocidade / 3.6F);
            player.SendMessage(MessageType.Success, velocidade == 0 ? "Você removeu a limitação de velocidade do veículo." :
                $"Você alterou a velocidade máxima do veículo para {velocidade} km/h.");
        }

        [Command("janela", "/janela (opção [fe, fd, te, td, todas])", Aliases = ["janelas", "ja"])]
        public static void CMD_janela(MyPlayer player, string janela)
        {
            if (player.Vehicle is not MyVehicle veh)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_ERRO_FORA_VEICULO);
                return;
            }

            if (!veh.HasWindows)
            {
                player.SendMessage(MessageType.Error, "Veículo não possui janelas.");
                return;
            }

            string texto;
            byte idJanela;

            switch (janela.ToLower())
            {
                case "todas":
                    if (player.Vehicle.Driver != player)
                    {
                        player.SendMessage(MessageType.Error, "Você não é o motorista do veículo.");
                        return;
                    }

                    texto = "todas as janelas";
                    idJanela = 255;
                    break;
                case "fe":
                    if (player.Vehicle.Driver != player)
                    {
                        player.SendMessage(MessageType.Error, "Você não é o motorista do veículo.");
                        return;
                    }

                    texto = "a janela frontal esquerda";
                    idJanela = 0;
                    break;
                case "fd":
                    if (player.Vehicle.Driver != player && player.Seat != 2)
                    {
                        player.SendMessage(MessageType.Error, "Você não é o motorista do veículo ou não está no banco dianteiro direito.");
                        return;
                    }

                    texto = "a janela frontal direita";
                    idJanela = 1;
                    break;
                case "te":
                    if (player.Vehicle.Driver != player && player.Seat != 3)
                    {
                        player.SendMessage(MessageType.Error, "Você não é o motorista do veículo ou não está no banco traseiro esquerdo.");
                        return;
                    }

                    texto = "a janela traseira esquerda";
                    idJanela = 2;
                    break;
                case "td":
                    if (player.Vehicle.Driver != player && player.Seat != 4)
                    {
                        player.SendMessage(MessageType.Error, "Você não é o motorista do veículo ou não está no banco traseiro direito.");
                        return;
                    }

                    texto = "a janela traseira direita";
                    idJanela = 3;
                    break;
                default:
                    player.SendMessage(MessageType.Error, "O parâmetro informado é inválido. Opções: fe, fd, te, td, todas");
                    return;
            }

            var status = !player.Vehicle.IsWindowOpened(idJanela == 255 ? (byte)0 : idJanela);
            if (idJanela == 255)
            {
                player.Vehicle.SetWindowOpened(0, status);
                player.Vehicle.SetWindowOpened(1, status);
                player.Vehicle.SetWindowOpened(2, status);
                player.Vehicle.SetWindowOpened(3, status);
            }
            else
            {
                player.Vehicle.SetWindowOpened(idJanela, status);
            }

            foreach (var target in Global.SpawnedPlayers.Where(x => x.Vehicle == veh))
                target.SetCanDoDriveBy(target.Seat, target == player || idJanela == 255 ? status : null);

            player.SendMessageToNearbyPlayers($"{(status ? "abaixa" : "levanta")} {texto} do veículo.", MessageCategory.Ame, 5);
        }

        [Command("vfechadura")]
        public async static Task CMD_vfechadura(MyPlayer player)
        {
            if (player.Vehicle is not MyVehicle veh || veh.Driver != player)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_DRIVER_ERROR_MESSAGE);
                return;
            }

            if (veh.VehicleDB.CharacterId != player.Character.Id)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_OWNER_ERROR_MESSAGE);
                return;
            }

            if (player.Money < Global.Parameter.LockValue)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, Global.Parameter.LockValue));
                return;
            }

            await using var context = new DatabaseContext();
            veh.VehicleDB.SetLockNumber(await context.Vehicles.MaxAsync(x => x.LockNumber) + 1);
            context.Vehicles.Update(veh.VehicleDB);
            await context.SaveChangesAsync();

            await player.RemoveStackedItem(ItemCategory.Money, Global.Parameter.LockValue);

            player.SendMessage(MessageType.Success, $"Você trocou a fechadura do veículo {veh.VehicleDB.Id} por ${Global.Parameter.LockValue:N0}.");
        }

        [Command("vchave")]
        public async static Task CMD_vchave(MyPlayer player)
        {
            if (player.Vehicle is not MyVehicle veh || veh.Driver != player)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_DRIVER_ERROR_MESSAGE);
                return;
            }

            if (veh.VehicleDB.CharacterId != player.Character.Id)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_OWNER_ERROR_MESSAGE);
                return;
            }

            if (player.Money < Global.Parameter.KeyValue)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, Global.Parameter.KeyValue));
                return;
            }

            var characterItem = new CharacterItem();
            characterItem.Create(ItemCategory.VehicleKey, veh.VehicleDB.LockNumber, 1, null);
            var res = await player.GiveItem(characterItem);
            if (!string.IsNullOrWhiteSpace(res))
            {
                player.SendMessage(MessageType.Error, res);
                return;
            }

            await player.RemoveStackedItem(ItemCategory.Money, Global.Parameter.KeyValue);

            player.SendMessage(MessageType.Success, $"Você criou uma cópia da chave para o veículo {veh.VehicleDB.Id} por ${Global.Parameter.KeyValue:N0}.");
        }

        [Command("usargalao")]
        public static async Task CMD_usargalao(MyPlayer player)
        {
            if (player.IsInVehicle)
            {
                player.SendMessage(MessageType.Error, "Você deve estar fora do veículo.");
                return;
            }

            var vehicle = Global.Vehicles.Where(x => player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)) <= 5
                && x.Dimension == player.Dimension
                && x.LockState == VehicleLockState.Unlocked)
                .OrderBy(x => player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)))
                .FirstOrDefault();

            if (vehicle == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum veículo destrancado.");
                return;
            }

            if (vehicle.VehicleDB.Fuel == vehicle.VehicleDB.GetMaxFuel())
            {
                player.SendMessage(MessageType.Error, "Veículo está com tanque cheio.");
                return;
            }

            if (player.CurrentWeapon != (uint)WeaponModel.JerryCan)
            {
                player.SendMessage(MessageType.Error, "Você não está com um galão de combustível em mãos.");
                return;
            }

            var jerryCan = player.Items.FirstOrDefault(x => x.Category == ItemCategory.Weapon
                && x.Type == player.CurrentWeapon
                && x.Slot < 0);
            if (jerryCan == null)
            {
                player.SendMessage(MessageType.Error, "Você não está com um galão de combustível em mãos.");
                return;
            }

            var extra = Functions.Deserialize<WeaponItem>(jerryCan.Extra);
            var fuel = Convert.ToInt32(Math.Ceiling(extra.Ammo / 50f));
            var necessaryFuel = vehicle.VehicleDB.GetMaxFuel() - vehicle.VehicleDB.Fuel;
            if (necessaryFuel > fuel)
                necessaryFuel = fuel;

            vehicle.VehicleDB.SetFuel(vehicle.VehicleDB.Fuel + necessaryFuel);
            vehicle.SetStreamSyncedMetaData(Constants.VEHICLE_META_DATA_FUEL, vehicle.FuelHUD);

            fuel -= necessaryFuel;
            if (fuel == 0)
            {
                await player.RemoveItem(jerryCan);
            }
            else
            {
                extra.Ammo = fuel * 50;
                jerryCan.SetExtra(Functions.Serialize(extra));
                await using var context = new DatabaseContext();
                context.CharactersItems.Update(jerryCan);
                await context.SaveChangesAsync();
            }

            player.SendMessage(MessageType.Success, $"Você usou um galão de combustível e abasteceu seu veículo com {necessaryFuel} litro{(necessaryFuel > 1 ? "s" : string.Empty)}.");
            player.SendMessageToNearbyPlayers("abastece o veículo usando um galão de combustível.", MessageCategory.Ame, 10);
        }

        [Command("drift")]
        public static void CMD_drift(MyPlayer player)
        {
            if (player.Vehicle is not MyVehicle veh || veh.Driver != player)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_DRIVER_ERROR_MESSAGE);
                return;
            }

            if (!veh.CanAccess(player))
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_ACCESS_ERROR_MESSAGE);
                return;
            }

            veh.DriftMode = !veh.DriftMode;

            player.SendMessage(MessageType.Success, $"Você {(veh.DriftMode ? "des" : string.Empty)}ativou o modo drift do veículo.");
        }

        [Command("vupgrade", "/vupgrade (código do veículo)")]
        public static async Task CMD_vupgrade(MyPlayer player, string idString)
        {
            var id = idString.ToGuid();
            if (Global.Vehicles.Any(x => x.VehicleDB.Id == id))
            {
                player.SendMessage(MessageType.Error, $"Veículo {idString} está spawnado.");
                return;
            }

            await using var context = new DatabaseContext();
            var veh = await context.Vehicles.FirstOrDefaultAsync(x => x.CharacterId == player.Character.Id && x.Id == id && !x.Sold);
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_OWNER_ERROR_MESSAGE);
                return;
            }

            var preco = Global.Prices.FirstOrDefault(x => x.IsVehicle && x.Name.Equals(veh.Model, StringComparison.CurrentCultureIgnoreCase));

            var conce = Global.Dealerships.FirstOrDefault(x => x.PriceType == preco?.Type);
            if (conce == null || player.Position.Distance(conce.Position) > Global.RP_DISTANCE)
            {
                player.SendMessage(MessageType.Error, $"Você não está na concessionária que vende este veículo.");
                return;
            }

            var itemsJSON = Functions.Serialize(
                new List<dynamic>
                {
                    new
                    {
                        Name = "Proteção Nível 1",
                        Value = $"${Math.Truncate(preco.Value * 0.05):N0}",
                    },
                    new
                    {
                        Name = "Proteção Nível 2",
                        Value = $"${Math.Truncate(preco.Value * 0.08):N0}",
                    },
                    new
                    {
                        Name = "Proteção Nível 3",
                        Value = $"${Math.Truncate(preco.Value * 0.2):N0}",
                    },
                    new
                    {
                        Name = "XMR",
                        Value = $"${Math.Truncate(preco.Value * 0.01):N0}",
                    },
                }
            );

            player.Emit("VehicleUpgrade", $"Upgrades • {veh.Model.ToUpper()} {veh.Plate} [{veh.Id}]", veh.Id, itemsJSON);
        }

        [Command("vrastrear", "/vrastrear (código do veículo)")]
        public static async Task CMD_vrastrear(MyPlayer player, string idString)
        {
            var id = idString.ToGuid();
            var veh = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == id);
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, $"Veículo {idString} não está spawnado.");
                return;
            }

            if (veh.VehicleDB.ProtectionLevel < 1)
            {
                player.SendMessage(MessageType.Error, $"Veículo {idString} não possui rastreador.");
                return;
            }

            player.ToggleGameControls(false);
            player.SendMessage(MessageType.Success, $"Aguarde 15 segundos. Pressione DELETE para cancelar a ação.");
            player.CancellationTokenSourceAcao?.Cancel();
            player.CancellationTokenSourceAcao = new System.Threading.CancellationTokenSource();
            await Task.Delay(15000, player.CancellationTokenSourceAcao.Token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;

                player.ToggleGameControls(true);
                player.Emit("Server:SetWaypoint", veh.Position.X, veh.Position.Y);
                player.SendMessage(MessageType.Success, "A posição do veículo foi marcada no GPS.");
                player.CancellationTokenSourceAcao = null;
            });
        }

        [Command("xmr")]
        public static void CMD_xmr(MyPlayer player)
        {
            if (player.Vehicle is not MyVehicle veh || veh.Driver != player)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_DRIVER_ERROR_MESSAGE);
                return;
            }

            if (!veh.CanAccess(player))
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_ACCESS_ERROR_MESSAGE);
                return;
            }

            if (!veh.VehicleDB.XMR)
            {
                player.SendMessage(MessageType.Error, "O veículo não possui XMR.");
                return;
            }

            player.Emit("XMR", veh.VehicleDB.Id, veh.AudioSpot?.Source ?? string.Empty, veh.AudioSpot?.Volume ?? 1);
        }

        [Command("quebrartrava")]
        public static async Task CMD_quebrartrava(MyPlayer player)
        {
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

            var veh = Global.Vehicles.Where(x =>
                player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)) <= 5
                && x.Dimension == player.Dimension
                && x.LockState == VehicleLockState.Locked
                && x.VehicleDB.CharacterId.HasValue)
                .MinBy(x => player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)));

            if (veh == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum veículo trancado.");
                return;
            }

            veh.ActivateProtection(player);

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
                    veh.LockState = VehicleLockState.Unlocked;
                    player.ToggleGameControls(true);
                    player.SendMessageToNearbyPlayers("quebra a trava do veículo.", MessageCategory.Ame, 5);
                    await player.GravarLog(LogType.LockBreak, veh.VehicleDB.Id.ToString(), null);
                    player.CancellationTokenSourceAcao = null;
                });
            });
        }

        [Command("reparar")]
        public static async Task CMD_reparar(MyPlayer player)
        {
            var price = Global.Prices.FirstOrDefault(x => x.Type == PriceType.Tuning && x.Name.Equals("repair", StringComparison.CurrentCultureIgnoreCase));
            if (price == null)
            {
                player.SendMessage(MessageType.Error, $"Não foi configurado o preço \"repair\" da categoria {PriceType.Tuning.GetDisplay()}.");
                return;
            }

            if (Global.SpawnedPlayers.Any(x => x.OnDuty && x.Character.Job == CharacterJob.Mechanic))
            {
                player.SendMessage(MessageType.Error, "Não é possível reparar pois há mecânicos em serviço.");
                return;
            }

            var emprego = Global.Jobs.FirstOrDefault(x => x.CharacterJob == CharacterJob.Mechanic);
            if (player.Position.Distance(new Position(emprego.VehicleRentPosition.X, emprego.VehicleRentPosition.Y, emprego.VehicleRentPosition.Z)) > Global.RP_DISTANCE)
            {
                player.SendMessage(MessageType.Error, "Você não está na central de mecânicos.");
                return;
            }

            if (player.Vehicle is not MyVehicle veh || veh == null || veh.Driver != player)
            {
                player.SendMessage(MessageType.Error, "Você não está dirigindo um veículo.");
                return;
            }

            var vehiclePrice = Global.Prices.FirstOrDefault(x => x.IsVehicle && x.Name.Equals(veh.VehicleDB.Model, StringComparison.CurrentCultureIgnoreCase));
            if (vehiclePrice == null)
            {
                player.SendMessage(MessageType.Error, "Preço do veículo não foi encontrado.");
                return;
            }

            var value = Convert.ToInt32(Math.Abs(vehiclePrice.Value * (price.Value / 100) * 2.5));
            if (player.Money < value)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, value));
                return;
            }

            player.ToggleGameControls(false);
            player.SendMessage(MessageType.Success, $"Você irá reparar seu veículo por ${value:N0}. Aguarde 30 segundos. Pressione DELETE para cancelar a ação.");
            player.CancellationTokenSourceAcao?.Cancel();
            player.CancellationTokenSourceAcao = new System.Threading.CancellationTokenSource();
            await Task.Delay(30000, player.CancellationTokenSourceAcao.Token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;

                Task.Run(async () =>
                {
                    veh = await veh.Reparar();
                    await player.RemoveStackedItem(ItemCategory.Money, value);
                    player.SendMessage(MessageType.Success, $"Você consertou o veículo e pagou ${value:N0}.");
                    player.ToggleGameControls(true);

                    await player.GravarLog(LogType.PlayerVehicleRepair, veh.VehicleDB.Id.ToString(), null);
                    player.CancellationTokenSourceAcao = null;
                });
            });
        }

        [Command("ligacaodireta", Aliases = ["hotwire"])]
        public static async Task CMD_ligacaodireta(MyPlayer player)
        {
            if (player.Vehicle is not MyVehicle veh || veh.Driver != player)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_DRIVER_ERROR_MESSAGE);
                return;
            }

            veh.ActivateProtection(player);
            player.ToggleGameControls(false);
            player.SendMessage(MessageType.Success, $"Aguarde 50 segundos. Pressione DELETE para cancelar a ação.");
            player.CancellationTokenSourceAcao?.Cancel();
            player.CancellationTokenSourceAcao = new System.Threading.CancellationTokenSource();
            await Task.Delay(50000, player.CancellationTokenSourceAcao.Token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;

                Task.Run(async () =>
                {
                    if (veh.VehicleDB.ProtectionLevel < 2)
                        veh.StopAlarm();
                    veh.EngineOn = true;
                    player.ToggleGameControls(true);
                    player.SendMessageToNearbyPlayers("faz uma ligação direta no veículo.", MessageCategory.Ame, 5);
                    await player.GravarLog(LogType.HotWire, veh.VehicleDB.Id.ToString(), null);
                    player.CancellationTokenSourceAcao = null;
                });
            });
        }

        [Command("desmanchar")]
        public static async Task CMD_desmanchar(MyPlayer player)
        {
            if ((player.User.CooldownDismantle ?? DateTime.MinValue) > DateTime.Now)
            {
                player.SendMessage(MessageType.Error, $"Aguarde o cooldown para desmanchar novamente. Será liberado em {player.User.CooldownDismantle}.");
                return;
            }

            var veh = Global.Vehicles.Where(x =>
                player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)) <= 5
                && x.Dimension == player.Dimension
                && x.LockState == VehicleLockState.Unlocked
                && x.EngineOn
                && x.VehicleDB.CharacterId != player.Character.Id
                && x.VehicleDB.CharacterId.HasValue)
                .MinBy(x => player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)));

            if (veh == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum veículo destrancado e com o motor ligado.");
                return;
            }

            var preco = Global.Prices.FirstOrDefault(x => x.IsVehicle && x.Name.Equals(veh.VehicleDB.Model, StringComparison.CurrentCultureIgnoreCase));
            if (preco == null)
            {
                player.SendMessage(MessageType.Error, "Preço não encontrado.");
                return;
            }

            veh.ActivateProtection(player);
            player.ToggleGameControls(false);
            player.SendMessage(MessageType.Success, $"Aguarde 300 segundos. Pressione DELETE para cancelar a ação.");
            player.CancellationTokenSourceAcao?.Cancel();
            player.CancellationTokenSourceAcao = new System.Threading.CancellationTokenSource();
            await Task.Delay(300_000, player.CancellationTokenSourceAcao.Token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;

                Task.Run(async () =>
                {
                    var value = Convert.ToInt32(Math.Truncate(preco.Value * 0.1));
                    var characterItem = new CharacterItem();
                    characterItem.Create(ItemCategory.Money, 0, value, null);
                    var res = await player.GiveItem(characterItem);
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.SendMessage(MessageType.Error, res);
                        return;
                    }

                    veh.VehicleDB.SetDismantledValue(value);
                    await veh.Estacionar(null);

                    player.User.SetCooldownDismantle(DateTime.Now.AddHours(Global.Parameter.CooldownDismantleHours));
                    player.ToggleGameControls(true);
                    player.SendMessageToNearbyPlayers("desmancha o veículo.", MessageCategory.Ame, 5);
                    player.SendMessage(MessageType.Success, $"Você desmanchou o veículo e recebeu ${value:N0}.");
                    await player.GravarLog(LogType.Dismantling, $"{veh.VehicleDB.Id} {value}", null);
                    player.CancellationTokenSourceAcao = null;
                });
            });
        }

        [Command("rebocar", "/rebocar (id)")]
        public static void CMD_rebocar(MyPlayer player, int id)
        {
            if (player.Vehicle is not MyVehicle veh ||
                !(veh?.VehicleDB?.Model?.ToUpper()).Equals(VehicleModel.Flatbed.ToString(), StringComparison.CurrentCultureIgnoreCase) && veh.Driver != player)
            {
                player.SendMessage(MessageType.Error, "Você não dirigindo um FLATBED.");
                return;
            }

            if (veh.Attached is MyVehicle attachedVehicle)
            {
                player.SendMessage(MessageType.Error, "Você já está rebocando um veículo.");
                return;
            }

            attachedVehicle = Global.Vehicles.FirstOrDefault(x => x.Id == id
                && x != player.Vehicle
                && x.Position.Distance(player.Vehicle.Position) <= 10);
            if (attachedVehicle == null)
            {
                player.SendMessage(MessageType.Error, $"Você não está próximo do veículo {id}.");
                return;
            }

            attachedVehicle.SetStreamSyncedMetaData(Constants.VEHICLE_META_DATA_ATTACHED, true);
            attachedVehicle.Position = attachedVehicle.Position;
            attachedVehicle.AttachToEntity(veh, 0, 0, new Position(0, -10.5f, 4.5f), new Rotation(0, 0, 0), true, false);
            player.SendMessage(MessageType.Success, $"Você rebocou o veículo {id}.");
        }

        [Command("rebocaroff")]
        public static async Task CMD_rebocaroff(MyPlayer player)
        {
            if (player.Vehicle is not MyVehicle veh)
            {
                player.SendMessage(MessageType.Error, "Você não está em um veículo.");
                return;
            }

            if (veh.Attached is not MyVehicle attachedVehicle)
            {
                player.SendMessage(MessageType.Error, "Você não está rebocando nenhum veículo.");
                return;
            }

            attachedVehicle.Detach();
            await Task.Delay(1000);
            attachedVehicle.AttachToEntity(veh, 0, 0, new Position(0, -45f, -0.5f), new Rotation(0, 0, 0), true, false);
            await Task.Delay(1000);
            attachedVehicle.Detach();
            attachedVehicle.DeleteSyncedMetaData(Constants.VEHICLE_META_DATA_ATTACHED);

            player.SendMessage(MessageType.Success, "Você soltou o veículo.");
        }

        [AsyncScriptEvent(ScriptEventType.PlayerLeaveVehicle)]
        public static async Task OnPlayerLeaveVehicle(MyVehicle vehicle, MyPlayer player, byte seat)
        {
            if (player.VehicleAnimation)
                player.StopAnimation();

            if (vehicle.Model == (uint)VehicleModel.Thruster)
            {
                await Task.Delay(2000);
                vehicle.Destroy();
            }

            player.Emit("Spotlight:Toggle", false);

            if (vehicle.VehicleDB.Model.Equals(VehicleModelMods.LSPDHELI.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || vehicle.VehicleDB.Model.Equals(VehicleModel.Polmav.ToString(), StringComparison.CurrentCultureIgnoreCase))
            {
                player.Emit("Helicam:Toggle", true);
                var spotlight = Global.Spotlights.FirstOrDefault(x => x.Id == vehicle.Id && x.Player == player.SessionId);
                if (spotlight != null)
                {
                    Global.Spotlights.Remove(spotlight);
                    Alt.EmitAllClients("Spotlight:Remove", spotlight.Id);
                }
            }

            if (seat == 1 && vehicle.VehicleDB.Job != CharacterJob.None && !vehicle.RentExpirationDate.HasValue)
            {
                await Task.Delay(2000);
                await vehicle.Estacionar(player);
            }
        }

        [ScriptEvent(ScriptEventType.PlayerEnterVehicle)]
        public static void OnPlayerEnterVehicle(MyVehicle vehicle, MyPlayer player, byte seat)
        {
            if (seat == 1)
                player.Emit("Spotlight:Toggle", vehicle.SpotlightActive);

            player.SetCanDoDriveBy(seat);
            vehicle.SetStreamSyncedMetaData(Constants.VEHICLE_META_DATA_RADIO_ENABLED, !vehicle.VehicleDB.FactionId.HasValue);

            if (vehicle.EngineOn)
            {
                if (vehicle.VehicleDB.Fuel == 0)
                {
                    vehicle.EngineOn = false;
                }
                else if (vehicle.VehicleDB.Job != CharacterJob.None && !vehicle.RentExpirationDate.HasValue)
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

            if (vehicle.VehicleDB.Job != CharacterJob.None && vehicle.NameInCharge == player.Character.Name && vehicle.RentExpirationDate.HasValue)
                player.SendMessage(MessageType.Error, $"O aluguel do veículo irá expirar em {vehicle.RentExpirationDate}.");
        }

        [ScriptEvent(ScriptEventType.VehicleDamage)]
        public static void OnVehicleDamage(MyVehicle veh, AltV.Net.Elements.Entities.IEntity attacker, uint bodyHealthDamage, uint additionalBodyHealthDamage, uint engineHealthDamage, uint petrolTankDamage, uint weaponHash)
        {
            var dano = new VehicleDamage()
            {
                BodyHealthDamage = bodyHealthDamage,
                AdditionalBodyHealthDamage = additionalBodyHealthDamage,
                EngineHealthDamage = engineHealthDamage,
                PetrolTankDamage = petrolTankDamage,
                WeaponHash = weaponHash,
            };

            MyPlayer? playerAttacker = null;
            if (attacker is not MyPlayer player)
            {
                if (attacker is MyVehicle vehicleAttacker)
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
        public async Task ComprarVeiculo(MyPlayer player, int tipo, string model, byte r1, byte g1, byte b1, byte r2, byte g2, byte b2)
        {
            if (string.IsNullOrWhiteSpace(model))
            {
                player.SendMessage(MessageType.Error, "Verifique se todos os campos foram preenchidos corretamente.", notify: true);
                return;
            }

            var preco = Global.Prices.FirstOrDefault(x => x.Type == (PriceType)tipo && x.Name.Equals(model, StringComparison.CurrentCultureIgnoreCase));
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

            var restricao = Functions.CheckVIPVehicle(model);
            if (restricao.Item2 != UserVIP.None && (restricao.Item2 > player.User.VIP || (player.User.VIPValidDate ?? DateTime.MinValue) < DateTime.Now))
            {
                player.SendMessage(MessageType.Error, $"O veículo é restrito para VIP {restricao.Item2}.", notify: true);
                return;
            }

            var concessionaria = Global.Dealerships.FirstOrDefault(x => x.PriceType == (PriceType)tipo)!;

            var vehicle = new Vehicle();
            vehicle.Create(model, await Functions.GenerateVehiclePlate(false), r1, g1, b1, r2, g2, b2);
            vehicle.ChangePosition(concessionaria.VehiclePosition.X, concessionaria.VehiclePosition.Y, concessionaria.VehiclePosition.Z,
                concessionaria.VehicleRotation.X, concessionaria.VehicleRotation.Y, concessionaria.VehicleRotation.Z);
            vehicle.SetOwner(player.Character.Id);
            vehicle.SetFuel(vehicle.GetMaxFuel());

            await using var context = new DatabaseContext();
            await context.Vehicles.AddAsync(vehicle);
            await context.SaveChangesAsync();
            await player.RemoveStackedItem(ItemCategory.Money, value);

            player.SendMessage(MessageType.Success, $"Você comprou {vehicle.Model.ToUpper()} por ${preco.Value:N0}. Use /vlista para spawnar.");
            player.Emit("Server:CloseView");
        }

        [ClientEvent(nameof(SetVehicleHasMutedSirens))]
        public void SetVehicleHasMutedSirens(MyPlayer player, bool hasMutedSirens) => player.Vehicle.SetStreamSyncedMetaData(Constants.VEHICLE_META_DATA_HAS_MUTED_SIRENS, hasMutedSirens);

        [ClientEvent(nameof(SetVehicleSpotlightX))]
        public void SetVehicleSpotlightX(MyPlayer player, float spotlightX) => player.Vehicle.SetStreamSyncedMetaData(Constants.VEHICLE_META_DATA_SPOTLIGHT_X, spotlightX);

        [ClientEvent(nameof(SetVehicleSpotlightZ))]
        public void SetVehicleSpotlightZ(MyPlayer player, float spotlightZ) => player.Vehicle.SetStreamSyncedMetaData(Constants.VEHICLE_META_DATA_SPOTLIGHT_Z, spotlightZ);

        [AsyncClientEvent(nameof(SpawnarVeiculoFaccao))]
        public async Task SpawnarVeiculoFaccao(MyPlayer player, string spotId, string vehicleIdString)
        {
            try
            {
                var vehicleId = new Guid(vehicleIdString);
                if (Global.Vehicles.Any(x => x.VehicleDB.Id == vehicleId))
                {
                    player.SendMessage(MessageType.Error, "Veículo já está spawnado.", notify: true);
                    return;
                }

                await using var context = new DatabaseContext();
                var veh = await context.Vehicles
                    .Include(x => x.Items)
                    .FirstOrDefaultAsync(x => x.Id == vehicleId);
                if (veh == null)
                {
                    player.SendMessage(MessageType.Error, "Veículo não encontrado.", notify: true);
                    return;
                }

                var spot = Global.Spots.FirstOrDefault(x => x.Id == new Guid(spotId));

                veh.ChangePosition(player.Position.X, player.Position.Y, player.Position.Z,
                    spot?.AuxiliarPosX ?? 0, spot?.AuxiliarPosY ?? 0, spot?.AuxiliarPosZ ?? 0);

                var vehicle = await veh.Spawnar(player);
                vehicle.NameInCharge = player.Character.Name;
                vehicle.LockState = VehicleLockState.Unlocked;
                player.SetIntoVehicle(vehicle, 1);
                player.Emit("Server:CloseView");
                player.SendMessage(MessageType.Success, $"Você spawnou o veículo {vehicleId}.", notify: true);
            }
            catch (Exception ex)
            {
                ex.HelpLink = $"Vehicle Id: {vehicleIdString}";
                Functions.GetException(ex);
            }
        }

        [AsyncClientEvent(nameof(AbastecerVeiculo))]
        public async Task AbastecerVeiculo(MyPlayer player, string id)
        {
            var veh = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == new Guid(id));
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, "Veículo não encontrado.");
                return;
            }

            var combustivelNecessario = veh.VehicleDB.GetMaxFuel() - veh.VehicleDB.Fuel;
            var valor = combustivelNecessario * Global.Parameter.FuelValue;
            if (valor > player.Money && !veh.VehicleDB.FactionId.HasValue)
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
                    veh.VehicleDB.SetFuel(veh.VehicleDB.GetMaxFuel());
                    veh.SetStreamSyncedMetaData(Constants.VEHICLE_META_DATA_FUEL, veh.FuelHUD);
                    if (!veh.VehicleDB.FactionId.HasValue)
                    {
                        await player.RemoveStackedItem(ItemCategory.Money, valor);
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
        public async Task SpawnarVeiculo(MyPlayer player, string idString)
        {
            try
            {
                var id = idString.ToGuid();
                if (Global.Vehicles.Any(x => x.VehicleDB.Id == id))
                {
                    player.SendMessage(MessageType.Error, "Veículo já está spawnado.", notify: true);
                    return;
                }

                await using var context = new DatabaseContext();
                var veh = await context.Vehicles.FirstOrDefaultAsync(x => x.Id == id);
                if (veh == null)
                {
                    player.SendMessage(MessageType.Error, "Veículo não encontrado.", notify: true);
                    return;
                }

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
                player.SendMessage(MessageType.Success, $"Você spawnou o {veh.Model.ToUpper()}.", notify: true);
                player.Emit("Server:CloseView");
            }
            catch (Exception ex)
            {
                ex.HelpLink = $"Vehicle Id: {idString}";
                Functions.GetException(ex);
            }
        }

        [AsyncClientEvent(nameof(VenderVeiculo))]
        public async Task VenderVeiculo(MyPlayer player) => await CMDVenderVeiculoConcessionaria(player, player.TargetConfirmation.FirstOrDefault(), true);

        [AsyncClientEvent(nameof(Trancar))]
        public async Task Trancar(MyPlayer player) => await Functions.CMDTrancar(player);

        [ClientEvent(nameof(Motor))]
        public static void Motor(MyPlayer player) => CMDMotor(player);

        [AsyncScriptEvent(ScriptEventType.VehicleDestroy)]
        public static async Task OnVehicleDestroy(MyVehicle vehicle)
        {
            await Functions.WriteLog(LogType.VehicleDestruction, $"{vehicle.VehicleDB.Id} | {Functions.Serialize(vehicle.Damages)}");
        }

        [AsyncClientEvent(nameof(BuyVehicleUpgrade))]
        public static async Task BuyVehicleUpgrade(MyPlayer player, string idString, string name)
        {
            var id = idString.ToGuid();
            if (Global.Vehicles.Any(x => x.VehicleDB.Id == id))
            {
                player.SendMessage(MessageType.Error, $"Veículo {idString} está spawnado.");
                return;
            }

            await using var context = new DatabaseContext();
            var veh = await context.Vehicles.FirstOrDefaultAsync(x => x.CharacterId == player.Character.Id && x.Id == id && !x.Sold);
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_OWNER_ERROR_MESSAGE, notify: true);
                return;
            }

            var preco = Global.Prices.FirstOrDefault(x => x.IsVehicle && x.Name.Equals(veh.Model, StringComparison.CurrentCultureIgnoreCase));
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

                    veh.SetProtectionLevel(1);
                    break;
                case "Proteção Nível 2":
                    if (veh.ProtectionLevel >= 2)
                    {
                        player.SendMessage(MessageType.Error, $"O veículo já possui um nível de proteção igual ou maior que 2.", notify: true);
                        return;
                    }

                    veh.SetProtectionLevel(2);
                    break;
                case "Proteção Nível 3":
                    if (veh.ProtectionLevel >= 3)
                    {
                        player.SendMessage(MessageType.Error, $"O veículo já possui um nível de proteção igual ou maior que 3.", notify: true);
                        return;
                    }

                    veh.SetProtectionLevel(3);
                    break;
                case "XMR":
                    if (veh.XMR)
                    {
                        player.SendMessage(MessageType.Error, $"O veículo já possui XMR.", notify: true);
                        return;
                    }

                    veh.SetXMR();
                    break;
            }

            context.Vehicles.Update(veh);
            await context.SaveChangesAsync();

            await player.RemoveStackedItem(ItemCategory.Money, value);
            player.SendMessage(MessageType.Success, $"Você comprou {name}.", notify: true);
        }

        [ClientEvent(nameof(ConfirmXMR))]
        public static void ConfirmXMR(MyPlayer player, string idString, string url, float volume)
        {
            if (!(Uri.TryCreate(url, UriKind.Absolute, out Uri? uriResult)
                && uriResult?.Scheme != Uri.UriSchemeHttp && uriResult?.Scheme != Uri.UriSchemeHttps))
            {
                player.EmitStaffShowMessage("URL está em um formato inválido.");
                return;
            }

            var id = idString.ToGuid();
            var vehicle = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == id);
            if (vehicle == null)
                return;

            vehicle.AudioSpot ??= new AudioSpot
            {
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
        public static void TurnOffXMR(MyPlayer player, string idString)
        {
            var id = idString.ToGuid();
            var vehicle = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == id);
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

                player.Vehicle.PrimaryColorRgb = new Rgba(drawingColor1.R, drawingColor1.G, drawingColor1.B, 255);
                player.Vehicle.SecondaryColorRgb = new Rgba(drawingColor2.R, drawingColor2.G, drawingColor2.B, 255);
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
                player.Vehicle.NeonColor = new Rgba(drawingNeonColor.R, drawingNeonColor.G, drawingNeonColor.B, 255);
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

            var vehicleTuning = Functions.Deserialize<VehicleTuning>(vehicleTuningJSON);

            async Task SetMods()
            {
                var realMods = Functions.Deserialize<List<VehicleMod>>(veh.VehicleDB.ModsJSON);
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
                var drawingColor2 = System.Drawing.ColorTranslator.FromHtml(vehicleTuning.Color2);
                var drawingNeonColor = System.Drawing.ColorTranslator.FromHtml(vehicleTuning.NeonColor);
                var drawingTireSmokeColor = System.Drawing.ColorTranslator.FromHtml(vehicleTuning.TireSmokeColor);

                veh.VehicleDB.SetColor(drawingColor1.R, drawingColor1.G, drawingColor1.B, drawingColor2.R, drawingColor2.G, drawingColor2.B);
                veh.VehicleDB.SetTunning(vehicleTuning.WheelType, vehicleTuning.WheelVariation, vehicleTuning.WheelColor,
                    drawingNeonColor.R, drawingNeonColor.G, drawingNeonColor.B,
                    Convert.ToBoolean(vehicleTuning.NeonLeft), Convert.ToBoolean(vehicleTuning.NeonRight),
                    Convert.ToBoolean(vehicleTuning.NeonFront), Convert.ToBoolean(vehicleTuning.NeonBack),
                    vehicleTuning.HeadlightColor, vehicleTuning.LightsMultiplier,
                    vehicleTuning.WindowTint, drawingTireSmokeColor.R, drawingTireSmokeColor.G, drawingTireSmokeColor.B,
                    Functions.Serialize(realMods));

                await using var context = new DatabaseContext();
                context.Vehicles.Update(veh.VehicleDB);
                await context.SaveChangesAsync();

                if (vehicleTuning.Repair == 1)
                    veh = await veh.Reparar();
            }

            if (confirm)
            {
                if (vehicleTuning.Staff)
                {
                    await SetMods();
                    await player.GravarLog(LogType.Staff, $"Tunar Veículo | {veh.VehicleDB.Id} | {vehicleTuningJSON}", null);
                    player.SendMessage(MessageType.Success, $"Você aplicou as modificações no veículo {veh.VehicleDB.Id}.");
                }
                else
                {
                    if (vehicleTuning.TargetId.HasValue)
                    {
                        var target = Global.SpawnedPlayers.FirstOrDefault(x => x.Character.Id == vehicleTuning.TargetId);
                        if (target == null)
                        {
                            player.SendMessage(MessageType.Error, $"O jogador não está mais conectado.");
                        }
                        else
                        {
                            vehicleTuning.VehicleId = veh.VehicleDB.Id;
                            vehicleTuning.TargetId = null;
                            vehicleTuning.Mods = [];

                            var invite = new Invite()
                            {
                                Type = InviteType.Mechanic,
                                SenderCharacterId = player.Character.Id,
                                Value = [Functions.Serialize(vehicleTuning)],
                            };
                            target.Invites.RemoveAll(x => x.Type == InviteType.Mechanic);
                            target.Invites.Add(invite);

                            player.SendMessage(MessageType.Success, $"Você solicitou enviar o catálogo de modificações veiculares para {target.ICName}.");
                            target.SendMessage(MessageType.Success, $"{player.ICName} solicitou enviar o catálogo de modificações veiculares para você. (/ac {(int)invite.Type} para aceitar ou /rc {(int)invite.Type} para recusar)");
                        }
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
                        await player.RemoveStackedItem(ItemCategory.Money, totalValue);
                        player.SendMessage(MessageType.Success, $"Você aplicou as modificações no veículo {veh.VehicleDB.Id} por ${totalValue:N0}.");
                        await player.GravarLog(LogType.MechanicTunning, $"{veh.VehicleDB.Id} | {vehicleTuningJSON}", null);
                    }
                }
            }

            veh.ClearMods();
            veh.SetDefaultMods();

            player.Emit("VehicleTuning:Close");
        }

        [Command("tunarver", "/tunarver (ID ou nome)")]
        public static void CMD_tunarver(MyPlayer player, string idOrName)
        {
            var target = player.GetCharacterByIdOrName(idOrName, false);
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

        [Command("colocar", "/colocar (ID ou nome)")]
        public static async Task CMD_colocar(MyPlayer player, string idOrName)
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

            var target = player.GetCharacterByIdOrName(idOrName, false);
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

            var passageiros = Global.SpawnedPlayers.Where(x => x.Vehicle == veh && x != veh.Driver).ToList();

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
            await player.GravarLog(LogType.PutInVehicle, veh.VehicleDB.Id.ToString(), target);
        }

        [Command("retirar", "/retirar (ID ou nome)")]
        public static async Task CMD_retirar(MyPlayer player, string idOrName)
        {
            if (player.IsInVehicle)
            {
                player.SendMessage(MessageType.Error, "Você não pode fazer isso dentro de um veículo");
                return;
            }

            var target = player.GetCharacterByIdOrName(idOrName, false);
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

            var vehId = veh.VehicleDB.Id;
            var pos = player.Position;
            pos.Y += 1.5f;
            target.SetPosition(pos, target.Dimension, false);

            player.SendMessage(MessageType.Success, $"Você retirou {target.ICName} do veículo.");
            target.SendMessage(MessageType.Success, $"{player.ICName} retirou você de dentro do veículo.");
            await player.GravarLog(LogType.RemoveFromVehicle, vehId.ToString(), target);
        }

        private static void CMDMotor(MyPlayer player)
        {
            if (player.Vehicle is not MyVehicle veh || veh.Driver != player)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_DRIVER_ERROR_MESSAGE);
                return;
            }

            if (!veh.CanAccess(player))
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_ACCESS_ERROR_MESSAGE);
                return;
            }

            if (veh.VehicleDB.Fuel == 0)
            {
                player.SendMessage(MessageType.Error, "Veículo não possui combustível.");
                return;
            }

            if (veh.Info.Type == VehicleModelType.BMX)
            {
                player.SendMessage(MessageType.Error, "Veículo não possui motor.");
                return;
            }

            player.SendMessageToNearbyPlayers($"{(player.Vehicle.EngineOn ? "des" : string.Empty)}liga o motor do veículo.", MessageCategory.Ame, 5);
            player.Vehicle.EngineOn = !player.Vehicle.EngineOn;
        }

        private static async Task CMDVenderVeiculoConcessionaria(MyPlayer player, Guid id, bool confirm)
        {
            if (Global.Vehicles.Any(x => x.VehicleDB.Id == id))
            {
                player.SendMessage(MessageType.Error, $"Veículo {id} está spawnado.");
                return;
            }

            await using var context = new DatabaseContext();
            var veh = await context.Vehicles.FirstOrDefaultAsync(x => x.CharacterId == player.Character.Id && x.Id == id && !x.Sold);
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_OWNER_ERROR_MESSAGE);
                return;
            }

            if (veh.StaffGift)
            {
                player.SendMessage(MessageType.Error, "Você não pode vender este veículo pois é um benefício da facção.");
                return;
            }

            var price = Global.Prices.FirstOrDefault(x => x.IsVehicle && x.Name.Equals(veh.Model, StringComparison.CurrentCultureIgnoreCase));
            if (price == null)
            {
                player.SendMessage(MessageType.Error, "Preço do veículo não foi encontrado.");
                return;
            }

            var dealership = Global.Dealerships.FirstOrDefault(x => x.PriceType == price.Type);
            if (dealership == null || player.Position.Distance(dealership.Position) > Global.RP_DISTANCE)
            {
                player.SendMessage(MessageType.Error, $"Você não está na concessionária que vende este veículo.");
                return;
            }

            var value = Convert.ToInt32(price.Value / 2);

            if (confirm)
            {
                var characterItem = new CharacterItem();
                characterItem.Create(ItemCategory.Money, 0, value, null);
                var res = await player.GiveItem(characterItem);

                if (!string.IsNullOrWhiteSpace(res))
                {
                    player.SendMessage(MessageType.Error, res);
                    return;
                }

                veh.SetSold();
                context.Vehicles.Update(veh);
                await context.SaveChangesAsync();

                player.SendMessage(MessageType.Success, $"Você vendeu seu veículo {veh.Model.ToUpper()} ({veh.Plate.ToUpper()}) [{veh.Id}] para a concessionária por ${value:N0}.");
                await player.GravarLog(LogType.Sell, $"/vvenderconce {veh.Id} {value}", null);
            }
            else
            {
                player.TargetConfirmation = [id];
                player.ShowConfirm("Confirmar Venda", $"Confirma vender o veículo {veh.Model.ToUpper()} para a concessionária por ${value:N0}?", "VenderVeiculo");
            }
        }
    }
}