using System.ComponentModel.DataAnnotations;

namespace RoleplayUCP.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Usuário é obrigatório")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Senha { get; set; }

        public string UrlRetorno { get; set; }
    }
}