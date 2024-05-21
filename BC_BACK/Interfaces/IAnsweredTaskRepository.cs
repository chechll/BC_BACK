using BC_BACK.Models;
using System.Collections.Generic;

namespace BC_BACK.Interfaces
{
    public interface IAnsweredTaskRepository
    {
        bool CreateAT(AnsweredTask at);
        bool DeleteAT(AnsweredTask at);
        bool DeleteATs(List<AnsweredTask> ats);
        AnsweredTask? GetAT(int id);
        ICollection<AnsweredTask> GetATs();
        ICollection<AnsweredTask> GetATsByTask(int taskId);
        ICollection<AnsweredTask> GetATsByTeam(int teamId);
        bool IsAT_Exist(int id);
        bool Save();
        bool UpdateAT(AnsweredTask at);
    }
}

