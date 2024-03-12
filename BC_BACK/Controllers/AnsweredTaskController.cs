using BC_BACK.Dto;
using AutoMapper;
using BC_BACK.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static BC_BACK.Controllers.UserController;

namespace BC_BACK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnsweredTaskController : Controller
    {
        private readonly IAnsweredTaskRepository _ansRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;
        public AnsweredTaskController(IAnsweredTaskRepository answeredTask, 
            IMapper mapper, ITeamRepository teamRepository, ITaskRepository taskRepository) 
        {
            _ansRepository = answeredTask;
            _mapper = mapper;
            _taskRepository = taskRepository;
            _teamRepository = teamRepository;
        }

        [HttpGet("GetAllAns")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AnsweredTask>))]
        [ProducesResponseType(400)]
        public IActionResult GetAllAns()
        {
            try
            {
                var allATs = _ansRepository.GetATs();

                return Ok(allATs);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost("CreateATs")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateAns([FromBody] AnsweredTaskDto answeredTask)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (answeredTask == null)
                return BadRequest();

            if (!_taskRepository.isTaskExist(answeredTask.IdTask) || !_teamRepository.isTeamExist(answeredTask.IdTeam) || _ansRepository.IsAT_Exist(answeredTask.Id))
            {
                ModelState.AddModelError("", "Wrong data");
                return StatusCode(500, ModelState);
            }

            var at = _ansRepository.GetATs().Where(c => c.IdTask == answeredTask.IdTask && c.IdTeam == answeredTask.IdTeam).FirstOrDefault();

            if (at != null)
            {
                ModelState.AddModelError("", "answer already exists");
                return StatusCode(422, ModelState);
            }

            var ansMap = _mapper.Map<AnsweredTask>(answeredTask);

            if (!_ansRepository.CreateAT(ansMap))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }

            return Ok();

        }

    }
}
