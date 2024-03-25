using AutoMapper;
using BC_BACK.Dto;
using BC_BACK.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BC_BACK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : Controller
    {
        public readonly IMapper _mapper;
        public readonly ITeamRepository _teamRepository;
        public readonly IGameRepository _gameRepository;
        public readonly IAnsweredTaskRepository _answeredTaskRepository;
        public readonly ICheckDataRepository _checkDataRepository;

        public TeamController(IMapper mapper, ITeamRepository teamRepository,
            IGameRepository gameRepository, IAnsweredTaskRepository answeredTaskRepository, ICheckDataRepository checkDataRepository)
        {
            _gameRepository = gameRepository;
            _mapper = mapper;
            _teamRepository = teamRepository;
            _answeredTaskRepository = answeredTaskRepository;
            _checkDataRepository = checkDataRepository;
        }

        [HttpGet("GetTeams")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Team>))]
        [ProducesResponseType(400)]
        public IActionResult GetAllTeams(int idGame)
        {
            try
            {
                var allTeams = _teamRepository.GetTeamsByGame(idGame);

                return Ok(allTeams);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("GetTeam")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Team>))]
        [ProducesResponseType(400)]
        public IActionResult GetTeam(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_teamRepository.isTeamExist(id))
                return NotFound();

            var team = _mapper.Map<TeamDto>(_teamRepository.GetTeam(id)); ;

            return Ok(team);
        }

        [HttpGet("LogIn")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        [ProducesResponseType(400)]
        public IActionResult LogIn(string password, int id)
        {
            if (!_teamRepository.isTeamExist(id))
                return NotFound();
            var team = _mapper.Map<TeamDto>(_teamRepository.GetTeam(id));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_checkDataRepository.CheckStringLengs(password, 30))
            {
                ModelState.AddModelError("", "your password length is more then 30");
                return StatusCode(422, ModelState);
            }

            if (BCrypt.Net.BCrypt.EnhancedVerify(password, team.Password))
            {
                return Ok(id);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("CreateTeams")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateTeams([FromBody] List<TeamDto> teamCreates)
        {

            if (teamCreates == null || !teamCreates.Any())
            {
                return BadRequest("No teams provided");
            }
            if (teamCreates.Count() > 6 || teamCreates.Count() < 2)
            {
                return BadRequest("Wrong number of teams provided");
            }

            foreach (var teamCreate in teamCreates)
            {
                if (teamCreate == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_gameRepository.isGameExist(teamCreate.IdGame))
                    return BadRequest();

                var size = _teamRepository.GetBoardSize(teamCreate.IdGame);

                if (!_checkDataRepository.CheckStringLengs(teamCreate.Name, 20) || size < teamCreate.PositionY || teamCreate.PositionY < 0 ||
                teamCreate.PositionX < 0 || teamCreate.PositionX > size)
                    return StatusCode(422);

                string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(teamCreate.Password, 13);
                teamCreate.Password = passwordHash;

                teamCreate.PositionX = (_teamRepository.GetBoardSize(teamCreate.IdGame) + 1) / 2 - 1;
                teamCreate.PositionY = (_teamRepository.GetBoardSize(teamCreate.IdGame) + 1) / 2 - 1;

                var teamMap = _mapper.Map<Team>(teamCreate);

                if (!_teamRepository.CreateTeam(teamMap))
                {
                    ModelState.AddModelError("", "Something went wrong");
                    return StatusCode(500, ModelState);
                }
            }

            return Ok();
        }

        [HttpPost("CreateTeam")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateTeam([FromBody] TeamDto teamCreate)
        {
            if (teamCreate == null)
                return BadRequest();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (!_gameRepository.isGameExist(teamCreate.IdGame))
                return BadRequest();
            var size = _teamRepository.GetBoardSize(teamCreate.IdGame);

            if (!_checkDataRepository.CheckStringLengs(teamCreate.Name, 20) || size < teamCreate.PositionY || teamCreate.PositionY < 0 ||
                teamCreate.PositionX < 0 || teamCreate.PositionX > size)
                return StatusCode(422);

            string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(teamCreate.Password, 13);
            teamCreate.Password = passwordHash;

            var teamMap = _mapper.Map<Team>(teamCreate);

            if (!_teamRepository.CreateTeam(teamMap))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }

            return Ok();

        }

        [HttpPut("UpdateTeams")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateTeams(
            [FromBody] List<TeamDto> updatTeam)
        {
            if (updatTeam == null || !updatTeam.Any())
            {
                return BadRequest("No teams provided");
            }
            if (updatTeam.Count() > 6 || updatTeam.Count() < 2)
            {
                return BadRequest("Wrong number of teams provided");
            }

            foreach (var updatedTeam in updatTeam)
            {
                bool isUpdateNeeded = false;
                if (updatedTeam == null)
                    return BadRequest(ModelState);

                if (!_teamRepository.isTeamExist(updatedTeam.IdTeam))
                    return NotFound();

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Model state is not valid");
                    return BadRequest(ModelState);
                }

                int size = _teamRepository.GetBoardSize(updatedTeam.IdGame);

                if (!_checkDataRepository.CheckStringLengs(updatedTeam.Name, 20))
                    return StatusCode(422);

                var team = _teamRepository.GetTeam(updatedTeam.IdTeam);

                if (updatedTeam.Name != team.Name)
                {
                    team.Name = updatedTeam.Name;
                    isUpdateNeeded = true;
                }

                if (updatedTeam.PositionX != team.PositionX)
                {
                    team.PositionX = updatedTeam.PositionX;
                    isUpdateNeeded = true;
                }

                if (updatedTeam.PositionY != team.PositionY)
                {
                    team.PositionY = updatedTeam.PositionY;
                    isUpdateNeeded = true;
                }
                if (updatedTeam.Colour != team.Colour)
                {
                    team.Colour = updatedTeam.Colour;
                    isUpdateNeeded = true;
                }

                if (updatedTeam.Steps != team.Steps)
                {
                    team.Steps = updatedTeam.Steps;
                    isUpdateNeeded = true;
                }

                if (updatedTeam.Password != team.Password)
                {
                    Console.WriteLine($"{updatedTeam.Password} {team.Password}");
                    var changeMail = BCrypt.Net.BCrypt.EnhancedVerify(updatedTeam.Password, team.Password);
                    if (!changeMail)
                    {
                        string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(updatedTeam.Password, 13);
                        team.Password = passwordHash;
                        isUpdateNeeded = true;
                    }
                }

                if (isUpdateNeeded)
                {
                    var teamMap = _mapper.Map<Team>(team);
                    if (!_teamRepository.UpdateTeam(teamMap))
                    {
                        ModelState.AddModelError("", "Something went wrong ");
                        return StatusCode(500, ModelState);
                    }
                }
            }
            return Ok("Successfully updated");
        }

        [HttpPut("UpdateTeam")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateTeam(
            [FromBody] TeamDto updatedTeam)
        {
            if (updatedTeam == null)
            {
                return BadRequest("No teams provided");
            }

            bool isUpdateNeeded = false;
            if (updatedTeam == null)
                return BadRequest(ModelState);

            if (!_teamRepository.isTeamExist(updatedTeam.IdTeam))
                return NotFound();

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Model state is not valid");
                return BadRequest(ModelState);
            }

            if (!_checkDataRepository.CheckStringLengs(updatedTeam.Name, 20))
                return StatusCode(422);

            var team = _teamRepository.GetTeam(updatedTeam.IdTeam);

            if (updatedTeam.PositionX != team.PositionX)
            {
                team.PositionX = updatedTeam.PositionX;
                isUpdateNeeded = true;
            }

            if (updatedTeam.PositionY != team.PositionY)
            {
                team.PositionY = updatedTeam.PositionY;
                isUpdateNeeded = true;
            }
            if (updatedTeam.Steps != team.Steps)
            {
                team.Steps = updatedTeam.Steps;
                isUpdateNeeded = true;
            }
            if ( updatedTeam.Score != team.Score)
            {
                team.Score = updatedTeam.Score;
                isUpdateNeeded = true;
            }

            if (isUpdateNeeded)
            {
                var teamMap = _mapper.Map<Team>(team);
                if (!_teamRepository.UpdateTeam(teamMap))
                {
                    ModelState.AddModelError("", "Something went wrong ");
                    return StatusCode(500, ModelState);
                }
            }

            return Ok("Successfully updated");
        }

        [HttpDelete("DeleteTeam")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteBoard(int teamId)
        {

            if (!_teamRepository.isTeamExist(teamId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ansToDelete = _answeredTaskRepository.GetATsByTeam(teamId).ToList();
            if (_answeredTaskRepository.DeleteATs(ansToDelete))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }

            var teamToDelete = _teamRepository.GetTeam(teamId);

            if (!_teamRepository.DeleteTeam(teamToDelete))
            {
                ModelState.AddModelError("", "Something went wrong ");
                return StatusCode(500, ModelState);
            }

            return Ok(0);

        }
    }
}
