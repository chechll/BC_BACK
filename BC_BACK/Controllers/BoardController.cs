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
        public BoardController(IBoardService boardService) 
        {
            _boardService = boardService;
        }

        [HttpGet("GetBoard")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Board>))]
        [ProducesResponseType(400)]
        public IActionResult GetBoard(int id)
        {
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
            return _boardService.UpdateBoard(updatedBoard);
        }
    }
}
