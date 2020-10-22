using Roleplay.Models;
using System;

namespace RoleplayUCP.Areas.Admin.Models
{
    public class LogsViewModel
    {
        public LogsViewModel()
        {
            var data = DateTime.Now;
            DataInicial = new DateTime(data.Year, data.Month, data.Day, 0, 0, 0);
            DataFinal = data;
        }

        public DateTime? DataInicial { get; set; }
        public DateTime? DataFinal { get; set; }
        public TipoLog Tipo { get; set; }
        public string PersonagemOrigem { get; set; }
        public string PersonagemDestino { get; set; }
        public string Descricao { get; set; }
    }
}