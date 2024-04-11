using AutoMapper;
using BC_BACK.Dto;
using BC_BACK.Interfaces;
using BC_BACK.Models;
using BC_BACK.Repository;
using BC_BACK.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BC_BACK.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;
        public TaskController(ITaskService taskService)
        {
           _taskService = taskService;
        }

        [HttpGet("GetTasks")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Models.Task>))]
        [ProducesResponseType(400)]
        public IActionResult GetAllTasks(int idGame)
        {
            return _taskService.GetAllTasks(idGame);
        }

        [HttpGet("GetAmountOfTasks")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Models.Task>))]
        [ProducesResponseType(400)]
        public IActionResult GetAmountOfTasks(int idGame)
        {
            return _taskService.GetAmountOfTasks(idGame);
        }

        [HttpPost("CreateTasks")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult CreateTasks([FromBody] List<TaskDto> taskCreates)
        {
            return _taskService.CreateTasks(taskCreates);
        }

        [HttpPut("UpdateTasks")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult UpdateTask(
            [FromBody] List<TaskDto> updatTask)
        {
            return _taskService.UpdateTasks(updatTask);
        }
    }
}
