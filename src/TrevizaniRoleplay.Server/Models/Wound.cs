using AltV.Net.Data;
using System.Numerics;

namespace TrevizaniRoleplay.Server.Models
{
    public class Wound
    {
        public DateTime Data { get; set; } = DateTime.Now;
        public ushort Dano { get; set; }
        public uint Arma { get; set; }
        public string? Attacker { get; set; }
        public BodyPart BodyPart { get; set; }
        public float Distancia { get; set; }
        public Vector3 ShotOffset { get; set; }
    }
}