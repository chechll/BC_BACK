using BC_BACK.Data;
using BC_BACK.Interfaces;

namespace BC_BACK.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly BcDbContext _context;

        public UserRepository(BcDbContext context)
        {
            _context = context;
        }

        public bool CreateUser(User user)
        {
            _context.Add(user);

            return Save();
        }

        public bool DeleteUser(User user)
        {
            _context.Remove(user);

            return Save();
        }

        public int GetId(string name)
        {
            int userId = _context.Users
            .Where(p => p.Username == name)
            .Select(p => p.IdUser)
            .FirstOrDefault();

            return userId;
        }

        public User? GetUser(int id)
        {
            return _context.Users.Where(p => p.IdUser == id).FirstOrDefault();
        }

        public ICollection<User> GetUsers()
        {
            return _context.Users.OrderBy(p => p.IdUser).ToList();
        }

        public bool IsAdmin(int id)
        {
            if (_context.Users.Where(p => p.IdUser == id).Select(c => c.Rights).FirstOrDefault() == 1) return true;
            return false;
        }

        public bool IsUserExist(int id)
        {
            return _context.Users.Any(p => p.IdUser == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        public bool UpdateUser(User user)
        {
            var existingUser = _context.Users.Find(user.IdUser);

            if (existingUser == null)
            {
                return false;
            }

            _context.Entry(existingUser).CurrentValues.SetValues(user);

            return Save();
        }
    }
}
