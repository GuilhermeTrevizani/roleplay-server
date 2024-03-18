using AltV.Net;
using AltV.Net.Elements.Entities;

namespace TrevizaniRoleplay.Server.Factories
{
    public class MyVehicleFactory : IEntityFactory<IVehicle>
    {
        public IVehicle Create(ICore server, nint playerPointer, uint id)
        {
            return new MyVehicle(server, playerPointer, id);
        }
    }
}