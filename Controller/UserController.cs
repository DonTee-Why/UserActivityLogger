using Microsoft.AspNetCore.Mvc;
using UserActivityLogger.Model;
using UserActivityLogger.Service;

namespace UserActivityLogger.Controller
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        private readonly RabbitMQService _rabbitMQService;

        public UserController(RabbitMQService rabbitMQService)
        {
            _rabbitMQService = rabbitMQService;
        }

        [HttpPost("register")]
        public IActionResult RegisterUser([FromBody] User user)
        {
            UserStore.Users.Add(user);
            _rabbitMQService.PublishMessage($"User {user.Username} registered with email {user.Email}");
            return Ok("User registered successfully");
        }
    }
}