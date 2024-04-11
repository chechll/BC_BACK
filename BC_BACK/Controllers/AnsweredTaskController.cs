using BC_BACK.Dto;
using AutoMapper;
using BC_BACK.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BC_BACK.Services.Interfaces;
using BC_BACK.Services;
using Microsoft.AspNetCore.Authorization;

namespace BC_BACK.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AnsweredTaskController : Controller
    {
        private readonly IAnsweredTaskService _answeredTaskService;
        public AnsweredTaskController(IAnsweredTaskService answeredTaskService) 
        {
            _answeredTaskService = answeredTaskService;
        }

        [HttpGet("GetAns")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AnsweredTask>))]
        [ProducesResponseType(400)]
        public IActionResult GetAns(int teamId)
        {
            return _answeredTaskService.GetAns(teamId);
        }


        [HttpPost("CreateATs")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult CreateAns([FromBody] List<AnsweredTaskDto> ansTask)
        {
            return _answeredTaskService.CreateAns(ansTask);
        }

        [HttpGet("CheckAns")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult CheckAns(int idTeam, int idTask, string answer)
        {
            return _answeredTaskService.CheckAns(idTeam, idTask, answer);
        }
    }
}
