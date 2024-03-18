namespace TrevizaniRoleplay.Server.Models
{
    public class CellphoneItem
    {
        public bool FlightMode { get; set; }
        public List<CellphoneItemContact> Contacts { get; set; } = [];
        public List<CellphoneItemCall> Calls { get; set; } = [];
        public List<CellphoneItemMessage> Messages { get; set; } = [];
    }

    public class CellphoneItemContact(uint number, string name)
    {
        public uint Number { get; set; } = number;
        public string Name { get; set; } = name;
    }

    public class CellphoneItemCall
    {
        public uint Number { get; set; }
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime EndDate { get; set; }
        public CellphoneCallType Type { get; set; } = CellphoneCallType.Lost;
        public bool Origin { get; set; } = true;
    }

    public class CellphoneItemMessage
    {
        public uint Number { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string? Message { get; set; }
        public CellphoneMessageType Type { get; set; }
        public float LocationX { get; set; }
        public float LocationY { get; set; }
    }
}