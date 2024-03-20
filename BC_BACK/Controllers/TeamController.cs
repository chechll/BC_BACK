using AutoMapper;
using BC_BACK.Dto;
using BC_BACK.Interfaces;
using BC_BACK.Models;
using BC_BACK.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

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

        [HttpGet("GetAllTeams")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Team>))]
        [ProducesResponseType(400)]
        public IActionResult GetAllTeams()
        {
            try
            {
                var allTeams = _teamRepository.GetTeams();

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

        [HttpPost("CreateTeams")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateTeams([FromBody] List<TeamDto> teamCreates)
        {
            Console.WriteLine("mcdvmsd");
            if (teamCreates == null || !teamCreates.Any())
            {
                return BadRequest("No teams provided");
            }
            Console.WriteLine("mcdvmsd");
            if (teamCreates.Count() > 6 || teamCreates.Count() < 2)
            {
                return BadRequest("Wrong number of teams provided");
            }
            Console.WriteLine("mcdvmsd");
            foreach (var teamCreate in teamCreates)
            {
                if (teamCreate == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_gameRepository.isGameExist(teamCreate.IdGame))
                    return BadRequest();

                var size = _teamRepository.GetBoardSize(teamCreate.IdGame);

                if (_checkDataRepository.CheckStringLengs(teamCreate.Name, 20) || size < teamCreate.PositionY || teamCreate.PositionY < 0 ||
                    teamCreate.PositionX > 0 || teamCreate.PositionX > size)
                    return StatusCode(422);

                string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(teamCreate.Password, 13);
                teamCreate.Password = passwordHash;

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

            if (_checkDataRepository.CheckStringLengs(teamCreate.Name, 20) || size < teamCreate.PositionY || teamCreate.PositionY < 0 ||
                teamCreate.PositionX > 0 || teamCreate.PositionX > size)
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

        [HttpPut("UpdateTeam")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateTeam(
            [FromForm] TeamDto updatedTeam)
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

            if (_checkDataRepository.CheckStringLengs(updatedTeam.Name, 20) || size  < updatedTeam.PositionY || updatedTeam.PositionY < 0 ||
                updatedTeam.PositionX > 0 || updatedTeam.PositionX > size)
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

            if (updatedTeam.Password != team.Password)
            {
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
