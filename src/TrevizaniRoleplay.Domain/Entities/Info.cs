namespace TrevizaniRoleplay.Domain.Entities
{
    public class Info : BaseEntity
    {
        public DateTime Date { get; private set; } = DateTime.Now;
        public float PosX { get; private set; }
        public float PosY { get; private set; }
        public float PosZ { get; private set; }
        public int Dimension { get; private set; }
        public DateTime ExpirationDate { get; private set; }
        public Guid UserId { get; private set; }
        public string Message { get; private set; } = string.Empty;

        public User? User { get; set; }
    }
}