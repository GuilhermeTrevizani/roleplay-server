using AltV.Net;
using AltV.Net.Elements.Entities;
using System;

namespace Roleplay.Factories
{
    public class MyColShapeFactory : IBaseObjectFactory<IColShape>
    {
        public IColShape Create(ICore server, IntPtr playerPointer)
        {
            return new MyColShape(server, playerPointer);
        }
    }
}