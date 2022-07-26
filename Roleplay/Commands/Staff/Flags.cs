using Roleplay.Entities;
using Roleplay.Factories;
using Roleplay.Models;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Roleplay.Commands.Staff
{
    public class Flags
    {
        [Command("portas")]
        public static void CMD_portas(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Doors))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffDoors", false, Functions.GetDoorsHTML());
        }

        [Command("precos")]
        public static void CMD_precos(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Prices))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var jsonTypes = JsonSerializer.Serialize(
                Enum.GetValues(typeof(PriceType))
                .Cast<PriceType>()
                .Select(x => new
                {
                    Id = x,
                    Name = Functions.GetEnumDisplay(x),
                })
            );

            player.Emit("StaffPrices", false, Functions.GetPricesHTML(), jsonTypes);
        }

        [Command("faccoes")]
        public static void CMD_faccoes(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Factions))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var jsonTypes = JsonSerializer.Serialize(
                Enum.GetValues(typeof(FactionType))
                .Cast<FactionType>()
                .Select(x => new
                {
                    Id = x,
                    Name = Functions.GetEnumDisplay(x),
                })
            );

            player.Emit("StaffFactions", false, Functions.GetFactionsHTML(), jsonTypes);
        }

        [Command("arsenais")]
        public static void CMD_arsenais(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsArmories))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffFactionsArmories", false, Functions.GetFactionsArmoriesHTML());
        }

        #region Properties
        [Command("propriedades")]
        public static void CMD_propriedades(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Properties))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var jsonInteriors = JsonSerializer.Serialize(
                Enum.GetValues(typeof(PropertyInterior))
                .Cast<PropertyInterior>()
                .Select(x => new
                {
                    Id = x,
                    Name = Functions.GetEnumDisplay(x),
                })
            );

            player.Emit("StaffProperties", false, Functions.GetPropertiesHTML(), jsonInteriors);
        }

        [Command("int", "/int (tipo)")]
        public static async Task CMD_int(MyPlayer player, byte type)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Properties))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!Enum.IsDefined(typeof(PropertyInterior), type))
            {
                player.SendMessage(MessageType.Error, "Tipo inválido.");
                return;
            }

            player.IPLs = Functions.GetIPLsByInterior((PropertyInterior)type);
            player.SetarIPLs();
            player.SetPosition(Functions.GetExitPositionByInterior((PropertyInterior)type), 0, false);
            await player.GravarLog(LogType.Staff, $"/int {type}", null);
        }
        #endregion Properties

        [Command("pontos")]
        public static void CMD_pontos(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Spots))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var jsonTypes = JsonSerializer.Serialize(
                Enum.GetValues(typeof(SpotType))
                .Cast<SpotType>()
                .Select(x => new
                {
                    Id = x,
                    Name = Functions.GetEnumDisplay(x),
                })
            );

            player.Emit("StaffSpots", false, Functions.GetSpotsHTML(), jsonTypes);
        }

        [Command("blips")]
        public static void CMD_blips(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Blips))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffBlips", false, Functions.GetBlipsHTML());
        }

        #region Vehicles
        [Command("veiculos")]
        public static async Task CMD_veiculos(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Vehicles))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffVehicles", false, await Functions.GetVehiclesHTML());
        }

        [Command("atunar")]
        public static void CMD_atunar(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Vehicles))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            Functions.CMDTuning(player, null, true);
        }
        #endregion Vehicles

        [Command("daritem")]
        public static void CMD_daritem(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.GiveItem))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            static string GetDefaultExtra(BaseItem baseItem)
            {
                if (baseItem.Category == ItemCategory.Weapon)
                    return JsonSerializer.Serialize(new WeaponItem());

                if (baseItem.IsCloth)
                    return JsonSerializer.Serialize(new ClotheAccessoryItem());

                return null;
            }

            var categorias = JsonSerializer.Serialize(Enum.GetValues(typeof(ItemCategory)).Cast<ItemCategory>()
                .Select(x => new
                {
                    ID = (int)x,
                    Name = Functions.GetEnumDisplay(x),
                    Extra = GetDefaultExtra(new BaseItem(x)),
                    HasType = !(new BaseItem(x).IsStack
                        || x == ItemCategory.WalkieTalkie
                        || x == ItemCategory.Cellphone
                        || x == ItemCategory.Microphone),
                    new BaseItem(x).IsStack,
                }));

            player.Emit("Staff:GiveItem", categorias);
        }

        [Command("drughouses")]
        public static void CMD_drughouses(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.FactionsDrugsHouses))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffFactionsDrugsHouses", false, Functions.GetFactionsDrugsHousesHTML());
        }

        [Command("bocasfumo")]
        public static void CMD_bocasfumo(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.CrackDens))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffCrackDens", false, Functions.GetCrackDensHTML());
        }

        [Command("acaminhoneiro")]
        public static void CMD_acaminhoneiro(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.TruckerLocations))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffTruckerLocations", false, Functions.GetTruckerLocationsHTML());
        }

        [Command("amobilias")]
        public static void CMD_amobilias(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Furnitures))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffFurnitures", false, Functions.GetFurnituresHTML());
        }

        [Command("animacoes")]
        public static void CMD_animacoes(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Animations))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffAnimations", false, Functions.GetAnimationsHTML());
        }

        [Command("empresas")]
        public static void CMD_empresas(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Companies))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            player.Emit("StaffCompanies", false, Functions.GetCompaniesHTML());
        }
    }
}