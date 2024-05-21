namespace BC_BACK.Interfaces
{
    public interface IBoardRepository
    {
        Board? GetBoard(int id);
        ICollection<Board> GetBoards();
        ICollection<Board> GetBoardsByGame(int gameId);
        bool IsBoardExist(int id);
        bool Save();
        string BoardToString(int[,] matrix);
        bool CreateBoard(Board board);
        int[,] CreateBorad(int size);
        bool DeleteBoard(Board board);
        bool DeleteBoards(List<Board> boards);
        bool UpdateBoard(Board board);
    }
}
