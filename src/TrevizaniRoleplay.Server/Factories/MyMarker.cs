using AltV.Net;
using AltV.Net.Async.Elements.Entities;

namespace TrevizaniRoleplay.Server.Factories
{
    public class MyMarker(ICore core, nint nativePointer, uint id) : AsyncMarker(core, nativePointer, id)
    {
        public Guid? TruckerLocationId { get; set; }
        public Guid? SpotId { get; set; }
        public Guid? PropertyId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? CrackDenId { get; set; }
        public Guid? InfoId { get; set; }
        public Guid? FactionStorageId { get; set; }
    }
}