using Roleplay.Models;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Roleplay.Entities
{
    public class SOS
    {
        public int Codigo { get; set; }
        public DateTime Data { get; set; } = DateTime.Now;
        public string Mensagem { get; set; } = string.Empty;
        public int Usuario { get; set; } = 0;
        public DateTime? DataResposta { get; set; } = null;
        public int UsuarioStaff { get; set; } = 0;
        public TipoRespostaSOS TipoResposta { get; set; } = TipoRespostaSOS.Aguardando;

        [NotMapped]
        public int IDPersonagem { get; set; }

        [NotMapped]
        public string NomePersonagem { get; set; }

        [NotMapped]
        public string NomeUsuario { get; set; }

        public Personagem Verificar(int usuario)
        {
            var p = Global.PersonagensOnline.FirstOrDefault(x => x.Codigo == IDPersonagem);
            if (p != null)
                return p;

            DataResposta = DateTime.Now;
            UsuarioStaff = usuario;
            TipoResposta = TipoRespostaSOS.SemResposta;

            using (var context = new DatabaseContext())
            {
                context.SOSs.Update(this);
                context.SaveChanges();
            }

            Global.SOSs.Remove(this);

            return null;
        }
    }
}