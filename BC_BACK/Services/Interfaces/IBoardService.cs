using Microsoft.AspNetCore.Mvc;

namespace BC_BACK.Services.Interfaces
{
    public interface IBoardService
    {
        IActionResult GetBoard(int id);
        IActionResult UpdateBoard(BoardArrayModel updatedBoard);
    }
}
