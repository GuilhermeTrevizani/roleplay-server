using AltV.Net;
using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using AltV.Net.Enums;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int Cor1R { get; set; } = 0;
        public int Cor1G { get; set; } = 0;
        public int Cor1B { get; set; } = 0;
        public int Cor2R { get; set; } = 0;
        public int Cor2G { get; set; } = 0;
        public int Cor2B { get; set; } = 0;
        public int Personagem { get; set; } = 0;
        public string Placa { get; set; } = string.Empty;
        public int Faccao { get; set; } = 0;
        public int BodyHealth { get; set; } = 1000;
        public int EngineHealth { get; set; } = 1000;
        public int Livery { get; set; } = 0;
        public int ValorApreensao { get; set; } = 0;

        [NotMapped]
        public IVehicle Vehicle { get; set; }

        /// <summary>
        /// Nome do personagem que usou o /fspawn
        /// </summary>
        [NotMapped]
        public string NomeEncarregado { get; set; }

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