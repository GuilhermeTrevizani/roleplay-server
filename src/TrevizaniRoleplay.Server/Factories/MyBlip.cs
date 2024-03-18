using AltV.Net;
using AltV.Net.Async.Elements.Entities;

namespace TrevizaniRoleplay.Server.Factories
{
    public class MyBlip(ICore core, nint nativePointer, uint id) : AsyncBlip(core, nativePointer, id)
    {
        public Guid? SpotId { get; set; }
        public Guid? BlipId { get; set; }
        public Guid? CompanyId { get; set; }
    }
}