using AutoMapper;
using BC_BACK.Dto;
using BC_BACK.Interfaces;
using BC_BACK.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BC_BACK.Services
{

    public class UserService : ControllerBase, IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ICheckDataRepository _checkDataRepository;
        private readonly IGameRepository _gameRepository;
        private readonly IBoardRepository _boardRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IAnsweredTaskRepository _answeredTaskRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IJwtService _jwtService;

        public UserService(IUserRepository userRepository, IMapper mapper,
            ICheckDataRepository checkDataRepository, IGameRepository gameRepository,
            IBoardRepository boardRepository, ITeamRepository teamRepository,
            IAnsweredTaskRepository answeredTaskRepository, ITaskRepository taskRepository,
            IJwtService jwtService) 
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _checkDataRepository = checkDataRepository;
            _gameRepository = gameRepository;
            _boardRepository = boardRepository;
            _teamRepository = teamRepository;
            _answeredTaskRepository = answeredTaskRepository;
            _taskRepository = taskRepository;
            _jwtService = jwtService;
        }

        public IActionResult GetUser(int id)
        {
            if (!_userRepository.isUserExist(id))
                return NotFound();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = _userRepository.GetUser(id);

            return Ok(user);
        }

        public IActionResult GetAllUsers()
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

        public IActionResult CreateUser(UserDto userCreate)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (userCreate == null)
                return BadRequest();

            var existingUser = _userRepository.GetUsers().Where(c => c.Username == userCreate.Username).FirstOrDefault();

            if (existingUser != null)
                return StatusCode(422, ModelState);

            userCreate.Rights = 0;
            userCreate.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(userCreate.Password, 13);

            if (!_userRepository.CreateUser(_mapper.Map<User>(userCreate)))
                return StatusCode(500, "Failed to create user");

            var userId = _userRepository.GetId(userCreate.Username);

            var user = _userRepository.GetUser(userId);

            return Ok(new OperatingData
            {
                IdUser = userId,
                Rights = user.Rights,
                Token = _jwtService.GenerateToken(user.IdUser.ToString())
            });
        }

        public IActionResult DeleteUser(int userId)
        {
            if (!_userRepository.isUserExist(userId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userToDelete = _userRepository.GetUser(userId);

            var gamesToDelete = _gameRepository.GetGamesByUser(userId).ToList();

                foreach (var game in gamesToDelete)
                {
                    var boardToDelete = _boardRepository.GetBoardsByGame(game.IdGame).ToList();
                    if (!_boardRepository.DeleteBoards(boardToDelete))
                        return StatusCode(500, "Failed to delete boards");

                    var teamsToDelete = _teamRepository.GetTeamsByGame(game.IdGame).ToList();
                    foreach (var team in teamsToDelete)
                    {
                        var ansToDelete = _answeredTaskRepository.GetATsByTeam(team.IdTeam).ToList();
                        if (_answeredTaskRepository.DeleteATs(ansToDelete))
                            return StatusCode(500, "Failed to delete ans");
                    }

                    if (!_teamRepository.DeleteTeams(teamsToDelete))
                        return StatusCode(500, "Failed to delete teams");

                    var tasksToDelete = _taskRepository.GetTasksByGame(game.IdGame).ToList();
                    if (!_taskRepository.DeleteTasks(tasksToDelete))
                        return StatusCode(500, "Failed to delete tasks");

                    if (!_gameRepository.DeleteGame(game))
                        return StatusCode(500, "Failed to delete game");
            }
            

            if (!_userRepository.DeleteUser(userToDelete))
                return StatusCode(500, "Failed to delete user");

            return Ok(0);
        }

        public IActionResult LogIn(string username, string user_password)
        {
            int id = _userRepository.GetId(username);

            if (!_userRepository.isUserExist(id))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_checkDataRepository.CheckStringLengs(username, 30))
                return StatusCode(422, "your mail length is more then 30");

            if (!_checkDataRepository.CheckStringLengs(user_password, 30))
                return StatusCode(422, "your password length is more then 30");

            var user = _mapper.Map<UserDto>(_userRepository.GetUser(id));

            if (BCrypt.Net.BCrypt.EnhancedVerify(user_password, user.Password))
            {
                var token = _jwtService.GenerateToken(user.IdUser.ToString());

                return Ok(new OperatingData
                {
                    IdUser = id,
                    Rights = user.Rights,
                    Token = token
                });
            }

            return Unauthorized();
        }

        public IActionResult UpdateUser(UserDto updatedUser)
        {
            if (!ModelState.IsValid || updatedUser == null)
                return BadRequest(ModelState);

            if (!_userRepository.isUserExist(updatedUser.IdUser))
                return NotFound();

            if (updatedUser.IdUser != _userRepository.GetId(updatedUser.Username) && _userRepository.isUserExist(_userRepository.GetId(updatedUser.Username)))
                return StatusCode(422, "there is a user with such username");

            if (updatedUser.Rights < 0 || updatedUser.Rights > 2)
                return StatusCode(422, "Invalid rights value");

            var user = _userRepository.GetUser(updatedUser.IdUser);

            bool isUpdateNeeded = false;

            if (updatedUser.Username != user.Username)
            {
                user.Username = updatedUser.Username;
                isUpdateNeeded = true;
            }

            if (updatedUser.Password != null && !BCrypt.Net.BCrypt.EnhancedVerify(updatedUser.Password, user.Password))
            {
                user.Password = BCrypt.Net.BCrypt.EnhancedHashPassword(updatedUser.Password, 13);
                isUpdateNeeded = true;
            }

            if (isUpdateNeeded)
            {
                if (!_userRepository.UpdateUser(_mapper.Map<User>(user)))
                    return StatusCode(500, "Failed to update user");
            }
            return Ok("Successfully updated");
        }

        public IActionResult UpdateUserRights(UserDto updatedUser)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (updatedUser == null)
                return BadRequest("Updated user is null");

            if (!_userRepository.isUserExist(updatedUser.IdUser))
                return NotFound();

            if (updatedUser.Rights < 0 || updatedUser.Rights > 2)
                return StatusCode(422, "Invalid rights value");

            var user = _userRepository.GetUser(updatedUser.IdUser);

            if (updatedUser.Rights != user.Rights)
            {
                user.Rights = updatedUser.Rights;

                if (!_userRepository.UpdateUser(_mapper.Map<User>(user)))
                    return StatusCode(500, "Failed to update user Rights");

                return Ok("Successfully updated");
            }

            return Ok();
        }
    }
}
