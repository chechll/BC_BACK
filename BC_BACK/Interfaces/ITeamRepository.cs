namespace BC_BACK.Interfaces
{
    public interface ITeamRepository
    {
        Team? GetTeam(int id);
        int? GetBoardSize(int idGame);
        int? GetId(string name);
        ICollection<Team> GetTeams();
        ICollection<Team> GetTeamsByGame(int gameId);
        bool IsTeamExist(int id);
        bool Save();
        bool CreateTeam(Team team);
        bool DeleteTeam(Team team);
        bool DeleteTeams(List<Team> teams);
        bool UpdateTeam(Team team);
    }
}
