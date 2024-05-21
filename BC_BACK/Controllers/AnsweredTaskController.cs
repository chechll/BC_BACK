using BC_BACK.Dto;
using Microsoft.AspNetCore.Mvc;
using BC_BACK.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BC_BACK.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AnsweredTaskController : Controller
    {
        private readonly IAnsweredTaskService _answeredTaskService;
        private readonly IJwtService _jwtService;
        public AnsweredTaskController(IAnsweredTaskService answeredTaskService, IJwtService jwtService)
        {
            _answeredTaskService = answeredTaskService;
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

        [HttpGet("GetAns")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AnsweredTask>))]
        [ProducesResponseType(400)]
        public IActionResult GetAns(int teamId)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _answeredTaskService.GetAns(teamId);
        }


        [HttpPost("CreateATs")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult CreateAns([FromBody] List<AnsweredTaskDto> ansTask)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _answeredTaskService.CreateAns(ansTask);
        }

        [HttpGet("CheckAns")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult CheckAns(int idTeam, int idTask, string answer)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _answeredTaskService.CheckAns(idTeam, idTask, answer);
        }
    }
}
