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

        // crud functions
        public IActionResult GetGameData(int id)
        {
            if (!_gameRepository.IsGameExist(id))
                return BadRequest("No such game");

            try
            {
                var nftm = 2;
                var nftk = 10;

                var boards = _boardRepository.GetBoardsByGame(id);
                var tasks = _taskRepository.GetTasksByGame(id);
                var teams = _teamRepository.GetTeamsByGame(id);

                if (!boards.Any())
                {
                    return BadRequest("No boards");
                }

                if (tasks.Any())
                {
                    nftk = tasks.Count;
                }

                if (teams.Any())
                {
                    nftm = teams.Count;
                }

                var size = boards.First().Size;

                var gm = _gameRepository.GetGame(id);
                if (gm != null)
                return Ok(new DataForUpdate
                {
                    Name = gm.Name,
                    Size = size,
                    NumberOfTasks = nftk,
                    NumberOfTeams = nftm,
                    IdGame = id,
                });
                return BadRequest();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return BadRequest(ex.Message);
            }
        }

        public IActionResult CheckGameData(int id)
        {
            bool gameExists = _gameRepository.IsGameExist(id);
            bool boardExists = _boardRepository.GetBoardsByGame(id).FirstOrDefault() != null;
            bool tasksExist = _taskRepository.GetTasksByGame(id).Any();
            bool teamsExist = _teamRepository.GetTeamsByGame(id).Any();

            if (gameExists && boardExists && tasksExist && teamsExist)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        public IActionResult GetAllGames(int idUser)
        {
            var allGame = _mapper.Map<List<GameDto>>(_gameRepository.GetGamesByUser(idUser));

            return Ok(allGame);
        }

        public IActionResult UpdateGameByGameData(DataForUpdate dataForUpdate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var game = _gameRepository.GetGame(dataForUpdate.IdGame);
            if (game == null)
                return BadRequest("Invalid game ID.");

            if (!_gameRepository.IsGameExist(game.IdGame))
                return NotFound();

            if (!_userRepository.IsUserExist(game.IdUser) || !_checkDataRepository.CheckStringLengs(dataForUpdate.Name, 20))
                return StatusCode(422, "Invalid data");

            var gm = _gameRepository.GetGame(dataForUpdate.IdGame);
            if (gm == null)
                return NotFound();
            var updatedGame = new GameDto
            {
                IdGame = dataForUpdate.IdGame,
                Name = dataForUpdate.Name,
                IdUser =gm.IdUser
            };
            if (!UpdateGame(game, updatedGame))
                return StatusCode(500, "Failed to update game");

            if (!UpdateBoard(dataForUpdate))
                return StatusCode(500, "Failed to update board");

            if (!UpdateTeams(dataForUpdate, game.IdGame))
                return StatusCode(500, "Failed to update teams");

            if (!UpdateTasks(dataForUpdate, game.IdGame))
                return StatusCode(500, "Failed to update tasks");

            return Ok("Successfully updated");
        }

        private bool UpdateGame(Game game, GameDto dataForUpdate)
        {
            bool isUpdateNeeded = false;

            if (dataForUpdate.Name != game.Name)
            {
                game.Name = dataForUpdate.Name;
                isUpdateNeeded = true;
            }

            if (dataForUpdate.DateGame != game.DateGame)
            {
                game.DateGame = dataForUpdate.DateGame;
                isUpdateNeeded = true;
            }

            if (isUpdateNeeded)
            {
                if (!_gameRepository.UpdateGame(_mapper.Map<Game>(game)))
                    return false;
            }

            return true;
        }

        private bool UpdateBoard(DataForUpdate dataForUpdate)
        {
            var board = _boardRepository.GetBoardsByGame(dataForUpdate.IdGame).FirstOrDefault();
            if (board != null && dataForUpdate.Size % 2 != 0 && dataForUpdate.Size >= 9 && dataForUpdate.Size <= 25 && dataForUpdate.Size != board.Size)
            {
                board.Size = dataForUpdate.Size;
                board.Board1 = _boardRepository.BoardToString(_boardRepository.CreateBorad(board.Size));

                if (!_boardRepository.UpdateBoard(board))
                    return false;
            }

            return true;
        }

        private bool UpdateTeams(DataForUpdate dataForUpdate, int gameId)
        {
            var currentNumberOfTeams = _teamRepository.GetTeamsByGame(gameId).Count;

            if (dataForUpdate.NumberOfTeams < currentNumberOfTeams)
            {
                var teamsToRemove = _teamRepository.GetTeamsByGame(gameId)
                                     .OrderByDescending(team => team.IdTeam)
                                     .Take(currentNumberOfTeams - dataForUpdate.NumberOfTeams)
                                     .ToList();

                _teamRepository.DeleteTeams(teamsToRemove);
            }
            else if (dataForUpdate.NumberOfTeams > currentNumberOfTeams)
            {
                var numberOfNewTeams = dataForUpdate.NumberOfTeams - currentNumberOfTeams;
                for (int i = 0; i < numberOfNewTeams; i++)
                {
                    var newTeam = new Team
                    {
                        Colour = "#000000",
                        Name = $"name{i}",
                        Password = BCrypt.Net.BCrypt.EnhancedHashPassword(i.ToString(), 13),
                        IdGame = gameId,
                        PositionX = (dataForUpdate.Size + 1) / 2 - 1,
                        PositionY = (dataForUpdate.Size + 1) / 2 - 1,
                        Score = 0
                    };
                    if (!_teamRepository.CreateTeam(newTeam))
                        return false;
                }
            }

            var teams = _teamRepository.GetTeamsByGame(gameId);
            foreach (var team in teams)
            {
                var newPositionX = (dataForUpdate.Size + 1) / 2 - 1;
                var newPositionY = (dataForUpdate.Size + 1) / 2 - 1;
                if (team.PositionX != newPositionX || team.PositionY != newPositionY)
                {
                    team.PositionX = newPositionX;
                    team.PositionY = newPositionY;
                    if (!_teamRepository.UpdateTeam(team))
                        return false;
                }
            }

            return true;
        }

        private bool UpdateTasks(DataForUpdate dataForUpdate, int gameId)
        {
            var currentNumberOfTasks = _taskRepository.GetTasksByGame(gameId).Count;

            if (dataForUpdate.NumberOfTasks < currentNumberOfTasks)
            {
                var tasksToRemove = _taskRepository.GetTasksByGame(gameId)
                                     .OrderByDescending(task => task.IdTask)
                                     .Take(currentNumberOfTasks - dataForUpdate.NumberOfTasks)
                                     .ToList();

                _taskRepository.DeleteTasks(tasksToRemove);
            }
            else if (dataForUpdate.NumberOfTasks > currentNumberOfTasks)
            {
                var numberOfNewTasks = dataForUpdate.NumberOfTasks - currentNumberOfTasks;
                for (int i = 0; i < numberOfNewTasks; i++)
                {
                    var newTask = new Models.Task
                    {
                        Number = i,
                        Answer = $"new ans{i}",
                        IdGame = gameId
                    };
                    if (!_taskRepository.CreateTask(newTask))
                        return false;
                }
            }

            return true;
        }

        public IActionResult CreateGame(CreateData createData)
        {
            var gameCreate = new GameDto
            {
                IdUser = createData.IdUser,
                Name = createData.Name
            };

            if (gameCreate == null || !ModelState.IsValid)
                return BadRequest();

            if (!_userRepository.IsUserExist(gameCreate.IdUser))
                return NotFound();

            if (!_checkDataRepository.CheckStringLengs(gameCreate.Name, 20))
                return StatusCode(422, "Invalid name of the game");

            if (createData.Size > 25 || createData.Size < 9 || createData.Size % 2 != 1)
                return StatusCode(422, "Invalid size");

            if (!_gameRepository.CreateGame(_mapper.Map<Game>(gameCreate)))
                return StatusCode(500, "Failed to update game");

            var idGame = _gameRepository.GetIdByName(gameCreate.Name);

            var board = new BoardDto
            {
                Board1 = _boardRepository.BoardToString(_boardRepository.CreateBorad(createData.Size)),
                IdGame = idGame,
                Size = createData.Size
            };

            if (!_boardRepository.CreateBoard(_mapper.Map<Board>(board)))
                return StatusCode(500, "Failed to create board");

            return Ok(idGame);
        }

        public IActionResult CloneGame(int idGame)
        {
            if (idGame == 0 || !_gameRepository.IsGameExist(idGame))
                return BadRequest("Invalid game ID.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var newGameId = CloneGameCore(idGame);
            if (newGameId == null)
                return StatusCode(500, "Failed to clone game");

            var boardResult = CloneBoard(idGame, newGameId.Value);
            if (!boardResult)
                return StatusCode(500, "Failed to create board");

            var teamResult = CloneTeams(idGame, newGameId.Value);
            if (!teamResult)
                return StatusCode(500, "Failed to create team");

            var taskResult = CloneTasks(idGame, newGameId.Value);
            if (!taskResult)
                return StatusCode(500, "Failed to create task");

            return Ok();
        }

        private int? CloneGameCore(int idGame)
        {
            var game1 = _gameRepository.GetGame(idGame);
            if (game1 == null)
                return null;
            var game = new Game
            {
                Name = game1.Name,
                IdUser = game1.IdUser,
            };

            if (!_gameRepository.CreateGame(game))
                return null;

            return _gameRepository.GetGames().Max(g => g.IdGame);
        }

        private bool CloneBoard(int idGame, int newGameId)
        {
            var board = _boardRepository.GetBoardsByGame(idGame).FirstOrDefault();

            if (board != null)
            {
                var newBoard = new Board
                {
                    Size = board.Size,
                    Board1 = _boardRepository.BoardToString(_boardRepository.CreateBorad(board.Size)),
                    IdGame = newGameId,
                };

                return _boardRepository.CreateBoard(newBoard);
            }

            return true; // No board to clone, so consider it a success.
        }

        private bool CloneTeams(int idGame, int newGameId)
        {
            var teams = _teamRepository.GetTeamsByGame(idGame).ToList();

            foreach (var tea in teams)
            {
                var team = new Team
                {
                    Name = tea.Name,
                    Password = tea.Password,
                    IdGame = newGameId,
                    Colour = tea.Colour,
                    PositionX = (tea.PositionX + 1) / 2 - 1,
                    PositionY = (tea.PositionY + 1) / 2 - 1,
                    Score = 0,
                };

                if (!_teamRepository.CreateTeam(team))
                    return false;
            }

            return true;
        }

        private bool CloneTasks(int idGame, int newGameId)
        {
            var tasks = _taskRepository.GetTasksByGame(idGame).ToList();

            foreach (var tas in tasks)
            {
                var task = new Models.Task
                {
                    Number = tas.Number,
                    Question = tas.Question,
                    Answer = tas.Answer,
                    IdGame = newGameId,
                };

                if (!_taskRepository.CreateTask(task))
                    return false;
            }

            return true;
        }

        public IActionResult DeleteGame(int idGame)
        {
            var gameToDelete = _gameRepository.GetGame(idGame);

            if (gameToDelete == null || !_gameRepository.IsGameExist(idGame))
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

        //functions foroperating active game

        public IActionResult StartGame(int idGame)
        {
            if (_gameRepository.IsGameExist(idGame))
            {
                var game = _gameRepository.GetGame(idGame);
                if (game != null)
                {
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
            }
            return BadRequest("there is no such game");
        }

        public IActionResult EndGame(int idGame)
        {
            if (_gameRepository.IsGameExist(idGame))
            {
                var game = _gameRepository.GetGame(idGame);
                if (game != null)
                {
                    var response = _gameManager.RemoveActiveGame(game);
                    if (response == "Removed successfully!")
                        return Ok(response);

                    return BadRequest(response);
                }
            }
            return BadRequest("there is no such game");
        }

        public IActionResult AddTeam(int idTeam)
        {
            if (_teamRepository.IsTeamExist(idTeam))
            {
                var team = _teamRepository.GetTeam(idTeam);
                if (team != null)
                {
                    var response = _gameManager.AddTeamToQueue(team);
                    if (response != "There is no such team in active games" && response != "Team is already in the queue")
                        return Ok(response);
                    return BadRequest(response);
                }
            }
            return BadRequest("There is no such team");
        }

        public IActionResult RemoveTeam(int idTeam)
        {
            var team = _teamRepository.GetTeam(idTeam);
            if (team != null)
            {
                var response = _gameManager.RemoveTeamFromQueue(team);
                if (response != "there is no such team")
                    return Ok(response);
                return BadRequest(response);
            }
            return BadRequest();
        }

        public IActionResult DateGame(int idGame)
        {
            if (_gameRepository.IsGameExist(idGame))
            {
                var game = _gameRepository.GetGame(idGame);
                if (game != null && game.DateGame != null)
                    return Ok(game.DateGame);
                return BadRequest("game didn't start");
            }
            return BadRequest("there is no such game");
        }

        public IActionResult CheckCurrent(int idTeam)
        {
            var team = _teamRepository.GetTeam(idTeam);
            if (team != null)
            {
                var response = _gameManager.CheckCurrentTeam(team.IdGame, idTeam);
                if (response != "There is no such team in active games")
                    return Ok(response);
                return BadRequest(response);
            }
            return NotFound();
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
            var response = _gameManager.UpdateTeam(_mapper.Map<Team>(team));
            if (response == "Ok")
                return Ok();
            return BadRequest(response);
        }

        public IActionResult CheckaGame(int idGame) => Ok(_gameManager.CheckGame(idGame));
    }
}
