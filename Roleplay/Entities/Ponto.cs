using AltV.Net.Data;
using Roleplay.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roleplay.Entities
{
    public class Ponto
    {
        public int Codigo { get; set; }
        public TipoPonto Tipo { get; set; }
        public float PosX { get; set; } = 0;
        public float PosY { get; set; } = 0;
        public float PosZ { get; set; } = 0;
        public string Configuracoes { get; set; }

        [NotMapped]
        public TextDraw TextLabel { get; set; }

        public void CriarIdentificador()
        {
            DeletarIdentificador();

            string nome = string.Empty;
            string descricao = string.Empty;
            switch (Tipo)
            {
                case TipoPonto.Banco:
                    nome = "Caixa Bancário";
                    descricao = "Use /sacar, /transferir, /depositar ou /multas";
                    break;
                case TipoPonto.ATM:
                    nome = "ATM";
                    descricao = "Use /sacar ou /transferir";
                    break;
                case TipoPonto.LojaConveniencia:
                    nome = "Loja de Conveniência";
                    descricao = "Use /comprar";
                    break;
                case TipoPonto.LojaRoupas:
                    nome = "Loja de Roupas";
                    descricao = "Use /roupas";
                    break;
                case TipoPonto.SpawnVeiculosFaccao:
                    nome = "Spawn de Veículos da Facção";
                    descricao = "Use /fspawn ou /vestacionar";
                    break;
                case TipoPonto.ApreensaoVeiculos:
                    nome = "Apreensão de Veículos";
                    descricao = "Use /apreender";
                    break;
                case TipoPonto.LiberacaoVeiculos:
                    nome = "Liberação de Veículos";
                    descricao = "Use /vliberar";
                    break;
                case TipoPonto.Barbearia:
                    nome = "Barbearia";
                    descricao = "Use /barbearia";
                    break;
                case TipoPonto.Uniforme:
                    nome = "Uniforme";
                    descricao = "Use /uniforme";
                    break;
                case TipoPonto.MDC:
                    nome = "MDC";
                    descricao = "Use /mdc";
                    break;
            }

            TextLabel = Functions.CriarTextDraw($"{nome}\n~w~{descricao}", new Position(PosX, PosY, PosZ), 10, 0.4f, 4, new Rgba(254, 189, 12, 255), 0);
        }

        public void DeletarIdentificador() => Functions.RemoverTextDraw(TextLabel);
    }
}