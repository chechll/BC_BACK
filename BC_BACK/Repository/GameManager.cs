namespace BC_BACK.Repository
{
    public class GameManager
    {
        private static Queue<Game>? _activeGames;
        private static Dictionary<int, List<Team>>? _gameTeams;

        public GameManager()
        {
            _activeGames = new Queue<Game>();
            _gameTeams = new Dictionary<int, List<Team>>();
        }

        public string CheckCurrentTeam(int idGame, int idTeam)
        {
            var game = FindGameById(idGame);
            if (game != null)
            {
                if (game.GetCurrentTeamId() == idTeam) {
                    return "Ok";
                }
                return "Still not current";
            }
            return "There is no such team in active games";
        }

        public string AddActiveGame(Game game, List<Team> teams)
        {
            if (_activeGames != null &&  _gameTeams != null && !_activeGames.Any(g => g.IdGame == game.IdGame))
            {
                _activeGames.Enqueue(game);
                game.DateGame = DateTime.Now;
                _gameTeams.Add(game.IdGame, ResetStepsForTeams(teams));
                System.Threading.Tasks.Task.Run(async () => await RemoveFirstGameFromQueueAsync());
                return "Added successfully!";
            }
            return "Game is already active";
        }

        public string RemoveActiveGame(Game game)
        {
            if (_activeGames != null && _gameTeams != null &&  _activeGames.Any(g => g.IdGame == game.IdGame))
            {
                _activeGames = new Queue<Game>(_activeGames.Where(g => g.IdGame != game.IdGame));
                _gameTeams.Remove(game.IdGame);
                return "Removed successfully!";
            }
            return "There is no such active game";
        }

        public Game? FindGameById(int gameId)
        {
            return _activeGames?.FirstOrDefault(game => game.IdGame == gameId);
        }

        public Queue<Game>? GetAllGames()
        {
            return _activeGames;
        }

        public string AddTeamToQueue(Team team)
        {
            var game = FindGameById(team.IdGame);
            if (game != null)
            {
                return game.EnqueueTeam(team);
            }
            return "There is no such team in active games";
        }

        public string RemoveTeamFromQueue(Team team)
        {
            var game = FindGameById(team.IdGame);
            if (game != null)
            {
                return game.RemoveTeamFromQueue(team);
            }
            return "There is no such team in active games";
        }

        public async Task<string> RemoveFirstGameFromQueueAsync()
        {
            if (_activeGames != null && _activeGames.Count > 0)
            {
                var firstGame = _activeGames.Peek();
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromMinutes(120));
                _activeGames.Dequeue();
                return $"Game {firstGame.Name} removed after waiting for a 60 minutes.";
            }
            return "There are no games in the queue.";
        }

        public List<Team>? GetGameTeams(int gameId)
        {
            return _gameTeams != null && _gameTeams.TryGetValue(gameId, out var teams) ? teams : null;
        }

        private static List<Team> ResetStepsForTeams(List<Team> teams)
        {
            foreach (var team in teams)
            {
                team.Steps = 0;
            }
            return teams;
        }

        public Team? GetTeam (int idGame, int idTeam)
        {
            return _gameTeams != null && _gameTeams.TryGetValue(idGame, out var teams) ? teams.FirstOrDefault(team => team.IdTeam == idTeam) : null;
        }

        public string UpdateTeam(Team updatedTeam)
        {
            if (_gameTeams != null &&_gameTeams.TryGetValue(updatedTeam.IdGame, out var teams))
            {
                var existingTeamIndex = teams.FindIndex(team => team.IdTeam == updatedTeam.IdTeam);
                if (existingTeamIndex != -1)
                {
                    teams[existingTeamIndex] = updatedTeam;
                    return "Ok";
                }
                return "Team not found for the given teamId.";
            }
            return "No teams found for the given gameId.";
        }

        public string CheckGame(int idGame)
        {
            if (_activeGames == null)
                return "No";
            return _activeGames.Any(g => g.IdGame == idGame) ? "Yes" : "No";
        }
    }
}