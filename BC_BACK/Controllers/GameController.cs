using BC_BACK.Dto;
using BC_BACK.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BC_BACK.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : Controller
    {
        private IGameServices _gameServices;
        private readonly IJwtService _jwtService;
        public GameController(IGameServices gameServices, IJwtService jwtService)
        {
            _gameServices = gameServices;
            _jwtService = jwtService;
        }

        private IActionResult ValidateTokenAndGetPrincipal()
        {
            var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var principal = _jwtService.GetPrincipalFromToken(token);

            if (principal == null)
            {
                return Unauthorized();
            }

            return null;
        }

        [Authorize]
        [HttpGet("GetAllGames")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Game>))]
        [ProducesResponseType(400)]
        public IActionResult GetAllGames(int idUser)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _gameServices.GetAllGames(idUser);
        }

        [Authorize]
        [HttpGet("GetGameData")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Game>))]
        [ProducesResponseType(400)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetGameData(int id)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _gameServices.GetGameData(id);
        }

        [Authorize]
        [HttpGet("CheckGameData")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Game>))]
        [ProducesResponseType(400)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult CheckGameData(int id)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _gameServices.CheckGameData(id);
        }

        [Authorize]
        [HttpGet("CloneGame")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult CloneGame(int idGame)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _gameServices.CloneGame(idGame);
        }

        [HttpGet("DateGame")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult DateGame(int idGame)
        {
            return _gameServices.DateGame(idGame);
        }

        [Authorize]
        [HttpPost("CreateGame")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult CreateGame([FromBody] CreateData createData)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _gameServices.CreateGame(createData);
        }

        [Authorize]
        [HttpPut("UpdateGameByGameData")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult UpdateGameByGameData(
            [FromBody] DataForUpdate dataForUpdate)
        {
            return _gameServices.UpdateGameByGameData(dataForUpdate);
        }

        [Authorize]
        [HttpDelete("DeleteGame")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult DeleteGame(int idGame)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _gameServices.DeleteGame(idGame);
        }

        [Authorize]
        [HttpPut("Start")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult StartGame(int idGame)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _gameServices.StartGame(idGame);
        }

        [Authorize]
        [HttpPut("End")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult EndGame(int idGame)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _gameServices.EndGame(idGame);
        }

        
        [HttpPut("AddTeam")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult AddTeam(int idTeam)
        {
            return _gameServices.AddTeam(idTeam);
        }

        [Authorize]
        [HttpPut("RemoveTeam")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult RemoveTeam(int idTeam)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _gameServices.RemoveTeam(idTeam);
        }

        
        [HttpGet("CheckCurrent")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult CheckCurrent(int idTeam)
        {
            return _gameServices.CheckCurrent(idTeam);
        }

        
        [HttpGet("GetGameTeams")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetGameTeams(int idGame)
        {
            return _gameServices.GetGameTeams(idGame);
        }

        [Authorize]
        [HttpGet("GetCurrentTeam")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult GetCurrentTeam(int idTeam, int idGame)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _gameServices.GetCurrentTeam(idTeam, idGame);
        }

        [Authorize]
        [HttpPut("UpdateCurrentTeam")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult UpdateCurrentTeam(TeamDto idTeam)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _gameServices.UpdateCurrentTeam(idTeam);
        }

        [Authorize]
        [HttpGet("CheckGame")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult CheckGame(int idGame)
        {
            var validationError = ValidateTokenAndGetPrincipal();
            if (validationError != null)
            {
                return validationError;
            }
            return _gameServices.CheckaGame(idGame);
        }
    }
}
