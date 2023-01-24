using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Microsoft.EntityFrameworkCore;
using Roleplay.Factories;
using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Roleplay.Entities
{
    public class Vehicle
    {
        public int Id { get; set; }

        public string Model { get; set; }

        public float PosX { get; set; }

        public float PosY { get; set; }

        public float PosZ { get; set; }

        public float RotR { get; set; }

        public float RotP { get; set; }

        public float RotY { get; set; }

        public byte Color1R { get; set; } = 255;

        public byte Color1G { get; set; } = 255;

        public byte Color1B { get; set; } = 255;

        public byte Color2R { get; set; } = 255;

        public byte Color2G { get; set; } = 255;

        public byte Color2B { get; set; } = 255;

        public int? CharacterId { get; set; }

        public Character Character { get; set; }

        public string Plate { get; set; }

        public int? FactionId { get; set; }

        public Faction Faction { get; set; }

        public int EngineHealth { get; set; } = 1000;

        public byte Livery { get; set; } = 1;

        public int SeizedValue { get; set; }

        public int Fuel { get; set; }

        public string StructureDamagesJSON { get; set; } = "{}";

        public bool Parked { get; set; }

        public bool Sold { get; set; }

        public CharacterJob Job { get; set; } = CharacterJob.None;

        public string DamagesJSON { get; set; } = "[]";

        public uint BodyHealth { get; set; } = 1000;

        public uint BodyAdditionalHealth { get; set; } = 1000;

        public int PetrolTankHealth { get; set; } = 1000;

        public uint LockNumber { get; set; }

        public bool FactionGift { get; set; }

        public byte ProtectionLevel { get; set; }

        public int DismantledValue { get; set; }

        public bool XMR { get; set; }

        public string ModsJSON { get; set; } = "[]";

        public byte WheelType { get; set; }

        public byte WheelVariation { get; set; }

        public byte WheelColor { get; set; }

        public byte NeonColorR { get; set; }

        public byte NeonColorG { get; set; }

        public byte NeonColorB { get; set; }

        public bool NeonLeft { get; set; }

        public bool NeonRight { get; set; }

        public bool NeonFront { get; set; }

        public bool NeonBack { get; set; }

        public byte HeadlightColor { get; set; }

        public float LightsMultiplier { get; set; } = 1;

        public byte WindowTint { get; set; }

        public byte TireSmokeColorR { get; set; }

        public byte TireSmokeColorG { get; set; }

        public byte TireSmokeColorB { get; set; }

        [NotMapped, JsonIgnore]
        public int MaxFuel { get; set; } = 100;

        [NotMapped, JsonIgnore]
        public string LiveryName
        {
            get
            {
                if (Model.ToUpper() == VehicleModelMods.NSCOUTLSPD.ToString().ToUpper())
                {
                    if (Livery == 1)
                        return "366-18";
                    else if (Livery == 2)
                        return "301-18";
                    else if (Livery == 3)
                        return "974-18";
                }
                else if (Model.ToUpper() == VehicleModel.Police3.ToString().ToUpper())
                {
                    if (Livery == 1)
                        return "671-18";
                    else if (Livery == 2)
                        return "033-18";
                }
                else if (Model.ToUpper() == VehicleModelMods.PSCOUT.ToString().ToUpper())
                {
                    if (Livery == 1)
                        return "015-18";
                    else if (Livery == 2)
                        return "540-18";
                    else if (Livery == 3)
                        return "560-18";
                }
                else if (Model.ToUpper() == VehicleModelMods.VVPI.ToString().ToUpper())
                {
                    if (Livery == 1)
                        return "450-18";
                    else if (Livery == 2)
                        return "525-18";
                }
                else if (Model.ToUpper() == VehicleModelMods.VVPI2.ToString().ToUpper())
                {
                    if (Livery == 1)
                        return "989-18";
                    else if (Livery == 2)
                        return "741-18";
                }
                return string.Empty;
            }
        }

        public async Task<MyVehicle> Spawnar(MyPlayer player)
        {
            MyVehicle veh;
            if (Enum.TryParse(Model, true, out VehicleModel v1))
                veh = (MyVehicle)Alt.CreateVehicle(v1, new Position(PosX, PosY, PosZ), new Rotation(RotR, RotP, RotY));
            else
                veh = (MyVehicle)Alt.CreateVehicle(Model, new Position(PosX, PosY, PosZ), new Rotation(RotR, RotP, RotY));

            veh.Vehicle = this;
            veh.ManualEngineControl = true;
            veh.NumberplateText = veh.Vehicle.Plate;
            veh.LockState = VehicleLockState.Locked;
            veh.Livery = veh.Vehicle.Livery;
            veh.SetSyncedMetaData("id", veh.Vehicle.Id);
            veh.SetSyncedMetaData("placa", veh.Vehicle.Plate);
            veh.SetSyncedMetaData("modelo", veh.Vehicle.Model.ToUpper());

            if (veh.TemTanqueCombustivel)
                veh.SetSyncedMetaData("combustivel", veh.CombustivelHUD);

            veh.SetDefaultMods();

            if (Job == CharacterJob.None)
            {
                veh.Damages = JsonSerializer.Deserialize<List<MyVehicle.Damage>>(DamagesJSON);

                await using var context = new DatabaseContext();
                veh.Itens = (await context.VehiclesItems.Where(x => x.VehicleId == Id).ToListAsync()).Select(x => new VehicleItem(x)).ToList();

                if (player != null)
                {
                    veh.EngineHealth = veh.Vehicle.EngineHealth;
                    veh.BodyHealth = veh.Vehicle.BodyHealth;
                    veh.BodyAdditionalHealth = veh.Vehicle.BodyAdditionalHealth;
                    veh.PetrolTankHealth = veh.Vehicle.PetrolTankHealth;

                    var dano = JsonSerializer.Deserialize<MyVehicle.Dano>(veh.Vehicle.StructureDamagesJSON);
                    if (dano?.WindowsDamaged?.Count > 0)
                    {
                        foreach (var x in dano.Bumpers)
                            veh.SetBumperDamageLevel((byte)x.VehicleBumper, (byte)x.VehicleBumperDamage);

                        foreach (var x in dano.Parts)
                        {
                            veh.SetPartDamageLevel((byte)x.VehiclePart, (byte)x.VehiclePartDamage);
                            veh.SetPartBulletHoles((byte)x.VehiclePart, x.BulletHoles);
                        }

                        for (byte i = 0; i < dano.LightsDamaged.Count; i++)
                        {
                            veh.SetLightDamaged(i, dano.LightsDamaged[i]);
                            (veh as IVehicle).SetSpecialLightDamaged(i, dano.SpecialLightsDamaged[i]);
                            veh.SetWindowDamaged(i, dano.WindowsDamaged[i]);

                            if (veh.HasArmoredWindows)
                            {
                                var armoredWindow = dano.ArmoredWindows[i];
                                veh.SetArmoredWindowHealth(i, armoredWindow.Health);
                                veh.SetArmoredWindowShootCount(i, armoredWindow.ShootCount);
                            }
                        }

                        foreach (var x in dano.Wheels)
                        {
                            veh.SetWheelHealth(x.Id, x.Health);
                            veh.SetWheelHasTire(x.Id, x.HasTire);

                            if (x.Detached)
                                veh.SetWheelDetached(x.Id, x.Detached);

                            if (x.Burst)
                                veh.SetWheelBurst(x.Id, x.Burst);
                        }
                    }
                }
            }

            veh.Timer = new System.Timers.Timer(60000);
            veh.Timer.Elapsed += (o, e) =>
            {
                try
                {
                    Alt.Log($"Vehicle Timer {veh.Vehicle.Id}");
                    if (veh.DataExpiracaoAluguel.HasValue)
                    {
                        if (veh.DataExpiracaoAluguel.Value < DateTime.Now)
                        {
                            veh.EngineOn = false;
                            veh.NomeEncarregado = string.Empty;
                            veh.DataExpiracaoAluguel = null;
                            if (veh.Driver is MyPlayer driver)
                                driver.SendMessage(MessageType.Error, "O aluguel do veículo expirou. Use /valugar para alugar novamente por uma hora. Se você sair do veículo, ele será levado para a central.");
                        }
                    }

                    if (veh.EngineOn && veh.Vehicle.Fuel > 0 && veh.TemTanqueCombustivel)
                    {
                        veh.Vehicle.Fuel--;
                        veh.SetSyncedMetaData("combustivel", veh.CombustivelHUD);
                        if (veh.Vehicle.Fuel == 0)
                            veh.EngineOn = false;
                    }
                }
                catch (Exception ex)
                {
                    ex.Source = veh.Vehicle.Id.ToString();
                    Functions.GetException(ex);
                }
            };
            veh.Timer.Start();

            if (player != null)
                await player.GravarLog(LogType.SpawnarVeiculo, Id.ToString(), null);

            return veh;
        }
    }
}