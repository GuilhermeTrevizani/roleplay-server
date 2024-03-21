using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Enums;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Factories;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Extensions
{
    public static class VehicleExtension
    {
        public static int GetMaxFuel(this Vehicle vehicle)
        {
            return 100;
        }

        public static string GetLiveryName(this Vehicle vehicle)
        {
            if (vehicle.Model.ToUpper() == VehicleModelMods.NSCOUTLSPD.ToString().ToUpper())
            {
                if (vehicle.Livery == 1)
                    return "366-18";
                else if (vehicle.Livery == 2)
                    return "301-18";
                else if (vehicle.Livery == 3)
                    return "974-18";
            }
            else if (vehicle.Model.ToUpper() == VehicleModel.Police3.ToString().ToUpper())
            {
                if (vehicle.Livery == 1)
                    return "671-18";
                else if (vehicle.Livery == 2)
                    return "033-18";
            }
            else if (vehicle.Model.ToUpper() == VehicleModelMods.PSCOUT.ToString().ToUpper())
            {
                if (vehicle.Livery == 1)
                    return "015-18";
                else if (vehicle.Livery == 2)
                    return "540-18";
                else if (vehicle.Livery == 3)
                    return "560-18";
            }
            else if (vehicle.Model.ToUpper() == VehicleModelMods.VVPI.ToString().ToUpper())
            {
                if (vehicle.Livery == 1)
                    return "450-18";
                else if (vehicle.Livery == 2)
                    return "525-18";
            }
            else if (vehicle.Model.ToUpper() == VehicleModelMods.VVPI2.ToString().ToUpper())
            {
                if (vehicle.Livery == 1)
                    return "989-18";
                else if (vehicle.Livery == 2)
                    return "741-18";
            }
            return string.Empty;
        }

        public static async Task<MyVehicle> Spawnar(this Vehicle vehicle, MyPlayer? player)
        {
            //MyVehicle veh;
            //if (Enum.TryParse(vehicle.Model, true, out VehicleModel v1))
            //    veh = (MyVehicle)Alt.CreateVehicle(v1, new Position(vehicle.PosX, vehicle.PosY, vehicle.PosZ), new Rotation(vehicle.RotR, vehicle.RotP, vehicle.RotY));
            //else
            var veh = (MyVehicle)Alt.CreateVehicle(vehicle.Model, new Position(vehicle.PosX, vehicle.PosY, vehicle.PosZ), new Rotation(vehicle.RotR, vehicle.RotP, vehicle.RotY));

            veh.VehicleDB = vehicle;
            veh.ManualEngineControl = true;
            veh.NumberplateText = veh.VehicleDB.Plate;
            veh.LockState = VehicleLockState.Locked;
            veh.Livery = veh.VehicleDB.Livery;
            veh.SetStreamSyncedMetaData(Constants.VEHICLE_META_DATA_ID, veh.VehicleDB.Id);
            veh.SetStreamSyncedMetaData(Constants.VEHICLE_META_DATA_PLATE, veh.VehicleDB.Plate);
            veh.SetStreamSyncedMetaData(Constants.VEHICLE_META_DATA_MODEL, veh.VehicleDB.Model.ToUpper());

            if (veh.HasFuelTank)
                veh.SetStreamSyncedMetaData(Constants.VEHICLE_META_DATA_FUEL, veh.FuelHUD);

            veh.SetDefaultMods();

            if (vehicle.Job == CharacterJob.None)
            {
                veh.Damages = Functions.Deserialize<List<VehicleDamage>>(vehicle.DamagesJSON);

                if (player != null)
                {
                    veh.EngineHealth = veh.VehicleDB.EngineHealth;
                    veh.BodyHealth = veh.VehicleDB.BodyHealth;
                    veh.BodyAdditionalHealth = veh.VehicleDB.BodyAdditionalHealth;
                    veh.PetrolTankHealth = veh.VehicleDB.PetrolTankHealth;

                    var dano = Functions.Deserialize<VehicleDamageInfo>(veh.VehicleDB.StructureDamagesJSON);
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
                            veh.SetSpecialLightDamaged(i, dano.SpecialLightsDamaged[i]);
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

            veh.Timer = new System.Timers.Timer(TimeSpan.FromMinutes(1));
            veh.Timer.Elapsed += (o, e) =>
            {
                try
                {
                    Alt.Log($"Vehicle Timer {veh.VehicleDB.Id}");
                    if (veh.RentExpirationDate.HasValue)
                    {
                        if (veh.RentExpirationDate.Value < DateTime.Now)
                        {
                            veh.EngineOn = false;
                            veh.NameInCharge = string.Empty;
                            veh.RentExpirationDate = null;
                            if (veh.Driver is MyPlayer driver)
                                driver.SendMessage(MessageType.Error, "O aluguel do veículo expirou. Use /valugar para alugar novamente por uma hora. Se você sair do veículo, ele será levado para a central.");
                        }
                    }

                    if (veh.EngineOn && veh.VehicleDB.Fuel > 0 && veh.HasFuelTank)
                    {
                        veh.VehicleDB.SetFuel(veh.VehicleDB.Fuel - 1);
                        veh.SetStreamSyncedMetaData(Constants.VEHICLE_META_DATA_FUEL, veh.FuelHUD);
                        if (veh.VehicleDB.Fuel == 0)
                            veh.EngineOn = false;
                    }
                }
                catch (Exception ex)
                {
                    ex.Source = veh.VehicleDB.Id.ToString();
                    Functions.GetException(ex);
                }
            };
            veh.Timer.Start();

            if (player != null)
                await player.GravarLog(LogType.SpawnVehicle, vehicle.Id.ToString(), null);

            return veh;
        }
    }
}