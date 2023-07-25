using AltV.Net;
using Roleplay.Factories;
using System.Numerics;

namespace Roleplay.Entities
{
    public class Door
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public long Hash { get; set; }

        public float PosX { get; set; }

        public float PosY { get; set; }

        public float PosZ { get; set; }

        public int? FactionId { get; set; }

        public bool Locked { get; set; } = true;

        public int? CompanyId { get; set; }

        public void Setup(MyPlayer player)
        {
            player.Emit("DoorControl", Hash, new Vector3(PosX, PosY, PosZ), Locked);
        }

        public void SetupAllClients()
        {
            Alt.EmitAllClients("DoorControl", Hash, new Vector3(PosX, PosY, PosZ), Locked);
        }
    }
}