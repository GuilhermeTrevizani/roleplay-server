using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using Newtonsoft.Json;
using Roleplay.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Roleplay.Entities
{
    public class Veiculo
    {
        public int Codigo { get; set; }
        public string Modelo { get; set; } = string.Empty;
        public float PosX { get; set; } = 0;
        public float PosY { get; set; } = 0;
        public float PosZ { get; set; } = 0;
        public float RotX { get; set; } = 0;
        public float RotY { get; set; } = 0;
        public float RotZ { get; set; } = 0;
        public int Cor1R { get; set; } = 255;
        public int Cor1G { get; set; } = 255;
        public int Cor1B { get; set; } = 255;
        public int Cor2R { get; set; } = 255;
        public int Cor2G { get; set; } = 255;
        public int Cor2B { get; set; } = 255;
        public int Personagem { get; set; } = 0;
        public string Placa { get; set; } = string.Empty;
        public int Faccao { get; set; } = 0;
        public int EngineHealth { get; set; } = 1000;
        public int Livery { get; set; } = 1;
        public int ValorApreensao { get; set; } = 0;
        public int Combustivel { get; set; } = 0;
        public string Danos { get; set; } = "{}";
        public bool Estacionou { get; set; } = false;
        public bool VendidoFerroVelho { get; set; } = false;
        public TipoEmprego Emprego { get; set; } = TipoEmprego.Nenhum;

        [NotMapped]
        public IVehicle Vehicle { get; set; }

        /// <summary>
        /// Nome do personagem que usou o /fspawn ou /valugar
        /// </summary>
        [NotMapped]
        public string NomeEncarregado { get; set; } = string.Empty;

        /// <summary>
        /// TRUE: FECHADA
        /// 0 = Front Left Door
        /// 1 = Front Right Door
        /// 2 = Back Left Door
        /// 3 = Back Right Door
        /// 4 = Hood
        /// </summary>
        [NotMapped]
        public List<bool> StatusPortas { get; set; } = new List<bool> { true, true, true, true, true, true };

        [NotMapped]
        public int TanqueCombustivel { get; set; } = 100;

        [NotMapped]
        public string CombustivelHUD
        {
            get
            {
                var combustivel = "COMBUSTÍVEL: ";
                var porcentagem = Convert.ToInt32(Convert.ToDecimal(Combustivel) / Convert.ToDecimal(TanqueCombustivel) * 100);
                if (porcentagem > 50)
                    combustivel += "~g~";
                else if (porcentagem > 20)
                    combustivel += "~o~";
                else
                    combustivel += "~r~";
                return $"{combustivel}{porcentagem}%";
            }
        }

        [NotMapped]
        public VehicleInfo Info { get => Global.VehicleInfos.FirstOrDefault(x => x.Name.ToLower() == Modelo.ToLower()); }

        [NotMapped]
        public DateTime DataUltimaVerificacao { get; set; } = DateTime.Now;

        /// <summary>
        /// Gambiarra necessária enquanto os setters dos healths não funcionam
        /// </summary>
        [NotMapped]
        public bool HealthSetado { get; set; } = false;

        [NotMapped]
        public DateTime? DataExpiracaoAluguel { get; set; } = null;

        [NotMapped]
        public string NumeracaoLivery
        {
            get
            {
                switch (Modelo.ToUpper())
                {
                    case "PULICE":
                        if (Livery == 1)
                            return "489-02";
                        else if (Livery == 2)
                            return "755-02";
                        else if (Livery == 3)
                            return "021-25";
                        else if (Livery == 4)
                            return "811-25";
                        break;
                    case "PULICE2":
                        if (Livery == 1)
                            return "180-02";
                        else if (Livery == 2)
                            return "528-02";
                        else if (Livery == 3)
                            return "405-25";
                        else if (Livery == 4)
                            return "466-25";
                        break;
                    case "PULICE3":
                        if (Livery == 1)
                            return "507-02";
                        else if (Livery == 2)
                            return "062-02";
                        break;
                    case "PSCOUT":
                        if (Livery == 1)
                            return "845-02";
                        else if (Livery == 2)
                            return "105-02";
                        else if (Livery == 3)
                            return "556-25";
                        else if (Livery == 4)
                            return "153-25";
                        break;
                    case "POLICESLICK":
                        if (Livery == 1)
                            return "206-02";
                        else if (Livery == 2)
                            return "378-02";
                        else if (Livery == 3)
                            return "494-25";
                        else if (Livery == 4)
                            return "549-25";
                        break;
                }

                return string.Empty;
            }
        }

        public void Spawnar()
        {
            Vehicle = Alt.CreateVehicle(Modelo, new Position(PosX, PosY, PosZ), new Rotation(RotX, RotY, RotZ));
            Vehicle.ManualEngineControl = true;
            Vehicle.NumberplateText = Placa;
            Vehicle.LockState = VehicleLockState.Locked;
            Vehicle.EngineHealth = EngineHealth;
            Vehicle.PrimaryColorRgb = new Rgba((byte)Cor1R, (byte)Cor1G, (byte)Cor1B, 255);
            Vehicle.SecondaryColorRgb = new Rgba((byte)Cor2R, (byte)Cor2G, (byte)Cor2B, 255);
            Vehicle.Livery = (byte)Livery;
            Vehicle.SetSyncedMetaData("id", Codigo);
            Vehicle.SetSyncedMetaData("placa", Placa);
            Vehicle.SetSyncedMetaData("modelo", Modelo.ToUpper());
            if (!Global.VeiculosSemCombustivel.Contains(Info?.Class ?? string.Empty))
                Vehicle.SetSyncedMetaData("combustivel", CombustivelHUD);

            if (Emprego == TipoEmprego.Nenhum)
            {
                var dano = JsonConvert.DeserializeObject<Dano>(Danos);
                if (dano?.WindowsDamaged?.Count > 0)
                {
                    foreach (var x in dano.Bumpers)
                        Vehicle.SetBumperDamageLevel(x.VehicleBumper, x.VehicleBumperDamage);

                    foreach (var x in dano.Parts)
                    {
                        Vehicle.SetPartDamageLevel(x.VehiclePart, x.VehiclePartDamage);
                        Vehicle.SetPartBulletHoles(x.VehiclePart, x.BulletHoles);
                    }

                    for (byte i = 0; i <= 10; i++)
                    {
                        Vehicle.SetLightDamaged(i, dano.LightsDamaged[i]);
                        Vehicle.SetSpecialLightDamaged(i, dano.SpecialLightsDamaged[i]);
                        Vehicle.SetWindowDamaged(i, dano.WindowsDamaged[i]);
                        var armoredWindow = dano.ArmoredWindows[i];
                        Vehicle.SetArmoredWindowHealth(i, armoredWindow.Health);
                        Vehicle.SetArmoredWindowShootCount(i, armoredWindow.ShootCount);
                    }

                    foreach (var x in dano.Wheels)
                    {
                        Vehicle.SetWheelHealth(x.Id, x.Health);
                        Vehicle.SetWheelDetached(x.Id, x.Detached);
                        Vehicle.SetWheelBurst(x.Id, x.Burst);
                        Vehicle.SetWheelHasTire(x.Id, x.HasTire);
                    }
                }
            }

            Global.Veiculos.Add(this);
        }

        public void Despawnar()
        {
            if (Emprego == TipoEmprego.Nenhum)
            {
                EngineHealth = Vehicle.EngineHealth;

                var dano = new Dano();

                foreach (var x in Enum.GetValues(typeof(VehicleBumper)).Cast<VehicleBumper>())
                    dano.Bumpers.Add(new Dano.Bumper(x, Vehicle.GetBumperDamageLevel(x)));

                foreach (var x in Enum.GetValues(typeof(VehiclePart)).Cast<VehiclePart>())
                    dano.Parts.Add(new Dano.Part(x, Vehicle.GetPartDamageLevel(x), Vehicle.GetPartBulletHoles(x)));

                for (byte i = 0; i <= 10; i++)
                {
                    dano.LightsDamaged.Add(Vehicle.IsLightDamaged(i));
                    dano.SpecialLightsDamaged.Add(Vehicle.IsSpecialLightDamaged(i));
                    dano.WindowsDamaged.Add(Vehicle.IsWindowDamaged(i));
                    dano.ArmoredWindows.Add(new Dano.ArmoredWindow(i, Vehicle.GetArmoredWindowHealth(i), Vehicle.GetArmoredWindowShootCount(i)));
                }

                for (byte i = 0; i < Vehicle.WheelsCount; i++)
                    dano.Wheels.Add(new Dano.Wheel(i, Vehicle.GetWheelHealth(i), Vehicle.IsWheelDetached(i), Vehicle.IsWheelBurst(i), Vehicle.DoesWheelHasTire(i)));

                Danos = JsonConvert.SerializeObject(dano);
            }

            using (var context = new DatabaseContext())
            {
                context.Veiculos.Update(this);
                context.SaveChanges();
            }

            Vehicle?.Remove();
            Global.Veiculos.Remove(this);
        }

        public void Reparar()
        {
            if (Vehicle.IsDestroyed)
            {
                var pos = Vehicle.Position;
                var rot = Vehicle.Rotation;
                Vehicle.Remove();
                Global.Veiculos.Remove(this);
                EngineHealth = 1000;
                var posOld = new Position(PosX, PosY, PosZ);
                var rotOld = new Position(RotX, RotY, RotZ);
                PosX = pos.X;
                PosY = pos.Y;
                PosZ = pos.Z;
                RotX = rot.Roll;
                RotY = rot.Pitch;
                RotZ = rot.Yaw;
                Spawnar();
                PosX = posOld.X;
                PosY = posOld.Y;
                PosZ = posOld.Z;
                RotX = rotOld.X;
                RotY = rotOld.Y;
                RotZ = rotOld.Z;
            }

            Alt.EmitAllClients("vehicle:setVehicleFixed", Vehicle);

            for (byte i = 0; i <= 10; i++)
            {
                Vehicle.SetWindowDamaged(i, false);
                Vehicle.SetLightDamaged(i, false);
                Vehicle.SetSpecialLightDamaged(i, false);
            }

            for (byte i = 0; i < Vehicle.WheelsCount; i++)
            {
                Vehicle.SetWheelHealth(i, 1000);
                Vehicle.SetWheelDetached(i, false);
                Vehicle.SetWheelBurst(i, false);
                Vehicle.SetWheelHasTire(i, true);
            }
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
    }
}