using AltV.Net;
using AltV.Net.Elements.Entities;

namespace TrevizaniRoleplay.Server.Factories
{
    public class MyBlipFactory : IBaseObjectFactory<IBlip>
    {
        public IBlip Create(ICore core, nint baseObjectPointer, uint id)
        {
            return new MyBlip(core, baseObjectPointer, id);
        }
    }
}