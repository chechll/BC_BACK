using AutoMapper;
using BC_BACK.Dto;
using BC_BACK.Interfaces;
using BC_BACK.Models;
using BC_BACK.Repository;
using Microsoft.AspNetCore.Mvc;

namespace BC_BACK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : Controller
    {
        public readonly IMapper _mapper;
        public readonly ITaskRepository _taskRepository;
        public readonly IGameRepository _gameRepository;
        public readonly IAnsweredTaskRepository _answeredTaskRepository;
        public readonly ICheckDataRepository _checkDataRepository;

        public TaskController(IMapper mapper, ITaskRepository taskRepository, 
            IGameRepository gameRepository, IAnsweredTaskRepository answeredTaskRepository, ICheckDataRepository checkDataRepository)
        {
            _gameRepository = gameRepository;
            _mapper = mapper;
            _taskRepository = taskRepository;
            _answeredTaskRepository = answeredTaskRepository;
            _checkDataRepository = checkDataRepository;
        }

        [HttpGet("GetAllTasks")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Models.Task>))]
        [ProducesResponseType(400)]
        public IActionResult GetAllTasks()
        {
            try
            {
                var allTask = _taskRepository.GetTasks();

                return Ok(allTask);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost("CreateTask")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateTask([FromBody] TaskDto taskCreate)
        {
            if (taskCreate == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_gameRepository.isGameExist(taskCreate.IdGame) || !_checkDataRepository.IsSlovakWord(taskCreate.Answer) || 
                taskCreate.Number != null)
                return BadRequest();

            var taskMap = _mapper.Map<Models.Task>(taskCreate);

            if (!_taskRepository.CreateTask(taskMap))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }

            return Ok();

        }

        [HttpPut("Update")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateTask(
            [FromForm] TaskDto updatedTask)
        {
            bool isUpdateNeeded = false;
            if (updatedTask == null)
                return BadRequest(ModelState);

            if (!_taskRepository.isTaskExist(updatedTask.IdTask))
                return NotFound();

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Model state is not valid");
                return BadRequest(ModelState);
            }

            if (!_checkDataRepository.IsSlovakWord(updatedTask.Answer))
                return BadRequest(ModelState);

            var task = _taskRepository.GetTask(updatedTask.IdTask);

            if (updatedTask.Answer != task.Answer)
            {
                task.Answer = updatedTask.Answer;
                isUpdateNeeded = true;
            }

            if (updatedTask.Question != task.Question)
            {
                task.Question = updatedTask.Question;
                isUpdateNeeded = true;
            }

            if (isUpdateNeeded)
            {
                var taskMap = _mapper.Map<Models.Task>(task);
                if (!_taskRepository.UpdateTask(taskMap))
                {
                    ModelState.AddModelError("", "Something went wrong ");
                    return StatusCode(500, ModelState);
                }
            }
            return Ok("Successfully updated");
        }

        [HttpDelete("DeleteTask")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeleteBoard(int taskId)
        {

            if (!_taskRepository.isTaskExist(taskId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ansToDelete = _answeredTaskRepository.GetATsByTask(taskId).ToList();
            if (_answeredTaskRepository.DeleteATs(ansToDelete))
            {
                ModelState.AddModelError("", "Something went wrong");
                return StatusCode(500, ModelState);
            }

            var taskToDelete = _taskRepository.GetTask(taskId);

            if (!_taskRepository.DeleteTask(taskToDelete))
            {
                ModelState.AddModelError("", "Something went wrong ");
                return StatusCode(500, ModelState);
            }

            return Ok(0);

        }
    }
}
