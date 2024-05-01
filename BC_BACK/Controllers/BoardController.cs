using AutoMapper;
using BC_BACK.Dto;
using BC_BACK.Interfaces;
using BC_BACK.Repository;
using BC_BACK.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Channels;
using static BC_BACK.Controllers.UserController;

namespace BC_BACK.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController : Controller
    {
        public readonly IBoardService _boardService;
        private readonly IJwtService _jwtService;
        public BoardController(IBoardService boardService, IJwtService jwtService) 
        {
            _boardService = boardService;
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

        [HttpGet("GetBoard")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Board>))]
        [ProducesResponseType(400)]
        public IActionResult GetBoard(int id)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _boardService.GetBoard(id);
            
        }

        [HttpPost("Update")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult UpdateBoard(
            [FromBody] BoardArrayModel updatedBoard)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _boardService.UpdateBoard(updatedBoard);
        }
    }
}
