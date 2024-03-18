using AltV.Net;
using AltV.Net.Async.Elements.Entities;

namespace TrevizaniRoleplay.Server.Factories
{
    public class MyObject(ICore core, nint nativePointer, uint id) : AsyncObject(core, nativePointer, id)
    {
        public Guid? CharacterId { get; set; }
        public Guid? FactionId { get; set; }
        public Guid? PropertyFurnitureId { get; set; }
        public Guid? ItemId { get; set; }
    }
}