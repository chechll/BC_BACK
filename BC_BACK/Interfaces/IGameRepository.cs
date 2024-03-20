namespace BC_BACK.Interfaces
{
    public interface IGameRepository
    {
        Game GetGame(int id);
        ICollection<Game> GetGames();
        ICollection<Game> GetGamesByUser(int userID);
        int GetIdByName(String name);
        bool isGameExist(int id);
        bool Save();
        bool CreateGame(Game game);
        bool DeleteGame(Game game);
        bool DeleteGames(List<Game> games);
        bool UpdateGame(Game game);
    }
}
