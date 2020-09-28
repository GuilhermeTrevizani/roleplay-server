using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
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
        public int BodyHealth { get; set; } = 1000;
        public int EngineHealth { get; set; } = 1000;
        public int Livery { get; set; } = 0;
        public int ValorApreensao { get; set; } = 0;
        public int Combustivel { get; set; } = 0;

        [NotMapped]
        public IVehicle Vehicle { get; set; }

        /// <summary>
        /// Nome do personagem que usou o /fspawn
        /// </summary>
        [NotMapped]
        public string NomeEncarregado { get; set; }

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
        public DateTime DataUltimaVerificacao { get; set; }

        public void Spawnar()
        {
            Vehicle = Alt.CreateVehicle(Modelo, new Position(PosX, PosY, PosZ), new Rotation(RotX, RotY, RotZ));
            Vehicle.ManualEngineControl = true;
            Vehicle.NumberplateText = Placa;
            Vehicle.LockState = VehicleLockState.Locked;
            Vehicle.BodyHealth = (uint)BodyHealth;
            Vehicle.EngineHealth = EngineHealth;
            Vehicle.PrimaryColorRgb = new Rgba((byte)Cor1R, (byte)Cor1G, (byte)Cor1B, 255);
            Vehicle.SecondaryColorRgb = new Rgba((byte)Cor2R, (byte)Cor2G, (byte)Cor2B, 255);
            Vehicle.Livery = (byte)Livery;
            if (!Global.VeiculosSemCombustivel.Contains(Info?.Class ?? string.Empty))
                Vehicle.SetSyncedMetaData("combustivel", CombustivelHUD);
            Global.Veiculos.Add(this);
        }

        public void Despawnar()
        {
            EngineHealth = Vehicle.EngineHealth;
            BodyHealth = (int)Vehicle.BodyHealth;

            using (var context = new DatabaseContext())
            {
                context.Veiculos.Update(this);
                context.SaveChanges();
            }

            Vehicle?.Remove();
            Global.Veiculos.Remove(this);
        }
    }
}