using BC_BACK.Dto;
using Microsoft.AspNetCore.Mvc;

namespace BC_BACK.Services.Interfaces
{
    public interface IUserService
    {
        IActionResult CreateUser(UserDto userCreate);
        IActionResult DeleteUser(int userId);
        IActionResult GetAllUsers();
        IActionResult GetUser(int id);
        IActionResult LogIn(string username, string user_password);
        IActionResult UpdateUser(UserDto updatedUser);
        IActionResult UpdateUserRights(UserDto updatedUser);
    }
}
