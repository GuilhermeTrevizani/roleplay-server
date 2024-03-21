﻿namespace TrevizaniRoleplay.Domain.Entities
{
    public class VehicleItem : BaseItem
    {
        public Guid VehicleId { get; private set; }
        public byte Slot { get; private set; }

        public Vehicle? Vehicle { get; private set; }

        public void SetVehicleId(Guid vehicleId)
        {
            VehicleId = vehicleId;
        }

        public void SetSlot(byte slot)
        {
            Slot = slot;
        }
    }
}