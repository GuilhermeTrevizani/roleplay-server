using Roleplay.Models;
using System;

namespace Roleplay.Entities
{
    public class EmergencyCall
    {
        public int Id { get; set; }

        public DateTime Date { get; set; } = DateTime.Now;

        public EmergencyCallType Type { get; set; }

        public uint Number { get; set; }

        public float PosX { get; set; }

        public float PosY { get; set; }

        public string Message { get; set; }

        public string Location { get; set; }

        public void SendMessage()
        {
            void Send911(FactionType tipoFaccao)
            {
                Functions.SendFactionTypeMessage(tipoFaccao, $"Central de Emergência | Ligação 911 {{#FFFFFF}}#{Id}", true, true);
                Functions.SendFactionTypeMessage(tipoFaccao, $"De: {{#FFFFFF}}{Number}", true, true);
                Functions.SendFactionTypeMessage(tipoFaccao, $"Localização: {{#FFFFFF}}{Location}", true, true);
                Functions.SendFactionTypeMessage(tipoFaccao, $"Mensagem: {{#FFFFFF}}{Message}", true, true);
            }

            if (Type == EmergencyCallType.Both || Type == EmergencyCallType.Police)
                Send911(FactionType.Police);

            if (Type == EmergencyCallType.Both || Type == EmergencyCallType.Medic)
                Send911(FactionType.Firefighter);
        }
    }
}