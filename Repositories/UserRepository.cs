using System.Data;
using System.Data.SqlClient;
using ECommerceAPI.Models;
using Dapper;

namespace ECommerceAPI.Repositories
{
    public class UserRepository : IUserRepository
    {

        private IDbConnection _connection;
        public UserRepository()
        {
            _connection = new SqlConnection("Server=localhost;Database=Ecommerce;User Id=SA;Password=BancoDeDados123#;");
        }


        public void Create(User user)
        {
            _connection.Open();
            var transaction = _connection.BeginTransaction();


            try
            {
                string query = "INSERT INTO Usuarios(Nome, Email, Sexo, RG, CPF, NomeMae, SituacaoCadastro, DataCadastro) VALUES (@Nome, @Email, @Sexo, @RG, @CPF, @NomeMae, @SituacaoCadastro, @DataCadastro); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                user.Id = _connection.Query<int>(query, user, transaction).Single();

                if (user.Contato == null)
                    return;

                user.Contato.UsuarioId = user.Id;
                string contactQuery = "INSERT INTO Contatos(UsuarioId, Telefone, Celular) VALUES (@UsuarioId, @Telefone, @Celular); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                user.Contato.Id = _connection.Query<int>(contactQuery, user.Contato, transaction).Single();

                if (user.EnderecosDeEntrega != null && user.EnderecosDeEntrega.Count > 0)
                {
                    foreach (var shippingAddress in user.EnderecosDeEntrega)
                    {
                        shippingAddress.Id = user.Id;
                        string shippingSql = "INSERT INTO EnderecosEntrega (UsuarioId, NomeEndereco, CEP, Estado, Cidade, Bairro, Endereco, Numero, Complemento) VALUES (@UsuarioId, @NomeEndereco, @CEP, @Estado, @Cidade, @Bairro, @Endereco, @Numero, @Complemento); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                        shippingAddress.Id = _connection.Query<int>(shippingSql, user.EnderecosDeEntrega, transaction).Single();
                    }
                }

                if (user.Departamentos != null && user.Departamentos.Count > 0)
                {
                    foreach (var departament in user.Departamentos)
                    {
                        string departmentSql = "INSERT INTO UsuariosDepartamentos (UsuarioId, DepartamentoId) VALUES (@UsuarioId, DepartamentoId);";
                        _connection.Execute(departmentSql, new { UsuarioId = user.Id, DepartamentoId = departament.Id }, transaction);
                    }
                }


                transaction.Commit();
            }
            catch (Exception)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception)
                {
                    // TODO - Return for userController some message. (Notification Pattern)
                }
            }
            finally
            {
                _connection.Close();
            }
        }

        public List<User> Get()
        {
            //return _connection.Query<Usuario>("SELECT * FROM Usuarios").ToList();
            List<User> users = new List<User>();
            string sql = "SELECT U.*, C.*, EE.*, D.* FROM Usuarios as U LEFT JOIN Contatos C ON C.UsuarioId = U.Id LEFT JOIN EnderecosEntrega EE ON EE.UsuarioId = U.Id LEFT JOIN UsuariosDepartamentos UD ON UD.UsuarioId = U.Id LEFT JOIN Departamentos D ON UD.DepartamentoId = D.Id";

            _connection.Query<User, Contact, ShippingAddress, Department, User>(sql,
                (user, contact, shippingAddress, department) =>
                {

                    //Verificação do usuário.
                    if (users.SingleOrDefault(a => a.Id == user.Id) == null)
                    {
                        user.Departamentos = new List<Department>();
                        user.EnderecosDeEntrega = new List<ShippingAddress>();
                        user.Contato = contact;
                        users.Add(user);
                    }
                    else
                    {
                        user = users.SingleOrDefault(a => a.Id == user.Id);
                    }

                    //Verificação do Endereço de Entrega.
                    if (user.EnderecosDeEntrega.SingleOrDefault(a => a.Id == shippingAddress.Id) == null)
                    {
                        user.EnderecosDeEntrega.Add(shippingAddress);
                    }

                    //Verificação do Departamento.
                    if (user.Departamentos.SingleOrDefault(a => a.Id == department.Id) == null)
                    {
                        user.Departamentos.Add(department);
                    }

                    return user;
                });

            return users;
        }

        public User GetById(int id)
        {
            List<User> users = new List<User>();
            string sql = "SELECT U.*, C.*, EE.*, D.* FROM Usuarios as U LEFT JOIN Contatos C ON C.UsuarioId = U.Id LEFT JOIN EnderecosEntrega EE ON EE.UsuarioId = U.Id LEFT JOIN UsuariosDepartamentos UD ON UD.UsuarioId = U.Id LEFT JOIN Departamentos D ON UD.DepartamentoId = D.Id WHERE U.Id = @Id";

            _connection.Query<User, Contact, ShippingAddress, Department, User>(sql,
                (user, contact, shippingAddress, department) =>
                {

                    //Verificação do usuário.
                    if (users.SingleOrDefault(a => a.Id == user.Id) == null)
                    {
                        user.Departamentos = new List<Department>();
                        user.EnderecosDeEntrega = new List<ShippingAddress>();
                        user.Contato = contact;
                        users.Add(user);
                    }
                    else
                    {
                        user = users.SingleOrDefault(a => a.Id == user.Id);
                    }

                    //Verificação do Endereço de Entrega.
                    if (user.EnderecosDeEntrega.SingleOrDefault(a => a.Id == shippingAddress.Id) == null)
                    {
                        user.EnderecosDeEntrega.Add(shippingAddress);
                    }

                    //Verificação do Departamento.
                    if (user.Departamentos.SingleOrDefault(a => a.Id == department.Id) == null)
                    {
                        user.Departamentos.Add(department);
                    }

                    return user;
                }, new { Id = id });

            return users.SingleOrDefault();

        }

        public void Update(User user, int id)
        {
            _connection.Open();
            var transaction = _connection.BeginTransaction();

            try
            {
                string query = "UPDATE Usuarios SET Nome = @Nome, Email = @Email, Sexo = @Sexo, RG = @RG, CPF = @CPF, NomeMae = @NomeMae, SituacaoCadastro = @SituacaoCadastro, DataCadastro = @DataCadastro WHERE Id = @Id";
                _connection.Execute(query, new { Id = id, Nome = user.Nome, Email = user.Email, Sexo = user.Sexo, RG = user.RG, CPF = user.CPF, NomeMae = user.NomeMae, SituacaoCadastro = user.SituacaoCadastro, DataCadastro = user.DataCadastro }, transaction);

                if (user.Contato != null)
                {

                    string contactQuery = "UPDATE Contatos SET UsuarioId = @UsuarioId, Telefone = @Telefone, Celular = @Celular WHERE Id = @Id";
                    _connection.Execute(contactQuery, user.Contato, transaction);
                }

                string deleteShippingSql = "DELETE * FROM EnderecosEntrega WHERE UsuarioId = @Id";
                _connection.Execute(deleteShippingSql, user, transaction);

                if (user.EnderecosDeEntrega != null && user.EnderecosDeEntrega.Count > 0)
                {
                    foreach (var shippingAddress in user.EnderecosDeEntrega)
                    {
                        string shippingSql = "INSERT INTO EnderecosEntrega (UsuarioId, NomeEndereco, CEP, Estado, Cidade, Bairro, Endereco, Numero, Complemento) VALUES (@UsuarioId, @NomeEndereco, @CEP, @Estado, @Cidade, @Bairro, @Endereco, @Numero, @Complemento); SELECT CAST(SCOPE_IDENTITY() AS INT);";
                        shippingAddress.Id = _connection.Query<int>(shippingSql, user.EnderecosDeEntrega, transaction).Single();
                    }
                }

                string deleteDepartmentSql = "DELETE * FROM UsuariosDepartamentos WHERE UsuarioId = @Id";
                _connection.Execute(deleteDepartmentSql, user, transaction);

                if (user.Departamentos != null && user.Departamentos.Count > 0)
                {
                    foreach (var departament in user.Departamentos)
                    {
                        string departmentSql = "INSERT INTO UsuariosDepartamentos (UsuarioId, DepartamentoId) VALUES (@UsuarioId, DepartamentoId);";
                        _connection.Execute(departmentSql, new { UsuarioId = user.Id, DepartamentoId = departament.Id }, transaction);
                    }
                }

                transaction.Commit();
            }
            catch (Exception)
            {
                try
                {
                    transaction.Rollback();
                }
                catch (Exception) { }
            }
            finally
            {
                _connection.Close();
            }


        }

        public void Delete(int id)
        {
            _connection.Execute("DELETE FROM Usuarios WHERE Id = @Id", new { Id = id });
        }

        public static List<User> _db = new List<User>(){
            new User(){Id = 1, Nome = "Italo Covas", Email = "italocovas@gmail.com"},
            new User(){Id = 2, Nome = "Arthur Galanti", Email = "galantim@gmail.com"},
            new User(){Id = 3, Nome = "Sergio Tostes", Email = "sergio1337@gmail.com"}
        };
    }
}