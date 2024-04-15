using AutoMapper;
using BC_BACK.Data;
using BC_BACK.Dto;
using BC_BACK.Interfaces;
using BC_BACK.Repository;
using BC_BACK.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace BC_BACK.Services
{
    public class GameService : ControllerBase, IGameServices
    {
        private readonly IUserRepository _userRepository;
        private readonly BcDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ICheckDataRepository _checkDataRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IBoardRepository _boardRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IAnsweredTaskRepository _answeredTaskRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly GameManager _gameManager;

        public GameService(IUserRepository userRepository, IMapper mapper, 
            ICheckDataRepository checkDataRepository, IGameRepository gameRepository,
            IBoardRepository boardRepository, ITeamRepository teamRepository,
            IAnsweredTaskRepository answeredTaskRepository, ITaskRepository taskRepository,
            GameManager gameManager1, BcDbContext context)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _checkDataRepository = checkDataRepository;
            _gameRepository = gameRepository;
            _boardRepository = boardRepository;
            _teamRepository = teamRepository;
            _answeredTaskRepository = answeredTaskRepository;
            _taskRepository = taskRepository;
            _gameManager = gameManager1;
            _dbContext = context;
        }
        
        public IActionResult StartGame(int idGame)
        {
            Console.WriteLine(idGame);
            if (_gameRepository.isGameExist(idGame))
            {
                var game = _gameRepository.GetGame(idGame);
                var teams = _teamRepository.GetTeamsByGame(game.IdGame);
                var response = _gameManager.AddActiveGame(game, (List<Team>)teams);
                if (response == "Added successfully!")
                {
                    var sql = "UPDATE Game SET date_game = @DateGame WHERE id_game = @IdGame";
                    _dbContext.Database.ExecuteSqlRaw(sql,
                        new SqlParameter("@DateGame", game.DateGame),
                        new SqlParameter("@IdGame", game.IdGame));
                    return Ok();
                }
                return BadRequest(response);
            }
            return BadRequest("there is no such game");
        }

        public IActionResult EndGame(int idGame)
        {
            if (_gameRepository.isGameExist(idGame))
            {
                var game = _gameRepository.GetGame(idGame);
                var response = _gameManager.RemoveActiveGame(game);
                if (response == "Removed successfully!")
                    return Ok(response);
                return BadRequest(response);
            }
            return BadRequest("there is no such game");
        }

        public IActionResult AddTeam(int idTeam)
        {
            if (_teamRepository.isTeamExist(idTeam))
            {
                var team = _teamRepository.GetTeam(idTeam);
                var response = _gameManager.AddTeamToQueue(team);
                if (response != "There is no such team in active games" && response != "Team is already in the queue")
                    return Ok(response);
                return BadRequest(response);
            }
            return BadRequest("There is no such team");
        }

        public IActionResult RemoveTeam(int idTeam)
        {
            
                var team = _teamRepository.GetTeam(idTeam);
                var response = _gameManager.RemoveTeamFromQueue(team);
            Console.WriteLine(response);
            if (response != "there is no such team")
                    return Ok(response);
            return BadRequest(response);
        }
        public IActionResult GetAllGames(int idUser)
        {
            try
            {
                var allGame = _mapper.Map<List<GameDto>>(_gameRepository.GetGamesByUser(idUser));

                return Ok(allGame);
            }
            catch (Exception ex)
            {
                return BadRequest("");
            }
        }

        public IActionResult CloneGame(int idGame)
        {
            if (idGame == 0 || !_gameRepository.isGameExist(idGame))
                return BadRequest("Invalid game ID.");
            
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
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
            
            int id = _gameRepository.GetGames().Max(g => g.IdGame);

            var board = _boardRepository.GetBoardsByGame(idGame).FirstOrDefault();

            if (board != null)
            {
                var newb = new Board
                {
                    Size = board.Size,
                    Board1 = _boardRepository.BoardToString(_boardRepository.CreateBorad(board.Size)),
                    IdGame = id,
                };
                if (!_boardRepository.CreateBoard(newb))
                {
                    ModelState.AddModelError("", "Something went wrong creating board");
                    return StatusCode(500, ModelState);
                }
            }
            
            var teams = _teamRepository.GetTeamsByGame(idGame).ToList();
            foreach (var tea in teams)
            {
                var team = new Team
                {
                    Name = tea.Name,
                    Password = tea.Password,
                    IdGame = id,
                    Colour = tea.Colour,
                    PositionX = (board.Size + 1) / 2 - 1,
                    PositionY = (board.Size + 1) / 2 - 1,
                    Score = 0,
                };
                if (!_teamRepository.CreateTeam(team))
                {
                    ModelState.AddModelError("", "Something went wrong creating team");
                    return StatusCode(500, ModelState);
                }
            }
            
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

        public IActionResult GetGameData(int id)
        {
            if (!_gameRepository.isGameExist(id)) { return BadRequest("No such game"); }
            Console.WriteLine("ans");
            try
            {
                var data = new DataForUpdate
                {
                    Name = _gameRepository.GetGame(id).Name,
                    Size = _boardRepository.GetBoardsByGame(id).First().Size,
                    NumberOfTasks = _taskRepository.GetTasksByGame(id).Count(),
                    NumberOfTeams = _teamRepository.GetTeamsByGame(id).Count(),
                    EnQuestions = _taskRepository.GetTasksByGame(id).First().Question != null,
                    IdGame = id,
                };

                Console.WriteLine(data);

                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        public IActionResult UpdateGameByGameData(DataForUpdate dataForUpdate)
        {
            var updatedGame = new GameDto
            {
                IdGame = dataForUpdate.IdGame,
                Name = dataForUpdate.Name,
                IdUser = _gameRepository.GetGame(dataForUpdate.IdGame).IdUser
            };

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

            if (!_userRepository.isUserExist(updatedGame.IdUser))
                return StatusCode(422);

            if (!_checkDataRepository.CheckStringLengs(updatedGame.Name, 20))
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
                if (!_gameRepository.UpdateGame(game))
                {
                    ModelState.AddModelError("", "Something went wrong ");
                    return StatusCode(500, ModelState);
                }
            }

            var board = _boardRepository.GetBoardsByGame(dataForUpdate.IdGame).FirstOrDefault();
            if (board != null && dataForUpdate.Size % 2 != 0 && dataForUpdate.Size >= 9 && dataForUpdate.Size <= 25 && dataForUpdate.Size != board.Size)
            {
                board.Size = dataForUpdate.Size;
                board.Board1 = _boardRepository.BoardToString(_boardRepository.CreateBorad(board.Size));
                if (!_boardRepository.UpdateBoard(board))
                {
                    ModelState.AddModelError("", "Something went wrong ");
                    return StatusCode(500, ModelState);
                }
            }
            else if (dataForUpdate.Size != board.Size)
            {
                Console.WriteLine(board.Size + ' ' + dataForUpdate.Size);
                ModelState.AddModelError("", "Wrong data");
                return BadRequest(ModelState);

            }
            var currentNumberOfTeams = _teamRepository.GetTeamsByGame(dataForUpdate.IdGame).Count();

            if (dataForUpdate.NumberOfTeams < currentNumberOfTeams)
            {
                List<Team> teamsToRemove = _teamRepository.GetTeamsByGame(dataForUpdate.IdGame)
                                           .OrderByDescending(team => team.IdTeam)
                                           .Take(currentNumberOfTeams - dataForUpdate.NumberOfTeams)
                                           .ToList();

                _teamRepository.DeleteTeams(teamsToRemove);
            }

            if (dataForUpdate.NumberOfTeams > currentNumberOfTeams)
            {
                int numberOfNewTeams = dataForUpdate.NumberOfTeams - currentNumberOfTeams;

                for (int i = 0; i < numberOfNewTeams; i++)
                {
                    var team = new Team
                    {

                        Colour = "#000000",

                        Name = $"name{i}",

                        Password = BCrypt.Net.BCrypt.EnhancedHashPassword(i.ToString(), 13),

                        IdGame = updatedGame.IdGame,

                        PositionX = (board.Size + 1) / 2 - 1,

                        PositionY = (board.Size + 1) / 2 - 1,

                        Score = 0
                    };

                    _teamRepository.CreateTeam(team);
                }
            }

            var teams = _teamRepository.GetTeamsByGame(updatedGame.IdGame);
            foreach (var team in teams)
            {
                if (team.PositionX != (board.Size + 1) / 2 - 1)
                {
                    team.PositionX = (board.Size + 1) / 2 - 1;
                    team.PositionY = (board.Size + 1) / 2 - 1;
                    _teamRepository.UpdateTeam(team);
                }
            }

            var currentNumberOfTasks = _taskRepository.GetTasksByGame(dataForUpdate.IdGame).Count();
            Console.WriteLine($"ADW {currentNumberOfTasks} ' '  {dataForUpdate.NumberOfTasks}");
            if (dataForUpdate.NumberOfTasks < currentNumberOfTasks)
            {
                List<Models.Task> tasksToRemove = _taskRepository.GetTasksByGame(dataForUpdate.IdGame)
                                           .OrderByDescending(team => team.IdTask)
                                           .Take(currentNumberOfTasks - dataForUpdate.NumberOfTasks)
                                           .ToList();

                _taskRepository.DeleteTasks(tasksToRemove);
            }

            if (dataForUpdate.NumberOfTasks > currentNumberOfTasks)
            {
                Console.WriteLine(4);
                int numberOfNewTasks = dataForUpdate.NumberOfTasks - currentNumberOfTasks;

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

        public IActionResult CreateGame(CreateData createData)
        {
            var gameCreate = new GameDto
            {
                IdUser = createData.IdUser,
                Name = createData.Name
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

            if (createData.Size > 25 || createData.Size < 9 || createData.Size % 2 != 1)
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
                Board1 = _boardRepository.BoardToString(_boardRepository.CreateBorad(createData.Size)),
                IdGame = idGame,
                Size = createData.Size
            };
            var boardMap = _mapper.Map<Board>(board);
            

            if (!_boardRepository.CreateBoard(boardMap))
            {
                ModelState.AddModelError("", "Something went wrong creating board");
                return StatusCode(500, ModelState);
            }

            return Ok(idGame);
        }

        public IActionResult DateGame(int idGame)
        {
            if (_gameRepository.isGameExist(idGame))
            {
                var game = _gameRepository.GetGame(idGame);
                Console.WriteLine(game.DateGame);
                if (game.DateGame != null)
                    return Ok(game.DateGame);
                return BadRequest("game didn't start");
            }
            return BadRequest("there is no such game");
        }

        public IActionResult CheckCurrent(int idTeam)
        {
                var team = _teamRepository.GetTeam(idTeam);
                var response = _gameManager.CheckCurrentTeam(team.IdGame,idTeam);
                if (response != "There is no such team in active games")
                    return Ok(response);
                return BadRequest(response);
        }

        public IActionResult GetCurrentTeam(int idTeam, int idGame)
        {
            var response = _gameManager.GetTeam(idGame, idTeam);
            if (response != null)
                return Ok(response);
            return BadRequest("there is no team");
        }

        public IActionResult GetGameTeams(int idGame)
        {
           var response = _gameManager.GetGameTeams(idGame);
            if (response != null)
            return Ok(response);
            return BadRequest("there is no team");
        }

        public IActionResult UpdateCurrentTeam(TeamDto team)
        {
            Team teamC = _mapper.Map<Team>(team);
            var response = _gameManager.UpdateTeam(teamC);
            if (response == "Ok") 
                return Ok();
            return BadRequest(response);
        }

        public IActionResult CheckaGame(int idGame)
        {
            return Ok(_gameManager.CheckGame(idGame));
        }
    }
}
