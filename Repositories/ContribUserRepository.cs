
using System.Data;
using System.Data.SqlClient;
using ECommerceAPI.Models;
using Dapper.Contrib.Extensions;

namespace ECommerceAPI.Repositories
{
    public class ContribUserRepository
 : IContribUserRepository
    {

        private IDbConnection _connection;
        public ContribUserRepository
()
        {
            _connection = new SqlConnection("Server=localhost;Database=Ecommerce;User Id=SA;Password=BancoDeDados123#;");
        }

        public List<User> Get()
        {
            return _connection.GetAll<User>().ToList();
        }

        public User GetById(int id)
        {
            return _connection.Get<User>(id);
        }

        public void Create(User user)
        {
            user.Id = Convert.ToInt32(_connection.Insert(user));
        }

        public void Update(User user)
        {
            _connection.Update(user);
        }
        public void Delete(int id)
        {
            _connection.Delete(GetById(id));
        }
    }
}