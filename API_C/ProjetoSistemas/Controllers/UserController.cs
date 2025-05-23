using Microsoft.AspNetCore.Mvc;
using ProjetoSistemas.Models;
using ProjetoSistemas.Data;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace ProjetoSistemas.Controllers
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
        public async Task<IActionResult> CreateEditUser(UserModel user)
        {
            var userExists = _context.Users.Find(user.UserId);

            if (userExists == null)
            {
                _context.Users.Add(user);
                _context.SaveChanges();

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

                return Ok(user);
            }

            return NotFound("User already exists, try again");
        }

        [HttpGet]
        public IActionResult Get(int id)
        {
            var result = _context.Users.Find(id);
            return result == null
                ? NotFound()
                : Ok(result);
        }

        [HttpGet("/GetAll")]
        public IActionResult GetAll()
        {
            var result = _context.Users.ToList();
            return Ok(result);
        }

        [HttpPost("NewFollow")]
        public async Task<IActionResult> NewFollow(int idUser, int idFollowing)
        {
            var user = _context.Users.Find(idUser);
            var userFollowing = _context.Users.Find(idFollowing);

            if (user == null || userFollowing == null)
                return NotFound("Usuário(s) não encontrado(s).");

            var alreadyFollows = _context.Follows
                .Any(f => f.FollowerId == idUser && f.FollowingId == idFollowing);

            if (alreadyFollows)
                return BadRequest("Follow já existente.");

            var follow = new UserFollowing
            {
                FollowerId = idUser,
                FollowingId = idFollowing
            };

            _context.Follows.Add(follow);
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

            return Ok("Follow registrado com sucesso.");
        }

        [HttpGet("GetPostsByUser/{id}")]
        public IActionResult GetPostsByUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return NotFound("Usuário não encontrado.");

            var posts = _context.Posts
                .Where(p => p.UserId == id)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new
                {
                    user = new { username = user.Username },
                    content = p.Content,
                    createdAt = p.CreatedAt
                })
                .ToList();

            return Ok(posts);
        }

    }
}
