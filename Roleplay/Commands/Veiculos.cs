using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Newtonsoft.Json;
using Roleplay.Entities;
using Roleplay.Models;
using System;
using System.Linq;

namespace Roleplay.Commands
{
    public class Veiculos
    {
        [Command("motor")]
        public void CMD_motor(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (!player.IsInVehicle || player.Vehicle?.Driver != player)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é o motorista de um veículo.");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Vehicle == player.Vehicle);
            if (veh?.Personagem != p.Codigo && (veh?.Faccao != p.Faccao || veh?.Faccao == 0) && veh?.NomeEncarregado != p.Nome)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui acesso ao veículo.");
                return;
            }

            if (veh?.Combustivel == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Veículo não possui combustível.");
                return;
            }

            if (veh?.Info?.Class == "CYCLE")
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Veículo não possui motor.");
                return;
            }

            Functions.SendMessageToNearbyPlayers(player, $"{(player.Vehicle.EngineOn ? "des" : string.Empty)}liga o motor do veículo.", TipoMensagemJogo.Ame, 5);
            player.Emit("vehicle:setVehicleEngineOn", player.Vehicle, !player.Vehicle.EngineOn);
        }

        [Command("vcomprarvaga")]
        public void CMD_vcomprarvaga(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);

            if (!player.IsInVehicle || player.Vehicle?.Driver != player)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é o motorista de um veículo.");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Personagem == p.Codigo && x.Vehicle == player.Vehicle);
            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é o proprietário do veículo.");
                return;
            }

            var valor = Global.Parametros.ValorVagaVeiculo;
            if (!veh.Estacionou
                || Global.Propriedades.Any(x => x.Personagem == p.Codigo
                    && player.Vehicle.Position.Distance(new Position(x.EntradaPosX, x.EntradaPosY, x.EntradaPosZ)) <= 25))
                valor = 0;

            if (p.Dinheiro < valor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Você não possui dinheiro suficiente (${valor:N0}).");
                return;
            }

            veh.PosX = player.Vehicle.Position.X;
            veh.PosY = player.Vehicle.Position.Y;
            veh.PosZ = player.Vehicle.Position.Z;
            veh.RotX = player.Vehicle.Rotation.Roll;
            veh.RotY = player.Vehicle.Rotation.Pitch;
            veh.RotZ = player.Vehicle.Rotation.Yaw;
            veh.Estacionou = true;

            using (var context = new DatabaseContext())
            {
                context.Veiculos.Update(veh);
                context.SaveChanges();
            }

            if (valor > 0)
            {
                p.Dinheiro -= valor;
                p.SetDinheiro();
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou a posição de estacionar seu veículo por ${valor:N0}.", notify: true);
            }
            else
            {
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou a posição de estacionar seu veículo.", notify: true);
            }
        }

        [Command("vestacionar")]
        public void CMD_vestacionar(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (!player.IsInVehicle || player.Vehicle?.Driver != player)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está no banco de motorista de um veículo.");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Vehicle == player.Vehicle);
            if (veh?.Personagem == p.Codigo)
            {
                if (player.Vehicle.Position.Distance(new Position(veh.PosX, veh.PosY, veh.PosZ)) > Global.DistanciaRP)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de sua vaga.");
                    return;
                }

                veh.Despawnar();
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você estacionou o veículo.", notify: true);
                return;
            }

            if (veh?.Faccao == p.Faccao && veh?.Faccao > 0)
            {
                if (!Global.Pontos.Any(x => x.Tipo == TipoPonto.SpawnVeiculosFaccao && player.Vehicle.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.DistanciaRP))
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum ponto de spawn de veículos da facção.");
                    return;
                }

                veh.Despawnar();
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você estacionou o veículo.", notify: true);
                return;
            }

            if (veh?.NomeEncarregado == p.Nome)
            {
                var emp = Global.Empregos.FirstOrDefault(x => x.Tipo == p.Emprego);
                if (player.Vehicle.Position.Distance(emp.PosicaoAluguel) > Global.DistanciaRP)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está no aluguel de veículos para esse emprego.");
                    return;
                }

                veh.Combustivel = veh.TanqueCombustivel;
                veh.Despawnar();
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você estacionou o veículo.", notify: true);
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui acesso ao veículo.");
        }

        [Command("vlista")]
        public void CMD_vlista(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            using var context = new DatabaseContext();
            var veiculos = context.Veiculos.AsQueryable().Where(x => x.Personagem == p.Codigo && !x.VendidoFerroVelho).ToList()
                .Select(x => new
                {
                    x.Codigo,
                    Modelo = x.Modelo.ToUpper(),
                    x.Placa,
                    Spawnado = Global.Veiculos.Any(y => y.Codigo == x.Codigo) ? $"<span class='label' style='background-color:{Global.CorSucesso}'>SIM</span>" : $"<span class='label' style='background-color:{Global.CorErro}'>NÃO</span>",
                    Apreendido = x.ValorApreensao > 0 ? $"<span class='label' style='background-color:{Global.CorSucesso}'>SIM (${x.ValorApreensao:N0})</span>" : $"<span class='label' style='background-color:{Global.CorErro}'>NÃO</span>",
                }).ToList();
            if (veiculos.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui veículos.");
                return;
            }

            player.Emit("Server:SpawnarVeiculos", $"Veículos de {p.Nome} [{p.Codigo}] ({DateTime.Now})", JsonConvert.SerializeObject(veiculos));
        }

        [Command("vvenderpara", "/vvenderpara (ID ou nome) (valor)")]
        public void CMD_vvenderpara(IPlayer player, string idNome, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            var prox = Global.Veiculos
                .Where(x => x.Personagem == p.Codigo && player.Position.Distance(x.Vehicle.Position) <= Global.DistanciaRP
                && x.Vehicle.Dimension == player.Dimension)
                .OrderBy(x => player.Position.Distance(x.Vehicle.Position))
                .FirstOrDefault();

            if (prox == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum veículo seu.");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (player.Position.Distance(target.Player.Position) > Global.DistanciaRP || player.Dimension != target.Player.Dimension)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo de você.");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor não é válido.");
                return;
            }

            var restricao = Functions.VerificarRestricaoVeiculo(prox.Modelo);
            if (restricao.Item2 > target.UsuarioBD.VIP || (target.UsuarioBD.VIP != TipoVIP.Nenhum && (target.UsuarioBD.DataExpiracaoVIP ?? DateTime.MinValue) < DateTime.Now))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"O veículo é restrito para VIP {restricao.Item2}.");
                return;
            }

            var convite = new Convite()
            {
                Tipo = TipoConvite.VendaVeiculo,
                Personagem = p.Codigo,
                Valor = new string[] { prox.Codigo.ToString(), valor.ToString() },
            };
            target.Convites.RemoveAll(x => x.Tipo == TipoConvite.VendaVeiculo);
            target.Convites.Add(convite);

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você ofereceu seu veículo {prox.Codigo} para {target.NomeIC} por ${valor:N0}.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.NomeIC} ofereceu para você o veículo {prox.Codigo} por ${valor:N0}. (/ac {(int)convite.Tipo} para aceitar ou /rc {(int)convite.Tipo} para recusar)");

            Functions.GravarLog(TipoLog.Venda, $"/vvender {prox.Codigo} {valor}", p, target);
        }

        [Command("vliberar", "/vliberar (código do veículo)")]
        public void CMD_vliberar(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if (!Global.Pontos.Any(x => x.Tipo == TipoPonto.LiberacaoVeiculos && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.DistanciaRP))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em ponto de liberação de veículos apreendidos.");
                return;
            }

            if (Global.Veiculos.Any(x => x.Codigo == codigo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Veículo não está apreendido.");
                return;
            }

            using var context = new DatabaseContext();
            var veh = context.Veiculos.FirstOrDefault(x => x.Personagem == p.Codigo && x.Codigo == codigo);
            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é o proprietário do veículo.");
                return;
            }

            if (veh.ValorApreensao == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Veículo não está apreendido.");
                return;
            }

            if (p.Dinheiro < veh.ValorApreensao)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente.");
                return;
            }

            p.Dinheiro -= veh.ValorApreensao;
            p.SetDinheiro();

            veh.ValorApreensao = 0;
            context.Veiculos.Update(veh);

            var apreensao = context.Apreensoes.AsQueryable().Where(x => x.Veiculo == veh.Codigo).ToList().LastOrDefault();
            apreensao.DataPagamento = DateTime.Now;
            context.Apreensoes.Update(apreensao);

            context.SaveChanges();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você liberou seu veículo.");
        }

        [Command("porta", "/porta (porta [1-4])", Alias = "p")]
        public void CMD_porta(IPlayer player, int porta)
        {
            if (porta < 1 || porta > 4)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Porta deve ser entre 1 e 4.");
                return;
            }

            var veh = Global.Veiculos.Where(x => player.Position.Distance(new Position(x.Vehicle.Position.X, x.Vehicle.Position.Y, x.Vehicle.Position.Z)) <= Global.DistanciaRP
                && x.Vehicle.Dimension == player.Dimension
                && x.Vehicle.LockState == VehicleLockState.Unlocked)
                .OrderBy(x => player.Position.Distance(new Position(x.Vehicle.Position.X, x.Vehicle.Position.Y, x.Vehicle.Position.Z)))
                .FirstOrDefault();

            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum veículo destrancado.");
                return;
            }

            porta--;
            veh.StatusPortas[porta] = !veh.StatusPortas[porta];
            Alt.EmitAllClients("SetVehicleDoorState", veh.Vehicle, porta, veh.StatusPortas[porta]);
        }

        [Command("capo")]
        public void CMD_capo(IPlayer player)
        {
            var veh = Global.Veiculos.Where(x => player.Position.Distance(new Position(x.Vehicle.Position.X, x.Vehicle.Position.Y, x.Vehicle.Position.Z)) <= Global.DistanciaRP
                && x.Vehicle.Dimension == player.Dimension
                && x.Vehicle.LockState == VehicleLockState.Unlocked)
                .OrderBy(x => player.Position.Distance(new Position(x.Vehicle.Position.X, x.Vehicle.Position.Y, x.Vehicle.Position.Z)))
                .FirstOrDefault();

            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum veículo destrancado.");
                return;
            }

            veh.StatusPortas[4] = !veh.StatusPortas[4];
            Alt.EmitAllClients("SetVehicleDoorState", veh.Vehicle, 4, veh.StatusPortas[4]);
        }

        [Command("portamalas")]
        public void CMD_portamalas(IPlayer player)
        {
            var veh = Global.Veiculos.Where(x => player.Position.Distance(new Position(x.Vehicle.Position.X, x.Vehicle.Position.Y, x.Vehicle.Position.Z)) <= Global.DistanciaRP
                && x.Vehicle.Dimension == player.Dimension
                && x.Vehicle.LockState == VehicleLockState.Unlocked)
                .OrderBy(x => player.Position.Distance(new Position(x.Vehicle.Position.X, x.Vehicle.Position.Y, x.Vehicle.Position.Z)))
                .FirstOrDefault();

            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum veículo destrancado.");
                return;
            }

            veh.StatusPortas[5] = !veh.StatusPortas[5];
            Alt.EmitAllClients("SetVehicleDoorState", veh.Vehicle, 5, veh.StatusPortas[5]);
        }

        [Command("abastecer")]
        public void CMD_abastecer(IPlayer player)
        {
            if (player.IsInVehicle)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você deve estar fora do veículo.");
                return;
            }

            var veh = Global.Veiculos.Where(x => player.Position.Distance(new Position(x.Vehicle.Position.X, x.Vehicle.Position.Y, x.Vehicle.Position.Z)) <= 5
                && x.Vehicle.Dimension == player.Dimension
                && x.Vehicle.LockState == VehicleLockState.Unlocked)
                .OrderBy(x => player.Position.Distance(new Position(x.Vehicle.Position.X, x.Vehicle.Position.Y, x.Vehicle.Position.Z)))
                .FirstOrDefault();

            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum veículo destrancado.");
                return;
            }

            if (veh.Combustivel == veh.TanqueCombustivel)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Veículo está com tanque cheio.");
                return;
            }

            player.Emit("Server:Abastecer", veh.Codigo);
        }

        [Command("mudarplaca", "/mudarplaca (código do veículo) (placa)")]
        public void CMD_mudarplaca(IPlayer player, int codigo, string placa)
        {
            var p = Functions.ObterPersonagem(player);
            if (!p.UsuarioBD.PossuiPlateChange)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui uma troca de placa.");
                return;
            }

            if (Global.Veiculos.Any(x => x.Codigo == codigo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Veículo {codigo} está spawnado.");
                return;
            }

            using var context = new DatabaseContext();
            var veh = context.Veiculos.FirstOrDefault(x => x.Personagem == p.Codigo && x.Codigo == codigo);
            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Você não é o proprietário do veículo {codigo}.");
                return;
            }

            if (context.Veiculos.Any(x => x.Placa.ToUpper() == placa.ToUpper()))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Já existe um veículo com a placa {placa.ToUpper()}.");
                return;
            }

            p.UsuarioBD.PossuiPlateChange = false;
            veh.Placa = placa.ToUpper();
            context.Veiculos.Update(veh);
            context.SaveChanges();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alterou a placa do veículo {codigo} para {veh.Placa}.");
            Functions.GravarLog(TipoLog.PlateChange, $"/mudarplaca {codigo} {placa.ToUpper()}", p, null);
        }

        [Command("vvender")]
        public void CMD_vvender(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (!Global.Pontos.Any(x => x.Tipo == TipoPonto.FerroVelho && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.DistanciaRP))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em um ferro velho.");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Personagem == p.Codigo && x.Vehicle == player.Vehicle);
            if (veh == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em um veículo seu.");
                return;
            }

            var valor = Convert.ToInt32((Global.Precos.FirstOrDefault(x => x.Veiculo && x.Nome.ToLower() == veh.Modelo.ToLower())?.Valor ?? 0) / 2);
            player.Emit("chat:ExibirConfirmacao", "Confirmar Venda", $"Confirma vender o veículo {veh.Modelo.ToUpper()} para o ferro velho por ${valor:N0}?", "VenderVeiculo");
        }

        [Command("valugar")]
        public void CMD_valugar(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p.Emprego == TipoEmprego.Nenhum || !p.EmTrabalho)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não tem um emprego ou não está em serviço.");
                return;
            }

            var emp = Global.Empregos.FirstOrDefault(x => x.Tipo == p.Emprego);
            if (!player.IsInVehicle)
            {
                if (player.Position.Distance(emp.PosicaoAluguel) > Global.DistanciaRP)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está no aluguel de veículos para seu emprego.");
                    return;
                }
            }

            if (Global.Veiculos.Any(x => x.NomeEncarregado == p.Nome && x.DataExpiracaoAluguel.HasValue))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já possui um veículo alugado.");
                return;
            }

            var preco = Global.Precos.FirstOrDefault(x => x.Tipo == TipoPreco.AluguelEmpregos && x.Nome.ToLower() == p.Emprego.ToString().ToLower())?.Valor ?? 0;
            if (p.Dinheiro < preco)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Você não possui dinheiro suficiente $({preco:N0}).");
                return;
            }

            Veiculo veh = null;
            if (player.IsInVehicle)
            {
                veh = Global.Veiculos.FirstOrDefault(x => x.Vehicle == player.Vehicle && x.Emprego == p.Emprego && !x.DataExpiracaoAluguel.HasValue);
                if (veh == null)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está dentro de um veículo disponível para aluguel.");
                    return;
                }
            }
            else
            {
                using var context = new DatabaseContext();
                var vehs = context.Veiculos.AsQueryable().Where(x => x.Emprego == p.Emprego).ToList();
                veh = vehs.FirstOrDefault(x => !Global.Veiculos.Any(y => y.Codigo == x.Codigo));
                if (veh == null)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Não há nenhum veículo disponível para aluguel.");
                    return;
                }
            }

            veh.NomeEncarregado = p.Nome;
            veh.DataExpiracaoAluguel = DateTime.Now.AddHours(1);

            if (!player.IsInVehicle)
            {
                veh.PosX = emp.PosicaoAluguel.X;
                veh.PosY = emp.PosicaoAluguel.Y;
                veh.PosZ = emp.PosicaoAluguel.Z;
                veh.RotX = emp.RotacaoAluguel.Roll;
                veh.RotY = emp.RotacaoAluguel.Pitch;
                veh.RotZ = emp.RotacaoAluguel.Yaw;
                veh.Combustivel = veh.TanqueCombustivel;
                veh.Spawnar();
                veh.Vehicle.LockState = VehicleLockState.Unlocked;
                player.Emit("setPedIntoVehicle", veh.Vehicle, -1);
            }

            p.Dinheiro -= preco;
            p.SetDinheiro();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você alugou um {emp.Veiculo.ToString().ToUpper()} por uma hora por ${preco:N0} com expiração em {veh.DataExpiracaoAluguel}.");
        }
    }
}