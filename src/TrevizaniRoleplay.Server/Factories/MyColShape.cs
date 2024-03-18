using AltV.Net;
using AltV.Net.Async.Elements.Entities;

namespace TrevizaniRoleplay.Server.Factories
{
    public class MyColShape(ICore server, nint nativePointer, uint id) : AsyncColShape(server, nativePointer, id)
    {
        public string? Description { get; set; }
        public Guid? PoliceOfficerCharacterId { get; set; }
        public int? MaxSpeed { get; set; }
        public Guid? InfoId { get; set; }
        public Guid? TruckerLocationId { get; set; }
        public Guid? SpotId { get; set; }
        public Guid? PropertyId { get; set; }
        public Guid? CompanyId { get; set; }
        public Guid? CrackDenId { get; set; }
        public Guid? FactionStorageId { get; set; }
    }
}