using System.ComponentModel.DataAnnotations;

namespace TrevizaniRoleplay.Server.Models
{
    public enum CompanyFlag : byte
    {
        [Display(Name = "Adicionar Funcionário")]
        InviteCharacter = 1,

        [Display(Name = "Editar Funcionário")]
        EditCharacter = 2,

        [Display(Name = "Remover Funcionário")]
        RemoveCharacter = 3,

        [Display(Name = "Abrir")]
        Open = 4,
    }
}