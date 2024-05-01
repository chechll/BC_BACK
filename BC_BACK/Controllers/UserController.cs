using AutoMapper;
using BCrypt.Net;
using BC_BACK.Dto;
using BC_BACK.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BC_BACK.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BC_BACK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        public readonly IUserService _userService;
        private readonly IJwtService _jwtService;
        public UserController(IUserService userService, IJwtService jwtService)
        {
            _userService = userService;
            _jwtService = jwtService;
        }

        private IActionResult ValidateTokenAndGetPrincipal()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var principal = _jwtService.GetPrincipalFromToken(token);

            if (principal == null)
            {
                return Unauthorized();
            }

            return null;
        }

        [Authorize]
        [HttpGet("GetAllUsers")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        [ProducesResponseType(400)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetAllUsers()
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _userService.GetAllUsers();
        }

        [HttpGet("LogIn")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        [ProducesResponseType(400)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult LogIn(string username, string user_password)
        {
            return _userService.LogIn(username, user_password);
        }

        [Authorize]
        [HttpGet("GetUser")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        [ProducesResponseType(400)]
        public IActionResult GetUser(int id)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _userService.GetUser(id);
        }

        [HttpPost("SignUp")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateUser([FromBody] UserDto userCreate)
        {
            Console.WriteLine("2",userCreate);
            return _userService.CreateUser(userCreate);
        }

        [Authorize]
        [HttpPut("Update")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult UpdateUser(
            [FromBody] UserDto updatedUser)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _userService.UpdateUser(updatedUser);
        }

        [Authorize]
        [HttpPut("UpdateAdmin")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult UpdateUserRights(
            [FromBody] UserDto updatedUser)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _userService.UpdateUserRights(updatedUser);
        }

        [Authorize]
        [HttpDelete("Delete")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult DeleteUser(int userId)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _userService.DeleteUser(userId);
        }


    }
}
