using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Data;

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

    }
}
