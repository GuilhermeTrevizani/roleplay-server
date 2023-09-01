using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Async.Elements.Entities;
using AltV.Net.Data;
using AltV.Net.Enums;
using Roleplay.Entities;
using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.Json;
using System.Threading.Tasks;

namespace Roleplay.Factories
{
    public class MyVehicle : AsyncVehicle
    {
        public MyVehicle(ICore server, IntPtr nativePointer, uint id) : base(server, nativePointer, id)
        {
        }

        public class Dano
        {
            public List<bool> WindowsDamaged { get; set; } = new List<bool>();
            public List<bool> LightsDamaged { get; set; } = new List<bool>();
            public List<bool> SpecialLightsDamaged { get; set; } = new List<bool>();
            public List<Wheel> Wheels { get; set; } = new List<Wheel>();
            public List<Part> Parts { get; set; } = new List<Part>();
            public List<Bumper> Bumpers { get; set; } = new List<Bumper>();
            public List<ArmoredWindow> ArmoredWindows { get; set; } = new List<ArmoredWindow>();

            public class Wheel
            {
                public Wheel(byte id, float health, bool detached, bool burst, bool hasTire)
                {
                    Id = id;
                    Health = health;
                    Detached = detached;
                    Burst = burst;
                    HasTire = hasTire;
                }

                public byte Id { get; set; } = 0;
                public float Health { get; set; } = 0;
                public bool Detached { get; set; } = false;
                public bool Burst { get; set; } = false;
                public bool HasTire { get; set; } = true;
            }

            public class ArmoredWindow
            {
                public ArmoredWindow(byte id, float health, byte shootCount)
                {
                    Id = id;
                    Health = health;
                    ShootCount = shootCount;
                }

                public byte Id { get; set; } = 0;
                public float Health { get; set; } = 0;
                public byte ShootCount { get; set; } = 0;
            }

            public class Part
            {
                public Part(VehiclePart vehiclePart, VehiclePartDamage vehiclePartDamage, byte bulletHoles)
                {
                    VehiclePart = vehiclePart;
                    VehiclePartDamage = vehiclePartDamage;
                    BulletHoles = bulletHoles;
                }

                public VehiclePart VehiclePart { get; set; }
                public VehiclePartDamage VehiclePartDamage { get; set; }
                public byte BulletHoles { get; set; } = 0;
            }

            public class Bumper
            {
                public Bumper(VehicleBumper vehicleBumper, VehicleBumperDamage vehicleBumperDamage)
                {
                    VehicleBumper = vehicleBumper;
                    VehicleBumperDamage = vehicleBumperDamage;
                }

                public VehicleBumper VehicleBumper { get; set; }
                public VehicleBumperDamage VehicleBumperDamage { get; set; }
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
            public string Attacker { get; set; }
            public float Distance { get; set; }
        }

        public Entities.Vehicle VehicleDB { get; set; } = new();

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
        public List<bool> StatusPortas { get; set; } = new() { true, true, true, true, true, true };

        public int Speed { get => Convert.ToInt32(Math.Abs(Velocity.GetMagnitude() * 3.6)); }

