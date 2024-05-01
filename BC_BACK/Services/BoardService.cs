﻿using AutoMapper;
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
            if (!_boardRepository.isBoardExist(id))
                return NotFound();

            var board = _mapper.Map<BoardDto>(_boardRepository.GetBoardsByGame(id).FirstOrDefault()); ;

            return Ok(board);
        }

        public IActionResult UpdateBoard(BoardArrayModel updatedBoard)
        {
            if (updatedBoard == null)
                return BadRequest("Invalid board data");

            if (!_boardRepository.isBoardExist(updatedBoard.IdBoard))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var board = _boardRepository.GetBoard(updatedBoard.IdBoard);

            if (board.Board1 != updatedBoard.Board1 && board.Board1.Length == updatedBoard.Board1.Length)
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
