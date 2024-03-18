using AltV.Net;
using System.Numerics;
using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Server.Factories;

namespace TrevizaniRoleplay.Server.Extensions
{
    public static class DoorExtension
    {
        public static void Setup(this Door door, MyPlayer player)
        {
            player.Emit("DoorControl", door.Hash, new Vector3(door.PosX, door.PosY, door.PosZ), door.Locked);
        }

        public static void SetupAllClients(this Door door)
        {
            Alt.EmitAllClients("DoorControl", door.Hash, new Vector3(door.PosX, door.PosY, door.PosZ), door.Locked);
        }
    }
}