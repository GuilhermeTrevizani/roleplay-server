using Microsoft.AspNetCore.Mvc;
using Roleplay;
using Roleplay.Entities;
using Roleplay.Models;
using System.Linq;
using System.Security.Claims;

namespace RoleplayUCP.Controllers
{
    public class BaseController : Controller
    {
        internal int CodigoUsuario
        {
            get
            {
                int.TryParse(((ClaimsIdentity)User.Identity).Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value, out int codigo);
                return codigo;
            }
        }

        internal void GravarLog(TipoLog tipo, string descricao)
        {
            using var context = new DatabaseContext();
            context.Logs.Add(new Log()
            {
                Tipo = tipo,
                Descricao = descricao,
                PersonagemOrigem = 0,
                IPOrigem = string.Empty,
                SocialClubOrigem = 0,
                PersonagemDestino = 0,
                IPDestino = string.Empty,
                SocialClubDestino = 0,
                HardwareIdHashOrigem = 0,
                HardwareIdHashDestino = 0,
                HardwareIdExHashOrigem = 0,
                HardwareIdExHashDestino = 0,
            });
            context.SaveChanges();
        }
    }
}