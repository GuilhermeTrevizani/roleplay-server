using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Newtonsoft.Json;
using Roleplay.Entities;
using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Roleplay.Commands
{
    public class Commands
    {
        [Command("ajuda")]
        public void CMD_ajuda(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
                return;

            var listaComandos = new List<Comando>()
            {
                new Comando("Teclas", "F2", "Jogadores online"),
                new Comando("Geral", "/stats", "Mostra as informações do seu personagem"),
                new Comando("Geral", "/id", "Procura o ID de um personagem"),
                new Comando("Geral", "/aceitar /ac", "Aceita um convite"),
                new Comando("Geral", "/recusar /rc", "Recusa um convite"),
                new Comando("Geral", "/pagar"),
                new Comando("Geral", "/revistar"),
                new Comando("Geral", "/multas"),
                new Comando("Geral", "/comprar"),
                new Comando("Geral", "/skin", "Compra roupas"),
                new Comando("Geral", "/emtrabalho"),
                new Comando("Geral", "/emprego"),
                new Comando("Geral", "/staff", "Lista os membros da staff que estão online"),
                new Comando("Geral", "/sos", "Envia solicitação de ajuda aos administradores em serviço"),
                new Comando("Geral", "/ferimentos", "Visualiza os ferimentos de um personagem"),
                new Comando("Geral", "/aceitartratamento", "Aceita o tratamento médico após estar ferido e é levado ao hospital"),
                new Comando("Geral", "/aceitarck", "Aceita o CK no personagem"),
                new Comando("Propriedades", "/entrar"),
                new Comando("Propriedades", "/sair"),
                new Comando("Propriedades", "/ptrancar"),
                new Comando("Propriedades", "/pcomprar"),
                new Comando("Propriedades", "/pvender"),
                new Comando("Chat IC", "/me"),
                new Comando("Chat IC", "/do"),
                new Comando("Chat IC", "/g"),
                new Comando("Chat IC", "/baixo"),
                new Comando("Chat IC", "/s"),
                new Comando("Chat IC", "/ame"),
                new Comando("Chat IC", "/ado"),
                new Comando("Chat OOC", "/b", "Chat OOC local"),
                new Comando("Chat OOC", "/pm", "Chat OOC privado"),
                new Comando("Celular", "/sms", "Envia um SMS"),
                new Comando("Celular", "/desligar /des", "Desliga a ligação"),
                new Comando("Celular", "/ligar", "Liga para um número"),
                new Comando("Celular", "/atender /at", "Atende uma ligação"),
                new Comando("Celular", " /celular /cel", "Abre o celular"),
                new Comando("Celular", "/gps", "Traça rota para uma propriedade"),
                new Comando("Veículos", "/vcomprar", "Compra um veículo em um concessionária"),
                new Comando("Veículos", "/motor", "Liga/desliga o motor de um veículo"),
                new Comando("Veículos", "/vtrancar", "Tranca/destranca um veículo"),
                new Comando("Veículos", "/vcomprarvaga", "Compra uma vaga para estacionar um veículo"),
                new Comando("Veículos", "/vestacionar", "Estaciona um veículo"),
                new Comando("Veículos", "/vspawn", "Spawna um veículo"),
                new Comando("Veículos", "/vlista", "Mostra seus veículos"),
                new Comando("Veículos", "/vvender", "Vende um veículo para outro personagem"),
                new Comando("Banco", "/depositar", "Deposita dinheiro no banco"),
                new Comando("Banco", "/sacar", "Saca dinheiro do banco"),
                new Comando("Banco", "/transferir", "Transfere dinheiro para outro personagem"),
                new Comando("Animações", "/stopanim /sa","Para as animações"),
                new Comando("Animações", "/crossarms", "Cruza os braços"),
                new Comando("Animações", "/handsup /hs", "Levanta as mãos"),
                new Comando("Animações", "/smoke"),
                new Comando("Animações", "/lean"),
                new Comando("Animações", "/police"),
                new Comando("Animações", "/incar"),
                new Comando("Animações", "/pushups"),
                new Comando("Animações", "/situps"),
                new Comando("Animações", "/blunt"),
                new Comando("Animações", "/afishing"),
                new Comando("Animações", "/acop"),
                new Comando("Animações", "/idle"),
                new Comando("Animações", "/barra"),
                new Comando("Animações", "/kneel"),
                new Comando("Animações", "/revistarc"),
                new Comando("Animações", "/ajoelhar"),
                new Comando("Animações", "/drink"),
                new Comando("Animações", "/morto"),
                new Comando("Animações", "/gsign"),
                new Comando("Animações", "/hurry"),
                new Comando("Animações", "/cair"),
                new Comando("Animações", "/wsup"),
                new Comando("Animações", "/render"),
                new Comando("Animações", "/mirar"),
                new Comando("Animações", "/sentar"),
                new Comando("Animações", "/dormir"),
                new Comando("Animações", "/pixar"),
                new Comando("Animações", "/sexo"),
                new Comando("Animações", "/jogado"),
                new Comando("Animações", "/reparando"),
                new Comando("Animações", "/luto"),
                new Comando("Animações", "/bar"),
                new Comando("Animações", "/necessidades"),
                new Comando("Animações", "/meth"),
                new Comando("Animações", "/mijar", "Mija"),
                new Comando("Rádio", "/canal", "Troca os canais de rádio"),
                new Comando("Rádio", "/r", "Fala no canal de rádio principal"),
                new Comando("Rádio", "/r2", "Fala no canal de rádio secundário"),
                new Comando("Rádio", "/r3", "Fala no canal de rádio terciário"),
            };

            if (p.Emprego > 0)
            {
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Emprego", "/sairemprego", "Sai do emprego"),
                });

                if (p.Emprego == TipoEmprego.Taxista)
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Emprego", "/taxiduty", "Entra ou sai de serviço como taxista"),
                        new Comando("Emprego", "/taxicha", "Exibe as chamadas aguardando taxistas"),
                        new Comando("Emprego", "/taxiac", "Atende uma chamada de taxista"),
                    });
            }

            if (p.Faccao > 0)
            {
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Facção", "/f", "Chat OOC da facção"),
                    new Comando("Facção", "/membros", "Mostra os membros da facção"),
                    new Comando("Facção", "/sairfaccao", "Sai da facção"),
                    new Comando("Facção", "/armario", "Usa o armário da facção"),
                });

                if (p.FaccaoBD.Tipo == TipoFaccao.Policial)
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Facção Policial", "/m", "Megafone"),
                        new Comando("Facção Policial", "/duty", "Entra/sai de trabalho"),
                        new Comando("Facção Policial", "/multar", "Multa um personagem online"),
                        new Comando("Facção Policial", "/multaroff", "Multa um personagem offline"),
                        new Comando("Facção Policial", "/prender", "Prende um personagem"),
                        new Comando("Facção Policial", "/algemar", "Algema um personagem"),
                        new Comando("Facção Policial", "/pegarcolete", "Pega colete em um armário da facção"),
                    });
                else if (p.FaccaoBD.Tipo == TipoFaccao.Medica)

                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Facção Médica", "/duty", "Entra/sai de trabalho"),
                        new Comando("Facção Médica", "/curar", "Cura um personagem ferido"),
                    });

                if (p.Rank >= p.FaccaoBD.RankGestor)
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Facção Gestor", "/blockf", "Bloqueia/desbloqueia o chat OOC da facção"),
                        new Comando("Facção Gestor", "/convidar", "Convida um personagem para a facção"),
                        new Comando("Facção Gestor", "/rank", "Altera o rank de um personagem na facção"),
                        new Comando("Facção Gestor", "/expulsar", "Expulsa um personagem da facção"),
                        new Comando("Facção Gestor", "/gov", "Envia um anúncio governamental da facção"),
                    });

                if (p.Rank >= p.FaccaoBD.RankLider)
                    listaComandos.AddRange(new List<Comando>()
                    {
                        new Comando("Facção Líder", "/crank", "Cria um rank na facção"),
                        new Comando("Facção Líder", "/eranknome", "Edita o nome de um rank da facção"),
                        new Comando("Facção Líder", "/rrank", "Remove um rank da facção"),
                        new Comando("Facção Líder", "/ranks", "Mostra os ranks da facção"),
                        new Comando("Facção Líder", "/earmirank", "Edita o rank de um item em um armário da facção"),
                    });
            }

            if ((int)p.UsuarioBD.Staff >= (int)TipoStaff.Helper)
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Helper", "/ir", "Vai a um personagem"),
                    new Comando("Helper", "/trazer", "Traz um personagem"),
                    new Comando("Helper", "/tp", "Teleporta um personagem para outro"),
                    new Comando("Helper", "/vw", "Altera o VW de um personagem"),
                    new Comando("Helper", "/o", "Chat OOC Global"),
                    new Comando("Helper", "/a", "Chat administrativo"),
                    new Comando("Helper", "/kick", "Expulsa um personagem"),
                    new Comando("Helper", "/irveh", "Vai a um veículo"),
                    new Comando("Helper", "/trazerveh", "Traz um veículo"),
                    new Comando("Helper", "/aduty", "Entra/sai de serviço administrativo"),
                    new Comando("Helper", "/listasos", "Lista os SOSs pendentes"),
                    new Comando("Helper", "/aj", "Aceita um SOS"),
                    new Comando("Helper", "/rj", "Rejeita um SOS"),
                });

            if ((int)p.UsuarioBD.Staff >= (int)TipoStaff.GameModerator)
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Game Moderator", "/vida", "Altera a vida de um personagem"),
                    new Comando("Game Moderator", "/colete", "Altera o colete de um personagem"),
                    new Comando("Game Moderator", "/checar", "Checa as informações de um personagem"),
                    new Comando("Game Moderator", "/ban", "Bane um usuário"),
                    new Comando("Game Moderator", "/unban", "Desbane um usuário"),
                    new Comando("Game Moderator", "/banoff", "Bane um usuário que está offline"),
                });

            if ((int)p.UsuarioBD.Staff >= (int)TipoStaff.GameAdministrator)
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Game Administrator", "/ck", "Aplica CK em um personagem"),
                    new Comando("Game Administrator", "/tempo", "Altera o tempo"),
                    new Comando("Game Administrator", "/acurar", "Cura um personagem ferido"),
                });

            if ((int)p.UsuarioBD.Staff >= (int)TipoStaff.Manager)
                listaComandos.AddRange(new List<Comando>()
                {
                    new Comando("Manager", "/gmx", "Salva todas as informações do servidor para reiniciá-lo"),
                    new Comando("Manager", "/proximo", "Lista os itens que estão próximos"),
                    new Comando("Manager", "/cblip", "Cria um blip"),
                    new Comando("Manager", "/rblip", "Remove um blip"),
                    new Comando("Manager", "/setstaff", "Altera o nível de staff de um usuário"),
                    new Comando("Manager", "/cfac", "Cria uma facção"),
                    new Comando("Manager", "/efacnome", "Edita o nome da facção"),
                    new Comando("Manager", "/efactipo", "Edita o tipo da facção"),
                    new Comando("Manager", "/efaccor", "Edita a cor da facção"),
                    new Comando("Manager", "/efacrankgestor", "Edita o rank gestor da facção"),
                    new Comando("Manager", "/efacranklider", "Edita o rank líder da facção"),
                    new Comando("Manager", "/rfac", "Remove uma facção"),
                    new Comando("Manager", "/faccoes", "Mostra as facções"),
                    new Comando("Manager", "/crank", "Cria um rank na facção"),
                    new Comando("Manager", "/eranknome", "Edita o nome de um rank da facção"),
                    new Comando("Manager", "/rrank", "Remove um rank da facção"),
                    new Comando("Manager", "/ranks", "Mostra os ranks da facção"),
                    new Comando("Manager", "/lider", "Atribui o personagem como líder de uma facção"),
                    new Comando("Manager", "/parametros"),
                    new Comando("Manager", "/cprop"),
                    new Comando("Manager", "/rprop"),
                    new Comando("Manager", "/epropvalor"),
                    new Comando("Manager", "/epropint"),
                    new Comando("Manager", "/eproppos"),
                    new Comando("Manager", "/irblip"),
                    new Comando("Manager", "/irprop"),
                    new Comando("Manager", "/cpreco"),
                    new Comando("Manager", "/rpreco"),
                    new Comando("Manager", "/cponto"),
                    new Comando("Manager", "/rponto"),
                    new Comando("Manager", "/irponto"),
                    new Comando("Manager", "/eranksalario"),
                    new Comando("Manager", "/carm"),
                    new Comando("Manager", "/earmpos"),
                    new Comando("Manager", "/earmfac"),
                    new Comando("Manager", "/rarm"),
                    new Comando("Manager", "/irarm"),
                    new Comando("Manager", "/carmi"),
                    new Comando("Manager", "/rarmi"),
                    new Comando("Manager", "/earmimun"),
                    new Comando("Manager", "/earminrank"),
                    new Comando("Manager", "/earminest"),
                    new Comando("Manager", "/irarm"),
                    new Comando("Manager", "/eblipinativo"),
                    new Comando("Manager", "/cveh", "Cria um veículo"),
                    new Comando("Manager", "/rveh", "Remove um veículo"),
                    new Comando("Manager", "/evehpos", "Edita a posição de um veículo"),
                    new Comando("Manager", "/evehcor", "Edita as cores de um veículo"),
                    new Comando("Manager", "/evehlivery", "Edita a livery de um veículo"),
                    new Comando("Manager", "/earmipintura", "Edita a pintura de uma arma do armário"),
                    new Comando("Manager", "/save", "Exibe sua posição e rotação ou do seu veículo no console"),
                    new Comando("Manager", "/pos", "Vai até a posição"),
                });

            var html = $@"<div class='box-header with-border'>
                <h3>Lista de Comandos<span onclick='closeView()' class='pull-right label label-danger'>X</span></h3> 
            </div>
            <div class='box-body'>
            <input id='pesquisa' type='text' autofocus class='form-control' placeholder='Pesquise os comandos...' />
            <br/><table class='table table-bordered table-striped'>
                <thead>
                    <tr>
                        <th>Categoria</th>
                        <th>Comando</th>
                        <th>Descrição</th>
                    </tr>
                </thead>
                <tbody>";

            listaComandos = listaComandos.OrderBy(x => x.Categoria).ThenBy(x => x.Nome).ToList();
            foreach (var x in listaComandos)
                html += $@"<tr class='pesquisaitem'><td>{x.Categoria}</td><td>{x.Nome}</td><td>{x.Descricao}</td></tr>";

            html += $@"
                </tbody>
            </table>
            </div>";

            player.Emit("Server:BaseHTML", html);
        }

        [Command("stats")]
        public void CMD_stats(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            Functions.MostrarStats(player, p);
        }

        [Command("id", "/id (ID ou nome)", GreedyArg = true)]
        public void CMD_id(IPlayer player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            int.TryParse(idNome, out int id);
            var personagens = Global.PersonagensOnline.Where(x => x.ID == id || x.Nome.ToLower().Contains(idNome.ToLower())).OrderBy(x => x.ID).ToList();
            if (personagens.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Nenhum jogador foi encontrado com a pesquisa: {idNome}");
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Titulo, $"Jogadores encontrados com a pesquisa: {idNome}");
            foreach (var pl in personagens)
                Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"{pl.Nome} [{pl.ID}] ({pl.UsuarioBD.Nome})");
        }

        [Command("aceitar", "/aceitar (tipo)", Alias = "ac")]
        public void CMD_aceitar(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (!Enum.IsDefined(typeof(TipoConvite), tipo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo inválido!");
                return;
            }

            var convite = p.Convites.FirstOrDefault(x => x.Tipo == (TipoConvite)tipo);
            if (convite == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Você não possui nenhum convite do tipo {tipo}!");
                return;
            }

            var target = Global.PersonagensOnline.FirstOrDefault(x => x.Codigo == convite.Personagem);

            switch ((TipoConvite)tipo)
            {
                case TipoConvite.Faccao:
                    int.TryParse(convite.Valor[0], out int faccao);
                    int.TryParse(convite.Valor[1], out int rank);
                    p.Faccao = faccao;
                    p.Rank = rank;

                    Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você aceitou o convite para entrar na facção.");
                    if (target != null)
                        Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.Nome} aceitou seu convite para entrar na facção.");
                    break;
                case TipoConvite.VendaPropriedade:
                    if (target == null)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Dono da propriedade não está online!");
                        break;
                    }

                    if (player.Position.Distance(target.Player.Position) > 2 || player.Dimension != target.Player.Dimension)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Dono da propriedade não está próximo de você!");
                        return;
                    }

                    int.TryParse(convite.Valor[0], out int propriedade);
                    int.TryParse(convite.Valor[1], out int valor);
                    if (p.Dinheiro < valor)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente!");
                        break;
                    }

                    var prop = Global.Propriedades.FirstOrDefault(x => x.Codigo == propriedade);
                    if (prop == null)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Propriedade inválida!");
                        break;
                    }

                    if (player.Position.Distance(new Position(prop.EntradaPosX, prop.EntradaPosY, prop.EntradaPosZ)) > 2)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo da propriedade!");
                        return;
                    }

                    p.Dinheiro -= valor;
                    p.SetDinheiro();
                    target.Dinheiro += valor;
                    target.SetDinheiro();

                    prop.Personagem = p.Codigo;

                    using (var context = new DatabaseContext())
                    {
                        context.Propriedades.Update(prop);
                        context.SaveChanges();
                    }

                    Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você comprou a propriedade {prop.Codigo} de {target.NomeIC} por ${valor:N0}.");
                    Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"Você vendeu a propriedade {prop.Codigo} para {p.NomeIC} por ${valor:N0}.");
                    break;
                case TipoConvite.Revista:
                    if (target == null)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Solicitante da revista não está online!");
                        break;
                    }

                    if (player.Position.Distance(target.Player.Position) > 2 || player.Dimension != target.Player.Dimension)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Solicitante da revista não está próximo de você!");
                        return;
                    }

                    Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você aceitou ser revistado.");
                    Functions.EnviarMensagem(target.Player, TipoMensagem.Titulo, $"Revista em {p.NomeIC}");
                    Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"Celular: {p.Celular} | Dinheiro: ${p.Dinheiro:N0}");
                    if (p.CanalRadio > -1)
                        Functions.EnviarMensagem(target.Player, TipoMensagem.Nenhum, $"Canal Rádio 1: {p.CanalRadio} | Canal Rádio 2: {p.CanalRadio2} | Canal Rádio 3: {p.CanalRadio3}");
                    break;
                case TipoConvite.VendaVeiculo:
                    if (target == null)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Dono do veículo não está online!");
                        break;
                    }

                    if (player.Position.Distance(target.Player.Position) > 2 || player.Dimension != target.Player.Dimension)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Dono do veículo não está próximo de você!");
                        return;
                    }

                    int.TryParse(convite.Valor[0], out int veiculo);
                    int.TryParse(convite.Valor[1], out int valorVeh);

                    if (p.Dinheiro < valorVeh)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente!");
                        break;
                    }

                    var veh = Global.Veiculos.FirstOrDefault(x => x.Codigo == veiculo);
                    if (veh == null)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Propriedade inválida!");
                        break;
                    }

                    if (player.Position.Distance(veh.Vehicle.Position) > 2)
                    {
                        Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo do veículo!");
                        return;
                    }

                    p.Dinheiro -= valorVeh;
                    p.SetDinheiro();
                    target.Dinheiro += valorVeh;
                    target.SetDinheiro();

                    veh.Personagem = p.Codigo;

                    using (var context = new DatabaseContext())
                    {
                        context.Veiculos.Update(veh);
                        context.SaveChanges();
                    }

                    Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você comprou o veículo {veh.Codigo} de {target.NomeIC} por ${valorVeh:N0}.");
                    Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"Você vendeu o veículo {veh.Codigo} para {p.NomeIC} por ${valorVeh:N0}.");
                    break;
            }

            p.Convites.RemoveAll(x => x.Tipo == (TipoConvite)tipo);
        }

        [Command("recusar", "/recusar (tipo)", Alias = "rc")]
        public void CMD_recusar(IPlayer player, int tipo)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (!Enum.IsDefined(typeof(TipoConvite), tipo))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Tipo inválido!");
                return;
            }

            var convite = p.Convites.FirstOrDefault(x => x.Tipo == (TipoConvite)tipo);
            if (convite == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"Você não possui nenhum convite do tipo {tipo}!");
                return;
            }

            var target = Global.PersonagensOnline.FirstOrDefault(x => x.Codigo == convite.Personagem);
            var strPlayer = string.Empty;
            var strTarget = string.Empty;

            switch ((TipoConvite)tipo)
            {
                case TipoConvite.Faccao:
                    strPlayer = strTarget = "entrar na facção";
                    break;
                case TipoConvite.VendaPropriedade:
                    strPlayer = "compra da propriedade";
                    strTarget = "venda da propriedade";
                    break;
                case TipoConvite.VendaVeiculo:
                    strPlayer = "compra de veículo";
                    strTarget = "venda de veículo";
                    break;
                case TipoConvite.Revista:
                    strPlayer = strTarget = "revista";
                    break;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você recusou o convite para {strPlayer}.");

            if (target != null)
                Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.NomeIC} recusou seu convite para {strTarget}.");

            p.Convites.RemoveAll(x => x.Tipo == (TipoConvite)tipo);
        }

        [Command("pagar", "/pagar (ID ou nome) (valor)")]
        public void CMD_pagar(IPlayer player, string idNome, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor inválido!");
                return;
            }

            if (p.Dinheiro < valor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (player.Position.Distance(target.Player.Position) > 2 || player.Dimension != target.Player.Dimension)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo de você!");
                return;
            }

            p.Dinheiro -= valor;
            target.Dinheiro += valor;
            p.SetDinheiro();
            target.SetDinheiro();

            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.NomeIC} te deu ${valor:N0}.");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você deu ${valor:N0} para {target.NomeIC}.");
            Functions.GravarLog(TipoLog.Dinheiro, $"/pagar {valor}", p, target);
        }

        [Command("revistar", "/revistar (ID ou nome)")]
        public void CMD_revistar(IPlayer player, string idNome)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            if (player.Position.Distance(target.Player.Position) > 2 || player.Dimension != target.Player.Dimension)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Jogador não está próximo de você!");
                return;
            }

            var convite = new Convite()
            {
                Tipo = TipoConvite.Revista,
                Personagem = p.Codigo,
            };
            target.Convites.RemoveAll(x => x.Tipo == TipoConvite.Revista);
            target.Convites.Add(convite);

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você solicitou uma revista para {target.Nome}.");
            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"Solicitou uma revista em você. (/ac {convite.Tipo} para aceitar ou /rc {convite.Tipo} para recusar)");
        }

        [Command("multas")]
        public void CMD_multas(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (!Global.Pontos.Any(x => x.Tipo == TipoPonto.Multas && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= 2))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum ponto de pagamento de multas!");
                return;
            }

            using var context = new DatabaseContext();
            var multas = context.Multas.Where(x => !x.DataPagamento.HasValue && x.PersonagemMultado == p.Codigo).OrderBy(x => x.Data).Select(x => new
            {
                x.Codigo,
                x.Motivo,
                Data = x.Data.ToString("dd/MM/yyyy HH:mm:ss"),
                Valor = $"${x.Valor:N0}",
            }).ToList();

            if (multas.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui multas pendentes!");
                return;
            }

            player.Emit("Server:AbrirMultas", JsonConvert.SerializeObject(multas));
        }

        [Command("transferir", "/transferir (ID ou nome) (valor)")]
        public void CMD_transferir(IPlayer player, string idNome, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (p.TempoPrisao > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você está preso!");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor inválido!");
                return;
            }

            if (!Global.Pontos.Any(x => (x.Tipo == TipoPonto.Banco || x.Tipo == TipoPonto.ATM) && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= 2) && p.Celular == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em um banco/ATM ou não possui um celular!");
                return;
            }

            if (p.Banco < valor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente no banco!");
                return;
            }

            var target = Functions.ObterPersonagemPorIdNome(player, idNome, false);
            if (target == null)
                return;

            p.Banco -= valor;
            target.Banco += valor;

            Functions.EnviarMensagem(target.Player, TipoMensagem.Sucesso, $"{p.Nome} transferiu para você ${valor:N0}.");
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você transferiu ${valor:N0} para {target.Nome}.");
            Functions.GravarLog(TipoLog.Dinheiro, $"/transferir {valor}", p, target);
        }

        [Command("sacar", "/sacar (valor)")]
        public void CMD_sacar(IPlayer player, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor inválido!");
                return;
            }

            if (!Global.Pontos.Any(x => (x.Tipo == TipoPonto.Banco || x.Tipo == TipoPonto.ATM) && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= 2))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em um banco/ATM!");
                return;
            }

            if (p.Banco < valor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente no banco!");
                return;
            }

            p.Banco -= valor;
            p.Dinheiro += valor;
            p.SetDinheiro();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você sacou ${valor:N0}.");
            Functions.GravarLog(TipoLog.Dinheiro, $"/sacar {valor}", p, null);
        }

        [Command("depositar", "/depositar (valor)")]
        public void CMD_depositar(IPlayer player, int valor)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (valor <= 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Valor inválido!");
                return;
            }

            if (!Global.Pontos.Any(x => x.Tipo == TipoPonto.Banco && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= 2))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em um banco!");
                return;
            }

            if (p.Dinheiro < valor)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui dinheiro suficiente!");
                return;
            }

            p.Dinheiro -= valor;
            p.Banco += valor;
            p.SetDinheiro();

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você depositou ${valor:N0}.");
            Functions.GravarLog(TipoLog.Dinheiro, $"/depositar {valor}", p, null);
        }

        [Command("comprar")]
        public void CMD_comprar(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            if (!Global.Pontos.Any(x => x.Tipo == TipoPonto.LojaConveniencia && player.Position.Distance(new Position(x.PosX, x.PosY, x.PosZ)) <= 2))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está em uma loja de conveniência!");
                return;
            }

            player.Emit("Server:ComprarConveniencia", JsonConvert.SerializeObject(Global.Precos.Where(x => x.Tipo == TipoPreco.Conveniencia).OrderBy(x => x.Nome).Select(x => new
            {
                x.Nome,
                Preco = $"${x.Valor:N0}",
            }).ToList()));
        }

        [Command("emtrabalho")]
        public void CMD_emtrabalho(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está conectado!");
                return;
            }

            Functions.EnviarMensagem(player, TipoMensagem.Titulo, "Jogadores trabalhando");
            Functions.EnviarMensagem(player, TipoMensagem.Nenhum, $"Policiais: {Global.PersonagensOnline.Count(x => x.FaccaoBD?.Tipo == TipoFaccao.Policial && x.IsEmTrabalho)} | Médicos: {Global.PersonagensOnline.Count(x => x.FaccaoBD?.Tipo == TipoFaccao.Medica && x.IsEmTrabalho)} | Taxistas: {Global.PersonagensOnline.Count(x => x.Emprego == TipoEmprego.Taxista && x.IsEmTrabalho)}");
        }

        [Command("sairemprego")]
        public void CMD_sairemprego(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p.Emprego == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não tem um emprego!");
                return;
            }

            p.Emprego = 0;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "Você saiu do seu emprego!");
        }

        [Command("emprego")]
        public void CMD_emprego(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p.Emprego > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já tem um emprego!");
                return;
            }

            if (p.FaccaoBD?.Tipo == TipoFaccao.Policial || p.FaccaoBD?.Tipo == TipoFaccao.Medica)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode pegar um emprego pois está em uma facção governamental!");
                return;
            }

            var emprego = TipoEmprego.Nenhum;
            foreach (var c in Global.Empregos)
            {
                if (emprego == TipoEmprego.Nenhum && player.Position.Distance(c.Posicao) <= 2)
                    emprego = c.Tipo;
            }

            if (emprego == TipoEmprego.Nenhum)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está próximo de nenhum local de emprego!");
                return;
            }

            p.Emprego = emprego;
            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, $"Você pegou o emprego {Functions.ObterDisplayEnum(emprego)}!");
        }

        [Command("staff")]
        public void CMD_staff(IPlayer player)
        {
            var players = Global.PersonagensOnline.Where(x => x.UsuarioBD?.Staff > 0).OrderByDescending(x => x.UsuarioBD.Staff).ThenBy(x => x.UsuarioBD.Nome).ToList();
            if (players.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Não há nenhum membro da staff online!");
                return;
            }

            var html = $@"<div class='box-header with-border'>
                <h3>{Constants.NomeServidor} • Staff Online<span onclick='closeView()' class='pull-right label label-danger'>X</span></h3> 
            </div>
            <div class='box-body'>
            <input id='pesquisa' type='text' autofocus class='form-control' placeholder='Pesquise os staffers...' />
            <br/><table class='table table-bordered table-striped'>
                <thead>
                    <tr>
                        <th>Rank</th>
                        <th>Staffer</th>
                        <th>Status</th>
                    </tr>
                </thead>
                <tbody>";

            foreach (var x in players)
            {
                var status = x.IsEmTrabalhoAdministrativo ? "<span style='color:#6EB469'>EM SERVIÇO</span>" : "<span style='color:#FF6A4D'>FORA DE SERVIÇO</span>";
                html += $@"<tr class='pesquisaitem'><td>{Functions.ObterDisplayEnum(x.UsuarioBD.Staff)}</td><td>{x.UsuarioBD.Nome}</td><td>{status}</td></tr>";
            }

            html += $@"
                </tbody>
            </table>
            </div>";

            player.Emit("Server:BaseHTML", html);
        }

        [Command("sos", "/sos (mensagem)", GreedyArg = true)]
        public void CMD_sos(IPlayer player, string mensagem)
        {
            var p = Functions.ObterPersonagem(player);
            if (p?.UsuarioBD?.Staff > 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não possui autorização para usar esse comando!");
                return;
            }

            if (Global.SOSs.Any(x => x.IDPersonagem == p.ID))
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você já possui um SOS pendente de resposta!");
                return;
            }

            var players = Global.PersonagensOnline.Where(x => x.IsEmTrabalhoAdministrativo && x.UsuarioBD?.Staff > 0).ToList();
            if (players.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Não há administradores em serviço!");
                return;
            }

            var sos = new SOS()
            {
                IDPersonagem = p.ID,
                Mensagem = mensagem,
                Usuario = p.UsuarioBD.Codigo,
                NomePersonagem = p.Nome,
                NomeUsuario = p.UsuarioBD.Nome,
            };

            using (var context = new DatabaseContext())
            {
                context.SOSs.Add(sos);
                context.SaveChanges();
            }

            Global.SOSs.Add(sos);

            foreach (var pl in players)
            {
                Functions.EnviarMensagem(pl.Player, TipoMensagem.Titulo, $"SOS de {p.Nome} [{p.ID}] ({p.UsuarioBD.Nome})");
                Functions.EnviarMensagem(pl.Player, TipoMensagem.Nenhum, mensagem);
            }

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "SOS enviado para os administradores em serviço!");
        }

        [Command("ferimentos", "/ferimentos (ID ou nome)", GreedyArg = true)]
        public void CMD_ferimentos(IPlayer player, string idNome)
        {
            var target = Functions.ObterPersonagemPorIdNome(player, idNome);
            if (target == null)
                return;

            if (target.Ferimentos.Count == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, $"{target.NomeIC} não possui ferimentos!");
                return;
            }

            var html = $@"<div class='box-header with-border'>
                <h3>Ferimentos de {target.NomeIC}<span onclick='closeView()' class='pull-right label label-danger'>X</span></h3> 
            </div>
            <div class='box-body'>
            <table class='table table-bordered table-striped'>
                <thead>
                    <tr>
                        <th>Data</th>
                        <th>Arma</th>
                        <th>Dano</th>
                        <th>Parte do Corpo</th>
                    </tr>
                </thead>
                <tbody>";

            foreach (var x in target.Ferimentos)
            {
                html += $@"<tr><td>{x.Data}</td><td>{(WeaponModel)x.Arma}</td><td>{x.Dano}</td><td>{Functions.ObterParteCorpo(x.BodyPart)}</td></tr>";
            }

            html += $@"
                </tbody>
            </table>
            </div>";

            player.Emit("Server:BaseHTML", html);
        }

        [Command("aceitartratamento")]
        public void CMD_aceitartratamento(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p.TimerFerido == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está ferido!");
                return;
            }

            if (p.TimerFerido.ElapsedCount == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode executar esse comando ainda!");
                return;
            }

            p.TimerFerido = null;
            p.Player.Dimension = 0;
            p.Player.RemoveAllWeapons();

            var pos = new Position(298.16702f, -584.2286f, 43.24829f);
            if (p.TempoPrisao > 0)
            {
                using var context = new DatabaseContext();
                var prisao = context.Prisoes.Where(x => x.Preso == p.Codigo).OrderByDescending(x => x.Codigo).FirstOrDefault();
                if (prisao.Cela == 1)
                    pos = new Position(460.4085f, -994.0992f, 25);
                else if (prisao.Cela == 2)
                    pos = new Position(460.4085f, -997.7994f, 25);
                else if (prisao.Cela == 3)
                    pos = new Position(460.4085f, -1001.342f, 25);
            }

            p.Ferimentos = new List<Ferimento>();
            p.Player.Emit("Server:SelecionarPersonagem");
            p.Player.Spawn(pos);

            Functions.EnviarMensagem(player, TipoMensagem.Sucesso, "Você aceitou o tratamento e foi levado para o hospital.");
        }

        [Command("aceitarck")]
        public async void CMD_aceitarck(IPlayer player)
        {
            var p = Functions.ObterPersonagem(player);
            if (p.TimerFerido == null)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não está ferido!");
                return;
            }

            if (p.TimerFerido.ElapsedCount == 0)
            {
                Functions.EnviarMensagem(player, TipoMensagem.Erro, "Você não pode executar esse comando ainda!");
                return;
            }

            p.TimerFerido = null;
            p.DataMorte = DateTime.Now;
            p.MotivoMorte = "Aceitou CK";
            Functions.SalvarPersonagem(p, false);
            await Task.Delay(1000);
            player.Kick("Você aceitou o CK no seu personagem.");

            Functions.GravarLog(TipoLog.Morte, $"/aceitarck", p, null);
        }
    }
}