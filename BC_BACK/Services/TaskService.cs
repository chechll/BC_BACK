using AutoMapper;
using BC_BACK.Dto;
using BC_BACK.Interfaces;
using BC_BACK.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BC_BACK.Services
{
    public class TaskService : ControllerBase, ITaskService
    {
        public readonly IMapper _mapper;
        public readonly ITaskRepository _taskRepository;
        public readonly IGameRepository _gameRepository;
        public readonly IAnsweredTaskRepository _answeredTaskRepository;
        public readonly ICheckDataRepository _checkDataRepository;

        public TaskService(IMapper mapper, ITaskRepository taskRepository,
            IGameRepository gameRepository, IAnsweredTaskRepository answeredTaskRepository, ICheckDataRepository checkDataRepository)
        {
            _gameRepository = gameRepository;
            _mapper = mapper;
            _taskRepository = taskRepository;
            _answeredTaskRepository = answeredTaskRepository;
            _checkDataRepository = checkDataRepository;
        }

        public IActionResult CreateTasks(List<TaskDto> taskCreates)
        {
            if (taskCreates == null || !taskCreates.Any())
            {
                return BadRequest("No task provided");
            }

            if (taskCreates.Count() > 50 || taskCreates.Count() < 10)
            {
                return BadRequest("Wrong number of tasks provided");
            }

            foreach (var taskCreate in taskCreates)
            {

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_gameRepository.isGameExist(taskCreate.IdGame) ||
                    taskCreate.Number == null)
                {
                    return BadRequest("Wrong Data");
                }

                var taskMap = _mapper.Map<Models.Task>(taskCreate);

                if (!_taskRepository.CreateTask(taskMap))
                {
                    ModelState.AddModelError("", "Something went wrong");
                    return StatusCode(500, ModelState);
                }
            }

            return Ok();
        }

        public IActionResult GetAllTasks(int idGame)
        {
            try
            {
                var allTask = _mapper.Map<List<TaskDto>>(_taskRepository.GetTasksByGame(idGame));

                return Ok(allTask);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        public IActionResult GetAmountOfTasks(int idGame)
        {
            try
            {
                var allTask = _mapper.Map<List<Task1Dto>>(_taskRepository.GetTasksByGame(idGame));

                return Ok(allTask);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        public IActionResult UpdateTasks(List<TaskDto> updatTask)
        {
            if (updatTask == null || !updatTask.Any())
            {
                return BadRequest("No teams provided");
            }
            if (updatTask.Count() > 50 || updatTask.Count() < 10)
            {
                return BadRequest("Wrong number of teams provided");
            }

            foreach (var updatedTask in updatTask)
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
            }
            return Ok("Successfully updated");
        }
    }
}
