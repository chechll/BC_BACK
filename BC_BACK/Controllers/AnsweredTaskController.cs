using BC_BACK.Dto;
using AutoMapper;
using BC_BACK.Interfaces;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("GetAns")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AnsweredTask>))]
        [ProducesResponseType(400)]
        public IActionResult GetAns(int teamId)
        {
            try
            {
                var allATs = _ansRepository.GetATs().Where(p => p.IdTeam == teamId).ToList();

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
        public IActionResult CreateAns([FromBody] List<AnsweredTaskDto> ansTask)
        {
            if (ansTask == null || !ansTask.Any())
            {
                return NoContent();
            }

            foreach (var answeredTask in ansTask)
            {
                Console.WriteLine(1);
                if (answeredTask != null)
                {
                    Console.WriteLine(2);
                    if (_taskRepository.isTaskExist(answeredTask.IdTask) && _teamRepository.isTeamExist(answeredTask.IdTeam) && !_ansRepository.IsAT_Exist(answeredTask.Id) && _teamRepository.GetTeam(answeredTask.IdTeam).IdGame == _taskRepository.GetTask(answeredTask.IdTask).IdGame)
                    {
                        var at = _ansRepository.GetATs().Where(c => c.IdTask == answeredTask.IdTask && c.IdTeam == answeredTask.IdTeam).FirstOrDefault();
                        Console.WriteLine(3);
                        if (at == null)
                        {
                            var ansMap = _mapper.Map<AnsweredTask>(answeredTask);
                            Console.WriteLine(4);
                            if (!_ansRepository.CreateAT(ansMap))
                            {
                                ModelState.AddModelError("", "Something went wrong");
                                return StatusCode(500, ModelState);
                            }
                        }
                    }
    
                }
                
            }

            return Ok();

        }



    }
}
