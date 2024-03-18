using AltV.Net;
using System.Numerics;
using TrevizaniRoleplay.Server.Factories;

namespace TrevizaniRoleplay.Server.Models
{
    public class AudioSpot
    {
        public int Id { get; } = Global.AudioSpots.Select(x => x.Id).DefaultIfEmpty(0).Max() + 1;
        public Vector3 Position { get; set; }
        public string Source { get; set; } = string.Empty;
        public int Dimension { get; set; }
        public float Volume { get; set; } = 1;
        public uint? VehicleId { get; set; }
        public bool Loop { get; set; }
        public Guid? PropertyId { get; set; }
        public Guid? ItemId { get; set; }

        public void Setup(MyPlayer player)
        {
            player.Emit("Audio:Setup", Id, Position, Source, Dimension, Volume, VehicleId, Loop);
        }

        public void SetupAllClients()
        {
            Alt.EmitAllClients("Audio:Setup", Id, Position, Source, Dimension, Volume, VehicleId, Loop);

            if (!Global.AudioSpots.Contains(this))
                Global.AudioSpots.Add(this);
        }

        public void Remove(MyPlayer player)
        {
            player.Emit("Audio:Remove", Id);
        }

        public void RemoveAllClients()
        {
            Alt.EmitAllClients("Audio:Remove", Id);
            Global.AudioSpots.Remove(this);
        }
    }
}