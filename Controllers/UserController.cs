using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Data;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDB _context;

        public UserController(UserDB context)
        {
            _context = context;
        }

        [HttpPost]
        public JsonResult CreateEditUser(UserModel user)
        {
            if (user.UserId == 0)
            {
                _context.Users.Add(user);
            }
            else
            {
                var userDb = _context.Users.Find(user.UserId);

                if (userDb == null)
                {
                    return new JsonResult(NotFound());
                }
                userDb = user;
            }
            _context.SaveChanges();

            return new JsonResult(Ok(user));


        }
        [HttpGet]
        public JsonResult Get(int id)
        {
            var result = _context.Users.Find(id);

            if (result == null)
            {
                return new JsonResult(NotFound());
            }


            return new JsonResult(result);
        }


        [HttpGet("/GetAll")]
        public JsonResult GetAll()
        {
            var result = _context.Users.ToList();

            return new JsonResult(Ok(result));
        }

        [HttpPost("/NewFollow")]
        public JsonResult NewFollow(int idUser, int idFollowing)
        {
            var user = _context.Users.Find(idUser);
            var userFollowing = _context.Users.Find(idFollowing); 

            if (user == null)
            {
                //Terminar
            }

        }
        
    }
}
