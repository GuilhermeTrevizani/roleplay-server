using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Data;
using Microsoft.EntityFrameworkCore;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class MDCScript : IScript
    {
        [Command("mdc")]
        public static async Task CMD_mdc(MyPlayer player)
        {
            if (!(player.Faction?.Government ?? false) || !player.OnDuty)
            {
                player.SendMessage(MessageType.Error, "Você não está em uma facção governamental ou não está em serviço.");
                return;
            }

            if (!Global.Spots.Any(x => x.Type == SpotType.MDC
                && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= Global.RP_DISTANCE)
                && !(Global.Vehicles.FirstOrDefault(x => x == player.Vehicle)?.VehicleDB?.FactionId == player.Character.FactionId
                    && (player.Seat == 1 || player.Seat == 2)))
            {
                player.SendMessage(MessageType.Error, "Você não está em ponto de MDC ou em um veículo da sua facção nos bancos dianteiros.");
                return;
            }

            var htmlLigacoes911 = string.Empty;
            var htmlAPB = string.Empty;
            var htmlBOLO = string.Empty;
            var htmlUnidades = GetFactionsUnitsHTML(player);
            var htmlRelatoriosPendentes = string.Empty;

            var ligacoes911 = Global.EmergencyCalls.Where(x => ((int)x.Type == (int)player.Faction.Type || x.Type == EmergencyCallType.Both)
                && (DateTime.Now - x.Date).TotalHours < 24)
                .OrderByDescending(x => x.Id).ToList();
            if (ligacoes911.Count == 0)
            {
                htmlLigacoes911 = "<div class='alert alert-danger'>Não houve nenhum 911 nas últimas 24 horas.</div>";
            }
            else
            {
                htmlLigacoes911 = $@"<div class='table-responsive' style='max-height:60vh;overflow-y:auto;overflow-x:auto;'>
                        <table class='table table-bordered table-striped'>
                            <thead>
                                <tr class='bg-dark'>
                                    <th>Código</th>
                                    <th>Data</th>
                                    <th>Número</th>
                                    <th>Localização</th>
                                    <th>Mensagem</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>";
                foreach (var x in ligacoes911)
                    htmlLigacoes911 += $@"<tr>
                                    <td>{x.Id}</td>
                                    <td>{x.Date}</td>
                                    <td>{x.Number}</td>
                                    <td>{x.Location}</td>
                                    <td>{x.Message}</td>
                                    <td class='text-center'><button class='btn btn-dark btn-xs' onclick='rastrear911({x.Id});'>Rastrear</button></td>
                                </tr>";
                htmlLigacoes911 += $@"</tbody>
                        </table>
                    </div>";
            }

            if (player.Faction.Type == FactionType.Police)
            {
                var procurados = await GetWantedsHTML();
                htmlAPB = procurados.Item1;
                htmlBOLO = procurados.Item2;

                // Apreensões veiculares, Confiscos e Prisões
                // Apreensões Veiculares e Prisões (/confiscar tbm? criar um Confiscos)
                // relatorios pendentes listar da facçção inteira  e ordenar para o do proprio usuario primeiro e aparecer botão só pra preencher se for o dono do relatorio
                // OBterHtmlRelatoriosPendentes pra depois atualizar
            }

            player.Emit("Server:AbrirMDC", (int)player.Faction.Type, player.Faction.Name, htmlLigacoes911, htmlAPB, htmlBOLO, htmlUnidades, htmlRelatoriosPendentes);
            player.SendMessageToNearbyPlayers("abre o MDC.", MessageCategory.Ame, 10);
        }

        private async static Task<Tuple<string, string>> GetWantedsHTML()
        {
            var htmlAPB = string.Empty;
            var htmlBOLO = string.Empty;
            await using var context = new DatabaseContext();
            var wanted = await context.Wanted
                .Where(x => !x.DeletedDate.HasValue)
                .Include(x => x.PoliceOfficerCharacter)
                .Include(x => x.WantedCharacter)
                .Include(x => x.WantedVehicle)
                .ToListAsync();

            var apbs = wanted.Where(x => x.WantedCharacterId.HasValue);
            var bolos = wanted.Where(x => x.WantedVehicleId.HasValue);

            if (!apbs.Any())
            {
                htmlAPB = $@"<div class='alert alert-danger'>Nenhum APB encontrado.</div>";
            }
            else
            {
                htmlAPB = $@"<div class='table-responsive' style='max-height:60vh;overflow-y:auto;overflow-x:auto;'>
                            <table class='table table-bordered table-striped'>
                            <thead>
                                <tr class='bg-dark'>
                                    <th>Código</th>
                                    <th>Data</th>
                                    <th>Policial</th>
                                    <th>Procurado</th>
                                    <th>Motivo</th>
                                </tr>
                            </thead>
                            <tbody>";

                foreach (var x in apbs)
                    htmlAPB += $@"<tr>
                            <td>{x.Id}</td>
                            <td>{x.Date}</td>
                            <td>{x.PoliceOfficerCharacter!.Name}</td>
                            <td>{x.WantedCharacter!.Name}</td>
                            <td>{x.Reason}</td>
                            </tr>";

                htmlAPB += @"</tbody>
                        </table>
                        </div>";
            }

            if (!bolos.Any())
            {
                htmlBOLO = $@"<div class='alert alert-danger'>Nenhum BOLO encontrado.</div>";
            }
            else
            {
                htmlBOLO = $@"<div class='table-responsive' style='max-height:60vh;overflow-y:auto;overflow-x:auto;'>
                            <table class='table table-bordered table-striped'>
                            <thead>
                                <tr class='bg-dark'>
                                    <th>Código</th>
                                    <th>Data</th>
                                    <th>Policial</th>
                                    <th>Veículo</th>
                                    <th>Motivo</th>
                                </tr>
                            </thead>
                            <tbody>";

                foreach (var x in bolos)
                    htmlBOLO += $@"<tr>
                            <td>{x.Id}</td>
                            <td>{x.Date}</td>
                            <td>{x.PoliceOfficerCharacter!.Name}</td>
                            <td>{x.WantedVehicle!.Model} {x.WantedVehicle.Plate}</td>
                            <td>{x.Reason}</td>
                            </tr>";

                htmlBOLO += @"</tbody>
                        </table>
                        </div>";
            }

            return new Tuple<string, string>(htmlAPB, htmlBOLO);
        }

        private static string GetFactionsUnitsHTML(MyPlayer player)
        {
            var html = string.Empty;
            var factionUnits = Global.FactionsUnits.Where(x => x.FactionId == player.Character.FactionId);
            if (!factionUnits.Any())
            {
                html = $@"<div class='alert alert-danger'>Nenhuma unidade em serviço.</div>";
            }
            else
            {
                html = @"<div class='table-responsive' style='max-height:60vh;overflow-y:auto;overflow-x:auto;'>
                            <table class='table table-bordered table-striped'>
                            <thead>
                                <tr class='bg-dark'>
                                    <th>Nome</th>
                                    <th>Numeração</th>
                                    <th>Placa</th>
                                    <th>Início</th>
                                    <th>Ocupantes</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>";

                foreach (var x in factionUnits)
                {
                    html += $@"<tr>
                            <td>{x.Name}</td>
                            <td>{x.Description}</td>
                            <td>{x.Plate}</td>
                            <td>{x.InitialDate}</td>
                            <td>{string.Join(", ", x.Characters!.Select(y => y.Character!.Name))}</td>
                            <td class='text-center'>{(x.CharacterId == player.Character.Id ? $"<button id='btn-encerrarunidade{x.Id}' class='btn btn-dark btn-xs' onclick='encerrarUnidade({x.Id});'>Encerrar</button>" : string.Empty)}</td>
                            </tr>";
                }

                html += @"</tbody>
                        </table>
                        </div>";
            }
            return html;
        }

        private static async Task<string> GetSearchPersonHTML(string search)
        {
            var html = string.Empty;
            await using var context = new DatabaseContext();
            var character = await context.Characters.Where(x => x.Name.ToLower() == search.ToLower())
                .Include(x => x.PoliceOfficerBlockedDriverLicenseCharacter)
                .FirstOrDefaultAsync();
            if (character == null)
            {
                html = $@"<div class='alert alert-danger'>Nenhuma pessoa foi encontrada com o nome <strong>{search}</strong>.</div>";
            }
            else
            {
                var properties = Global.Properties
                    .Where(x => x.CharacterId == character.Id)
                    .ToList();

                var vehicles = await context.Vehicles
                    .Where(x => x.CharacterId == character.Id && !x.Sold)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

                var fines = await context.Fines
                    .Where(x => x.CharacterId == character.Id)
                    .Include(x => x.PoliceOfficerCharacter)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

                var jails = await context.Jails
                    .Where(x => x.CharacterId == character.Id)
                    .Include(x => x.PoliceOfficerCharacter)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

                var confiscations = await context.Confiscations
                    .Where(x => x.CharacterId == character.Id)
                    .Include(x => x.PoliceOfficerCharacter)
                    .Include(x => x.Items)
                    .OrderByDescending(x => x.Id)
                    .ToListAsync();

                var job = character.Job.GetDisplay();
                if (character.FactionId.HasValue)
                {
                    var faction = Global.Factions.FirstOrDefault(x => x.Id == character.FactionId);
                    if (faction?.Type != FactionType.Criminal)
                        job = $"{Global.FactionsRanks.FirstOrDefault(x => x.Id == character.FactionRankId)?.Name ?? string.Empty} - {faction?.Name ?? string.Empty}";
                }

                var revokeDriverLicense = $@"<button onclick='revogarLicenca({character.Id},""{character.Name}"");' type='button' class='btn btn-xs btn-dark'>Revogar Licença de Motorista</button>";
                var driverLicense = $"<span class='label' style='background-color:{Global.SUCCESS_COLOR};'>VÁLIDA</span>";
                if (character.PoliceOfficerBlockedDriverLicenseCharacterId.HasValue)
                {
                    driverLicense = $"<span class='label' style='background-color:{Global.ERROR_COLOR};'>REVOGADA POR {character.PoliceOfficerBlockedDriverLicenseCharacter?.Name?.ToUpper() ?? string.Empty}</span>";
                    revokeDriverLicense = string.Empty;
                }
                else if (!character.DriverLicenseValidDate.HasValue)
                {
                    driverLicense = $"<span class='label' style='background-color:{Global.ERROR_COLOR};'>NÃO POSSUI</span>";
                    revokeDriverLicense = string.Empty;
                }
                else if (character.DriverLicenseValidDate?.Date < DateTime.Now.Date)
                {
                    driverLicense = $"<span class='label' style='background-color:{Global.ERROR_COLOR};'>VENCIDA</span>";
                    revokeDriverLicense = string.Empty;
                }

                var strBolo = string.Empty;
                var boloButton = $@"<button onclick='adicionarBOLO(1, {character.Id},""{character.Name}"");' type='button' class='btn btn-xs btn-dark'>Adicionar APB</button>";

                var bolo = await context.Wanted.Where(x => x.WantedCharacterId == character.Id && !x.DeletedDate.HasValue).Include(x => x.PoliceOfficerCharacter).FirstOrDefaultAsync();
                if (bolo != null)
                {
                    boloButton = $@"<button onclick='removerBOLO({bolo.Id},""{character.Name}"");' type='button' class='btn btn-xs btn-dark'>Remover APB</button>";
                    strBolo = $@"<div class='row'>
                            <div class='col-md-12'>
                                <div class='well' style='margin-top:10px'>
                                    <p> <span class='label label-danger label-md'>APB</span> {bolo.Reason}</p> 
                                    <p class='text-right'>Por <strong>{bolo.PoliceOfficerCharacter?.Name ?? string.Empty}</strong> em <strong>{bolo.Date}</strong>.</p>
                                </div>
                            </div>
                        </div>";
                }

                html = $@"<div class='row'>
                                <div class='col-md-6'>
                                    <h3>{character.Name}</h3>
                                </div>
                                <div class='col-md-6 text-right middle' style='vertical-align:middle;'>
                                    <button onclick='multar({character.Id}, ""{character.Name}"");' type='button' class='btn btn-xs btn-dark'>Multar</button>
                                    {boloButton}
                                    {revokeDriverLicense}
                                </div>
                            </div>
                            <div class='row'>
                                <div class='col-md-12'>
                                    <p>Data de Nascimento: <strong>{character.BirthdayDate.ToShortDateString()} ({Math.Truncate((DateTime.Now.Date - character.BirthdayDate).TotalDays / 365):N0} anos)</strong></p>
                                </div>
                                <div class='col-md-12'>
                                    <p>Sexo: <strong>{character.Sex.GetDisplay()}</strong></p>
                                </div>
                                <div class='col-md-12'>
                                    <p>Emprego: <strong>{job}</strong></p>
                                </div>
                                <div class='col-md-12'>
                                    <p>Licença de Motorista: {driverLicense}</p>
                                </div>
                            </div>
                            {strBolo}";

                if (!string.IsNullOrWhiteSpace(character.Image))
                    html += $"<img src='data:image/png;base64, {character.Image}' alt='headshot' />";

                if (properties.Count > 0)
                {
                    html += $@"<h4>Propriedades</h4>";
                    foreach (var x in properties)
                        html += $@"<p>Nº {x.Id} - {x.Address}</p>";
                }

                if (vehicles.Count > 0)
                {
                    html += $@"<h4>Veículos</h4>";
                    foreach (var x in vehicles)
                        html += $@"<p>Modelo: <strong>{x.Model.ToUpper()}</strong> Placa: <strong>{x.Plate}</strong></p>";
                }

                if (fines.Count > 0)
                {
                    html += $@"<h4>Multas</h4>";
                    foreach (var x in fines)
                    {
                        var details = !string.IsNullOrWhiteSpace(x.Description) ? $@"<button onclick='$.alert(""<p>{x.Description}</p>"");' class='btn btn-dark btn-xs'>Detalhes</button>" : string.Empty;
                        html += $@"<p>Data: <strong>{x.Date}</strong> Valor: <strong>${x.Value:N0}</strong> Policial: <strong>{x.PoliceOfficerCharacter?.Name ?? string.Empty}</strong> Motivo: <strong>{x.Reason}</strong> Status: <strong>{(x.PaymentDate.HasValue ? $"<span class='label' style='background-color:{Global.SUCCESS_COLOR};'>PAGA</span>" : $"<span class='label' style='background-color:{Global.ERROR_COLOR};'>PENDENTE</span>")}</strong> {details}</p>";
                    }
                }

                if (jails.Count > 0)
                {
                    html += $@"<h4>Prisões</h4>";
                    foreach (var x in jails)
                    {
                        var details = !string.IsNullOrWhiteSpace(x.Description) ? $@"<button onclick='$.alert(""<p>{x.Description}</p>"");' class='btn btn-dark btn-xs'>Detalhes</button>" : string.Empty;
                        html += $@"<p>Data: <strong>{x.Date}</strong> Tempo: <strong>{Convert.ToInt32((x.EndDate - x.Date).TotalMinutes)} minutos</strong> Policial: <strong>{x.PoliceOfficerCharacter?.Name ?? string.Empty}</strong></p> {details}</p>";
                    }
                }

                if (confiscations.Count > 0)
                {
                    html += $@"<h4>Confiscos</h4>";
                    foreach (var x in confiscations)
                    {
                        var details = !string.IsNullOrWhiteSpace(x.Description) ? $@"<button onclick='$.alert(""<p>{x.Description}</p>"");' class='btn btn-dark btn-xs'>Detalhes</button>" : string.Empty;

                        html += $@"<div class=""well"">Data: <strong>{x.Date}</strong> Policial: <strong>{x.PoliceOfficerCharacter?.Name ?? string.Empty}</strong> Itens: <br/>";

                        foreach (var item in x.Items!)
                            html += $"<p>Nome: <strong>{item.GetName()}</strong> | Quantidade: <strong>{item.Quantity:N0}</strong>{(!string.IsNullOrWhiteSpace(item.Extra) ? $" | Extra: <strong>{item.GetExtra().Replace("<br/>", ", ")}</strong>" : string.Empty)}<p/>";

                        html += $"<br/>{details}</div>";
                    }
                }
            }

            return html;
        }

        public static async Task<string> GetSearchVehicleHTML(string search)
        {
            var html = string.Empty;
            await using var context = new DatabaseContext();
            var vehicle = await context.Vehicles.Where(x => x.Plate.ToLower() == search.ToLower())
                .Include(x => x.Character)
                .Include(x => x.Faction)
                .FirstOrDefaultAsync();
            if (vehicle == null)
            {
                html = $@"<div class='alert alert-danger'>Nenhum veículo foi encontrado com a pesquisa <strong>{search}</strong>.</div>";
            }
            else
            {
                var strBolo = string.Empty;
                var boloButton = string.Empty;
                var owner = $"Emprego de {vehicle.Job.GetDisplay()}";
                if (vehicle.CharacterId.HasValue)
                {
                    boloButton = $@"<button onclick='adicionarBOLO(2, {vehicle.Id},""{search}"");' type='button' class='btn btn-xs btn-dark'>Adicionar BOLO</button>";
                    if (vehicle.Sold)
                        owner = "CONCESSIONÁRIA";
                    else
                        owner = vehicle.Character!.Name;

                    var bolo = await context.Wanted.Where(x => x.WantedVehicleId == vehicle.Id && !x.DeletedDate.HasValue)
                        .Include(x => x.PoliceOfficerCharacter)
                        .FirstOrDefaultAsync();
                    if (bolo != null)
                    {
                        boloButton = $@"<button onclick='removerBOLO({bolo.Id},""{search}"");' type='button' class='btn btn-xs btn-dark'>Remover BOLO</button>";
                        strBolo = $@"<div class='row'>
                            <div class='col-md-12'>
                                <div class='well' style='margin-top:10px'>
                                    <p> <span class='label label-danger label-md'>BOLO</span> {bolo.Reason}</p> 
                                    <p class='text-right'>Por <strong>{bolo.PoliceOfficerCharacter?.Name ?? string.Empty}</strong> em <strong>{bolo.Date}</strong>.</p>
                                </div>
                            </div>
                            </div>";
                    }
                }
                else if (vehicle.FactionId.HasValue)
                {
                    owner = vehicle.Faction!.Name;
                }

                html += $@"<div class='row'>
                    <div class='col-md-6'>
                        <h3>{vehicle.Plate}</h3>
                    </div>
                    <div class='col-md-6 text-right middle' style='vertical-align:middle;'>
                        {boloButton}
                        {(vehicle.FactionId.HasValue || vehicle.Job != CharacterJob.None ? $"<button onclick='rastrearVeiculo({vehicle.Id});' type='button' class='btn btn-xs btn-dark'>Rastrear</button>" : string.Empty)}
                    </div>
                    </div>
                    <div class='row'>
                        <div class='col-md-12'>
                            <p>Modelo: <strong>{vehicle.Model.ToUpper()}</strong></p>
                        </div>
                        <div class='col-md-12'>
                            <p>Proprietário: <strong>{owner}</strong></p>
                        </div>
                        <div class='col-md-12'>
                            <p>Apreendido: <strong>{(vehicle.SeizedValue > 0 ? $"SIM (${vehicle.SeizedValue:N0})" : "NÃO")}</strong></p>
                        </div>
                    </div>
                    {strBolo}";
            }

            return html;
        }

        [AsyncClientEvent(nameof(MDCPesquisarPessoa))]
        public async Task MDCPesquisarPessoa(MyPlayer player, string pesquisa)
        {
            player.Emit("Server:AtualizarMDC", "btn-pesquisarpessoa", "div-pesquisarpessoa",
                await GetSearchPersonHTML(pesquisa));
        }

        [AsyncClientEvent(nameof(MDCPesquisarVeiculo))]
        public async Task MDCPesquisarVeiculo(MyPlayer player, string pesquisa)
        {
            player.Emit("Server:AtualizarMDC", "btn-pesquisarveiculo", "div-pesquisarveiculo",
                await GetSearchVehicleHTML(pesquisa));
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
        public void MDCRastrearVeiculo(MyPlayer player, string idString)
        {
            var id = idString.ToGuid();
            var veh = Global.Vehicles.FirstOrDefault(x => x.VehicleDB.Id == id);
            if (veh == null)
            {
                player.SendMessage(MessageType.Error, "O veículo não foi encontrado.", notify: true);
                return;
            }

            player.Emit("Server:SetWaypoint", veh.Position.X, veh.Position.Y);
            player.SendMessage(MessageType.Success, "A posição do veículo foi marcada no seu GPS.", notify: true);
        }

        [AsyncClientEvent(nameof(MDCAdicionarBOLO))]
        public async Task MDCAdicionarBOLO(MyPlayer player, int type, string idString, string reason, string pesquisa)
        {
            var wanted = new Wanted();

            var id = idString.ToGuid();
            if (type == 1)
                wanted.CreateByCharacter(player.Character.Id, id.Value, reason);
            else
                wanted.CreateByVehicle(player.Character.Id, id.Value, reason);

            await using var context = new DatabaseContext();
            await context.Wanted.AddAsync(wanted);
            await context.SaveChangesAsync();

            var procurados = await GetWantedsHTML();

            string html;
            string botao;
            string div;

            string html2;
            string div2;

            if (wanted.WantedCharacterId.HasValue)
            {
                html = await GetSearchPersonHTML(pesquisa);
                botao = "btn-pesquisarpessoa";
                div = "div-pesquisarpessoa";

                html2 = procurados.Item1;
                div2 = "tab-apb";
            }
            else
            {
                html = await GetSearchVehicleHTML(pesquisa);
                botao = "btn-pesquisarveiculo";
                div = "div-pesquisarveiculo";

                html2 = procurados.Item2;
                div2 = "tab-bolo";
            }

            foreach (var x in Global.SpawnedPlayers.Where(x => x.Character.FactionId == player.Character.FactionId))
            {
                x.Emit("Server:AtualizarMDC", botao, div, html);
                x.Emit("Server:AtualizarMDC", string.Empty, div2, html2);
            }

            player.SendMessage(MessageType.Success, $"{(type == 1 ? "APB" : "BOLO")} adicionado.", notify: true);
        }

        [AsyncClientEvent(nameof(MDCRemoverBOLO))]
        public async Task MDCRemoverBOLO(MyPlayer player, string idString, string pesquisa)
        {
            var id = idString.ToGuid();
            await using var context = new DatabaseContext();
            var wanted = await context.Wanted.FirstOrDefaultAsync(x => x.Id == id);
            if (wanted == null)
            {
                player.SendMessage(MessageType.Error, Global.RECORD_NOT_FOUND, notify: true);
                return;
            }

            wanted.Delete(player.Character.Id);

            context.Wanted.Update(wanted);
            await context.SaveChangesAsync();

            var procurados = await GetWantedsHTML();

            string html;
            string botao;
            string div;

            string html2;
            string div2;

            if (wanted.WantedCharacterId.HasValue)
            {
                html = await GetSearchPersonHTML(pesquisa);
                botao = "btn-pesquisarpessoa";
                div = "div-pesquisarpessoa";

                html2 = procurados.Item1;
                div2 = "tab-apb";
            }
            else
            {
                html = await GetSearchVehicleHTML(pesquisa);
                botao = "btn-pesquisarveiculo";
                div = "div-pesquisarveiculo";

                html2 = procurados.Item2;
                div2 = "tab-bolo";
            }

            foreach (var x in Global.SpawnedPlayers.Where(x => x.Character.FactionId == player.Character.FactionId))
            {
                x.Emit("Server:AtualizarMDC", botao, div, html);
                x.Emit("Server:AtualizarMDC", string.Empty, div2, html2);
            }

            player.SendMessage(MessageType.Success, $"{(wanted.WantedCharacterId.HasValue ? "APB" : "BOLO")} removido.", notify: true);
        }

        [ClientEvent(nameof(MDCRastrear911))]
        public void MDCRastrear911(MyPlayer player, string idString)
        {
            var id = idString.ToGuid();
            var emergencyCall = Global.EmergencyCalls.FirstOrDefault(x => x.Id == id);
            if (emergencyCall == null)
            {
                player.SendMessage(MessageType.Error, Global.RECORD_NOT_FOUND, notify: true);
                return;
            }

            player.Emit("Server:SetWaypoint", emergencyCall.PosX, emergencyCall.PosY);
            player.SendMessage(MessageType.Success, $"A localização da ligação de emergência foi marcada no seu GPS.", notify: true);
        }

        [AsyncClientEvent(nameof(MDCMultar))]
        public async Task MDCMultar(MyPlayer player, string idString, string nome, int value, string reason)
        {
            var id = idString.ToGuid();
            if (value <= 0)
            {
                player.SendMessage(MessageType.Error, "Valor inválido.", notify: true);
                return;
            }

            if (string.IsNullOrWhiteSpace(reason) || reason.Length > 255)
            {
                player.SendMessage(MessageType.Error, "Motivo precisa ter entre 1 e 255 caracteres.", notify: true);
                return;
            }

            await using var context = new DatabaseContext();
            var fine = new Fine();
            fine.Create(id.Value, player.Character.Id, reason, value);
            await context.Fines.AddAsync(fine);
            await context.SaveChangesAsync();

            player.SendMessage(MessageType.Success, $"Você multou {nome} por ${value:N0}. Motivo: {reason}", notify: true);

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.Character.Id == id && x.Cellphone > 0 && !x.CellphoneItem.FlightMode);
            target?.SendMessage(MessageType.None, $"[CELULAR] SMS de {target.ObterNomeContato(Global.EMERGENCY_NUMBER)}: Você recebeu uma multa de ${value:N0}. Motivo: {reason}", Global.CELLPHONE_MAIN_COLOR);

            var html = await GetSearchPersonHTML(nome);
            foreach (var x in Global.SpawnedPlayers.Where(x => x.Character.FactionId == player.Character.FactionId))
                x.Emit("Server:AtualizarMDC", "btn-pesquisarpessoa", "div-pesquisarpessoa", html);
        }

        [AsyncClientEvent(nameof(MDCRevogarLicencaMotorista))]
        public async Task MDCRevogarLicencaMotorista(MyPlayer player, string idString)
        {
            var id = idString.ToGuid();
            await using var context = new DatabaseContext();
            var character = await context.Characters.FirstOrDefaultAsync(x => x.Id == id);
            if (character == null)
            {
                player.SendMessage(MessageType.Error, Global.RECORD_NOT_FOUND, notify: true);
                return;
            }

            var target = Global.SpawnedPlayers.FirstOrDefault(x => x.Character.Id == id);
            target?.Character.SetPoliceOfficerBlockedDriverLicenseCharacterId(player.Character.Id);

            character.SetPoliceOfficerBlockedDriverLicenseCharacterId(player.Character.Id);
            context.Characters.Update(character);
            await context.SaveChangesAsync();

            var html = await GetSearchPersonHTML(character.Name);
            foreach (var x in Global.SpawnedPlayers.Where(x => x.Character.FactionId == player.Character.FactionId))
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
                || Global.FactionsUnits.SelectMany(x => x.Characters!).Any(x => x.CharacterId == player.Character.Id))
            {
                player.Emit("Server:MostrarErro", "Você já está em uma unidade.", "btn-adicionarunidade");
                return;
            }

            var factionUnitCharacters = new List<FactionUnitCharacter>();
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

                    var target = Global.SpawnedPlayers.FirstOrDefault(x => x.SessionId == personagem);
                    if (target == null || target.Character == null || target.Character.FactionId != player.Character.FactionId)
                    {
                        player.Emit("Server:MostrarErro", $"ID {personagem} não é da sua facção ou está offline.", "btn-adicionarunidade");
                        return;
                    }

                    if (Global.FactionsUnits.Any(x => x.CharacterId == target.Character.Id)
                        || Global.FactionsUnits.SelectMany(x => x.Characters!).Any(x => x.CharacterId == target.Character.Id))
                    {
                        player.Emit("Server:MostrarErro", $"ID {personagem} já está em uma unidade.", "btn-adicionarunidade");
                        return;
                    }

                    var factionUnitCharacter = new FactionUnitCharacter();
                    factionUnitCharacter.Create(target.Character.Id);
                }
            }

            var factionUnit = new FactionUnit();
            factionUnit.Create(nome, numeracao, player.Character.FactionId!.Value, player.Character.Id, plate, factionUnitCharacters);

            await using var context = new DatabaseContext();
            await context.FactionsUnits.AddAsync(factionUnit);
            await context.SaveChangesAsync();

            Global.FactionsUnits = await context.FactionsUnits
                .Where(x => !x.FinalDate.HasValue)
                .Include(x => x.Character!)
                .Include(x => x.Characters!)
                    .ThenInclude(x => x.Character)
                .ToListAsync();

            foreach (var target in Global.SpawnedPlayers.Where(x => x.Character.FactionId == player.Character.FactionId))
                target.Emit("Server:AtualizarMDC", "btn-adicionarunidade", "div-unidades", GetFactionsUnitsHTML(player));
        }

        [AsyncClientEvent(nameof(MDCEncerrarUnidade))]
        public async Task MDCEncerrarUnidade(MyPlayer player, string idString)
        {
            var id = idString.ToGuid();
            var factionUnit = Global.FactionsUnits.FirstOrDefault(x => x.Id == id && x.CharacterId == player.Character.Id);
            if (factionUnit == null)
                return;

            factionUnit.End();

            await using var context = new DatabaseContext();
            context.FactionsUnits.Update(factionUnit);
            await context.SaveChangesAsync();

            Global.FactionsUnits.Remove(factionUnit);

            foreach (var target in Global.SpawnedPlayers.Where(x => x.Character.FactionId == player.Character.FactionId))
                target.Emit("Server:AtualizarMDC", $"btn-encerrarunidade{factionUnit.Id}", "div-unidades", GetFactionsUnitsHTML(player));
        }
    }
}