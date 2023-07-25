using AltV.Net;
using Roleplay.Factories;
using System.Linq;
using System.Numerics;

namespace Roleplay.Models
{
    public class AudioSpot
    {
        public int Id { get; } = Global.AudioSpots.Select(x => x.Id).DefaultIfEmpty(0).Max() + 1;

        public Vector3 Position { get; set; }

        public string Source { get; set; }

        public uint MaxRange { get; set; }

        public int Dimension { get; set; }

        public float Volume { get; set; } = 0.1f;

        public uint? VehicleId { get; set; }

        public bool Loop { get; set; }

        public bool FixVolume { get; set; }

        public void Setup(MyPlayer player)
        {
            player.Emit("Audio:Setup", Id, Position, Source, MaxRange, Dimension, Volume, VehicleId, Loop, FixVolume);
        }

        public void SetupAllClients()
        {
            Alt.EmitAllClients("Audio:Setup", Id, Position, Source, MaxRange, Dimension, Volume, VehicleId, Loop, FixVolume);

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