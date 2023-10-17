using AltV.Net.Async;
using AltV.Net.Data;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using System.Text.Json;

namespace Roleplay.Commands
{
    public class Vehicles
    {
        [Command("motor")]
        public static void CMD_motor(MyPlayer player) => Functions.CMDMotor(player);

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

            veh.VehicleDB.PosX = player.Vehicle.Position.X;
            veh.VehicleDB.PosY = player.Vehicle.Position.Y;
            veh.VehicleDB.PosZ = player.Vehicle.Position.Z;
            veh.VehicleDB.RotR = player.Vehicle.Rotation.Roll;
            veh.VehicleDB.RotP = player.Vehicle.Rotation.Pitch;
            veh.VehicleDB.RotY = player.Vehicle.Rotation.Yaw;
            veh.VehicleDB.Parked = true;

            await using var context = new DatabaseContext();
            context.Vehicles.Update(veh.VehicleDB);
            await context.SaveChangesAsync();

            if (valor > 0)
            {
                await player.RemoveItem(new CharacterItem(ItemCategory.Money)
                {
                    Quantity = valor
                });
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
                if (!Global.Spots.Any(x => x.Type == SpotType.SpawnVeiculosFaccao
                    && player.Vehicle.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE))
                {
                    player.SendMessage(MessageType.Error, "Você não está próximo de nenhum ponto de spawn de veículos da facção.");
                    return;
                }

                await veh.Estacionar(player);
                player.SendMessage(MessageType.Success, $"Você estacionou o veículo.", notify: true);
                return;
            }

            if (veh.NomeEncarregado == player.Character.Name)
            {
                var emp = Global.Jobs.FirstOrDefault(x => x.CharacterJob == player.Character.Job);
                if (player.Vehicle.Position.Distance(emp.VehicleRentPosition) > Global.RP_DISTANCE)
                {
                    player.SendMessage(MessageType.Error, "Você não está no aluguel de veículos para esse emprego.");
                    return;
                }

                veh.VehicleDB.Fuel = veh.VehicleDB.MaxFuel;
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

            player.Emit("Server:SpawnarVeiculos", $"Veículos de {player.Character.Name} [{player.Character.Id}] ({DateTime.Now})", JsonSerializer.Serialize(veiculos));
        }

