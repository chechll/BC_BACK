using AutoMapper;
using BC_BACK.Dto;
using BC_BACK.Interfaces;
using BC_BACK.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Channels;
using static BC_BACK.Controllers.UserController;

namespace BC_BACK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoardController : Controller
    {
        public readonly IMapper _mapper;
        public readonly IBoardRepository _boardRepository;
        public readonly IGameRepository _gameRepository;
        public BoardController( IGameRepository gameRepository, IMapper mapper,IBoardRepository boardRepository) 
        {
            _mapper = mapper;
            _boardRepository = boardRepository;
            _gameRepository = gameRepository;
        }

        [HttpGet("GetAllBorads")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Board>))]
        [ProducesResponseType(400)]
        public IActionResult GetAllBoards()
        {
            try
            {
                var allBoards = _boardRepository.GetBoards();

                return Ok(allBoards);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("GetBoard")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Board>))]
        [ProducesResponseType(400)]
        public IActionResult GetBoard(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_boardRepository.isBoardExist(id))
                return NotFound();

            var board = _mapper.Map<BoardDto>(_boardRepository.GetBoardsByGame(id).FirstOrDefault()); ;

            return Ok(board);
        }

        public class BoardArrayModel
        {
            public int IdBoard { get; set; }
            public String Board1{ get; set; }
        }

        [HttpPost("Update")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateBoard(
            [FromBody] BoardArrayModel updatedBoard)
        {
            bool isUpdateNeeded = false;
            if (updatedBoard == null)
                return BadRequest(ModelState);
            if (!_boardRepository.isBoardExist(updatedBoard.IdBoard))
                return NotFound();

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Model state is not valid");
                return BadRequest(ModelState);
            }

            var board = _boardRepository.GetBoard(updatedBoard.IdBoard);

            if (board.Board1 != updatedBoard.Board1 && board.Board1.Length == updatedBoard.Board1.Length)
            {
                board.Board1 = updatedBoard.Board1;
                isUpdateNeeded = true;
            }

            Console.WriteLine("isupdate = " + isUpdateNeeded);

            if (isUpdateNeeded)
            {
                var boardMap = _mapper.Map<Board>(board);
                if (!_boardRepository.UpdateBoard(boardMap))
                {
                    ModelState.AddModelError("", "Something went wrong ");
                    return StatusCode(500, ModelState);
                }
            }
            return Ok("Successfully updated");
        }

        [HttpDelete("Delete")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteBoard(int boardId)
        {

            if (!_boardRepository.isBoardExist(boardId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var boardToDelete = _boardRepository.GetBoard(boardId);

            if (!_boardRepository.DeleteBoard(boardToDelete))
            {
                ModelState.AddModelError("", "Something went wrong ");
                return StatusCode(500, ModelState);
            }

            return Ok(0);

        }
    }
}
