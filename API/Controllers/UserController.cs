using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjetoSistemas.Models;
using TodoApi.Data;
using TodoApi.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;


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
        public async Task<JsonResult> CreateEditUser(UserModel user)
        {
            var userExists = _context.Users.Find(user.UserId);

            if (userExists == null)
            { 
                _context.Users.Add(user);
            }
            else
            {
                return new JsonResult(NotFound("User already exists, try again"));
            }

            _context.SaveChanges();

            // 🔁 Payload para replicação
            var replicationPayload = new
            {
                action = "create_user",
                userId = user.UserId,
                username = user.Username,
                timestamp = 0
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(replicationPayload),
                Encoding.UTF8,
                "application/json"
            );

            using var httpClient = new HttpClient();
            var replicas = new[]
            {
                "http://localhost:5001/post",
                "http://localhost:5002/post",
                "http://localhost:5003/post"
            };

            foreach (var url in replicas)
            {
                try
                {
                    var response = await httpClient.PostAsync(url, jsonContent);
                    Console.WriteLine($"[USER] Replicado para {url} - status {response.StatusCode}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[USER] Erro replicando para {url}: {ex.Message}");
                }
            }

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
        public async Task<JsonResult> NewFollow(int idUser, int idFollowing)
        {
            var user = _context.Users.Find(idUser);
            var userFollowing = _context.Users.Find(idFollowing); 

            if (user == null || userFollowing == null)
            {
                return new JsonResult(NotFound());
            }

            userFollowing.Followers.Add(idUser);
            user.Following.Add(idFollowing);

            _context.SaveChanges();

            var replicationPayload = new
            {
                action = "follow",
                userId = idUser,
                follows = idFollowing,
                timestamp = 0
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(replicationPayload),
                Encoding.UTF8,
                "application/json"
            );

            using var httpClient = new HttpClient();
            var replicas = new[]
            {
                "http://localhost:5001/post",
                "http://localhost:5002/post",
                "http://localhost:5003/post"
            };

            foreach (var url in replicas)
            {
                try
                {
                    var response = await httpClient.PostAsync(url, jsonContent);
                    Console.WriteLine($"[FOLLOW] Replicado para {url} - status {response.StatusCode}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[FOLLOW] Erro replicando para {url}: {ex.Message}");
                }
            }

            return new JsonResult(Ok("Follow registrado com sucesso."));
            }

        
    }
}
