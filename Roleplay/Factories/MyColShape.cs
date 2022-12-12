using AltV.Net;
using AltV.Net.Async.Elements.Entities;
using System;

namespace Roleplay.Factories
{
    public class MyColShape : AsyncColShape
    {
        public MyColShape(ICore server, IntPtr nativePointer) : base(server, nativePointer)
        {
        }

        public string Description { get; set; }

        public int? PoliceOfficerCharacterId { get; set; }

        public int? MaxSpeed { get; set; }

        public int? InfoId { get; set; }
    }
}