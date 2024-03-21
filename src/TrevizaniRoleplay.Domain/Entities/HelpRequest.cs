using System.ComponentModel.DataAnnotations.Schema;

namespace TrevizaniRoleplay.Domain.Entities
{
    public class HelpRequest : BaseEntity
    {
        public DateTime Date { get; private set; } = DateTime.Now;
        public string Message { get; private set; } = string.Empty;
        public Guid UserId { get; private set; }
        public DateTime? AnswerDate { get; private set; }
        public Guid? StaffUserId { get; private set; }

        public User? User { get; private set; }
        public User? StaffUser { get; private set; }

        [NotMapped]
        public int CharacterSessionId { get; private set; }

        [NotMapped]
        public string CharacterName { get; private set; } = string.Empty;

        [NotMapped]
        public string UserName { get; private set; } = string.Empty;

        public void Create(string message, Guid userId, int characterSessionId, string characterName, string userName)
        {
            Message = message;
            UserId = userId;
            CharacterSessionId = characterSessionId;
            CharacterName = characterName;
            UserName = userName;
        }

        public void Answer(Guid? staffUserId)
        {
            AnswerDate = DateTime.Now;
            StaffUserId = staffUserId;
        }
    }
}