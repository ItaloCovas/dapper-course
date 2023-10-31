
using ECommerceAPI.Models;
using ECommerceAPI.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.Controllers
{

    [Route("contrib/users")]
    [ApiController]
    public class ContribUsersController : ControllerBase
    {
        private IContribUserRepository _repository;

        public ContribUsersController()
        {
            _repository = new ContribUserRepository();
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
        public ActionResult<User> Update(User user)
        {
            _repository.Update(user);

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