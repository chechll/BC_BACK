namespace BC_BACK.Models
{
    public class ActualGame
    {
        public Game _game;
        public Board _board;
        public Team _team;
        public Queue<Team> _teams;
        public ActualGame(Game game) 
        { 
            _game = game;
        }


    }
}
