using AltV.Net;
using AltV.Net.Elements.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using Roleplay.Streamer;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Roleplay.Entities
{
    public class Spot
    {
        public int Id { get; set; }

        public SpotType Type { get; set; }

        public float PosX { get; set; }

        public float PosY { get; set; }

        public float PosZ { get; set; }

        public float AuxiliarPosX { get; set; }

        public float AuxiliarPosY { get; set; }

        public float AuxiliarPosZ { get; set; }

        [NotMapped, JsonIgnore]
        public Marker Marker { get; set; }

        [NotMapped, JsonIgnore]
        public MyColShape ColShape { get; set; }

        [NotMapped, JsonIgnore]
        public IBlip Blip { get; set; }

        public void CreateIdentifier()
        {
            RemoveIdentifier();

            string name;
            string description;

            switch (Type)
            {
                case SpotType.Banco:
                    name = "BANCO";
                    description = "Use /banco.";
                    break;
                case SpotType.LojaConveniencia:
                    name = "LOJA DE CONVENIÊNCIA";
                    description = "Use /comprar.";
                    break;
                case SpotType.LojaRoupas:
                    name = "LOJA DE ROUPAS";
                    description = "Use /comprar.";
                    break;
                case SpotType.SpawnVeiculosFaccao:
                    name = "SPAWN DE VEÍCULOS DA FACÇÃO";
                    description = "Use /fspawn, /vestacionar ou /freparar.";
                    break;
                case SpotType.ApreensaoVeiculos:
                    name = "APREENSÃO DE VEÍCULOS";
                    description = "Use /apreender.";
                    break;
                case SpotType.LiberacaoVeiculos:
                    name = "LIBERAÇÃO DE VEÍCULOS";
                    description = "Use /vliberar.";
                    break;
                case SpotType.Barbearia:
                    name = "BARBEARIA";
                    description = "Use /comprar.";
                    break;
                case SpotType.Uniforme:
                    name = "UNIFORME";
                    description = "Use /uniforme.";
                    break;
                case SpotType.MDC:
                    name = "MDC";
                    description = "Use /mdc.";
                    break;
                case SpotType.DMV:
                    name = "DMV";
                    description = $"Use /dmv para comprar ou renovar sua licença de motorista.";
                    break;
                case SpotType.Entrada:
                    name = "ENTRADA";
                    description = "Pressione Y para entrar.";
                    break;
                case SpotType.MeCurar:
                    name = "TRATAMENTO DE FERIDOS";
                    description = "Use /mecurar.";
                    break;
                case SpotType.Prisao:
                    name = "PRISÃO";
                    description = "Use /prender.";
                    break;
                case SpotType.AtendimentoLSPD:
                    name = "ATENDIMENTO LSPD";
                    description = "Use /historicocriminal.";
                    break;
                case SpotType.Confisco:
                    name = "CONFISCO";
                    description = "Use /confisco.";
                    break;
                case SpotType.TattooShop:
                    name = "ESTÚDIO DE TATUAGENS";
                    description = "Use /comprar.";
                    break;
                case SpotType.MechanicWorkshop:
                    name = "OFICINA MECÂNICA";
                    description = "Use /tunarver ou /tunarcomprar.";
                    break;
                default:
                    return;
            }

            var pos = new Vector3(PosX, PosY, PosZ - 0.95f);

            Marker = MarkerStreamer.Create(MarkerTypes.MarkerTypeHorizontalCircleSkinny,
                pos,
                new Vector3(1, 1, 1.5f),
                color: Global.MainRgba);

            ColShape = (MyColShape) Alt.CreateColShapeCylinder(pos, 1, 1.5f);
            ColShape.Description = $"[{name}] {{#FFFFFF}}{description}";
        }

        public void RemoveIdentifier()
        {
            Marker?.Destroy();
            Marker = null;

            ColShape?.Destroy();
            ColShape = null;
        }
    }
}