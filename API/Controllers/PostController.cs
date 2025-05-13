﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjetoSistemas.Models;
using TodoApi.Data;
using TodoApi.Models;
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
        public JsonResult AddPost(int userId , string postText)
        {
            var user = _context.Users.Find(userId);

            var newPost = new PostModel(user, DateTime.Now, postText);

            _context.Posts.Add(newPost);
            _context.SaveChanges();

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
            var userRef = _context.Users.Find(userId);
            var posts = _context.Posts.Include(p => p.User).ToList();

            IEnumerable<PostModel> feedList =
                from teste in posts
                where userRef.Following.Contains(teste.User.UserId)
                select teste;

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
