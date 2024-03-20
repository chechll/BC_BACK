using AutoMapper;
using BC_BACK.Dto;
using BC_BACK.Interfaces;
using BC_BACK.Repository;
using Microsoft.AspNetCore.Mvc;
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

            var board = _mapper.Map<BoardDto>(_boardRepository.GetBoard(id)); ;

            return Ok(board);
        }
        /*
        [HttpPost("CreateBoard")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateBoard([FromBody] BoardDto boardCreate)
        {
            if (boardCreate == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var board = _boardRepository.GetBoards().Where(c => c.IdGame == boardCreate.IdGame).FirstOrDefault();

            if (board != null)
            {
                ModelState.AddModelError("", "board already exists");
                return StatusCode(422, ModelState);
            }

            if (boardCreate.Size % 2 != 1 || boardCreate.Size > 25 || boardCreate.Size < 9)
            {
                return StatusCode(422, "wrong data");
            }

            boardCreate.Board1 = _boardRepository.BoardToString(_boardRepository.CreateBorad(boardCreate.Size)); ;

            var boardMap = _mapper.Map<Board>(boardCreate);

            if (!_boardRepository.CreateBoard(boardMap))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }

            return Ok();

        }
        */
        [HttpPut("Update")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateBoard(
            [FromForm] BoardDto updatedBoard)
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

            if(updatedBoard.Size % 2 != 1 || updatedBoard.IdGame != _boardRepository.GetBoard(updatedBoard.IdBoard).IdGame || updatedBoard.Size > 25 || updatedBoard.Size < 9)
            { 
                return StatusCode(422, "wrong data");
            }

            var board = _boardRepository.GetBoard(updatedBoard.IdBoard);
                    
            if (updatedBoard.Size != board.Size)
            {
                board.Size = updatedBoard.Size;
                isUpdateNeeded = true;

                board.Board1 = _boardRepository.BoardToString(_boardRepository.CreateBorad(board.Size));
            }
            
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
