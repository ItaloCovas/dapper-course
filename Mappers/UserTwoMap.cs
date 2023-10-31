using Dapper.FluentMap.Mapping;
using eCommerce.API.Models;

namespace ECommerceAPI.Mappers
{
    public class UserTwoMap : EntityMap<UserTwo>
    {
        public UserTwoMap()
        {
            Map(p => p.Cod).ToColumn("Id");
            Map(p => p.NomeCompleto).ToColumn("Nome");
            Map(p => p.NomeCompletoMae).ToColumn("NomeMae");
            Map(p => p.Situacao).ToColumn("SituacaoCadastro");
        }
    }
}