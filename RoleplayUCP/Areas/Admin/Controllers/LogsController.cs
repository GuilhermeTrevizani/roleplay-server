using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Roleplay;
using RoleplayUCP.Areas.Admin.Models;
using System.Linq;

namespace RoleplayUCP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [Authorize(Roles = "LeadAdministrator,Manager")]
    public class LogsController : Controller
    {
        [Route("Index")]
        public IActionResult Index() => View(new LogsViewModel());

        [Route("List")]
        [HttpPost]
        public IActionResult Pesquisar(LogsViewModel viewModel)
        {
            using var context = new DatabaseContext();
            var sql = $@"SELECT l.*, 
                    po.Nome NomePersonagemOrigem, 
                    pd.Nome NomePersonagemDestino,
                    uo.Nome UsuarioOrigem,
                    ud.Nome UsuarioDestino
                    FROM Logs l
                    LEFT JOIN Personagens po ON l.PersonagemOrigem = po.Codigo
                    LEFT JOIN Personagens pd ON l.PersonagemDestino = pd.Codigo
                    LEFT JOIN Usuarios uo ON po.Usuario = uo.Codigo
                    LEFT JOIN Usuarios ud ON pd.Usuario = ud.Codigo
                    WHERE 1=1";

            if (viewModel.Tipo != 0)
                sql += $" AND l.Tipo = {(int)viewModel.Tipo}";

            if (viewModel.DataInicial.HasValue)
                sql += $" AND l.Data >= STR_TO_DATE('{viewModel.DataInicial}', '%d/%m/%Y %H:%i:%s')";

            if (viewModel.DataFinal.HasValue)
                sql += $" AND l.Data <= STR_TO_DATE('{viewModel.DataFinal}', '%d/%m/%Y %H:%i:%s')";

            if (!string.IsNullOrWhiteSpace(viewModel.Descricao))
                sql += $@" AND l.Descricao LIKE '%{viewModel.Descricao.Replace("'", "''")}%'";

            if (!string.IsNullOrWhiteSpace(viewModel.PersonagemOrigem))
            {
                int.TryParse(viewModel.PersonagemOrigem, out int personagemOrigem);
                sql += $@" AND (po.Codigo = {personagemOrigem} OR po.Nome = '{viewModel.PersonagemOrigem.Replace("'", "''")}')";
            }

            if (!string.IsNullOrWhiteSpace(viewModel.PersonagemDestino))
            {
                int.TryParse(viewModel.PersonagemDestino, out int personagemDestino);
                sql += $@" AND (pd.Codigo = {personagemDestino} OR pd.Nome = '{viewModel.PersonagemDestino.Replace("'", "''")}')";
            }

            var logs = context.LogsInfos.FromSqlRaw(sql).ToList();

            return PartialView("_List", logs);
        }
    }
}