using AutoMapper;
using BCrypt.Net;
using BC_BACK.Dto;
using BC_BACK.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BC_BACK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ICheckDataRepository _checkDataRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IBoardRepository _boardRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IAnsweredTaskRepository _answeredTaskRepository; 
        private readonly ITaskRepository _taskRepository;

        public UserController(IUserRepository userRepository, IMapper mapper, 
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

        public class OperatingData
        {
            public int idUser { get; set; }
            public int Rights { get; set; }
        }

        [HttpGet("GetAllUsers")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        [ProducesResponseType(400)]
        public IActionResult GetAllErrors()
        {
            try
            {
                var alluser = _userRepository.GetUsers();

                return Ok(alluser);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("LogIn")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        [ProducesResponseType(400)]
        public IActionResult LogIn(string username, string user_password)
        {
            Console.WriteLine("1");
            Console.WriteLine($"{username} + {user_password}");
            int id = _userRepository.GetId(username);
            if (!_userRepository.isUserExist(id))
                return NotFound();
            var user = _mapper.Map<UserDto>(_userRepository.GetUser(id));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_checkDataRepository.CheckStringLengs(username, 30))
            {
                ModelState.AddModelError("", "your mail length is more then 30");
                return StatusCode(422, ModelState);
            }

            if (!_checkDataRepository.CheckStringLengs(user_password, 30))
            {
                ModelState.AddModelError("", "your password length is more then 30");
                return StatusCode(422, ModelState);
            }

            if (BCrypt.Net.BCrypt.EnhancedVerify(user_password, user.Password))
            {
                var response = new OperatingData
                {
                    idUser = id,
                    Rights = user.Rights
                };

                return Ok(response);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet("GetUser")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        [ProducesResponseType(400)]
        public IActionResult GetUser(int id)
        {
            if (!_userRepository.isUserExist(id))
                return NotFound();

            var user = _mapper.Map<UserDto>(_userRepository.GetUser(id)); ;

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(user);
        }

        [HttpPost("SignUp")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateUser([FromBody] UserDto userCreate)
        {
            if (userCreate == null)
                return BadRequest();

            var user = _userRepository.GetUsers().Where(c => c.Username == userCreate.Username).FirstOrDefault();

            if (user != null)
            {
                ModelState.AddModelError("", "user already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            userCreate.Rights = 0;
            if (userCreate.Rights != 0 && userCreate.Rights != null)
            {
                ModelState.AddModelError("", "You have no right to do it");
                return StatusCode(422, ModelState);
            }

            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(userCreate.Password, 13);
            userCreate.Password = passwordHash;

            var userMap = _mapper.Map<User>(userCreate);

            if (!_userRepository.CreateUser(userMap))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }

            var userId = _userRepository.GetId(userCreate.Username);

            user = _userRepository.GetUser(userId);

            var response = new OperatingData
            {
                idUser = userId,
                Rights = user.Rights
            };

            return Ok(response);

        }

        [HttpPut("Update")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateUser(
            [FromForm] UserDto updatedUser)
        {
            bool isUpdateNeeded = false;
            if (updatedUser == null)
                return BadRequest(ModelState);

            if (!_userRepository.isUserExist(updatedUser.IdUser))
                return NotFound();

            if (updatedUser.IdUser != _userRepository.GetId(updatedUser.Username) && _userRepository.isUserExist(_userRepository.GetId(updatedUser.Username)))
                return BadRequest(ModelState);

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Model state is not valid");
                return BadRequest(ModelState);
            }

            if (updatedUser.Rights > 2 && updatedUser.Rights < 0)
            {
                ModelState.AddModelError("", "there is no such isAdmin");
                return StatusCode(422, ModelState);
            }

            var user = _userRepository.GetUser(updatedUser.IdUser);


            if (updatedUser.Username != user.Username)
            {
                user.Username = updatedUser.Username;
                isUpdateNeeded = true;
            }

            if (updatedUser.Rights != user.Rights)
            {
                user.Rights = updatedUser.Rights;
                isUpdateNeeded = true;
            }

            if (updatedUser.Password != user.Password)
            {
                var changeMail = BCrypt.Net.BCrypt.EnhancedVerify(updatedUser.Password, user.Password);
                if (!changeMail)
                {
                    string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(updatedUser.Password, 13);
                    user.Password = passwordHash;
                    isUpdateNeeded = true;
                }
            }


            if (isUpdateNeeded)
            {
                var userMap = _mapper.Map<User>(user);
                if (!_userRepository.UpdateUser(userMap))
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
        public IActionResult DeleteUser(int userId)
        {
            
            if (!_userRepository.isUserExist(userId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userToDelete = _userRepository.GetUser(userId);

            var gamesToDelete = _gameRepository.GetGamesByUser(userId).ToList();
            
            if (gamesToDelete != null && gamesToDelete.Any())
            {
                foreach (var game in gamesToDelete)
                {
                    var boardToDelete = _boardRepository.GetBoardsByGame(game.IdGame).ToList();
                    if (!_boardRepository.DeleteBoards(boardToDelete))
                    {
                        ModelState.AddModelError("", "Something went wrong");
                        return StatusCode(500, ModelState);
                    }
                    
                    var teamsToDelete = _teamRepository.GetTeamsByGame(game.IdGame).ToList();
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

                    var tasksToDelete = _taskRepository.GetTasksByGame(game.IdGame).ToList();
                    if (!_taskRepository.DeleteTasks(tasksToDelete))
                    {
                        ModelState.AddModelError("", "Something went wrong");
                        return StatusCode(500, ModelState);
                    }

                }
            }

            if (!_userRepository.DeleteUser(userToDelete))
            {
                ModelState.AddModelError("", "Something went wrong ");
                return StatusCode(500, ModelState);
            }
            
            return Ok(0);
            
        }
    }
}
