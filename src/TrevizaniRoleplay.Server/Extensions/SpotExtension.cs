using AltV.Net;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Factories;

namespace TrevizaniRoleplay.Server.Extensions
{
    public static class SpotExtension
    {
        public static void CreateIdentifier(this Spot spot)
        {
            RemoveIdentifier(spot);

            string name;
            string description;

            switch (spot.Type)
            {
                case SpotType.Bank:
                    name = "BANCO";
                    description = "Use /banco.";
                    break;
                case SpotType.ConvenienceStore:
                    name = "LOJA DE CONVENIÊNCIA";
                    description = "Use /comprar.";
                    break;
                case SpotType.ClothesStore:
                    name = "LOJA DE ROUPAS";
                    description = "Use /comprar.";
                    break;
                case SpotType.FactionVehicleSpawn:
                    name = "SPAWN DE VEÍCULOS DA FACÇÃO";
                    description = "Use /fspawn, /vestacionar ou /freparar.";
                    break;
                case SpotType.VehicleSeizure:
                    name = "APREENSÃO DE VEÍCULOS";
                    description = "Use /apreender.";
                    break;
                case SpotType.VehicleRelease:
                    name = "LIBERAÇÃO DE VEÍCULOS";
                    description = "Use /vliberar.";
                    break;
                case SpotType.BarberShop:
                    name = "BARBEARIA";
                    description = "Use /comprar.";
                    break;
                case SpotType.Uniform:
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
                case SpotType.Entrance:
                    name = "ENTRADA";
                    description = "Pressione Y para entrar.";
                    break;
                case SpotType.HealMe:
                    name = "TRATAMENTO DE FERIDOS";
                    description = "Use /mecurar.";
                    break;
                case SpotType.Prison:
                    name = "PRISÃO";
                    description = "Use /prender.";
                    break;
                case SpotType.LSPDService:
                    name = "ATENDIMENTO LSPD";
                    description = "Use /historicocriminal.";
                    break;
                case SpotType.Confiscation:
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

            var pos = new Vector3(spot.PosX, spot.PosY, spot.PosZ - 0.95f);

            // TODO: Rollback commentary when alt:V implements
            //var marker = (MyMarker)Alt.CreateMarker(MarkerType.MarkerHalo, pos, Global.MainRgba);
            //marker.Scale = new Vector3(1, 1, 1.5f);
            //marker.SpotId = spot.Id;

            var colShape = (MyColShape)Alt.CreateColShapeCylinder(pos, 1, 1.5f);
            colShape.Description = $"[{name}] {{#FFFFFF}}{description}";
            colShape.SpotId = spot.Id;
        }

        public static void RemoveIdentifier(this Spot spot)
        {
            var marker = Global.Markers.FirstOrDefault(x => x.SpotId == spot.Id);
            marker?.Destroy();

            var colShape = Global.ColShapes.FirstOrDefault(x => x.SpotId == spot.Id);
            colShape?.Destroy();

            var blip = Global.MyBlips.FirstOrDefault(x => x.SpotId == spot.Id);
            blip?.Destroy();
        }
    }
}