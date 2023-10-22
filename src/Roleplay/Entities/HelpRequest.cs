using Roleplay.Factories;
using Roleplay.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Roleplay.Entities
{
    public class HelpRequest
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public string Message { get; set; }

        public int UserId { get; set; }

        public DateTime? AnswerDate { get; set; }

        public int StaffUserId { get; set; }

        [NotMapped]
        public int CharacterSessionId { get; set; }

        [NotMapped]
        public string CharacterName { get; set; }

        [NotMapped]
        public string UserName { get; set; }

        public async Task<MyPlayer> Check()
        {
            var p = Global.Players.FirstOrDefault(x => x.SessionId == CharacterSessionId && x.Character.Name == CharacterName);
            if (p != null)
                return p;

            AnswerDate = DateTime.Now;

            await using var context = new DatabaseContext();
            context.HelpRequests.Update(this);
            await context.SaveChangesAsync();

            Global.HelpRequests.Remove(this);

            var html = Functions.GetSOSJSON();
            foreach (var target in Global.Players.Where(x => x.User.Staff != UserStaff.None))
                target.Emit("ACPUpdateSOS", html);

            return null;
        }
    }
}