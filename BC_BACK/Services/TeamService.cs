using AutoMapper;
using BC_BACK.Dto;
using BC_BACK.Interfaces;
using BC_BACK.Models;
using BC_BACK.Repository;
using BC_BACK.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BC_BACK.Services
{
    public class TeamService : ControllerBase, ITeamService
    {
        public readonly IMapper _mapper;
        public readonly ITeamRepository _teamRepository;
        public readonly IGameRepository _gameRepository;
        public readonly IAnsweredTaskRepository _answeredTaskRepository;
        public readonly ICheckDataRepository _checkDataRepository;
        public readonly IJwtService _jwtService;
        public TeamService(IMapper mapper, ITeamRepository teamRepository,
            IGameRepository gameRepository, IAnsweredTaskRepository answeredTaskRepository,
            ICheckDataRepository checkDataRepository, IJwtService jwtService) 
        {
            _gameRepository = gameRepository;
            _mapper = mapper;
            _teamRepository = teamRepository;
            _answeredTaskRepository = answeredTaskRepository;
            _checkDataRepository = checkDataRepository;
            _jwtService = jwtService;
        }

        public IActionResult CreateTeams(List<TeamDto> teamCreates)
        {
            if (teamCreates == null || !teamCreates.Any())
            {
                return BadRequest("No teams provided");
            }
            if (teamCreates.Count() > 6 || teamCreates.Count() < 2)
            {
                return BadRequest("Wrong number of teams provided");
            }

            foreach (var teamCreate in teamCreates)
            {
                if (teamCreate == null)
                    return BadRequest();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_gameRepository.isGameExist(teamCreate.IdGame))
                    return BadRequest();

                var size = _teamRepository.GetBoardSize(teamCreate.IdGame);

                if (!_checkDataRepository.CheckStringLengs(teamCreate.Name, 20) || size < teamCreate.PositionY || teamCreate.PositionY < 0 ||
                teamCreate.PositionX < 0 || teamCreate.PositionX > size)
                    return StatusCode(422);

                string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(teamCreate.Password, 13);
                teamCreate.Password = passwordHash;

                teamCreate.PositionX = (_teamRepository.GetBoardSize(teamCreate.IdGame) + 1) / 2 - 1;
                teamCreate.PositionY = (_teamRepository.GetBoardSize(teamCreate.IdGame) + 1) / 2 - 1;

                var teamMap = _mapper.Map<Team>(teamCreate);

                if (!_teamRepository.CreateTeam(teamMap))
                {
                    ModelState.AddModelError("", "Something went wrong");
                    return StatusCode(500, ModelState);
                }
            }

            return Ok();
        }

        public IActionResult GetAllTeams(int idGame)
        {
            try
            {
                var allTeams = _teamRepository.GetTeamsByGame(idGame);
                var teamDtos = _mapper.Map<List<TeamDto>>(allTeams);
                return Ok(teamDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        public IActionResult LogIn(string password, int id)
        {
            try
            {
                if (!_teamRepository.isTeamExist(id))
                    return NotFound();

                var team = _teamRepository.GetTeam(id);

                if (team == null)
                    return NotFound();

                if (!_checkDataRepository.CheckStringLengs(password, 30))
                {
                    ModelState.AddModelError("", "Your password length is more than 30 characters.");
                    return BadRequest(ModelState);
                }

                if (BCrypt.Net.BCrypt.EnhancedVerify(password, team.Password))
                {
                    var token = _jwtService.GenerateToken(team.IdTeam.ToString());
                    return Ok(new { id, token });
                }
                else
                {
                    ModelState.AddModelError("", "Invalid password.");
                    return BadRequest(ModelState);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during login: {ex.Message}");
                return StatusCode(500, "An error occurred during login. Please try again later.");
            }
        }

        public IActionResult UpdateTeam(TeamDto updatedTeam)
        {
            if (updatedTeam == null)
            {
                return BadRequest("No teams provided");
            }

            bool isUpdateNeeded = false;
            if (updatedTeam == null)
                return BadRequest(ModelState);

            if (!_teamRepository.isTeamExist(updatedTeam.IdTeam))
                return NotFound();

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", "Model state is not valid");
                return BadRequest(ModelState);
            }

            if (!_checkDataRepository.CheckStringLengs(updatedTeam.Name, 20))
                return StatusCode(422);

            var team = _teamRepository.GetTeam(updatedTeam.IdTeam);

            this.IsUpdateNeeded(ref team, updatedTeam, ref isUpdateNeeded);

            if (isUpdateNeeded)
            {
                var teamMap = _mapper.Map<Team>(team);
                if (!_teamRepository.UpdateTeam(teamMap))
                {
                    ModelState.AddModelError("", "Something went wrong ");
                    return StatusCode(500, ModelState);
                }
            }

            return Ok("Successfully updated");
        }

        public IActionResult UpdateTeams(List<TeamDto> updatTeam)
        {
            if (updatTeam == null || !updatTeam.Any())
            {
                return BadRequest("No teams provided");
            }
            if (updatTeam.Count() > 6 || updatTeam.Count() < 2)
            {
                return BadRequest("Wrong number of teams provided");
            }

            foreach (var updatedTeam in updatTeam)
            {
                bool isUpdateNeeded = false;
                if (updatedTeam == null)
                    return BadRequest(ModelState);

                if (!_teamRepository.isTeamExist(updatedTeam.IdTeam))
                    return NotFound();

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Model state is not valid");
                    return BadRequest(ModelState);
                }

                if (!_checkDataRepository.CheckStringLengs(updatedTeam.Name, 20))
                    return StatusCode(422);

                var team = _teamRepository.GetTeam(updatedTeam.IdTeam);

                if (updatedTeam.Name != team.Name)
                {
                    team.Name = updatedTeam.Name;
                    isUpdateNeeded = true;
                }

                this.IsUpdateNeeded(ref team, updatedTeam,ref isUpdateNeeded);

                updatedTeam.PositionX = (_teamRepository.GetBoardSize(updatedTeam.IdGame) + 1) / 2 - 1;
                updatedTeam.PositionY = (_teamRepository.GetBoardSize(updatedTeam.IdGame) + 1) / 2 - 1;

                if (updatedTeam.Password != team.Password)
                {
                    var changeMail = BCrypt.Net.BCrypt.EnhancedVerify(updatedTeam.Password, team.Password);
                    if (!changeMail)
                    {
                        string passwordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(updatedTeam.Password, 13);
                        team.Password = passwordHash;
                        isUpdateNeeded = true;
                    }
                }

                if (isUpdateNeeded)
                {
                    var teamMap = _mapper.Map<Team>(team);
                    if (!_teamRepository.UpdateTeam(teamMap))
                    {
                        ModelState.AddModelError("", "Something went wrong ");
                        return StatusCode(500, ModelState);
                    }
                }
            }
            return Ok("Successfully updated");
        }

        private void IsUpdateNeeded(ref Team team, TeamDto updatedTeam, ref bool isUpdateNeeded)
        {
            if (updatedTeam.PositionX != team.PositionX)
            {
                team.PositionX = updatedTeam.PositionX;
                isUpdateNeeded = true;
            }

            if (updatedTeam.PositionY != team.PositionY)
            {
                team.PositionY = updatedTeam.PositionY;
                isUpdateNeeded = true;
            }
            if (updatedTeam.Score != team.Score)
            {
                team.Score = updatedTeam.Score;
                isUpdateNeeded = true;
            }
        }
    }
}
