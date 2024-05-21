using BC_BACK.Data;
using BC_BACK.Interfaces;

namespace BC_BACK.Repository
{
    public class TeamRepository : ITeamRepository
    {
        private readonly BcDbContext _context;

        public TeamRepository(BcDbContext context)
        {
            _context = context;
        }

        public int? GetBoardSize(int idGame)
        {
            var board = _context.Boards.Where(p => p.IdGame == idGame).FirstOrDefault();
            if (board == null)
                return null;
            return board.Size;
        }

        public bool CreateTeam(Team team)
        {
            _context.Add(team);

            return Save();
        }

        public bool DeleteTeam(Team team)
        {
            _context.Remove(team);

            return Save();
        }

        public bool DeleteTeams(List<Team> teams)
        {
            _context.RemoveRange(teams);

            return Save();
        }

        public Team? GetTeam(int id)
        {
            return _context.Teams.Where(p => p.IdTeam == id).FirstOrDefault();
        }

        public ICollection<Team> GetTeams()
        {
            return _context.Teams.OrderBy(p => p.IdTeam).ToList();
        }

        public ICollection<Team> GetTeamsByGame(int gameId)
        {
            return _context.Teams.Where(p => p.IdGame == gameId).ToList();
        }

        public bool IsTeamExist(int id)
        {
            return _context.Teams.Any(p => p.IdTeam == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0;
        }

        public bool UpdateTeam(Team team)
        {
            var existingTeam = _context.Teams.Find(team.IdTeam);

            if (existingTeam == null)
            {
                return false;
            }

            _context.Entry(existingTeam).CurrentValues.SetValues(team);

            return Save();
        }

        public int? GetId(string name)
        {
            var t = _context.Teams.Where(p => name.Equals(p.Name)).FirstOrDefault();
            if (t == null)
                return null;
            return t.IdTeam;
        }
    }
}
