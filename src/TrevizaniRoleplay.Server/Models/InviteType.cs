using System.ComponentModel.DataAnnotations;

namespace TrevizaniRoleplay.Server.Models
{
    public enum InviteType
    {
        [Display(Name = "Facção")]
        Faccao = 1,

        [Display(Name = "Venda de Propriedade")]
        VendaPropriedade = 2,

        Revista = 3,

        [Display(Name = "Venda de Veículo")]
        VendaVeiculo = 4,

        [Display(Name = "Localização de Celular")]
        LocalizacaoCelular = 5,

        [Display(Name = "Empresa")]
        Company = 6,

        [Display(Name = "Mecânico")]
        Mechanic = 7,
    }
}