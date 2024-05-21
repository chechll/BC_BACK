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

        public TaskService(IMapper mapper, ITaskRepository taskRepository, IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
            _mapper = mapper;
            _taskRepository = taskRepository;
        }

        public IActionResult GetAllTasks(int idGame)
        {
            try
            {
                var allTask = _mapper.Map<List<TaskDto>>(_taskRepository.GetTasksByGame(idGame));

                return Ok(allTask);
            }
            catch (Exception)
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
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        public IActionResult CreateTasks(List<TaskDto> taskCreates)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (taskCreates == null || !taskCreates.Any())
                return BadRequest("No task provided");

            if (taskCreates.Count > 50 || taskCreates.Count < 10)
                return BadRequest("Wrong number of tasks provided");


            foreach (var taskCreate in taskCreates)
            {

                if (!_gameRepository.IsGameExist(taskCreate.IdGame))
                    return BadRequest("Invalid data");

                var taskMap = _mapper.Map<Models.Task>(taskCreate);

                if (!_taskRepository.CreateTask(taskMap))
                    return StatusCode(500, "Failed to create tasks");
            }

            return Ok();
        }

        public IActionResult UpdateTasks(List<TaskDto> updatTask)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (updatTask == null || !updatTask.Any())
                return BadRequest("No tasks provided");

            if (updatTask.Count > 50 || updatTask.Count < 10)
                return BadRequest("Wrong number of teams provided");

            foreach (var updatedTask in updatTask)
            {
                if (!_taskRepository.IsTaskExist(updatedTask.IdTask))
                    return NotFound();

                var task = _taskRepository.GetTask(updatedTask.IdTask);
                bool isUpdateNeeded = false;

                if (task != null && updatedTask.Answer != task.Answer)
                {
                    task.Answer = updatedTask.Answer;
                    isUpdateNeeded = true;
                }

                if (task != null && updatedTask.Question != task.Question)
                {
                    task.Question = updatedTask.Question;
                    isUpdateNeeded = true;
                }

                if (isUpdateNeeded)
                {
                    var taskMap = _mapper.Map<Models.Task>(task);
                    if (!_taskRepository.UpdateTask(taskMap))
                        return StatusCode(500, "Failed to update tasks");
                }
            }
            return Ok("Successfully updated");
        }
    }
}
