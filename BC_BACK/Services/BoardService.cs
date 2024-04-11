using AutoMapper;
using BC_BACK.Dto;
using BC_BACK.Interfaces;
using BC_BACK.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BC_BACK.Services
{
    public class BoardService : ControllerBase, IBoardService
    {
        public readonly IMapper _mapper;
        public readonly IBoardRepository _boardRepository;
        public readonly IGameRepository _gameRepository;

        public BoardService(IGameRepository gameRepository, IMapper mapper, IBoardRepository boardRepository) 
        {
            _mapper = mapper;
            _boardRepository = boardRepository;
            _gameRepository = gameRepository;
        }

        public IActionResult GetBoard(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_boardRepository.isBoardExist(id))
                return NotFound();

            var board = _mapper.Map<BoardDto>(_boardRepository.GetBoardsByGame(id).FirstOrDefault()); ;

            return Ok(board);
        }

        public IActionResult UpdateBoard(BoardArrayModel updatedBoard)
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
    }
}
