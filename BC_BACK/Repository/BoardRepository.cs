using BC_BACK.Data;
using BC_BACK.Interfaces;
using System.Text;

namespace BC_BACK.Repository
{
    public class BoardRepository : IBoardRepository
    {
        private readonly BcDbContext _context;
        private readonly int[,] _board;

        public BoardRepository(BcDbContext context)
        {
            _context = context;
            _board = new int[25, 25]
            {
            { 4, 9, 1, 7, 3, 8, 3, 5, 7, 9, 3, 8, 7, 2, 6, 7, 5, 6, 8, 4, 7, 5, 6, 8, 4 },
            { 6, 4, 3, 5, 7, 1, 4, 2, 8, 5, 7, 6, 1, 9, 4, 5, 8, 1, 7, 9, 4, 6, 7, 5, 3 },
            { 5, 8, 2, 9, 3, 6, 7, 2, 3, 4, 9, 6, 5, 1, 8, 2, 6, 9, 5, 4, 5, 2, 8, 1, 7 },
            { 2, 9, 8, 6, 4, 7, 4, 6, 5, 3, 8, 4, 6, 7, 3, 2, 1, 4, 7, 6, 8, 1, 2, 4, 6 },
            { 7, 5, 6, 8, 3, 3, 8, 9, 4, 5, 1, 3, 8, 2, 4, 6, 9, 7, 8, 4, 3, 5, 4, 2, 7 },
            { 7, 7, 9, 1, 1, 9, 5, 6, 8, 6, 7, 5, 9, 5, 6, 8, 6, 7, 5, 9, 3, 3, 1, 9, 9 },
            { 4, 5, 3, 2, 4, 5, 1, 2, 4, 2, 2, 1, 2, 5, 6, 7, 4, 8, 1, 5, 6, 5, 7, 8, 6 },
            { 7, 4, 8, 9, 6, 7, 8, 5, 9, 3, 3, 4, 3, 4, 3, 5, 3, 5, 2, 6, 3, 6, 2, 1, 4 },
            { 5, 7, 3, 8, 3, 6, 4, 3, 4, 5, 2, 1, 2, 1, 2, 1, 4, 9, 4, 8, 5, 3, 7, 2, 9 },
            { 1, 8, 5, 1, 2, 8, 7, 5, 1, 4, 3, 2, 1, 2, 3, 4, 5, 3, 2, 6, 9, 2, 5, 9, 8 },
            { 9, 6, 6, 3, 2, 6, 6, 3, 2, 1, 2, 1, 0, 1, 2, 1, 2, 3, 2, 7, 3, 4, 8, 7, 6 },
            { 7, 9, 5, 4, 1, 5, 5, 4, 3, 2, 1, 0, 0, 0, 1, 2, 3, 4, 1, 5, 3, 6, 9, 3, 9 },
            { 6, 4, 2, 9, 3, 5, 2, 1, 2, 1, 0, 0, 0, 0, 0, 1, 2, 1, 2, 9, 3, 3, 1, 6, 8 },
            { 2, 1, 5, 4, 3, 5, 1, 4, 3, 2, 1, 0, 0, 0, 1, 2, 3, 4, 5, 5, 1, 9, 5, 6, 7 },
            { 7, 2, 4, 3, 7, 7, 2, 3, 2, 1, 2, 1, 0, 1, 2, 1, 2, 3, 6, 6, 3, 8, 6, 2, 4 },
            { 3, 1, 5, 6, 4, 6, 2, 3, 5, 4, 3, 2, 1, 2, 3, 4, 1, 5, 7, 8, 2, 9, 5, 4, 6 },
            { 8, 3, 8, 4, 6, 7, 4, 9, 4, 1, 2, 1, 2, 1, 2, 5, 4, 3, 4, 6, 7, 2, 9, 6, 4 },
            { 3, 9, 4, 1, 6, 8, 5, 3, 5, 3, 4, 3, 4, 3, 3, 9, 5, 8, 7, 7, 2, 5, 8, 3, 1 },
            { 4, 6, 2, 7, 9, 1, 2, 4, 7, 6, 5, 2, 1, 2, 2, 4, 2, 1, 5, 5, 2, 7, 4, 5, 8 },
            { 9, 8, 6, 5, 4, 7, 5, 7, 6, 8, 6, 5, 9, 5, 7, 6, 8, 6, 5, 9, 7, 2, 3, 5, 6 },
            { 7, 2, 8, 5, 4, 6, 1, 9, 3, 7, 2, 7, 5, 3, 1, 7, 2, 3, 8, 4, 2, 8, 3, 5, 6 },
            { 3, 6, 1, 9, 2, 9, 6, 8, 2, 5, 3, 4, 9, 1, 6, 5, 2, 9, 3, 1, 3, 6, 9, 1, 8 },
            { 8, 4, 2, 1, 6, 4, 3, 8, 7, 6, 1, 4, 5, 9, 2, 8, 4, 1, 5, 6, 2, 4, 8, 9, 4 },
            { 5, 9, 3, 7, 8, 3, 6, 4, 5, 7, 2, 6, 4, 3, 7, 8, 9, 6, 3, 4, 5, 1, 7, 3, 2 },
            { 1, 6, 7, 3, 4, 7, 2, 1, 6, 5, 9, 7, 2, 8, 6, 4, 1, 3, 2, 6, 9, 4, 3, 7, 6 }
            };
        }

        public bool CreateBoard(Board board)
        {
            _context.Add(board);

            return Save();
        }

        public int[,] CreateBorad(int size)
        {
            int[,] matrix = new int[size, size];
            int startPosition;
            startPosition = (int)((25 - size) / 2);

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    matrix[i, j] = _board[startPosition + i, startPosition + j];

            return matrix;
        }

        public string BoardToString(int[,] matrix)
        {
            StringBuilder sb = new StringBuilder();

            foreach (int num in matrix)
            {
                sb.Append(num);
            }

            return sb.ToString();
        }

        public bool DeleteBoard(Board board)
        {
            _context.Remove(board);

            return Save();
        }

        public bool DeleteBoards(List<Board> boards)
        {
            _context.RemoveRange(boards);

            return Save();
        }

        public Board GetBoard(int id)
        {
            return _context.Boards.Where(p => p.IdBoard == id).FirstOrDefault();
        }

        public ICollection<Board> GetBoards()
        {
            return _context.Boards.OrderBy(p => p.IdBoard).ToList();
        }

        public ICollection<Board> GetBoardsByGame(int gameId)
        {
            return _context.Boards.Where(p => p.IdGame == gameId).ToList();
        }

        public bool isBoardExist(int id)
        {
            return _context.Boards.Any(p => p.IdBoard == id);
        }

        public bool Save()
        {
            var saved = _context.SaveChanges();
            return saved > 0 ? true : false;
        }

        public bool UpdateBoard(Board board)
        {
            var existingBoard = _context.Boards.Find(board.IdBoard);

            if (existingBoard == null)
            {
                return false;
            }

            _context.Entry(existingBoard).CurrentValues.SetValues(board);

            return Save();
        }
    }
}