        [Command("vvender", "/vvender (ID ou nome) (valor)")]
        public static void CMD_vvender(MyPlayer player, string idNome, int valor)
        {
            var prox = Global.Vehicles
                .Where(x => x.VehicleDB.CharacterId == player.Character.Id
                    && player.Position.Distance(x.Position) <= Global.RP_DISTANCE
                    && x.Dimension == player.Dimension)
                .MinBy(x => player.Position.Distance(x.Position));

            if (prox == null)
            {
                player.SendMessage(MessageType.Error, "Você não está próximo de nenhum veículo seu.");
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

            var restricao = Functions.CheckVIPVehicle(prox.VehicleDB.Model);
            if (restricao.Item2 != UserVIP.None && (restricao.Item2 > target.User.VIP || (target.User.VIPValidDate ?? DateTime.MinValue) < DateTime.Now))
            {
                player.SendMessage(MessageType.Error, $"O veículo é restrito para VIP {restricao.Item2}.");
                return;
            }

            var convite = new Invite()
            {
                Type = InviteType.VendaVeiculo,
                SenderCharacterId = player.Character.Id,
                Value = new string[] { prox.VehicleDB.Id.ToString(), valor.ToString() },
            };
            target.Invites.RemoveAll(x => x.Type == InviteType.VendaVeiculo);
            target.Invites.Add(convite);

            player.SendMessage(MessageType.Success, $"Você ofereceu seu veículo {prox.VehicleDB.Id} para {target.ICName} por ${valor:N0}.");
            target.SendMessage(MessageType.Success, $"{player.ICName} ofereceu para você o veículo {prox.VehicleDB.Id} por ${valor:N0}. (/ac {(int)convite.Type} para aceitar ou /rc {(int)convite.Type} para recusar)");
        }

        [Command("vliberar", "/vliberar (código do veículo)")]
        public static async Task CMD_vliberar(MyPlayer player, int codigo)
        {
            if (!Global.Spots.Any(x => x.Type == SpotType.LiberacaoVeiculos
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE))
            {
                player.SendMessage(MessageType.Error, "Você não está em ponto de liberação de veículos apreendidos.");
                return;
            }

            if (Global.Vehicles.Any(x => x.VehicleDB.Id == codigo))
            {
                player.SendMessage(MessageType.Error, "Veículo está spawnado.");
                return;
            }

            await using var context = new DatabaseContext();
            var veh = await context.Vehicles.FirstOrDefaultAsync(x => x.CharacterId == player.Character.Id && x.Id == codigo);
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

            await player.RemoveItem(new CharacterItem(ItemCategory.Money)
            {
                Quantity = value
            });

            if (veh.SeizedValue > 0)
            {
                var seizedVehicle = await context.SeizedVehicles.Where(x => x.VehicleId == veh.Id).OrderByDescending(x => x.Id).FirstOrDefaultAsync();
                seizedVehicle.PaymentDate = DateTime.Now;
                context.SeizedVehicles.Update(seizedVehicle);
                await context.SaveChangesAsync();
            }

            player.SendMessage(MessageType.Success, $"Você liberou seu veículo por ${value:N0}.");
            veh.SeizedValue = veh.DismantledValue = 0;
            context.Vehicles.Update(veh);
            await context.SaveChangesAsync();
        }

        [Command("vporta", "/vporta (porta [1-4])", Aliases = new string[] { "vp" })]
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
            veh.StatusPortas[porta] = !veh.StatusPortas[porta];
            player.Emit("SetVehicleDoorState", veh, porta, veh.StatusPortas[porta]);
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

            veh.StatusPortas[4] = !veh.StatusPortas[4];
            player.SendMessageToNearbyPlayers($"{(veh.StatusPortas[4] ? "fecha" : "abre")} o capô do veículo.", MessageCategory.Ame, 5);
            player.Emit("SetVehicleDoorState", veh, 4, veh.StatusPortas[4]);
        }

        [Command("portamalas", "/portamalas (opção [abrir, fechar, ver])")]
        public static void CMD_portamalas(MyPlayer player, string option)
        {
            var veh = Global.Vehicles.Where(x => x.Dimension == player.Dimension
                && player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)) <= Global.RP_DISTANCE)
                .MinBy(x => player.Position.Distance(new Position(x.Position.X, x.Position.Y, x.Position.Z)));

            if (veh == null || !(veh?.TemArmazenamento ?? false) || veh?.VehicleDB?.Job != CharacterJob.None)
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

