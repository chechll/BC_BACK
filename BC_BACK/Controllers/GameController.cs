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

        public class DataForUpdate
        {
            public string name { get; set; }
            public int size { get; set; }
            public int numberOfTeams { get; set; }
            public int numberOfTasks { get; set; }
            public bool enQuestions { get; set; }
            public int idGame { get; set; }
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

        [HttpGet("GetGameData")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Game>))]
        [ProducesResponseType(400)]
        public IActionResult GetGameData(int id)
        {

            try
            {
                var data = new DataForUpdate
                {
                    name = _gameRepository.GetGame(id).Name,
                    size = _boardRepository.GetBoardsByGame(id).First().Size,
                    numberOfTasks = _taskRepository.GetTasksByGame(id).Count(),
                    numberOfTeams = _teamRepository.GetTeamsByGame(id).Count(),
                    enQuestions = _taskRepository.GetTasksByGame(id).First().Question != null,
                    idGame = id,
                };



                return Ok(data);
            } catch (Exception ex)
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

        [HttpGet("CloneGame")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CloneGame(int idGame)
        {
            Console.WriteLine(idGame);
            if (idGame == 0 || !_gameRepository.isGameExist(idGame))
                return BadRequest("Invalid game ID.");
            Console.WriteLine(2);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            Console.WriteLine(3);
            var game1 = _gameRepository.GetGame(idGame);
            var game = new Game 
            {
                Name = game1.Name,
                IdUser = game1.IdUser,
            };

            if (!_gameRepository.CreateGame(game))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }
            Console.WriteLine(4);
            int id = _gameRepository.GetGames().Max(g=>g.IdGame);

            var board = _boardRepository.GetBoardsByGame(idGame).FirstOrDefault();

            if (board != null)
            {
                var newb = new Board
                {
                    Size = board.Size,
                    Board1 = board.Board1,
                    IdGame = id,
                };
                if (!_boardRepository.CreateBoard(newb))
                {
                    ModelState.AddModelError("", "Something went wrong creating board");
                    return StatusCode(500, ModelState);
                }
            }
            Console.WriteLine(5);
            var teams = _teamRepository.GetTeamsByGame(idGame).ToList();
            foreach (var tea in teams)
            {
                var team = new Team
                {
                    Name = tea.Name,
                    Password = tea.Password,
                    IdGame = id,
                    Colour = tea.Colour,
                    Score = 0,
                };
                if (!_teamRepository.CreateTeam(team))
                {
                    ModelState.AddModelError("", "Something went wrong creating team");
                    return StatusCode(500, ModelState);
                }
            }
            Console.WriteLine(6);
            var tasks = _taskRepository.GetTasksByGame(idGame).ToList();
            foreach (var tas in tasks)
            {
                var task = new Models.Task
                {
                    Number = tas.Number,
                    Question = tas.Question,
                    Answer = tas.Answer,
                    IdGame = id,
                };
                if (!_taskRepository.CreateTask(task))
                {
                    ModelState.AddModelError("", "Something went wrong creating task");
                    return StatusCode(500, ModelState);
                }
            }

            return Ok();

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

            if (createData.size > 25 || createData.size < 9 || createData.size % 2 != 1)
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

        [HttpPut("UpdateGameByGameData")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateGameByGameData(
            [FromBody] DataForUpdate dataForUpdate)
        {

            Console.WriteLine(1);
            var updatedGame = new GameDto
            {
                IdGame = dataForUpdate.idGame,
                Name = dataForUpdate.name,
                IdUser = _gameRepository.GetGame(dataForUpdate.idGame).IdUser
            };

            bool isUpdateNeeded = false;
            if (updatedGame == null)
                return BadRequest(ModelState);
            Console.WriteLine(1);

            if (!_gameRepository.isGameExist(updatedGame.IdGame))
                return NotFound();

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Model state is not valid");
                return BadRequest(ModelState);
            }
            Console.WriteLine(1);

            if (!_userRepository.isUserExist(updatedGame.IdUser))
                return StatusCode(422);

            if (!_checkDataRepository.CheckStringLengs(updatedGame.Name, 20))
                return StatusCode(422);
            Console.WriteLine(1);

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

            Console.WriteLine(1);
            var board = _boardRepository.GetBoardsByGame(dataForUpdate.idGame).FirstOrDefault();
            if (board != null && dataForUpdate.size % 2 != 0 && dataForUpdate.size >= 9 && dataForUpdate.size <= 25 && dataForUpdate.size != board.Size)
            {
                board.Size = dataForUpdate.size;
                if (!_boardRepository.UpdateBoard(board))
                {
                    ModelState.AddModelError("", "Something went wrong ");
                    return StatusCode(500, ModelState);
                }
            }
            else if(dataForUpdate.size != board.Size)
            {
                Console.WriteLine(board.Size + ' ' + dataForUpdate.size);
                ModelState.AddModelError("", "Wrong data");
                return BadRequest(ModelState);

            }
            Console.WriteLine(1);
            var currentNumberOfTeams = _teamRepository.GetTeamsByGame(dataForUpdate.idGame).Count();

            if (dataForUpdate.numberOfTeams < currentNumberOfTeams)
            {
                List<Team> teamsToRemove = _teamRepository.GetTeamsByGame(dataForUpdate.idGame)
                                           .OrderByDescending(team => team.IdTeam)
                                           .Take(currentNumberOfTeams - dataForUpdate.numberOfTeams)
                                           .ToList();

                _teamRepository.DeleteTeams(teamsToRemove);
            }

            if (dataForUpdate.numberOfTeams > currentNumberOfTeams)
            {
                int numberOfNewTeams = dataForUpdate.numberOfTeams - currentNumberOfTeams;

                for (int i = 0; i < numberOfNewTeams; i++)
                {
                    var team = new Team
                    {

                        Colour = "#000000",

                        Name = $"name{i}",

                        Password = BCrypt.Net.BCrypt.EnhancedHashPassword(i.ToString(), 13),

                        IdGame = updatedGame.IdGame,

                        PositionX = (board.Size + 1)/2 - 1,

                        PositionY = (board.Size + 1)/2 - 1,

                        Score = 0
                    };

                    _teamRepository.CreateTeam(team);
                }


            }

            var teams = _teamRepository.GetTeamsByGame(updatedGame.IdGame);
            foreach (var team in teams)
            {
                if(team.PositionX != (board.Size+1)/2 -1)
                {
                    team.PositionX = (board.Size + 1) / 2 - 1;
                    team.PositionY = (board.Size + 1) /2 - 1;
                    _teamRepository.UpdateTeam(team);
                }
            }

            var currentNumberOfTasks = _taskRepository.GetTasksByGame(dataForUpdate.idGame).Count();
            Console.WriteLine($"ADW {currentNumberOfTasks} ' '  {dataForUpdate.numberOfTasks}");
            if (dataForUpdate.numberOfTasks < currentNumberOfTasks)
            {
                List<Models.Task> tasksToRemove = _taskRepository.GetTasksByGame(dataForUpdate.idGame)
                                           .OrderByDescending(team => team.IdTask)
                                           .Take(currentNumberOfTasks - dataForUpdate.numberOfTasks)
                                           .ToList();

                _taskRepository.DeleteTasks(tasksToRemove);
            }

            if (dataForUpdate.numberOfTasks > currentNumberOfTasks)
            {
                Console.WriteLine(4);
                int numberOfNewTasks = dataForUpdate.numberOfTasks - currentNumberOfTasks;

                for (int i = 0; i < numberOfNewTasks; i++)
                {
                    var task = new Models.Task
                    {
                        Number = i,

                        Answer = $"new ans{i}",

                        IdGame = updatedGame.IdGame
                    };
                    Console.WriteLine(3);
                    _taskRepository.CreateTask(task);
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
