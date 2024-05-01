using BC_BACK.Data;
using BC_BACK.Interfaces;

namespace BC_BACK.Repository
{
    public class TaskRepository : ITaskRepository
    {
        private readonly BcDbContext _context;

        public TaskRepository(BcDbContext context)
        {
            _context = context;
        }

        public int AmountOfTasks(int id_game)
        {
            return _context.Tasks.Where(p => p.IdGame == id_game).Count();
        }

        public bool CreateTask(Models.Task task)
        {
            _context.Add(task);

            return Save();
        }

        public bool DeleteTask(Models.Task task)
        {
            _context.Remove(task);

            return Save();
        }

        public bool DeleteTasks(List<Models.Task> tasks)
        {
            _context.RemoveRange(tasks);

            return Save();
        }

        public Models.Task GetTask(int id)
        {
            return _context.Tasks.Where(p => p.IdTask == id).FirstOrDefault();
        }

        public ICollection<Models.Task> GetTasks()
        {
            return _context.Tasks.OrderBy(p => p.IdTask).ToList();
        }

        public ICollection<Models.Task> GetTasksByGame(int gameId)
        {
            return _context.Tasks.Where(p => p.IdGame == gameId).ToList();
        }

        public bool isTaskExist(int id)
        {
            return _context.Tasks.Any(p => p.IdTask == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateTask(Models.Task task)
        {
            var existingTask = _context.Tasks.Find(task.IdTask);

            if (existingTask == null)
            {
                return false;
            }

            _context.Entry(existingTask).CurrentValues.SetValues(task);

            return Save();
        }
    }
}
