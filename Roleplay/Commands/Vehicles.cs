using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Roleplay.Models;
using System;
using System.Linq;

namespace Roleplay.Commands
{
    public class Vehicles
    {
        [Command("motor")]
        public void CMD_motor(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado.");
                return;
            }

            if (!player.IsInVehicle || player.Vehicle?.Driver != player)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é o motorista de um veículo.");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Vehicle == player.Vehicle);
            if (veh.Personagem != p.Codigo && (veh.Faccao != p.Faccao || veh.Faccao == 0))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui acesso ao veículo.");
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você {(player.Vehicle.EngineOn ? "des" : string.Empty)}ligou o motor do veículo.", notify: true);
            player.Emit("vehicle:setVehicleEngineOn", player.Vehicle, !player.Vehicle.EngineOn);
        }

        [Command("vcomprarvaga")]
        public void CMD_vcomprarvaga(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado.");
                return;
            }

            if (p.Dinheiro < Global.Parametros.ValorVagaVeiculo)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente.");
                return;
            }

            if (!player.IsInVehicle || player.Vehicle?.Driver != player)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está no banco de motorista de um veículo.");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Vehicle == player.Vehicle);
            if (veh.Personagem != p.Codigo)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é o proprietário do veículo.");
                return;
            }

            veh.PosX = player.Vehicle.Position.X;
            veh.PosY = player.Vehicle.Position.Y;
            veh.PosZ = player.Vehicle.Position.Z;
            veh.RotX = player.Vehicle.Rotation.Roll;
            veh.RotY = player.Vehicle.Rotation.Pitch;
            veh.RotZ = player.Vehicle.Rotation.Yaw;

            using (var context = new DatabaseContext())
            {
                context.Veiculos.Update(veh);
                context.SaveChanges();
            }

            p.Dinheiro -= Global.Parametros.ValorVagaVeiculo;
            p.SetDinheiro();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você comprou uma vaga por ${Global.Parametros.ValorVagaVeiculo:N0}.", notify: true);
        }

        [Command("vestacionar")]
        public void CMD_vestacionar(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado.");
                return;
            }

            if (!player.IsInVehicle || player.Vehicle?.Driver != player)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está no banco de motorista de um veículo.");
                return;
            }

            var veh = Global.Veiculos.FirstOrDefault(x => x.Vehicle == player.Vehicle);
            if (veh.Personagem == p.Codigo)
            {
                if (player.Vehicle.Position.Distance(new Position(veh.PosX, veh.PosY, veh.PosZ)) > Constants.DistanciaRP)
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de sua vaga.");
                    return;
                }

                veh.Despawnar();
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você estacionou o veículo.", notify: true);
                return;
            }

            if (veh.Faccao == p.Faccao && veh.Faccao > 0)
            {
                if (!Global.Pontos.Any(x => x.Tipo == TipoPonto.SpawnVeiculosFaccao && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Constants.DistanciaRP))
                {
                    Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum ponto de spawn de veículos da facção.");
                    return;
                }

                veh.Despawnar();
                Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você estacionou o veículo.", notify: true);
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui acesso ao veículo.");
        }

        [Command("vspawn", "/vspawn (código do veículo)")]
        public void CMD_vspawn(IPlayer player, int codigo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado.");
                return;
            }

            if (Global.Veiculos.Any(x => x.Codigo == codigo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Veículo já está spawnado.");
                return;
            }

            using var context = new DatabaseContext();
            var veh = context.Veiculos.FirstOrDefault(x => x.Codigo == codigo);
            if (veh?.Personagem != p.Codigo)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não é o proprietário do veículo.");
                return;
            }

            if (veh.ValorApreensao > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Veículo está apreendido.");
                return;
            }

            veh.Spawnar();
            player.Emit("Server:SetWaypoint", veh.PosX, veh.PosY);
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você spawnou seu veículo.", notify: true);
        }

        [Command("vlista")]
        public void CMD_vlista(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado.");
                return;
            }

            using var context = new DatabaseContext();
            var veiculos = context.Veiculos.Where(x => x.Personagem == p.Codigo).ToList();
            if (veiculos.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui veículos.");
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Titulo, $"Veículos de {p.Nome} [{p.Codigo}]");
            foreach (var v in veiculos)
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Código: {v.Codigo} | Modelo: {v.Modelo.ToUpper()} | Placa: {v.Placa} | Spawnado: {(Global.Veiculos.Any(x => x.Codigo == v.Codigo) ? $"{{{Global.CorSucesso}}}SIM" : $"{{{Global.CorErro}}}NÃO")}{{#FFFFFF}} | Apreendido: {(v.ValorApreensao > 0 ? $"{{{Global.CorSucesso}}}SIM (${v.ValorApreensao:N0})" : $"{{{Global.CorErro}}}NÃO")}");
        }

        [Command("vvender", "/vvender (ID ou nome) (valor)")]
        public void CMD_vvender(IPlayer player, string idNome, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado.");
                return;
            }

            var prox = Global.Veiculos
                .Where(x => x.Personagem == p.Codigo && player.Position.Distance(x.Vehicle.Position) <= Constants.DistanciaRP)
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

            if (player.Position.Distance(target.Player.Position) > Constants.DistanciaRP || player.Dimension != target.Player.Dimension)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo de você.");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor não é válido.");
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
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado.");
                return;
            }

            if (!Global.Pontos.Any(x => x.Tipo == TipoPonto.LiberacaoVeiculos && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Constants.DistanciaRP))
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
            var veh = context.Veiculos.FirstOrDefault(x => x.Codigo == codigo);
            if (veh?.Personagem != p.Codigo)
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

            var apreensao = context.Apreensoes.Where(x => x.Veiculo == veh.Codigo).ToList().LastOrDefault();
            apreensao.DataPagamento = DateTime.Now;
            context.Apreensoes.Update(apreensao);

            context.SaveChanges();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você liberou seu veículo.");
        }
    }
}