using Dapper.Contrib.Extensions;
using ECommerceAPI.Models;
namespace eCommerce.API.Models
{
    [Table("Usuarios")]
    public class UserTwo
    {
        [Key]
        public int Cod { get; set; }
        public string NomeCompleto { get; set; }
        public string Email { get; set; }
        public string Sexo { get; set; }
        public string RG { get; set; }
        public string CPF { get; set; }
        public string NomeCompletoMae { get; set; }
        public string Situacao { get; set; }
        public DateTimeOffset DataCadastro { get; set; }

        [Write(false)]
        public Contact Contato { get; set; }

        [Write(false)]
        public ICollection<ShippingAddress> EnderecosEntrega { get; set; }

        [Write(false)]
        public ICollection<Department> Departamentos { get; set; }
    }
}