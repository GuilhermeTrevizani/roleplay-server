﻿using TrevizaniRoleplay.Domain.Enums;

namespace TrevizaniRoleplay.Domain.Entities
{
    public class EmergencyCall : BaseEntity
    {
        public DateTime Date { get; private set; } = DateTime.Now;
        public EmergencyCallType Type { get; private set; }
        public uint Number { get; private set; }
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public string Location { get; private set; } = string.Empty;

        public void Create(EmergencyCallType type, uint number, float posX, float posY, string message, string location)
        {
            Type = type;
            Number = number;
            PosX = posX;
            PosY = posY;
            Message = message;
            Location = location;
        }

        public void SetLocation(string location)
        {
            Location = location;
        }
    }
}