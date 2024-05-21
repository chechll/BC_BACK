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

        public BoardService(IMapper mapper, IBoardRepository boardRepository) 
        {
            _mapper = mapper;
            _boardRepository = boardRepository;
        }

        public IActionResult GetBoard(int id)
        {
            var board = _boardRepository.GetBoardsByGame(id).FirstOrDefault();
            if (board != null && !_boardRepository.IsBoardExist(board.IdBoard))
                return NotFound();

            return Ok(board);
        }

        public IActionResult UpdateBoard(BoardArrayModel updatedBoard)
        {
            if (updatedBoard == null)
                return BadRequest("Invalid board data");

            if (!_boardRepository.IsBoardExist(updatedBoard.IdBoard))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var board = _boardRepository.GetBoard(updatedBoard.IdBoard);

            if (board != null && board.Board1 != updatedBoard.Board1 && board.Board1.Length == updatedBoard.Board1.Length)
            {
                board.Board1 = updatedBoard.Board1;
                var boardMap = _mapper.Map<Board>(board);
                if (!_boardRepository.UpdateBoard(boardMap))
                    return StatusCode(500, "Failed to update board.");
            }
            return Ok("Successfully updated");
        }
    }
}
