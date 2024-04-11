using AutoMapper;
using BC_BACK.Dto;
using BC_BACK.Interfaces;
using BC_BACK.Models;
using BC_BACK.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BC_BACK.Services
{
    public class AnsweredTaskService : ControllerBase, IAnsweredTaskService
    {
        private readonly IAnsweredTaskRepository _ansRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IMapper _mapper;

        public AnsweredTaskService(IAnsweredTaskRepository answeredTaskRepository,
            IMapper mapper, ITeamRepository teamRepository, ITaskRepository taskRepository)
        {
            _ansRepository = answeredTaskRepository;
            _mapper = mapper;
            _teamRepository = teamRepository;
            _taskRepository = taskRepository;
        }

        public IActionResult CheckAns(int idTeam, int idTask, string answer)
        {
            if (idTeam != null && idTask != null && answer != null)
            {
                var taskExists = _taskRepository.isTaskExist(idTask);
                var teamExists = _teamRepository.isTeamExist(idTeam);

                if (taskExists && teamExists)
                {
                    var team = _teamRepository.GetTeam(idTeam);
                    var task = _taskRepository.GetTask(idTask);

                    if (team.IdGame == task.IdGame && task.Answer == answer)
                    {
                        var existingAT = _ansRepository.GetATs().FirstOrDefault(at => at.IdTask == idTask && at.IdTeam == idTeam);

                        if (existingAT == null)
                        {
                            AnsweredTaskDto answeredTaskDto = new()
                            {
                                IdTask = idTask,
                                IdTeam = idTeam,
                            };
                            AnsweredTaskDto answeredTask = answeredTaskDto;

                            var ansMap = _mapper.Map<AnsweredTask>(answeredTask);

                            if (!_ansRepository.CreateAT(ansMap))
                            {
                                ModelState.AddModelError("", "Failed to create answered task.");
                                return StatusCode(500, ModelState);
                            }
                            return Ok(1);
                        }
                    }
                }
                return BadRequest("there is already such answer");
            }
            return BadRequest();
        }

        public IActionResult CreateAns(List<AnsweredTaskDto> ansTasks)
        {
            if (ansTasks == null || !ansTasks.Any())
            {
                return NoContent();
            }

            try
            {
                foreach (var answeredTask in ansTasks)
                {
                    if (answeredTask != null)
                    {
                        var taskExists = _taskRepository.isTaskExist(answeredTask.IdTask);
                        var teamExists = _teamRepository.isTeamExist(answeredTask.IdTeam);
                        var ansExists = _ansRepository.IsAT_Exist(answeredTask.Id);

                        if (taskExists && teamExists && !ansExists)
                        {
                            var team = _teamRepository.GetTeam(answeredTask.IdTeam);
                            var task = _taskRepository.GetTask(answeredTask.IdTask);

                            if (team.IdGame == task.IdGame)
                            {
                                var existingAT = _ansRepository.GetATs().FirstOrDefault(at => at.IdTask == answeredTask.IdTask && at.IdTeam == answeredTask.IdTeam);

                                if (existingAT == null)
                                {
                                    var ansMap = _mapper.Map<AnsweredTask>(answeredTask);

                                    if (!_ansRepository.CreateAT(ansMap))
                                    {
                                        ModelState.AddModelError("", "Failed to create answered task.");
                                        return StatusCode(500, ModelState);
                                    }
                                }
                            }
                        }
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500);
            }
        }

        public IActionResult GetAns(int teamId)
        {
            try
            {
                var allATs = _mapper.Map<List<AnsweredTaskDto>>(
                    _ansRepository.GetATs().Where(p => p.IdTeam == teamId).ToList());

                return Ok(allATs);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500);
            }
        }
    }
}
