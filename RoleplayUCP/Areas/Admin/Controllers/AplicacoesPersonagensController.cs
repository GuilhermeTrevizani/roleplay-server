using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Roleplay;
using Roleplay.Entities;
using RoleplayUCP.Areas.Admin.Models;
using RoleplayUCP.Controllers;
using System;
using System.Linq;

namespace RoleplayUCP.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    [Authorize(Roles = "Moderator,GameAdministrator,LeadAdministrator,Manager")]
    public class AplicacoesPersonagensController : BaseController
    {
        [Route("Index")]
        public IActionResult Index()
        {
            using var context = new DatabaseContext();
            var app = context.Personagens.FirstOrDefault(x => x.UsuarioStaffAvaliando == CodigoUsuario && x.UsuarioStaffAvaliador == 0);
            if (app == null)
            {
                app = context.Personagens.FromSqlRaw(@"SELECT per.* FROM Personagens per
                INNER JOIN Usuarios usu ON per.Usuario = usu.Codigo
                WHERE per.UsuarioStaffAvaliador = 0 AND per.UsuarioStaffAvaliando = 0
                ORDER BY CASE WHEN usu.VIP = 1 THEN 0 ELSE usu.VIP END DESC, usu.ContentCreator DESC").FirstOrDefault();

                if (app == null)
                    return View(new AplicacoesPersonagensViewModel());
            }

            var userApp = context.Usuarios.FirstOrDefault(x => x.Codigo == app.Usuario);

            app.PersonalizacaoDados = JsonConvert.DeserializeObject<Personagem.Personalizacao>(app.InformacoesPersonalizacao);
            app.UsuarioStaffAvaliando = CodigoUsuario;
            context.Update(app);
            context.SaveChanges();

            return View(new AplicacoesPersonagensViewModel
            {
                Codigo = app.Codigo,
                Nome = app.Nome,
                Historia = app.Historia,
                Sexo = app.PersonalizacaoDados.sex == 1 ? "Homem" : "Mulher",
                Nascimento = $"{app.DataNascimento.ToShortDateString()} ({Math.Truncate((DateTime.Now.Date - app.DataNascimento).TotalDays / 365):N0} anos)",
                OOC = $"{userApp?.Nome ?? string.Empty} [{userApp.Codigo}]",
                DataRegistro = app.DataRegistro.ToString(),
            });
        }

        [Route("Aceitar/{codigo?}")]
        [HttpPost]
        public IActionResult Aceitar(int codigo)
        {
            using var context = new DatabaseContext();
            var app = context.Personagens.FirstOrDefault(x => x.Codigo == codigo && x.UsuarioStaffAvaliador == 0 && x.UsuarioStaffAvaliando == CodigoUsuario);
            if (app == null)
                return Json($"Não localizei nenhuma aplicação com o código {codigo}.");

            app.UsuarioStaffAvaliador = CodigoUsuario;
            context.Update(app);
            context.SaveChanges();

            var usuarioPersonagem = context.Usuarios.FirstOrDefault(x => x.Codigo == app.Usuario);

            Functions.EnviarEmail(usuarioPersonagem.Email, $"Aplicação de {app.Nome} Aceita", $"A aplicação do seu personagem <strong>{app.Nome}</strong> foi aceita.");

            return Json($"Você aceitou a aplicação {app.Nome} [{app.Codigo}].");
        }

        [Route("Negar")]
        [HttpPost]
        public IActionResult Negar(AplicacoesPersonagensViewModel viewModel)
        {
            if (string.IsNullOrWhiteSpace(viewModel.Motivo))
                return Json($"Motivo não foi preenchido.");

            if (viewModel.Motivo.Length > 1000)
                return Json($"Motivo deve ter no máximo 1000 caracteres.");

            using var context = new DatabaseContext();
            var app = context.Personagens.FirstOrDefault(x => x.Codigo == viewModel.Codigo && x.UsuarioStaffAvaliador == 0 && x.UsuarioStaffAvaliando == CodigoUsuario);
            if (app == null)
                return Json($"Não localizei nenhuma aplicação com o código {viewModel.Codigo}.");

            app.UsuarioStaffAvaliador = CodigoUsuario;
            app.MotivoRejeicao = viewModel.Motivo;
            context.Update(app);
            context.SaveChanges();

            var usuarioPersonagem = context.Usuarios.FirstOrDefault(x => x.Codigo == app.Usuario);

            Functions.EnviarEmail(usuarioPersonagem.Email, $"Aplicação de {app.Nome} Negada", $"A aplicação do seu personagem <strong>{app.Nome}</strong> foi negada. Motivo: {viewModel.Motivo}");

            return Json($"Você negou a aplicação {app.Nome} [{app.Codigo}].");
        }
    }
}