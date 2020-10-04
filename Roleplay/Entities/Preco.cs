using Roleplay.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roleplay.Entities
{
    public class Preco
    {
        public TipoPreco Tipo { get; set; }
        public string Nome { get; set; }
        public int Valor { get; set; }

        [NotMapped]
        public bool Veiculo => Tipo == TipoPreco.VAPID || Tipo == TipoPreco.AlbanyDeclasseAutos || Tipo == TipoPreco.BenefactorEuroCars
            || Tipo == TipoPreco.DinkaOrientalAutos || Tipo == TipoPreco.PremiumDeluxeMotorsport || Tipo == TipoPreco.SandersMotorcyclesDealership
            || Tipo == TipoPreco.Avioes || Tipo == TipoPreco.Industrial || Tipo == TipoPreco.Helicopteros 
            || Tipo == TipoPreco.Barcos || Tipo == TipoPreco.LuxuryAutos;
    }
}