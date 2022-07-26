using AltV.Net;
using AltV.Net.Elements.Entities;
using System;

namespace Roleplay.Factories
{
    public class MyPlayerFactory : IEntityFactory<IPlayer>
    {
        public IPlayer Create(ICore server, IntPtr playerPointer, ushort id)
        {
            return new MyPlayer(server, playerPointer, id);
        }
    }
}