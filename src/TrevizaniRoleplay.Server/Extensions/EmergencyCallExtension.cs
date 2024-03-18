using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Domain.Enums;

namespace TrevizaniRoleplay.Server.Extensions
{
    public static class EmergencyCallExtension
    {
        public static void SendMessage(this EmergencyCall emergencyCall)
        {
            void Send911(FactionType factionType)
            {
                Functions.SendFactionTypeMessage(factionType, $"Central de Emergência | {{#FFFFFF}}Ligação 911", true, true);
                Functions.SendFactionTypeMessage(factionType, $"De: {{#FFFFFF}}{emergencyCall.Number}", true, true);
                Functions.SendFactionTypeMessage(factionType, $"Localização: {{#FFFFFF}}{emergencyCall.Location}", true, true);
                Functions.SendFactionTypeMessage(factionType, $"Mensagem: {{#FFFFFF}}{emergencyCall.Message}", true, true);
            }

            if (emergencyCall.Type == EmergencyCallType.Both || emergencyCall.Type == EmergencyCallType.Police)
                Send911(FactionType.Police);

            if (emergencyCall.Type == EmergencyCallType.Both || emergencyCall.Type == EmergencyCallType.Firefighter)
                Send911(FactionType.Firefighter);
        }
    }
}