                veh.StatusPortas[5] = option == "fechar";
                player.SendMessageToNearbyPlayers($"{(veh.StatusPortas[5] ? "fecha" : "abre")} o porta-malas do veículo.", MessageCategory.Ame, 5);
                player.Emit("SetVehicleDoorState", veh, 5, veh.StatusPortas[5]);
            }
            else if (option == "ver")
            {
                if (player.IsInVehicle)
                {
                    player.SendMessage(MessageType.Error, "Você não pode fazer isso estando dentro do veículo.");
                    return;
                }

                if (veh.StatusPortas[5])
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

            if (veh.VehicleDB.Fuel == veh.VehicleDB.MaxFuel)
            {
                player.SendMessage(MessageType.Error, "Veículo está com tanque cheio.");
                return;
            }

            player.Emit("Server:Abastecer", veh.VehicleDB.Id);
        }

        [Command("vplaca", "/vplaca (código do veículo) (placa)")]
        public static async Task CMD_vplaca(MyPlayer player, int codigo, string placa)
        {
            if (placa.Length > 8)
            {
                player.SendMessage(MessageType.Error, "A placa deve ter até 8 caracteres.");
                return;
            }

            if (player.User.PlateChanges == 0)
            {
                player.SendMessage(MessageType.Error, "Você não possui uma mudança de placa.");
                return;
            }

            if (Global.Vehicles.Any(x => x.VehicleDB.Id == codigo))
            {
                player.SendMessage(MessageType.Error, $"Veículo {codigo} está spawnado.");
                return;
            }

            await using var context = new DatabaseContext();
            var veh = await context.Vehicles.FirstOrDefaultAsync(x => x.CharacterId == player.Character.Id && x.Id == codigo);
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_OWNER_ERROR_MESSAGE);
                return;
            }

            if (await context.Vehicles.AnyAsync(x => x.Plate.ToUpper() == placa.ToUpper()))
            {
                player.SendMessage(MessageType.Error, $"Já existe um veículo com a placa {placa.ToUpper()}.");
                return;
            }

            player.User.PlateChanges--;
            veh.Plate = placa.ToUpper();
            context.Vehicles.Update(veh);
            await context.SaveChangesAsync();
            await player.Save();

            player.SendMessage(MessageType.Success, $"Você alterou a placa do veículo {veh.Id} para {veh.Plate}.");
            await player.GravarLog(LogType.PlateChange, $"{veh.Id} {veh.Plate}", null);
        }

        [Command("vvenderconce", "/vvenderconce (código do veículo)")]
        public static async Task CMD_vvenderconce(MyPlayer player, int codigo) => await Functions.CMDVenderVeiculoConcessionaria(player, codigo, false);

        [Command("valugar")]
        public static async Task CMD_valugar(MyPlayer player)
        {
            if (player.Character.Job == CharacterJob.None || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não tem um emprego ou não está em serviço.");
                return;
            }

            var emp = Global.Jobs.FirstOrDefault(x => x.CharacterJob == player.Character.Job);
            if (!player.IsInVehicle)
            {
                if (player.Position.Distance(emp.VehicleRentPosition) > Global.RP_DISTANCE)
                {
                    player.SendMessage(MessageType.Error, "Você não está no aluguel de veículos para seu emprego.");
                    return;
                }
            }

            if (Global.Vehicles.Any(x => x.NomeEncarregado == player.Character.Name))
            {
                player.SendMessage(MessageType.Error, "Você já possui um veículo alugado.");
                return;
            }

            var preco = Convert.ToInt32(Math.Abs(Global.Prices.FirstOrDefault(x => x.Type == PriceType.AluguelEmpregos && x.Name.ToLower() == player.Character.Job.ToString().ToLower())?.Value ?? 0));
            if (player.Money < preco)
            {
                player.SendMessage(MessageType.Error, string.Format(Global.INSUFFICIENT_MONEY_ERROR_MESSAGE, preco));
                return;
            }

            MyVehicle veh = null;
            if (player.IsInVehicle)
            {
                veh = Global.Vehicles.FirstOrDefault(x => x == player.Vehicle && x.VehicleDB.Job == player.Character.Job && !x.DataExpiracaoAluguel.HasValue);
                if (veh == null)
                {
                    player.SendMessage(MessageType.Error, "Você não está dentro de um veículo disponível para aluguel.");
                    return;
                }
            }
            else
            {
                await using var context = new DatabaseContext();
                var veiculos = await context.Vehicles.Where(x => x.Job == player.Character.Job && !x.Sold).ToListAsync();
                var veiculo = veiculos.FirstOrDefault(x => !Global.Vehicles.Any(y => y.VehicleDB.Id == x.Id));
                if (veiculo == null)
                {
                    player.SendMessage(MessageType.Error, "Não há nenhum veículo disponível para aluguel.");
                    return;
                }

                veiculo.PosX = emp.VehicleRentPosition.X;
                veiculo.PosY = emp.VehicleRentPosition.Y;
                veiculo.PosZ = emp.VehicleRentPosition.Z;
                veiculo.RotR = emp.VehicleRentRotation.Roll;
                veiculo.RotP = emp.VehicleRentRotation.Pitch;
                veiculo.RotY = emp.VehicleRentRotation.Yaw;
                veiculo.Fuel = veiculo.MaxFuel;
                veh = await veiculo.Spawnar(player);
                veh.LockState = VehicleLockState.Unlocked;
                player.SetIntoVehicle(veh, 1);
            }

            veh.NomeEncarregado = player.Character.Name;
            veh.DataExpiracaoAluguel = DateTime.Now.AddHours(1);

            await player.RemoveItem(new CharacterItem(ItemCategory.Money)
            {
                Quantity = preco
            });
            player.SendMessage(MessageType.Success, $"Você alugou um {emp.VehicleModel.ToString().ToUpper()} por ${preco:N0} com expiração em {veh.DataExpiracaoAluguel}.");
        }

        [Command("danos", "/danos (veículo)")]
        public static void CMD_danos(MyPlayer player, int veiculo)
        {
            var veh = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == veiculo);
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, $"Nenhum veículo encontrado com o código {veiculo}.");
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
                html += $@"<tr><td>{x.Data}</td><td>{(WeaponModel)x.WeaponHash}</td><td>{x.BodyHealthDamage}</td><td>{x.AdditionalBodyHealthDamage}</td><td>{x.EngineHealthDamage}</td><td>{x.PetrolTankDamage}</td></tr>";

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

        [Command("janela", "/janela (opção [fe, fd, te, td, todas])", Aliases = new string[] { "janelas", "ja" })]
        public static async Task CMD_janela(MyPlayer player, string janela)
        {
            if (player.Vehicle is not MyVehicle veh)
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_ERRO_FORA_VEICULO);
                return;
            }

            if (!veh.TemJanelas)
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

            var status = !await player.Vehicle.IsWindowOpenedAsync(idJanela == 255 ? (byte)0 : idJanela);
            if (idJanela == 255)
            {
                await player.Vehicle.SetWindowOpenedAsync(0, status);
                await player.Vehicle.SetWindowOpenedAsync(1, status);
                await player.Vehicle.SetWindowOpenedAsync(2, status);
                await player.Vehicle.SetWindowOpenedAsync(3, status);
            }
            else
            {
                await player.Vehicle.SetWindowOpenedAsync(idJanela, status);
            }

            foreach (var target in Global.Players.Where(x => x.Vehicle == veh))
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
            veh.VehicleDB.LockNumber = await context.Vehicles.MaxAsync(x => x.LockNumber) + 1;
            context.Vehicles.Update(veh.VehicleDB);
            await context.SaveChangesAsync();

            await player.RemoveItem(new CharacterItem(ItemCategory.Money)
            {
                Quantity = Global.Parameter.LockValue
            });

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

            var res = await player.GiveItem(new CharacterItem(ItemCategory.VehicleKey, veh.VehicleDB.LockNumber));
            if (!string.IsNullOrWhiteSpace(res))
            {
                player.SendMessage(MessageType.Error, res);
                return;
            }

            await player.RemoveItem(new CharacterItem(ItemCategory.Money)
            {
                Quantity = Global.Parameter.KeyValue
            });

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

            if (veh.VehicleDB.Fuel == veh.VehicleDB.MaxFuel)
            {
                player.SendMessage(MessageType.Error, "Veículo está com tanque cheio.");
                return;
            }

            if (player.CurrentWeapon != (uint)WeaponModel.JerryCan)
            {
                player.SendMessage(MessageType.Error, "Você não está com um galão de combustível em mãos.");
                return;
            }

            var galaoCombustivel = player.Items.FirstOrDefault(x => x.Category == ItemCategory.Weapon
                && x.Type == player.CurrentWeapon
                && x.Slot < 0);
            if (galaoCombustivel == null)
            {
                player.SendMessage(MessageType.Error, "Você não está com um galão de combustível em mãos.");
                return;
            }

            var extra = JsonSerializer.Deserialize<WeaponItem>(galaoCombustivel.Extra);
            var combustivel = Convert.ToInt32(Math.Ceiling(extra.Ammo / 50f));
            var combustivelNecessario = veh.VehicleDB.MaxFuel - veh.VehicleDB.Fuel;
            if (combustivelNecessario > combustivel)
                combustivelNecessario = combustivel;

            veh.VehicleDB.Fuel += combustivelNecessario;
            veh.SetStreamSyncedMetaData(Constants.VEHICLE_META_DATA_FUEL, veh.CombustivelHUD);

            combustivel -= combustivelNecessario;
            if (combustivel == 0)
            {
                await player.RemoveItem(galaoCombustivel);
            }
            else
            {
                extra.Ammo = combustivel * 50;
                galaoCombustivel.Extra = JsonSerializer.Serialize(extra);
                await using var context = new DatabaseContext();
                context.CharactersItems.Update(galaoCombustivel);
                await context.SaveChangesAsync();
            }

            player.SendMessage(MessageType.Success, $"Você usou um galão de combustível e abasteceu seu veículo com {combustivelNecessario} litro{(combustivelNecessario > 1 ? "s" : string.Empty)}.");
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
        public static async Task CMD_vupgrade(MyPlayer player, int vehicleId)
        {
            if (Global.Vehicles.Any(x => x.VehicleDB.Id == vehicleId))
            {
                player.SendMessage(MessageType.Error, $"Veículo {vehicleId} está spawnado.");
                return;
            }

            await using var context = new DatabaseContext();
            var veh = await context.Vehicles.FirstOrDefaultAsync(x => x.CharacterId == player.Character.Id && x.Id == vehicleId && !x.Sold);
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_OWNER_ERROR_MESSAGE);
                return;
            }

            var preco = Global.Prices.FirstOrDefault(x => x.Vehicle && x.Name.ToLower() == veh.Model.ToLower());

            var conce = Global.Dealerships.FirstOrDefault(x => x.PriceType == preco?.Type);
            if (conce == null || player.Position.Distance(conce.Position) > Global.RP_DISTANCE)
            {
                player.SendMessage(MessageType.Error, $"Você não está na concessionária que vende este veículo.");
                return;
            }

            var itemsJSON = JsonSerializer.Serialize(
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
        public static async Task CMD_vrastrear(MyPlayer player, int vehicleId)
        {
            var veh = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == vehicleId);
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, $"Veículo {vehicleId} não está spawnado.");
                return;
            }

            if (veh.VehicleDB.ProtectionLevel < 1)
            {
                player.SendMessage(MessageType.Error, $"Veículo {vehicleId} não possui rastreador.");
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

            await veh.ActivateProtection(player);

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
                    await player.GravarLog(LogType.QuebrarTrava, veh.VehicleDB.Id.ToString(), null);
                    player.CancellationTokenSourceAcao = null;
                });
            });
        }

        [Command("reparar")]
        public static async Task CMD_reparar(MyPlayer player)
        {
            var price = Global.Prices.FirstOrDefault(x => x.Type == PriceType.Tuning && x.Name.ToLower() == "repair");
            if (price == null)
            {
                player.SendMessage(MessageType.Error, $"Não foi configurado o preço \"repair\" da categoria {Functions.GetEnumDisplay(PriceType.Tuning)}.");
                return;
            }

            if (Global.Players.Any(x => x.OnDuty && x.Character.Job == CharacterJob.Mechanic))
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

            var vehiclePrice = Global.Prices.FirstOrDefault(x => x.Vehicle && x.Name.ToLower() == veh.VehicleDB.Model.ToLower());
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
                    await player.RemoveItem(new CharacterItem(ItemCategory.Money)
                    {
                        Quantity = value,
                    });
                    player.SendMessage(MessageType.Success, $"Você consertou o veículo e pagou ${value:N0}.");
                    player.ToggleGameControls(true);

                    await player.GravarLog(LogType.RepararVeiculoJogador, veh.VehicleDB.Id.ToString(), null);
                    player.CancellationTokenSourceAcao = null;
                });
            });
        }

        [Command("ligacaodireta", Aliases = new string[] { "hotwire" })]
        public static async Task CMD_ligacaodireta(MyPlayer player)
        {
            if (player.Vehicle is not MyVehicle veh || veh.Driver != player)
            {
                player.SendMessage(MessageType.Error, Global.VEHICLE_DRIVER_ERROR_MESSAGE);
                return;
            }

            await veh.ActivateProtection(player);
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
                    await player.GravarLog(LogType.LigacaoDireta, veh.VehicleDB.Id.ToString(), null);
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

            var preco = Global.Prices.FirstOrDefault(x => x.Vehicle && x.Name.ToLower() == veh.VehicleDB.Model.ToLower());
            if (preco == null)
            {
                player.SendMessage(MessageType.Error, "Preço não encontrado.");
                return;
            }

            await veh.ActivateProtection(player);
            player.ToggleGameControls(false);
            player.SendMessage(MessageType.Success, $"Aguarde 300 segundos. Pressione DELETE para cancelar a ação.");
            player.CancellationTokenSourceAcao?.Cancel();
            player.CancellationTokenSourceAcao = new System.Threading.CancellationTokenSource();
            await Task.Delay(300000, player.CancellationTokenSourceAcao.Token).ContinueWith(t =>
            {
                if (t.IsCanceled)
                    return;

                Task.Run(async () =>
                {
                    var value = Convert.ToInt32(Math.Truncate(preco.Value * 0.1));
                    var res = await player.GiveItem(new CharacterItem(ItemCategory.Money) { Quantity = value });
                    if (!string.IsNullOrWhiteSpace(res))
                    {
                        player.SendMessage(MessageType.Error, res);
                        return;
                    }

                    veh.VehicleDB.DismantledValue = value;
                    await veh.Estacionar(null);

                    player.User.CooldownDismantle = DateTime.Now.AddHours(Global.Parameter.CooldownDismantleHours);
                    player.ToggleGameControls(true);
                    player.SendMessageToNearbyPlayers("desmancha o veículo.", MessageCategory.Ame, 5);
                    player.SendMessage(MessageType.Success, $"Você desmanchou o veículo e recebeu ${value:N0}.");
                    await player.GravarLog(LogType.Desmanche, $"{veh.VehicleDB.Id} {value}", null);
                    player.CancellationTokenSourceAcao = null;
                });
            });
        }

        [Command("rebocar", "/rebocar (id)")]
        public static void CMD_rebocar(MyPlayer player, int id)
        {
            if (player.Vehicle is not MyVehicle veh ||
                veh?.VehicleDB?.Model?.ToUpper() != VehicleModel.Flatbed.ToString().ToUpper() && veh.Driver != player)
            {
                player.SendMessage(MessageType.Error, "Você não dirigindo um FLATBED.");
                return;
            }

            if (veh.Attached is MyVehicle attachedVehicle)
            {
                player.SendMessage(MessageType.Error, "Você já está rebocando um veículo.");
                return;
            }

            attachedVehicle = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == id
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

            await attachedVehicle.DetachAsync();
            await Task.Delay(1000);
            await attachedVehicle.AttachToEntityAsync(veh, 0, 0, new Position(0, -45f, -0.5f), new Rotation(0, 0, 0), true, false);
            await Task.Delay(1000);
            await attachedVehicle.DetachAsync();
            attachedVehicle.DeleteSyncedMetaData(Constants.VEHICLE_META_DATA_ATTACHED);

            player.SendMessage(MessageType.Success, "Você soltou o veículo.");
        }
    }
}