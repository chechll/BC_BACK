using AutoMapper;
using BC_BACK.Dto;
using BC_BACK.Interfaces;
using BC_BACK.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BC_BACK.Services
{
    public class AnsweredTaskService : ControllerBase, IAnsweredTaskService
    {
        private readonly IAnsweredTaskRepository _ansRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;

        public AnsweredTaskService(IAnsweredTaskRepository answeredTaskRepository,
            IMapper mapper, ITeamRepository teamRepository, ITaskRepository taskRepository)
        {
            _ansRepository = answeredTaskRepository;
            _mapper = mapper;
            _teamRepository = teamRepository;
            _taskRepository = taskRepository;
        }

        public IActionResult CheckAns(int idTeam, int idTask, string answer)
        {
            var taskExists = _taskRepository.isTaskExist(idTask);
            var teamExists = _teamRepository.isTeamExist(idTeam);

            if (!taskExists || !teamExists)
                return BadRequest("invalid data");

            var team = _teamRepository.GetTeam(idTeam);
            var task = _taskRepository.GetTask(idTask);

            if (team.IdGame != task.IdGame)
                return BadRequest("Invalid team, task, or answer.");

            Console.WriteLine(task.Answer + " " + answer);
            if (task.Answer != answer)
                return StatusCode(422, "Wrong answer");

            if (_ansRepository.GetATs().Any(at => at.IdTask == idTask && at.IdTeam == idTeam))
                return BadRequest("An answer already exists for this team and task.");

            var ansMap = _mapper.Map<AnsweredTask>(new AnsweredTaskDto { IdTask = idTask, IdTeam = idTeam });

            if (!_ansRepository.CreateAT(ansMap))
            {
                ModelState.AddModelError("", "Failed to create answered task.");
                return StatusCode(500, ModelState);
            }
            return Ok(1);
        }

        public IActionResult CreateAns(List<AnsweredTaskDto> ansTasks)
        {
            if (ansTasks == null || !ansTasks.Any())
            {
                return NoContent();
            }

            try
            {
                foreach (var answeredTask in ansTasks)
                {
                    if (_taskRepository.isTaskExist(answeredTask.IdTask) &&
                    _teamRepository.isTeamExist(answeredTask.IdTeam) &&
                    !_ansRepository.IsAT_Exist(answeredTask.Id))
                    {
                        var team = _teamRepository.GetTeam(answeredTask.IdTeam);
                        var task = _taskRepository.GetTask(answeredTask.IdTask);

                        if (team.IdGame == task.IdGame && _ansRepository.GetATs().FirstOrDefault(at => at.IdTask == answeredTask.IdTask && at.IdTeam == answeredTask.IdTeam) == null)
                        {

                            if (!_ansRepository.CreateAT(_mapper.Map<AnsweredTask>(answeredTask)))
                                return StatusCode(500, "Failed to create answered task.");

                        }
                    }
                }

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        public IActionResult GetAns(int teamId)
        {
            try
            {
                var allATs = _mapper.Map<List<AnsweredTaskDto>>(
                    _ansRepository.GetATs().Where(p => p.IdTeam == teamId).ToList());

                return Ok(allATs);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
