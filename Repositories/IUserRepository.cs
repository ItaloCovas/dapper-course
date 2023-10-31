using ECommerceAPI.Models;

namespace ECommerceAPI.Repositories
{
    interface IUserRepository
    {
        public List<User> Get();

        public User GetById(int id);

        public void Create(User user);

        public void Update(User user, int id);

        public void Delete(int id);
    }
}