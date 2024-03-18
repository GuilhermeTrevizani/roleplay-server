using AltV.Net;
using AltV.Net.Elements.Entities;

namespace TrevizaniRoleplay.Server.Factories
{
    public class MyColShapeFactory : IBaseObjectFactory<IColShape>
    {
        public IColShape Create(ICore core, nint baseObjectPointer, uint id)
        {
            return new MyColShape(core, baseObjectPointer, id);
        }
    }
}