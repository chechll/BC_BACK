using BC_BACK.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BC_BACK.Services.Interfaces
{
    public interface IAnsweredTaskService
    {
        IActionResult CheckAns(int idTeam, int idTask, string answer);
        IActionResult CreateAns(List<AnsweredTaskDto> ansTasks);
        IActionResult GetAns(int teamId);
    }
}
