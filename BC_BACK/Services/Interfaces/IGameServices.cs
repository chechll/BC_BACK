using BC_BACK.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BC_BACK.Services.Interfaces
{
    public interface IGameServices
    {
        IActionResult AddTeam(int idTeam);
        IActionResult CheckaGame(int idGame);
        IActionResult CheckCurrent(int idTeam);
        IActionResult CloneGame(int idGame);
        IActionResult CreateGame(CreateData createData);
        IActionResult DateGame(int idGame);
        IActionResult DeleteGame(int idGame);
        IActionResult EndGame(int idGame);
        IActionResult GetAllGames(int idUser);
        IActionResult GetCurrentTeam(int idTeam, int idGame);
        IActionResult GetGameData(int id);
        IActionResult GetGameTeams(int idGame);
        IActionResult RemoveTeam(int idTeam);
        IActionResult StartGame(int idGame);
        IActionResult UpdateCurrentTeam(TeamDto team);
        IActionResult UpdateGameByGameData(DataForUpdate dataForUpdate);
    }
}
