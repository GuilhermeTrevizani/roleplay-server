using AltV.Net;
using AltV.Net.Async.Elements.Entities;

namespace Roleplay.Factories
{
    public class MyColShape : AsyncColShape
    {
        public MyColShape(ICore server, nint nativePointer, uint id) : base(server, nativePointer, id)
        {
        }

        public string Description { get; set; }

        public int? PoliceOfficerCharacterId { get; set; }

        public int? MaxSpeed { get; set; }

        public int? InfoId { get; set; }
    }
}