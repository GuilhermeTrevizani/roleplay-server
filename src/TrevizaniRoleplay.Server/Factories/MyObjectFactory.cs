using AltV.Net;
using AltV.Net.Elements.Entities;

namespace TrevizaniRoleplay.Server.Factories
{
    public class MyObjectFactory : IEntityFactory<IObject>
    {
        public IObject Create(ICore core, nint baseObjectPointer, uint id)
        {
            return new MyObject(core, baseObjectPointer, id);
        }
    }
}