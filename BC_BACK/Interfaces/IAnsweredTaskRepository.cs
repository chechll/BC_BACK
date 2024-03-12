using AT = BC_BACK.Models.AnsweredTask;

namespace BC_BACK.Interfaces
{
    public interface IAnsweredTaskRepository
    {
        AT GetAT(int id);
        ICollection<AT> GetATs();
        ICollection<AT> GetATsByTeam(int teamId);
        ICollection<AT> GetATsByTask(int taskId);
        bool IsAT_Exist(int id);
        bool Save();
        bool CreateAT(AT at);
        bool DeleteAT(AT at);
        bool DeleteATs(List<AT> ats);
        bool UpdateAT(AT at);
    }
}
