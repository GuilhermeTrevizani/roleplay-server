namespace Roleplay.Models
{
    public class Invite
    {
        public InviteType Type { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public string[] Value { get; set; }

        public int SenderCharacterId { get; set; }
    }
}