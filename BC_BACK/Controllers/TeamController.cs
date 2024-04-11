using BC_BACK.Dto;
using BC_BACK.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BC_BACK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamController : Controller
    {
        public readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }
        [Authorize]
        [HttpGet("GetTeams")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Team>))]
        [ProducesResponseType(400)]
        public IActionResult GetAllTeams(int idGame)
        {
            return _teamService.GetAllTeams(idGame);
        }

        [HttpGet("LogIn")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<User>))]
        [ProducesResponseType(400)]
        public IActionResult LogIn(string password, int id)
        {
            return _teamService.LogIn(password, id);
        }
        [Authorize]
        [HttpPost("CreateTeams")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult CreateTeams([FromBody] List<TeamDto> teamCreates)
        {
            return _teamService.CreateTeams(teamCreates);
        }
        [Authorize]
        [HttpPut("UpdateTeams")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult UpdateTeams(
            [FromBody] List<TeamDto> updatTeam)
        {
            return _teamService.UpdateTeams(updatTeam);
        }
        [Authorize]
        [HttpPut("UpdateTeam")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult UpdateTeam(
            [FromBody] TeamDto updatedTeam)
        {
            return _teamService.UpdateTeam(updatedTeam);
        }
    }
}
