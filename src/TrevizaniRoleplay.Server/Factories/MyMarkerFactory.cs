using AltV.Net;
using AltV.Net.Elements.Entities;

namespace TrevizaniRoleplay.Server.Factories
{
    public class MyMarkerFactory : IBaseObjectFactory<IMarker>
    {
        public IMarker Create(ICore core, nint baseObjectPointer, uint id)
        {
            return new MyMarker(core, baseObjectPointer, id);
        }
    }
}