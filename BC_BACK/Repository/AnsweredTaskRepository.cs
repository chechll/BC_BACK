using BC_BACK.Data;
using BC_BACK.Interfaces;

namespace BC_BACK.Repository
{
    public class AnsweredTaskRepository : IAnsweredTaskRepository
    {
        private readonly BcDbContext _context;

        public AnsweredTaskRepository(BcDbContext context)
        {
            _context = context;
        }

        public bool CreateAT(AnsweredTask at)
        {
            _context.Add(at);

            return Save();
        }

        public bool DeleteAT(AnsweredTask at)
        {
            _context.Remove(at);

            return Save();
        }

        public bool DeleteATs(List<AnsweredTask> ats)
        {
            _context.RemoveRange(ats);

            return Save();
        }

        public AnsweredTask GetAT(int id)
        {
            return _context.AnsweredTasks.Where(p => p.Id == id).FirstOrDefault();
        }

        public ICollection<AnsweredTask> GetATs()
        {
            return _context.AnsweredTasks.OrderBy(p => p.Id).ToList();
        }

        public ICollection<AnsweredTask> GetATsByTask(int taskId)
        {
            return _context.AnsweredTasks.Where(p => p.IdTask == taskId).ToList();
        }

        public ICollection<AnsweredTask> GetATsByTeam(int teamId)
        {
            return _context.AnsweredTasks.Where(p => p.IdTeam == teamId).ToList();
        }

        public bool IsAT_Exist(int id)
        {
            return _context.AnsweredTasks.Any(p => p.Id == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateAT(AnsweredTask at)
        {
            var existingAnsweredTask = _context.AnsweredTasks.Find(at.Id);

            if (existingAnsweredTask == null)
            {
                return false;
            }

            _context.Entry(existingAnsweredTask).CurrentValues.SetValues(at);

            return Save();
        }
    }
}
