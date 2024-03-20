using AutoMapper;
using BC_BACK.Dto;
using BC_BACK.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BC_BACK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ICheckDataRepository _checkDataRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IBoardRepository _boardRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IAnsweredTaskRepository _answeredTaskRepository;
        private readonly ITaskRepository _taskRepository;

        public GameController(IUserRepository userRepository, IMapper mapper,
            ICheckDataRepository checkDataRepository, IGameRepository gameRepository,
            IBoardRepository boardRepository, ITeamRepository teamRepository,
            IAnsweredTaskRepository answeredTaskRepository, ITaskRepository taskRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _checkDataRepository = checkDataRepository;
            _gameRepository = gameRepository;
            _boardRepository = boardRepository;
            _teamRepository = teamRepository;
            _answeredTaskRepository = answeredTaskRepository;
            _taskRepository = taskRepository;
        }

        public class CreateData
        {
            public string name { get; set; }
            public int idUser { get; set; }
            public int size { get; set; }
        }

        [HttpGet("GetAllGames")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Game>))]
        [ProducesResponseType(400)]
        public IActionResult GetAllGames()
        {
            try
            {
                var allGame = _gameRepository.GetGames();

                return Ok(allGame);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("GetGame")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Game>))]
        [ProducesResponseType(400)]
        public IActionResult GetGame(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_gameRepository.isGameExist(id))
                return NotFound();

            var game = _mapper.Map<GameDto>(_gameRepository.GetGame(id)); ;

            return Ok(game);
        }

        [HttpPost("CreateGame")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateGame([FromBody] CreateData createData)
        {

            var gameCreate = new GameDto
            {
                IdUser = createData.idUser,
                Name = createData.name
            };
            if (gameCreate == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_userRepository.isUserExist(gameCreate.IdUser))
            {
                ModelState.AddModelError("", "Wrong idUser");
                return StatusCode(422, ModelState);
            }

            if (!_checkDataRepository.CheckStringLengs(gameCreate.Name, 20))
            {
                ModelState.AddModelError("", "Wrong Name");
                return StatusCode(422, ModelState);
            }

            if (createData.size > 25 || createData.size < 9 || createData.size %2 != 1)
            {
                ModelState.AddModelError("", "Wrong size");
                return StatusCode(422, ModelState);
            }

            var gameMap = _mapper.Map<Game>(gameCreate);

            if (!_gameRepository.CreateGame(gameMap))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }

            var idGame = _gameRepository.GetIdByName(gameCreate.Name);

            var board = new BoardDto
            {
                Board1 = _boardRepository.BoardToString(_boardRepository.CreateBorad(createData.size)),
                IdGame = idGame,                     
                Size = createData.size                       
            };

            var boardMap = _mapper.Map<Board>(board);

            if (!_boardRepository.CreateBoard(boardMap))
            {
                ModelState.AddModelError("", "Something went wrong creating board");
                return StatusCode(500, ModelState);
            }

            return Ok(idGame);

        }

        [HttpPut("UpdateGame")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateGame(
            [FromForm] GameDto updatedGame)
        {
            bool isUpdateNeeded = false;
            if (updatedGame == null)
                return BadRequest(ModelState);

            if (!_gameRepository.isGameExist(updatedGame.IdGame))
                return NotFound();

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Model state is not valid");
                return BadRequest(ModelState);
            }

            if (!_userRepository.isUserExist(updatedGame.IdUser) || updatedGame.DateGame <= DateTime.Now)
                return StatusCode(422);

            if (_checkDataRepository.CheckStringLengs(updatedGame.Name, 20))
                return StatusCode(422);

            var game = _gameRepository.GetGame(updatedGame.IdGame);

            if (updatedGame.Name != game.Name)
            {
                game.Name = updatedGame.Name;
                isUpdateNeeded = true;
            }

            if (updatedGame.DateGame != game.DateGame)
            {
                game.DateGame = updatedGame.DateGame;
                isUpdateNeeded = true;
            }

            if (isUpdateNeeded)
            {
                var gameMap = _mapper.Map<Game>(game);
                if (!_gameRepository.UpdateGame(gameMap))
                {
                    ModelState.AddModelError("", "Something went wrong ");
                    return StatusCode(500, ModelState);
                }
            }
            return Ok("Successfully updated");
        }

        [HttpDelete("DeleteGame")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteGame(int idGame)
        {
            var gameToDelete = _gameRepository.GetGame(idGame);
            if (gameToDelete == null)
                return NotFound();

            var boardsToDelete = _boardRepository.GetBoardsByGame(idGame)?.ToList();
            if (boardsToDelete != null && boardsToDelete.Any())
            {
                if (!_boardRepository.DeleteBoards(boardsToDelete))
                    return StatusCode(500, "Failed to delete boards.");
            }

            var teamsToDelete = _teamRepository.GetTeamsByGame(idGame)?.ToList();
            if (teamsToDelete != null && teamsToDelete.Any())
            {
                foreach (var team in teamsToDelete)
                {
                    var answeredTasksToDelete = _answeredTaskRepository.GetATsByTeam(team.IdTeam)?.ToList();
                    if (answeredTasksToDelete != null && answeredTasksToDelete.Any())
                    {
                        if (!_answeredTaskRepository.DeleteATs(answeredTasksToDelete))
                            return StatusCode(500, "Failed to delete answered tasks.");
                    }
                }
                if (!_teamRepository.DeleteTeams(teamsToDelete))
                    return StatusCode(500, "Failed to delete teams.");
            }

            var tasksToDelete = _taskRepository.GetTasksByGame(idGame)?.ToList();
            if (tasksToDelete != null && tasksToDelete.Any())
            {
                if (!_taskRepository.DeleteTasks(tasksToDelete))
                    return StatusCode(500, "Failed to delete tasks.");
            }

            if (!_gameRepository.DeleteGame(gameToDelete))
                return StatusCode(500, "Failed to delete game.");

            return NoContent();
        }
    }
}
