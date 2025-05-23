using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoSistemas.Models;
using ProjetoSistemas.Data;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjetoSistemas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly UserDB _context;

        public PostController(UserDB context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<JsonResult> AddPost(int userId , string postText)
        {
            var user = _context.Users.Find(userId);

            if (user == null)
                return new JsonResult(BadRequest("User not found"));

            var newPost = new PostModel(user, DateTime.Now, postText);
            _context.Posts.Add(newPost);
            _context.SaveChanges();

            var replicationPayload = new
            {
                user = user.Username,
                content = postText,
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
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Falha na réplica para {url}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro replicando para {url}: {ex.Message}");
                }
            }

            return new JsonResult(newPost);
        }

        [HttpGet]
        public JsonResult Get()
        {
            var result = _context.Posts.Include(p => p.User).ToList();
            return new JsonResult(Ok(result));
        }

        [HttpGet("/getUserFeed")]
        public JsonResult getUserFeed(int userId)
        {
            var userRef = _context.Users.Include(u => u.Following).FirstOrDefault(u => u.UserId == userId);
            var posts = _context.Posts.Include(p => p.User).ToList();

            if (userRef == null)
                return new JsonResult(BadRequest("User not found"));

            var followingIds = userRef.Following.Select(f => f.FollowingId).ToList();
            var feedList = posts.Where(p => followingIds.Contains(p.User.UserId)).ToList();

            return new JsonResult(Ok(feedList));
        }

        [HttpPost("sendMessage")]
        public async Task<IActionResult> SendMessage(int senderId, int receiverId, string messageText)
        {
            var httpClient = new HttpClient();

            var messagePayload = new
            {
                senderId = senderId,
                receiverId = receiverId,
                message = messageText
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(messagePayload),
                Encoding.UTF8,
                "application/json"
            );

            try
            {
                var response = await httpClient.PostAsync("http://localhost:8080/api/messages/send", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    return Ok(new { success = true, response = responseContent });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, new { success = false, message = "Failed to send message to external service." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message });
            }
        }
    }
}