        public string CombustivelHUD
        {
            get
            {
                var combustivel = "COMBUSTÍVEL: ";
                var porcentagem = Convert.ToInt32(Convert.ToDecimal(VehicleDB.Fuel) / Convert.ToDecimal(VehicleDB.MaxFuel) * 100);
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
            get => !(VehicleDB.Model.ToUpper() == VehicleModel.Polmav.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModelMods.LSPDHELI.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Predator.ToString().ToUpper()
                || new List<VehicleModelType> { VehicleModelType.BOAT, VehicleModelType.BMX, VehicleModelType.PLANE, VehicleModelType.HELI, VehicleModelType.TRAILER, VehicleModelType.TRAIN }.Contains(Info.Type));
        }

        public DateTime? DataExpiracaoAluguel { get; set; }

        public List<Damage> Damages { get; set; } = new();

        public bool TemJanelas
        {
            get => !(VehicleDB.Model.ToUpper() == VehicleModel.Policeb.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModelMods.POLTHRUST.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModelMods.LSPDB.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Predator.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Wastlndr.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Raptor.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Veto.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Veto2.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Banshee2.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Voltic2.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Airtug.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Caddy.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Caddy2.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Caddy3.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Forklift.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Mower.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Tractor.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Bifta.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Blazer.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Blazer2.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Blazer3.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Blazer4.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Blazer5.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Bodhi2.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Dune.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Dune2.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Dune3.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Dune4.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Dune5.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Outlaw.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Trophytruck.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Vagrant.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Verus.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Winky.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Faction.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Faction2.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Faction3.ToString().ToUpper()
                || new List<VehicleModelType> { VehicleModelType.BOAT, VehicleModelType.BMX, VehicleModelType.BIKE }.Contains(Info.Type));
        }

        public bool SpotlightActive { get; set; }

        public bool TemArmazenamento
        {
            get => !(VehicleDB.Model.ToUpper() == VehicleModel.Policeb.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModelMods.POLTHRUST.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModelMods.LSPDB.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Predator.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Wastlndr.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Raptor.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Veto.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Veto2.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Banshee2.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Voltic2.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Tractor.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Tractor2.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Tractor3.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Bifta.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Blazer.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Blazer2.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Blazer3.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Blazer4.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Blazer5.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Bodhi2.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Dune.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Dune2.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Dune3.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Dune4.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Dune5.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Outlaw.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Trophytruck.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Vagrant.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Verus.ToString().ToUpper()
                || VehicleDB.Model.ToUpper() == VehicleModel.Winky.ToString().ToUpper()
                || new List<VehicleModelType> { VehicleModelType.BOAT, VehicleModelType.BMX, VehicleModelType.BIKE }.Contains(Info.Type));
        }

        public List<VehicleItem> Itens { get; set; } = new();

        public System.Timers.Timer Timer { get; set; }

        public List<Spot> CollectSpots { get; set; } = new();

        public TruckerLocation TruckerLocation { get; set; }

        public AudioSpot AudioSpot { get; set; }

        public AudioSpot AlarmAudioSpot { get; set; }

        public async Task<bool> Estacionar(MyPlayer player)
        {
            Timer?.Stop();
            StopAlarm();

            if (VehicleDB.Job == CharacterJob.None)
            {
                VehicleDB.EngineHealth = EngineHealth;
                VehicleDB.BodyHealth = BodyHealth;
                VehicleDB.BodyAdditionalHealth = BodyAdditionalHealth;
                VehicleDB.PetrolTankHealth = PetrolTankHealth;

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

                VehicleDB.StructureDamagesJSON = JsonSerializer.Serialize(dano);
                VehicleDB.DamagesJSON = JsonSerializer.Serialize(Damages);
            }

            await using var context = new DatabaseContext();
            context.Vehicles.Update(VehicleDB);
            await context.SaveChangesAsync();

            Destroy();

            if (player != null)
                await player.GravarLog(LogType.EstacionarVeiculo, VehicleDB.Id.ToString(), null);

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
            var occupants = Global.Players.Where(x => x.Vehicle == this)
                .Select(x => new
                {
                    Player = x,
                    x.Seat,
                }).ToList();

            var alarmOn = AlarmAudioSpot != null;

            Timer?.Stop();
            StopAlarm();
            Destroy();

            VehicleDB.PosX = pos.X;
            VehicleDB.PosY = pos.Y;
            VehicleDB.PosZ = pos.Z;

            VehicleDB.RotR = rot.Roll;
            VehicleDB.RotP = rot.Pitch;
            VehicleDB.RotY = rot.Yaw;

            var veh = await VehicleDB.Spawnar(null);
            veh.LockState = lockState;
            veh.EngineOn = engineOn;
            veh.VehicleDB.PosX = vehPos.X;
            veh.VehicleDB.PosY = vehPos.Y;
            veh.VehicleDB.PosZ = vehPos.Z;

            veh.VehicleDB.RotR = vehRot.X;
            veh.VehicleDB.RotP = vehRot.Y;
            veh.VehicleDB.RotY = vehRot.Z;

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
                JsonSerializer.Serialize(
                    Itens.Select(x => new
                    {
                        ID = x.Id,
                        x.Image,
                        x.Name,
                        x.Quantity,
                        Slot = 1000 + x.Slot,
                        Extra = Functions.GetItemExtra(x),
                        Weight = (x.Quantity * x.Weight).ToString("N2"),
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

        public async Task ActivateProtection(MyPlayer player)
        {
            if (VehicleDB.ProtectionLevel >= 1)
                StartAlarm();

            if (VehicleDB.ProtectionLevel >= 2)
            {
                var target = Global.Players.FirstOrDefault(x => x.Character.Id == VehicleDB.CharacterId && x.Cellphone > 0 && !x.CellphoneItem.ModoAviao);
                target?.SendMessage(MessageType.None, $"[CELULAR] SMS de {target.ObterNomeContato(Global.EMERGENCY_NUMBER)}: O alarme do seu {VehicleDB.Model.ToUpper()} {VehicleDB.Plate.ToUpper()} foi acionado.", Global.CELLPHONE_MAIN_COLOR);
            }

            if (VehicleDB.ProtectionLevel >= 3)
            {
                var emergencyCall = new EmergencyCall
                {
                    Type = EmergencyCallType.Police,
                    Number = Global.EMERGENCY_NUMBER,
                    PosX = Position.X,
                    PosY = Position.Y,
                    Message = $"O alarme do veículo {VehicleDB.Model.ToUpper()} {VehicleDB.Plate.ToUpper()} foi acionado.",
                };

                player.AreaNameType = 1;
                player.AreaNameJSON = JsonSerializer.Serialize(emergencyCall);
                await player.EmitAsync("SetAreaName");
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

                var mods = JsonSerializer.Deserialize<List<VehicleMod>>(VehicleDB.ModsJSON);
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