using AltV.Net;
using AltV.Net.Async.Elements.Entities;

namespace Roleplay.Factories
{
    public class MyObject : AsyncNetworkObject
    {
        public MyObject(ICore core, nint nativePointer, uint id) : base(core, nativePointer, id)
        {
        }

        public int? CharacterId { get; set; }

        public int? FactionId { get; set; }
    }
}