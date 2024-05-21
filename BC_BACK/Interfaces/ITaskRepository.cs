using Task = BC_BACK.Models.Task;

namespace BC_BACK.Interfaces
{
    public interface ITaskRepository
    {
        Task? GetTask(int id);
        ICollection<Task> GetTasks();
        ICollection<Task> GetTasksByGame(int gameId);
        bool IsTaskExist(int id);
        bool Save();
        bool CreateTask(Task task);
        bool DeleteTask(Task task);
        bool DeleteTasks(List<Task> tasks);
        bool UpdateTask(Task task);
        int AmountOfTasks(int id_game);
    }
}
