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
        private readonly IJwtService _jwtService;

        public TeamController(ITeamService teamService, IJwtService jwtService)
        {
            _teamService = teamService;
            _jwtService = jwtService;
        }

        private IActionResult? ValidateTokenAndGetPrincipal()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var principal = _jwtService.GetPrincipalFromToken(token);

            if (principal == null)
            {
                return Unauthorized();
            }

            return null;
        }

        [Authorize]
        [HttpGet("GetTeams")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Team>))]
        [ProducesResponseType(400)]
        public IActionResult GetAllTeams(int idGame)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
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
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
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
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
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
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _teamService.UpdateTeam(updatedTeam);
        }
    }
}
