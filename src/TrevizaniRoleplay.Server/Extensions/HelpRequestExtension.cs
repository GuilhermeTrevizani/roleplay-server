using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;
using TrevizaniRoleplay.Server.Factories;

namespace TrevizaniRoleplay.Server.Extensions
{
    public static class HelpRequestExtension
    {
        public static async Task<MyPlayer?> Check(this HelpRequest helpRequest)
        {
            var p = Global.SpawnedPlayers.FirstOrDefault(x => x.SessionId == helpRequest.CharacterSessionId && x.Character.Name == helpRequest.CharacterName);
            if (p != null)
                return p;

            helpRequest.Answer(null);

            await using var context = new DatabaseContext();
            context.HelpRequests.Update(helpRequest);
            await context.SaveChangesAsync();

            Global.HelpRequests.Remove(helpRequest);

            var html = Functions.GetSOSJSON();
            foreach (var target in Global.SpawnedPlayers.Where(x => x.User.Staff != UserStaff.None))
                target.Emit("ACPUpdateSOS", html);

            return null;
        }
    }
}