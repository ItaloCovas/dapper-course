
using ECommerceAPI.Models;

namespace ECommerceAPI.Repositories
{
    interface IContribUserRepository
    {
        public List<User> Get();

        public User GetById(int id);

        public void Create(User user);

        public void Update(User user);

        public void Delete(int id);
    }
}