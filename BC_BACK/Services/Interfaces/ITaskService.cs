using BC_BACK.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BC_BACK.Services.Interfaces
{
    public interface ITaskService
    {
        IActionResult CreateTasks(List<TaskDto> taskCreates);
        IActionResult GetAllTasks(int idGame);
        IActionResult GetAmountOfTasks(int idGame);
        IActionResult UpdateTasks(List<TaskDto> updatTask);
    }
}
