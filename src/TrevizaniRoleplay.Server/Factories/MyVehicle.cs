using AltV.Net;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Data;
using AltV.Net.Enums;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Extensions;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Factories
{
    public class MyVehicle(ICore server, IntPtr nativePointer, uint id) : AsyncVehicle(server, nativePointer, id)
    {
        public class Dano
        {
            public List<bool> WindowsDamaged { get; set; } = [];
            public List<bool> LightsDamaged { get; set; } = [];
            public List<bool> SpecialLightsDamaged { get; set; } = [];
            public List<Wheel> Wheels { get; set; } = [];
            public List<Part> Parts { get; set; } = [];
            public List<Bumper> Bumpers { get; set; } = [];
            public List<ArmoredWindow> ArmoredWindows { get; set; } = [];

            public class Wheel(byte id, float health, bool detached, bool burst, bool hasTire)
            {
                public byte Id { get; set; } = id;
                public float Health { get; set; } = health;
                public bool Detached { get; set; } = detached;
                public bool Burst { get; set; } = burst;
                public bool HasTire { get; set; } = hasTire;
            }

            public class ArmoredWindow(byte id, float health, byte shootCount)
            {
                public byte Id { get; set; } = id;
                public float Health { get; set; } = health;
                public byte ShootCount { get; set; } = shootCount;
            }

            public class Part(VehiclePart vehiclePart, VehiclePartDamage vehiclePartDamage, byte bulletHoles)
            {
                public VehiclePart VehiclePart { get; set; } = vehiclePart;
                public VehiclePartDamage VehiclePartDamage { get; set; } = vehiclePartDamage;
                public byte BulletHoles { get; set; } = bulletHoles;
            }

            public class Bumper(VehicleBumper vehicleBumper, VehicleBumperDamage vehicleBumperDamage)
            {
                public VehicleBumper VehicleBumper { get; set; } = vehicleBumper;
                public VehicleBumperDamage VehicleBumperDamage { get; set; } = vehicleBumperDamage;
            }
        }

        public class Damage
        {
            public DateTime Data { get; set; } = DateTime.Now;
            public uint BodyHealthDamage { get; set; }
            public uint AdditionalBodyHealthDamage { get; set; }
            public uint EngineHealthDamage { get; set; }
            public uint PetrolTankDamage { get; set; }
            public uint WeaponHash { get; set; }
            public string? Attacker { get; set; }
            public float Distance { get; set; }
        }

        public Domain.Entities.Vehicle VehicleDB { get; set; } = new();

        /// <summary>
        /// Nome do personagem que usou o /fspawn ou /valugar
        /// </summary>
        public string NomeEncarregado { get; set; } = string.Empty;

        /// <summary>
        /// TRUE: FECHADA
        /// 0 = Front Left Door
        /// 1 = Front Right Door
        /// 2 = Back Left Door
        /// 3 = Back Right Door
        /// 4 = Hood
        /// </summary>
        public List<bool> StatusPortas { get; set; } = [true, true, true, true, true, true];

        public int Speed { get => Convert.ToInt32(Math.Abs(Velocity.GetMagnitude() * 3.6)); }

        public string CombustivelHUD
        {
            get
            {
                var combustivel = "COMBUSTÍVEL: ";
                var porcentagem = Convert.ToInt32(Convert.ToDecimal(VehicleDB.Fuel) / Convert.ToDecimal(VehicleDB.GetMaxFuel()) * 100);
                if (porcentagem > 50)
                    combustivel += "~g~";
                else if (porcentagem > 20)
                    combustivel += "~o~";
                else
                    combustivel += "~r~";
                return $"{combustivel}{porcentagem}%";
            }
        }

        public VehicleModelInfo Info { get => Alt.GetVehicleModelInfo(Model); }

        public bool TemTanqueCombustivel
        {
            get => !(VehicleDB.Model.Equals(VehicleModel.Polmav.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModelMods.LSPDHELI.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Predator.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || new List<VehicleModelType> { VehicleModelType.BOAT, VehicleModelType.BMX, VehicleModelType.PLANE, VehicleModelType.HELI, VehicleModelType.TRAILER, VehicleModelType.TRAIN }.Contains(Info.Type));
        }

        public DateTime? DataExpiracaoAluguel { get; set; }

        public List<Damage> Damages { get; set; } = [];

        public bool TemJanelas
        {
            get => !(VehicleDB.Model.Equals(VehicleModel.Policeb.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModelMods.POLTHRUST.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModelMods.LSPDB.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Predator.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Wastlndr.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Raptor.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Veto.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Veto2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Banshee2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Voltic2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Airtug.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Caddy.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Caddy2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Caddy3.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Forklift.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Mower.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Tractor.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Bifta.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Blazer.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Blazer2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Blazer3.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Blazer4.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Blazer5.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Bodhi2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Dune.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Dune2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Dune3.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Dune4.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Dune5.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Outlaw.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Trophytruck.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Vagrant.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Verus.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Winky.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Faction.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Faction2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Faction3.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || new List<VehicleModelType> { VehicleModelType.BOAT, VehicleModelType.BMX, VehicleModelType.BIKE }.Contains(Info.Type));
        }

        public bool SpotlightActive { get; set; }

        public bool TemArmazenamento
        {
            get => !(VehicleDB.Model.Equals(VehicleModel.Policeb.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModelMods.POLTHRUST.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModelMods.LSPDB.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Predator.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Wastlndr.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Raptor.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Veto.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Veto2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Banshee2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Voltic2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Tractor.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Tractor2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Tractor3.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Bifta.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Blazer.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Blazer2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Blazer3.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Blazer4.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Blazer5.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Bodhi2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Dune.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Dune2.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Dune3.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Dune4.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Dune5.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Outlaw.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Trophytruck.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Vagrant.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Verus.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || VehicleDB.Model.Equals(VehicleModel.Winky.ToString(), StringComparison.CurrentCultureIgnoreCase)
                || new List<VehicleModelType> { VehicleModelType.BOAT, VehicleModelType.BMX, VehicleModelType.BIKE }.Contains(Info.Type));
        }

        public List<VehicleItem> Itens { get; set; } = [];
        public System.Timers.Timer Timer { get; set; }
        public List<Spot> CollectSpots { get; set; } = [];
        public TruckerLocation? TruckerLocation { get; set; }
        public AudioSpot? AudioSpot { get; set; }
        public AudioSpot? AlarmAudioSpot { get; set; }

        public async Task<bool> Estacionar(MyPlayer? player)
        {
            Timer?.Stop();
            StopAlarm();

            if (VehicleDB.Job == CharacterJob.None)
            {
                var dano = new Dano();

                foreach (var x in Enum.GetValues(typeof(VehicleBumper)).Cast<VehicleBumper>())
                    dano.Bumpers.Add(new Dano.Bumper(x, (VehicleBumperDamage)GetBumperDamageLevel((byte)x)));

                foreach (var x in Enum.GetValues(typeof(VehiclePart)).Cast<VehiclePart>())
                    dano.Parts.Add(new Dano.Part(x, (VehiclePartDamage)GetPartDamageLevel((byte)x), GetPartBulletHoles((byte)x)));

                for (byte i = 0; i <= 20; i++)
                {
                    dano.LightsDamaged.Add(IsLightDamaged(i));
                    dano.SpecialLightsDamaged.Add(IsSpecialLightDamaged(i));
                    dano.WindowsDamaged.Add(IsWindowDamaged(i));
                    dano.ArmoredWindows.Add(new Dano.ArmoredWindow(i, GetArmoredWindowHealth(i), GetArmoredWindowShootCount(i)));
                }

                for (byte i = 0; i < WheelsCount; i++)
                    dano.Wheels.Add(new Dano.Wheel(i, GetWheelHealth(i), IsWheelDetached(i), IsWheelBurst(i), DoesWheelHasTire(i)));

                VehicleDB.SetDamages(EngineHealth, BodyHealth, BodyAdditionalHealth, PetrolTankHealth,
                    Functions.Serialize(dano), Functions.Serialize(Damages));
            }

            await using var context = new DatabaseContext();
            context.Vehicles.Update(VehicleDB);
            await context.SaveChangesAsync();

            Destroy();

            if (player != null)
                await player.GravarLog(LogType.ParkVehicle, VehicleDB.Id.ToString(), null);

            return true;
        }

        public async Task<MyVehicle> Reparar()
        {
            var vehPos = new Vector3(VehicleDB.PosX, VehicleDB.PosY, VehicleDB.PosZ);
            var vehRot = new Vector3(VehicleDB.RotR, VehicleDB.RotP, VehicleDB.RotY);

            var pos = Position;
            var rot = Rotation;
            var lockState = LockState;
            var engineOn = EngineOn;
            var occupants = Global.SpawnedPlayers.Where(x => x.Vehicle == this)
                .Select(x => new
                {
                    Player = x,
                    x.Seat,
                }).ToList();

            var alarmOn = AlarmAudioSpot != null;

            Timer?.Stop();
            StopAlarm();
            Destroy();

            VehicleDB.ChangePosition(pos.X, pos.Y, pos.Z, rot.Roll, rot.Pitch, rot.Yaw);

            var veh = await VehicleDB.Spawnar(null);
            veh.LockState = lockState;
            veh.EngineOn = engineOn;
            VehicleDB.ChangePosition(vehPos.X, vehPos.Y, vehPos.Z, vehRot.X, vehRot.Y, vehRot.Z);

            foreach (var occupant in occupants)
                occupant.Player.SetIntoVehicle(veh, occupant.Seat);

            if (alarmOn)
                veh.StartAlarm();

            return veh;
        }

        public void ShowInventory(MyPlayer player, bool update)
        {
            player.ShowInventory(player, InventoryShowType.Vehicle,
                $"{VehicleDB.Model.ToUpper()} ({VehicleDB.Plate.ToUpper()}) [{VehicleDB.Id}]",
                Functions.Serialize(
                    Itens.Select(x => new
                    {
                        ID = x.Id,
                        Image = x.GetImageName(),
                        Name = x.GetName(),
                        x.Quantity,
                        Slot = 1000 + x.Slot,
                        Extra = x.GetExtra(),
                        Weight = (x.Quantity * x.GetWeight()).ToString("N2"),
                    })
            ), update, VehicleDB.Id);
        }

        public bool CanAccess(MyPlayer player)
        {
            return VehicleDB.CharacterId == player.Character.Id
                || (VehicleDB.FactionId.HasValue && VehicleDB.FactionId == player.Character.FactionId)
                || (!string.IsNullOrWhiteSpace(NomeEncarregado) && NomeEncarregado == player.Character.Name)
                || player.Items.Any(x => x.Category == ItemCategory.VehicleKey && x.Type == VehicleDB.LockNumber);
        }

        public void ActivateProtection(MyPlayer player)
        {
            if (VehicleDB.ProtectionLevel >= 1)
                StartAlarm();

            if (VehicleDB.ProtectionLevel >= 2)
            {
                var target = Global.SpawnedPlayers.FirstOrDefault(x => x.Character.Id == VehicleDB.CharacterId && x.Cellphone > 0 && !x.CellphoneItem.FlightMode);
                target?.SendMessage(MessageType.None, $"[CELULAR] SMS de {target.ObterNomeContato(Global.EMERGENCY_NUMBER)}: O alarme do seu {VehicleDB.Model.ToUpper()} {VehicleDB.Plate.ToUpper()} foi acionado.", Global.CELLPHONE_MAIN_COLOR);
            }

            if (VehicleDB.ProtectionLevel >= 3)
            {
                var emergencyCall = new EmergencyCall();
                emergencyCall.Create(EmergencyCallType.Police, Global.EMERGENCY_NUMBER, Position.X, Position.Y,
                    $"O alarme do veículo {VehicleDB.Model.ToUpper()} {VehicleDB.Plate.ToUpper()} foi acionado.", string.Empty);

                player.AreaNameType = 1;
                player.AreaNameJSON = Functions.Serialize(emergencyCall);
                player.Emit("SetAreaName");
            }
        }

        public void StartAlarm()
        {
            if (AlarmAudioSpot == null)
            {
                AlarmAudioSpot = new AudioSpot
                {
                    Dimension = Dimension,
                    VehicleId = Id,
                    Source = Global.URL_AUDIO_VEHICLE_ALARM,
                    Loop = true,
                };
                AlarmAudioSpot.SetupAllClients();
            }
        }

        public void StopAlarm()
        {
            AlarmAudioSpot?.RemoveAllClients();
            AlarmAudioSpot = null;
        }

        public void ClearMods()
        {
            foreach (var mod in Enum.GetValues(typeof(VehicleModType)).Cast<byte>())
                SetMod(mod, 0);
        }

        public void SetDefaultMods()
        {
            if (ModKitsCount > 0)
            {
                ModKit = 1;

                PrimaryColorRgb = new Rgba(VehicleDB.Color1R, VehicleDB.Color1G, VehicleDB.Color1B, 255);
                SecondaryColorRgb = new Rgba(VehicleDB.Color2R, VehicleDB.Color2G, VehicleDB.Color2B, 255);
                SetWheels(VehicleDB.WheelType, VehicleDB.WheelVariation);
                WheelColor = VehicleDB.WheelColor;
                NeonColor = new Rgba(VehicleDB.NeonColorR, VehicleDB.NeonColorG, VehicleDB.NeonColorB, 255);
                SetNeonActive(VehicleDB.NeonLeft, VehicleDB.NeonRight, VehicleDB.NeonFront, VehicleDB.NeonBack);

                var mods = Functions.Deserialize<List<VehicleMod>>(VehicleDB.ModsJSON);
                foreach (var mod in mods)
                    SetMod(mod.Type, mod.Id);

                HeadlightColor = VehicleDB.HeadlightColor;
                LightsMultiplier = VehicleDB.LightsMultiplier;
                WindowTint = VehicleDB.WindowTint;
                TireSmokeColor = new Rgba(VehicleDB.TireSmokeColorR, VehicleDB.TireSmokeColorG, VehicleDB.TireSmokeColorB, 255);
            }
        }
    }
}