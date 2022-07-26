using AltV.Net;
using AltV.Net.Async;
using Microsoft.EntityFrameworkCore;
using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Roleplay.Scripts
{
    public class MDCScript : IScript
    {
        [AsyncClientEvent(nameof(MDCPesquisarPessoa))]
        public async Task MDCPesquisarPessoa(MyPlayer player, string pesquisa)
        {
            player.Emit("Server:AtualizarMDC", "btn-pesquisarpessoa", "div-pesquisarpessoa",
                await Functions.GetSearchPersonHTML(pesquisa));
        }

        [AsyncClientEvent(nameof(MDCPesquisarVeiculo))]
        public async Task MDCPesquisarVeiculo(MyPlayer player, string pesquisa)
        {
            player.Emit("Server:AtualizarMDC", "btn-pesquisarveiculo", "div-pesquisarveiculo",
                await Functions.GetSearchVehicleHTML(pesquisa));
        }

        [AsyncClientEvent(nameof(MDCPesquisarPropriedade))]
        public async Task MDCPesquisarPropriedade(MyPlayer player, string pesquisa)
        {
            var html = string.Empty;
            var prop = Global.Properties.FirstOrDefault(x => x.Id.ToString() == pesquisa);
            if (prop == null)
            {
                html = $@"<div class='alert alert-danger'>Nenhuma propriedade foi encontrada com a pesquisa <strong>{pesquisa}</strong>.</div>";
            }
            else
            {
                var proprietario = "N/A";
                if (prop.CharacterId.HasValue)
                {
                    await using var context = new DatabaseContext();
                    var per = await context.Characters.FirstOrDefaultAsync(x => x.Id == prop.CharacterId);
                    proprietario = per?.Name ?? string.Empty;
                }

                html = $@"<h3>Propriedade Nº {prop.Id}</h3>
                    <div class='row'>
                        <div class='col-md-12'>
                            <p>Endereço: <strong>{prop.Address}</strong></p>
                        </div>
                        <div class='col-md-12'>
                            <p>Proprietário: <strong>{proprietario}</strong></p>
                        </div>
                        <div class='col-md-12'>
                            <p>Valor: <strong>${prop.Value:N0}</strong></p>
                        </div>
                    </div>";
            }

            player.Emit("Server:AtualizarMDC", "btn-pesquisarpropriedade", "div-pesquisarpropriedade", html);
        }

        [ClientEvent(nameof(MDCRastrearVeiculo))]
        public void MDCRastrearVeiculo(MyPlayer player, int codigo)
        {
            var veh = Global.Vehicles.FirstOrDefault(x => x.Vehicle.Id == codigo);
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, "O veículo não foi encontrado.", notify: true);
                return;
            }

            player.Emit("Server:SetWaypoint", veh.Position.X, veh.Position.Y);
            player.SendMessage(MessageType.Success, "A posição do veículo foi marcada no seu GPS.", notify: true);
        }

        [AsyncClientEvent(nameof(MDCAdicionarBOLO))]
        public async Task MDCAdicionarBOLO(MyPlayer player, int tipo, int codigo, string motivo, string pesquisa)
        {
            var bolo = new Wanted
            {
                PoliceOfficerCharacterId = player.Character.Id,
                Reason = motivo,
            };

            if (tipo == 1)
                bolo.WantedCharacterId = codigo;
            else
                bolo.WantedVehicleId = codigo;

            await using var context = new DatabaseContext();
            await context.Wanted.AddAsync(bolo);
            await context.SaveChangesAsync();

            var procurados = await Functions.GetWantedsHTML();

            string html;
            string botao;
            string div;

            string html2;
            string div2;

            if (bolo.WantedCharacterId > 0)
            {
                html = await Functions.GetSearchPersonHTML(pesquisa);
                botao = "btn-pesquisarpessoa";
                div = "div-pesquisarpessoa";

                html2 = procurados.Item1;
                div2 = "tab-apb";
            }
            else
            {
                html = await Functions.GetSearchVehicleHTML(pesquisa);
                botao = "btn-pesquisarveiculo";
                div = "div-pesquisarveiculo";

                html2 = procurados.Item2;
                div2 = "tab-bolo";
            }

            foreach (var x in Global.Players.Where(x => x.Character.FactionId == player.Character.FactionId))
            {
                x.Emit("Server:AtualizarMDC", botao, div, html);
                x.Emit("Server:AtualizarMDC", string.Empty, div2, html2);
            }

            player.SendMessage(MessageType.Success, $"{(tipo == 1 ? "APB" : "BOLO")} adicionado.", notify: true);
        }

        [AsyncClientEvent(nameof(MDCRemoverBOLO))]
        public async Task MDCRemoverBOLO(MyPlayer player, int codigo, string pesquisa)
        {
            await using var context = new DatabaseContext();
            var bolo = await context.Wanted.FirstOrDefaultAsync(x => x.Id == codigo);
            bolo.PoliceOfficerDeletedCharacterId = player.Character.Id;
            bolo.DeletedDate = DateTime.Now;
            context.Wanted.Update(bolo);
            await context.SaveChangesAsync();

            var procurados = await Functions.GetWantedsHTML();

            string html;
            string botao;
            string div;

            string html2;
            string div2;

            if (bolo.WantedCharacterId > 0)
            {
                html = await Functions.GetSearchPersonHTML(pesquisa);
                botao = "btn-pesquisarpessoa";
                div = "div-pesquisarpessoa";

                html2 = procurados.Item1;
                div2 = "tab-apb";
            }
            else
            {
                html = await Functions.GetSearchVehicleHTML(pesquisa);
                botao = "btn-pesquisarveiculo";
                div = "div-pesquisarveiculo";

                html2 = procurados.Item2;
                div2 = "tab-bolo";
            }

            foreach (var x in Global.Players.Where(x => x.Character.FactionId == player.Character.FactionId))
            {
                x.Emit("Server:AtualizarMDC", botao, div, html);
                x.Emit("Server:AtualizarMDC", string.Empty, div2, html2);
            }

            player.SendMessage(MessageType.Success, $"{(bolo.WantedCharacterId > 0 ? "APB" : "BOLO")} removido.", notify: true);
        }

        [ClientEvent(nameof(MDCRastrear911))]
        public void MDCRastrear911(MyPlayer player, int codigo)
        {
            var ligacao911 = Global.EmergencyCalls.FirstOrDefault(x => x.Id == codigo);
            player.Emit("Server:SetWaypoint", ligacao911.PosX, ligacao911.PosY);
            player.SendMessage(MessageType.Success, $"A localização da ligação de emergência #{codigo} foi marcada no seu GPS.", notify: true);
        }

        [AsyncClientEvent(nameof(MDCMultar))]
        public async Task MDCMultar(MyPlayer player, int codigo, string nome, int valor, string motivo)
        {
            if (valor <= 0)
            {
                player.SendMessage(MessageType.Error, "Valor inválido.", notify: true);
                return;
            }

            if (string.IsNullOrWhiteSpace(motivo) || motivo.Length > 255)
            {
                player.SendMessage(MessageType.Error, "Motivo precisa ter entre 1 e 255 caracteres.", notify: true);
                return;
            }

            await using var context = new DatabaseContext();
            await context.Fines.AddAsync(new Fine()
            {
                Reason = motivo,
                CharacterId = codigo,
                PoliceOfficerCharacterId = player.Character.Id,
                Value = valor,
            });
            await context.SaveChangesAsync();

            player.SendMessage(MessageType.Success, $"Você multou {nome} por ${valor:N0}. Motivo: {motivo}", notify: true);

            var target = Global.Players.FirstOrDefault(x => x.Character.Id == codigo && x.Cellphone > 0 && !x.CellphoneItem.ModoAviao);
            if (target != null)
                target.SendMessage(MessageType.None, $"[CELULAR] SMS de {target.ObterNomeContato(Global.EMERGENCY_NUMBER)}: Você recebeu uma multa de ${valor:N0}. Motivo: {motivo}", Global.CELLPHONE_MAIN_COLOR);

            var html = await Functions.GetSearchPersonHTML(nome);
            foreach (var x in Global.Players.Where(x => x.Character.FactionId == player.Character.FactionId))
                x.Emit("Server:AtualizarMDC", "btn-pesquisarpessoa", "div-pesquisarpessoa", html);
        }

        [AsyncClientEvent(nameof(MDCRevogarLicencaMotorista))]
        public async Task MDCRevogarLicencaMotorista(MyPlayer player, int codigo)
        {
            var target = Global.Players.FirstOrDefault(x => x.Character.Id == codigo);
            if (target != null)
                target.Character.PoliceOfficerBlockedDriverLicenseCharacterId = player.Character.Id;

            await using var context = new DatabaseContext();
            var per = await context.Characters.FirstOrDefaultAsync(x => x.Id == codigo);
            per.PoliceOfficerBlockedDriverLicenseCharacterId = player.Character.Id;
            context.Characters.Update(per);
            await context.SaveChangesAsync();

            var html = await Functions.GetSearchPersonHTML(per.Name);
            foreach (var x in Global.Players.Where(x => x.Character.FactionId == player.Character.FactionId))
                x.Emit("Server:AtualizarMDC", "btn-pesquisarpessoa", "div-pesquisarpessoa", html);
        }

        [AsyncClientEvent(nameof(MDCAdicionarUnidade))]
        public async Task MDCAdicionarUnidade(MyPlayer player, string nome, string numeracao, string plate, string parceiros)
        {
            if (string.IsNullOrWhiteSpace(nome) || nome.Length > 25)
            {
                player.Emit("Server:MostrarErro", "Nome precisa ter entre 1 e 25 caracteres.", "btn-adicionarunidade");
                return;
            }

            if (string.IsNullOrWhiteSpace(numeracao) || numeracao.Length > 25)
            {
                player.Emit("Server:MostrarErro", "Numeração precisa ter entre 1 e 25 caracteres.", "btn-adicionarunidade");
                return;
            }

            if (string.IsNullOrWhiteSpace(plate) || plate.Length > 25)
            {
                player.Emit("Server:MostrarErro", "Placa precisa ter entre 1 e 8 caracteres.", "btn-adicionarunidade");
                return;
            }

            if (Global.FactionsUnits.Any(x => x.CharacterId == player.Character.Id)
                || Global.FactionsUnitsCharacters.Any(x => x.CharacterId == player.Character.Id))
            {
                player.Emit("Server:MostrarErro", "Você já está em uma unidade.", "btn-adicionarunidade");
                return;
            }

            var unidadesPersonagens = new List<MyPlayer>();
            if (!string.IsNullOrWhiteSpace(parceiros))
            {
                var personagens = (parceiros ?? string.Empty).Split(',').ToList();
                foreach (var x in personagens)
                {
                    if (!int.TryParse(x.Trim(), out int personagem))
                    {
                        player.Emit("Server:MostrarErro", $"O ID {x} informado em parceiros é inválido.", "btn-adicionarunidade");
                        return;
                    }

                    if (personagem == player.SessionId)
                    {
                        player.Emit("Server:MostrarErro", $"O ID {x} informado em parceiros é o seu ID.", "btn-adicionarunidade");
                        return;
                    }

                    var target = Global.Players.FirstOrDefault(x => x.SessionId == personagem);
                    if (target?.Character?.FactionId != player.Character.FactionId)
                    {
                        player.Emit("Server:MostrarErro", $"ID {personagem} não é da sua facção ou está offline.", "btn-adicionarunidade");
                        return;
                    }

                    if (Global.FactionsUnits.Any(x => x.CharacterId == target.Character.Id)
                        || Global.FactionsUnitsCharacters.Any(x => x.CharacterId == target.Character.Id))
                    {
                        player.Emit("Server:MostrarErro", $"ID {personagem} já está em uma unidade.", "btn-adicionarunidade");
                        return;
                    }

                    unidadesPersonagens.Add(target);
                }
            }

            var factionUnit = new FactionUnit
            {
                CharacterId = player.Character.Id,
                FactionId = player.Character.FactionId ?? 0,
                Name = nome,
                Description = numeracao,
                Plate = plate,
            };

            await using var context = new DatabaseContext();
            await context.FactionsUnits.AddAsync(factionUnit);
            await context.SaveChangesAsync();

            factionUnit.Characters = new List<FactionUnitCharacter>();

            if (unidadesPersonagens.Any())
            {
                var factionsUnitsCharacters = unidadesPersonagens.Select(x => new FactionUnitCharacter
                {
                    FactionUnitId = factionUnit.Id,
                    CharacterId = x.Character.Id,
                }).ToList();
                await context.FactionsUnitsCharacters.AddRangeAsync(factionsUnitsCharacters);
                await context.SaveChangesAsync();

                foreach (var factionUnitCharacter in factionsUnitsCharacters)
                    factionUnitCharacter.Character = unidadesPersonagens.FirstOrDefault(x => x.Character.Id == factionUnitCharacter.CharacterId)?.Character;

                factionUnit.Characters = factionsUnitsCharacters;
                Global.FactionsUnitsCharacters.AddRange(factionsUnitsCharacters);
            }

            factionUnit.Character = player.Character;
            Global.FactionsUnits.Add(factionUnit);

            foreach (var target in Global.Players.Where(x => x.Character.FactionId == player.Character.FactionId))
                target.Emit("Server:AtualizarMDC", "btn-adicionarunidade", "div-unidades", Functions.GetFactionsUnitsHTML(player));
        }

        [AsyncClientEvent(nameof(MDCEncerrarUnidade))]
        public async Task MDCEncerrarUnidade(MyPlayer player, int id)
        {
            var factionUnit = Global.FactionsUnits.FirstOrDefault(x => x.Id == id && x.CharacterId == player.Character.Id);
            if (factionUnit == null)
                return;

            factionUnit.FinalDate = DateTime.Now;

            await using var context = new DatabaseContext();
            context.FactionsUnits.Update(factionUnit);
            await context.SaveChangesAsync();

            Global.FactionsUnits.Remove(factionUnit);
            Global.FactionsUnitsCharacters.RemoveAll(x => x.FactionUnitId == factionUnit.Id);

            foreach (var target in Global.Players.Where(x => x.Character.FactionId == player.Character.FactionId))
                target.Emit("Server:AtualizarMDC", $"btn-encerrarunidade{factionUnit.Id}", "div-unidades", Functions.GetFactionsUnitsHTML(player));
        }
    }
}