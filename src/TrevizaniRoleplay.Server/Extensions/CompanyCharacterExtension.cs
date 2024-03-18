using TrevizaniRoleplay.Domain.Entities;
using TrevizaniRoleplay.Server.Models;

namespace TrevizaniRoleplay.Server.Extensions
{
    public static class CompanyCharacterExtension
    {
        public static List<CompanyFlag> GetFlags(this CompanyCharacter companyCharacter)
            => Functions.Deserialize<List<CompanyFlag>>(companyCharacter.FlagsJSON);
    }
}