using AltV.Net;
using AltV.Net.Elements.Entities;
using System;

namespace Roleplay.Factories
{
    public class MyVehicleFactory : IEntityFactory<IVehicle>
    {
        public IVehicle Create(ICore server, IntPtr playerPointer, ushort id)
        {
            return new MyVehicle(server, playerPointer, id);
        }
    }
}