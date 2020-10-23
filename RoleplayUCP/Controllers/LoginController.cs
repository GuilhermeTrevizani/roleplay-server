using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Roleplay;
using Roleplay.Models;
using RoleplayUCP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace RoleplayUCP.Controllers
{
    [AllowAnonymous]
    public class LoginController : BaseController
    {
        public IActionResult Index(string ReturnUrl)
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View(new LoginViewModel
            {
                UrlRetorno = ReturnUrl
            });
        }

        [HttpPost]
        public IActionResult Autenticar(LoginViewModel viewModel)
        {
            try
            {
                if (User.Identity.IsAuthenticated)
                    return RedirectToAction("Index", "Home");

                if (!ModelState.IsValid)
                    return View("Index", viewModel);

                var senhaCriptografada = Functions.Criptografar(viewModel.Senha);
                using var context = new DatabaseContext();
                var usuario = context.Usuarios.FirstOrDefault(x => x.Nome.ToLower() == viewModel.Usuario.ToLower() && x.Senha == senhaCriptografada && x.Staff != TipoStaff.Nenhum);
                if (usuario == null)
                {
                    GravarLog(TipoLog.LoginFalhaUCP, $"Usuário: {viewModel.Usuario} | {JsonConvert.SerializeObject(HttpContext.Request.Headers)}");
                    ModelState.AddModelError(nameof(viewModel.Usuario), "Usuário ou senha inválidos");
                    return View("Index", viewModel);
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Sid, usuario.Codigo.ToString()),
                    new Claim(ClaimTypes.Name, usuario.Nome),
                    new Claim(ClaimTypes.Role, usuario.Staff.ToString()),
                };
                var claimsIdentity = new ClaimsIdentity(claims, "Identity");
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                HttpContext.SignInAsync(claimsPrincipal);

                GravarLog(TipoLog.EntradaUCP, $"Usuário: {viewModel.Usuario} | {JsonConvert.SerializeObject(HttpContext.Request.Headers)}");

                if ((!string.IsNullOrWhiteSpace(viewModel.UrlRetorno) || Url.IsLocalUrl(viewModel.UrlRetorno)) && viewModel.UrlRetorno != "/")
                    return Redirect(viewModel.UrlRetorno);

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                Functions.RecuperarErro(ex);
                return View("Index", viewModel);
            }
        }
    }
}