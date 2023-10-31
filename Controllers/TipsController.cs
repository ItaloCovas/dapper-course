using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using ECommerceAPI.Models;
using Dapper.FluentMap;
using ECommerceAPI.Mappers;
using eCommerce.API.Models;

namespace ECommerceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipsController : ControllerBase
    {
        private IDbConnection _connection;
        public TipsController()
        {
            _connection = new SqlConnection(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=eCommerce;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            string sql = "SELECT * FROM Usuarios WHERE Id = @Id;" +
                            "SELECT * FROM Contatos WHERE UsuarioId = @Id;" +
                            "SELECT * FROM EnderecosEntrega WHERE UsuarioId = @Id;" +
                            "SELECT D.* FROM UsuariosDepartamentos UD INNER JOIN Departamentos D ON UD.DepartamentoId = D.Id WHERE UD.UsuarioId = @Id;";

            using (var multipleResultSets = _connection.QueryMultiple(sql, new { Id = id }))
            {
                var user = multipleResultSets.Read<User>().SingleOrDefault();
                var contact = multipleResultSets.Read<Contact>().SingleOrDefault();
                var addresses = multipleResultSets.Read<ShippingAddress>().ToList();
                var departments = multipleResultSets.Read<Department>().ToList();

                if (user != null)
                {
                    user.Contato = contact;
                    user.EnderecosDeEntrega = addresses;
                    user.Departamentos = departments;

                    return Ok(user);
                }
            }

            return NotFound();
        }

        [HttpGet("stored/users")]
        public IActionResult StoredGet()
        {
            var users = _connection.Query<User>("SelecionarUsuarios", commandType: CommandType.StoredProcedure);

            return Ok(users);
        }

        [HttpGet("stored/users/{id}")]
        public IActionResult StoredGet(int id)
        {
            var users = _connection.Query<User>("SelecionarUsuario", new { Id = id }, commandType: CommandType.StoredProcedure);

            return Ok(users);
        }

        [HttpGet("mapper1/users")]
        public IActionResult Mapper1()
        {
            /*
             * Prblema: Mapear colunas com nomes diferentes das propriedades do objeto.
             * Solução 01: SQL(MER) => Renomear a coluna.
             */
            var users = _connection.Query<UserTwo>("SELECT Id Cod, Nome NomeCompleto, Email, Sexo, RG, CPF, NomeMae NomeCompletoMae, SituacaoCadastro Situacao, DataCadastro FROM Usuarios;");
            return Ok(users);
        }

        [HttpGet("mapper2/users")]
        public IActionResult Mapper2()
        {
            FluentMapper.Initialize(config =>
            {
                config.AddMap(new UserTwoMap());
            });
            /*
             * Prblema: Mapear colunas com nomes diferentes das propriedades do objeto.
             * Solução 02: C#(POO) => Mapeamento por meio da Biblioteca Dapper.FluentMap.
             */
            var users = _connection.Query<UserTwo>("SELECT * FROM Usuarios;");
            return Ok(users);
        }
    }
}