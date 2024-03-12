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
        public IActionResult CreateGame([FromBody] GameDto gameCreate)
        {
            if (gameCreate == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_userRepository.isUserExist(gameCreate.IdUser) || gameCreate.DateGame <= DateTime.Now)
                return StatusCode(422);

            if (_checkDataRepository.CheckStringLengs(gameCreate.Name, 20))
                return StatusCode(422);

            var gameMap = _mapper.Map<Game>(gameCreate);

            if (!_gameRepository.CreateGame(gameMap))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }

            return Ok();

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

            if (!_gameRepository.isGameExist(idGame))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var gameToDelete = _gameRepository.GetGame(idGame);

            var boardToDelete = _boardRepository.GetBoardsByGame(gameToDelete.IdGame).ToList();
            if (!_boardRepository.DeleteBoards(boardToDelete))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }

            var teamsToDelete = _teamRepository.GetTeamsByGame(gameToDelete.IdGame).ToList();
            foreach (var team in teamsToDelete)
            {
                var ansToDelete = _answeredTaskRepository.GetATsByTeam(team.IdTeam).ToList();
                if (_answeredTaskRepository.DeleteATs(ansToDelete))
                {
                    ModelState.AddModelError("", "Something went wrong");
                    return StatusCode(500, ModelState);
                }
            }

            if (!_teamRepository.DeleteTeams(teamsToDelete))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }

            var tasksToDelete = _taskRepository.GetTasksByGame(gameToDelete.IdGame).ToList();
            if (!_taskRepository.DeleteTasks(tasksToDelete))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }


            if (!_gameRepository.DeleteGame(gameToDelete))
            {
                ModelState.AddModelError("", "Something went wrong ");
                return StatusCode(500, ModelState);
            }

            return Ok(0);

        }
    }
}
