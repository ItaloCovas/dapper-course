using ECommerceAPI.Models;
using ECommerceAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{

    [Route("users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserRepository _repository;

        public UsersController()
        {
            _repository = new UserRepository();
        }

        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            return _repository.Get();
        }


        [HttpGet("{id}")]
        public ActionResult<User> Get(int id)
        {
            var user = _repository.GetById(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost()]
        public ActionResult<User> Create(User user)
        {
            _repository.Create(user);
            return user;
        }

        [HttpPut("{id}")]
        public ActionResult<User> Update(int id, [FromBody] User user)
        {
            _repository.Update(user, id);

            return user;
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            _repository.Delete(id);
            return NoContent();
        }


    }
}