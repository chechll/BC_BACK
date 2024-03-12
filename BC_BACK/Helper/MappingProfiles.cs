using AutoMapper;
using BC_BACK.Dto;
using Task = BC_BACK.Models.Task;

namespace BC_BACK.Helper
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles() 
        {
            CreateMap<User, UserDto>();
            CreateMap<UserDto, User>();
            CreateMap<Board, BoardDto>();
            CreateMap<BoardDto, Board>();
            CreateMap<Game, GameDto>();
            CreateMap<GameDto, Game>();
            CreateMap<Task, TaskDto>();
            CreateMap<TaskDto, Task>();
            CreateMap<Team, TeamDto>();
            CreateMap<TeamDto, Team>();
            CreateMap<AnsweredTask, AnsweredTaskDto>();
            CreateMap<AnsweredTaskDto, AnsweredTask>();
        }
    }
}
