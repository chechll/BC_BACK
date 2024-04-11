using BC_BACK.Data;
using BC_BACK.Dto;
using BC_BACK.Interfaces;
using BC_BACK.Models;

namespace BC_BACK.Repository
{
    public class GameRepository : IGameRepository
    {
        private readonly BcDbContext _context;

        public GameRepository(BcDbContext context)
        {
            _context = context;
        }

        public bool CreateGame(Game game)
        {
            _context.Add(game);

            return Save();
        }

        public bool DeleteGame(Game game)
        {
            _context.Remove(game);

            return Save();
        }

        public bool DeleteGames(List<Game> games)
        {
            _context.RemoveRange(games);

            return Save();
        }

        public Game GetGame(int id)
        {
            return _context.Games.Where(p => p.IdGame == id).FirstOrDefault();
        }

        public ICollection<Game> GetGames()
        {
            return _context.Games.OrderBy(p => p.IdGame).ToList();
        }

        public ICollection<Game> GetGamesByUser(int userID)
        {
            return _context.Games.Where(p => p.IdUser == userID).ToList();
        }

        public int GetIdByName(String name)
        {
            return _context.Games.Where(p => p.Name == name).FirstOrDefault().IdGame;
        }

        public bool isGameExist(int id)
        {
            return _context.Games.Any(p => p.IdGame == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateGame(Game game)
        {
            var existingGame = _context.Games.Find(game.IdGame);

            if (existingGame == null)
            {
                return false;
            }

            _context.Entry(existingGame).CurrentValues.SetValues(game);

            return Save();
        }
    }
}
