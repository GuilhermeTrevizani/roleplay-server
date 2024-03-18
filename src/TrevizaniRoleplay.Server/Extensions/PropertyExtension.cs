using AltV.Net;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Extensions
{
    public static class PropertyExtension
    {
        public static void CreateIdentifier(this Property property)
        {
            RemoveIdentifier(property);

            var pos = new Vector3(property.EntrancePosX, property.EntrancePosY, property.EntrancePosZ - 0.95f);

            // TODO: Rollback commentary when alt:V implements
            //var entranceMarker = (MyMarker)Alt.CreateMarker(MarkerType.MarkerHalo, pos, Global.MainRgba);
            //entranceMarker.Scale = new Vector3(1, 1, 1.5f);
            //entranceMarker.Dimension = property.Number;
            //entranceMarker.PropertyId = property.Id;

            var entranceColShape = (MyColShape)Alt.CreateColShapeCylinder(pos, 1, 1.5f);
            entranceColShape.Description = $"[PROPRIEDADE Nº {property.Number}] {{#FFFFFF}}{(!property.CharacterId.HasValue ? $"Use /comprar para comprar por ${property.Value:N0}." : string.Empty)}";
            entranceColShape.PropertyId = property.Id;
        }

        public static void RemoveIdentifier(this Property property)
        {
            var entranceMarker = Global.Markers.FirstOrDefault(x => x.PropertyId == property.Id);
            entranceMarker?.Destroy();

            var entranceColShape = Global.ColShapes.FirstOrDefault(x => x.PropertyId == property.Id);
            entranceColShape?.Destroy();
        }

        public static void ShowInventory(this Property property, MyPlayer player, bool update)
        {
            player.ShowInventory(player, InventoryShowType.Property,
                $"Propriedade Nº {property.Number}",
                Functions.Serialize(
                    property.Items!.Select(x => new
                    {
                        ID = x.Id,
                        Image = x.GetImageName(),
                        Name = x.GetName(),
                        x.Quantity,
                        Slot = 1000 + x.Slot,
                        Extra = x.GetExtra(),
                        Weight = (x.Quantity * x.GetWeight()).ToString("N2"),
                    })
            ), update, property.Id);
        }

        public static bool CanAccess(this Property property, MyPlayer player)
        {
            return property.CharacterId == player.Character.Id
                || player.Items.Any(x => x.Category == ItemCategory.PropertyKey && x.Type == property.LockNumber);
        }
        public static async Task ActivateProtection(this Property property)
        {
            if (property.ProtectionLevel >= 1)
                StartAlarm(property);

            if (property.ProtectionLevel >= 2)
            {
                var target = Global.SpawnedPlayers.FirstOrDefault(x => x.Character.Id == property.CharacterId && x.Cellphone > 0 && !x.CellphoneItem.FlightMode);
                target?.SendMessage(MessageType.None, $"[CELULAR] SMS de {target.ObterNomeContato(Global.EMERGENCY_NUMBER)}: O alarme da sua propriedade {property.Number} em {property.Address} foi acionado.", Global.CELLPHONE_MAIN_COLOR);
            }

            if (property.ProtectionLevel >= 3)
            {
                var emergencyCall = new EmergencyCall();
                emergencyCall.Create(EmergencyCallType.Police, Global.EMERGENCY_NUMBER,
                    property.EntrancePosX, property.EntrancePosY,
                    $"O alarme da propriedade {property.Number} foi acionado.", property.Address);
                await using var context = new DatabaseContext();
                await context.EmergencyCalls.AddAsync(emergencyCall);
                await context.SaveChangesAsync();
                Global.EmergencyCalls.Add(emergencyCall);
                emergencyCall.SendMessage();
            }
        }

        public static void StartAlarm(this Property property)
        {
            if (Global.AudioSpots.Any(x => x.PropertyId == property.Id))
                return;

            var exteriorAlarmAudioSpot = new AudioSpot
            {
                Position = new Vector3(property.EntrancePosX, property.EntrancePosY, property.EntrancePosZ),
                Dimension = property.EntranceDimension,
                Source = Global.URL_AUDIO_PROPERTY_ALARM,
                Loop = true,
                PropertyId = property.Id,
            };
            exteriorAlarmAudioSpot.SetupAllClients();

            var interiorAlarmAudioSpot = new AudioSpot
            {
                Position = new Vector3(property.ExitPosX, property.ExitPosY, property.ExitPosZ),
                Dimension = property.EntranceDimension,
                Source = Global.URL_AUDIO_PROPERTY_ALARM,
                Loop = true,
                PropertyId = property.Id,
            };
            interiorAlarmAudioSpot.SetupAllClients();
        }

        public static void StopAlarm(this Property property)
        {
            var audioSpots = Global.AudioSpots.Where(x => x.PropertyId == property.Id);
            foreach (var audioSpot in audioSpots)
                audioSpot.RemoveAllClients();
        }
    }
}