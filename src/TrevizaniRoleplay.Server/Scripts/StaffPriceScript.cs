using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Enums;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Scripts
{
    public class StaffPriceScript : IScript
    {
        [Command("precos")]
        public static void CMD_precos(MyPlayer player)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Prices))
            {
                player.SendMessage(MessageType.Error, Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var jsonTypes = Functions.Serialize(
                Enum.GetValues(typeof(PriceType))
                .Cast<PriceType>()
                .Select(x => new
                {
                    Id = x,
                    Name = x.GetDisplay(),
                })
            );

            player.Emit("StaffPrices", false, Functions.GetPricesHTML(), jsonTypes);
        }

        [AsyncClientEvent(nameof(StaffPriceSave))]
        public static async Task StaffPriceSave(MyPlayer player, int id, int type, string name, float value)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Prices))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            if (!Enum.IsDefined(typeof(PriceType), Convert.ToByte(type)))
            {
                player.EmitStaffShowMessage("Tipo inválido.");
                return;
            }

            if (value <= 0)
            {
                player.EmitStaffShowMessage("Valor inválido.");
                return;
            }

            var priceType = (PriceType)type;
            if (priceType == PriceType.Weapons)
            {
                if (!Enum.TryParse(name, true, out WeaponModel wep))
                {
                    player.EmitStaffShowMessage($"Arma {name} não existe.");
                    return;
                }
            }
            else if (priceType == PriceType.Jobs || priceType == PriceType.JobVehicleRental)
            {
                if (!Global.Jobs.Any(x => x.CharacterJob.ToString().Equals(name, StringComparison.CurrentCultureIgnoreCase)))
                {
                    player.EmitStaffShowMessage($"Emprego {name} não existe.");
                    return;
                }
            }
            else if (priceType == PriceType.Drugs)
            {
                if (!Enum.TryParse(name, true, out ItemCategory itemCategory))
                {
                    player.EmitStaffShowMessage($"Droga {name} não existe.");
                    return;
                }

                if (itemCategory != ItemCategory.Weed && itemCategory != ItemCategory.Cocaine
                    && itemCategory != ItemCategory.Crack && itemCategory != ItemCategory.Heroin
                    && itemCategory != ItemCategory.MDMA && itemCategory != ItemCategory.Xanax
                    && itemCategory != ItemCategory.Oxycontin && itemCategory != ItemCategory.Metanfetamina)
                {
                    player.EmitStaffShowMessage($"Droga {itemCategory} não existe.");
                    return;
                }
            }
            else if (priceType != PriceType.ConvenienceStore && priceType != PriceType.Tuning)
            {
                if (!Enum.TryParse(name, true, out VehicleModel v1) && !Enum.TryParse(name, true, out VehicleModelMods v2))
                {
                    player.EmitStaffShowMessage($"Veículo {name} não existe.");
                    return;
                }
            }

            if (Global.Prices.Any(x => x.Id != id && x.Type == priceType && x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase)))
            {
                player.EmitStaffShowMessage($"Já existe um preço do tipo {priceType.GetDisplay()} com o nome {name}.");
                return;
            }

            var price = new Price();
            if (id > 0)
                price = Global.Prices.FirstOrDefault(x => x.Id == id);

            price.Type = priceType;
            price.Name = name;
            price.Value = value;

            await using var context = new DatabaseContext();

            if (price.Id == 0)
                await context.Prices.AddAsync(price);
            else
                context.Prices.Update(price);

            await context.SaveChangesAsync();

            if (id == 0)
                Global.Prices.Add(price);

            player.EmitStaffShowMessage($"Preço {(id == 0 ? "criado" : "editado")}.", true);

            await player.GravarLog(LogType.Staff, $"Gravar Preço | {Functions.Serialize(price)}", null);

            var html = Functions.GetPricesHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Prices)))
                target.Emit("StaffPrices", true, html);
        }

        [AsyncClientEvent(nameof(StaffPriceRemove))]
        public static async Task StaffPriceRemove(MyPlayer player, int id)
        {
            if (!player.StaffFlags.Contains(StaffFlag.Prices))
            {
                player.EmitStaffShowMessage(Global.MENSAGEM_SEM_AUTORIZACAO);
                return;
            }

            var price = Global.Prices.FirstOrDefault(x => x.Id == id);
            if (price != null)
            {
                await using var context = new DatabaseContext();
                context.Prices.Remove(price);
                await context.SaveChangesAsync();
                Global.Prices.Remove(price);
                await player.GravarLog(LogType.Staff, $"Remover Preço | {Functions.Serialize(price)}", null);
            }

            player.EmitStaffShowMessage($"Preço {id} excluído.");

            var html = Functions.GetPricesHTML();
            foreach (var target in Global.Players.Where(x => x.StaffFlags.Contains(StaffFlag.Prices)))
                target.Emit("StaffPrices", true, html);
        }
    }
}