using BC_BACK.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BC_BACK.Services.Interfaces
{
    public interface ITeamService
    {
        IActionResult CreateTeams(List<TeamDto> teamCreates);
        IActionResult GetAllTeams(int idGame);
        IActionResult LogIn(string password, int id);
        IActionResult UpdateTeam(TeamDto updatedTeam);
        IActionResult UpdateTeams(List<TeamDto> updatTeam);
    }
}
