namespace BC_BACK.Interfaces
{
    public interface ITeamRepository
    {
        Team GetTeam(int id);
        int GetBoardSize(int idGame);
        ICollection<Team> GetTeams();
        ICollection<Team> GetTeamsByGame(int gameId);
        bool isTeamExist(int id);
        bool Save();
        bool CreateTeam(Team team);
        bool DeleteTeam(Team team);
        bool DeleteTeams(List<Team> teams);
        bool UpdateTeam(Team team);
    }
}
