namespace BC_BACK.Interfaces
{
    public interface IUserRepository
    {
        User? GetUser(int id);
        ICollection<User> GetUsers();
        bool IsUserExist(int id);
        int GetId(string name);
        bool IsAdmin(int id);
        bool Save();
        bool CreateUser(User user);
        bool DeleteUser(User user);
        bool UpdateUser(User user);
    }
}
