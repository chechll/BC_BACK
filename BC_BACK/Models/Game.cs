using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Threading;

namespace BC_BACK.Models
{
    public partial class Game
    {
        public int IdGame { get; set; }
        public DateTime? DateGame { get; set; }
        public int IdUser { get; set; }
        public string Name { get; set; } = null!;

        [NotMapped]
        public Queue<Team> TeamQueue { get; set; }

        [NotMapped]
        private Team currentTeam { get; set; }

        [NotMapped]
        private Timer timer;

        public Game()
        {
            TeamQueue = new Queue<Team>();
        }

        public string EnqueueTeam(Team team)
        {
            lock (TeamQueue)
            {

                if (TeamQueue.Contains(team) || (currentTeam != null && currentTeam.IdTeam == team.IdTeam))
                {
                    return $"Team is already in the queue";
                }

                TeamQueue.Enqueue(team);
                Console.WriteLine($"Team {team.Name} added to the queue");

                if (currentTeam == null)
                {
                    StartNextTeamTimer();
                    return $"Team started";
                }
                return $"Team {team.Name} added to the queue";
            }
        }

        public int getCurrentTeamId()
        {
            if (currentTeam != null)
            return currentTeam.IdTeam;
            return 0;
        }

        private void StartNextTeamTimer()
        {
            if (TeamQueue.Count > 0)
            {
                currentTeam = TeamQueue.Dequeue();
                Console.WriteLine($"Timer started for team: {currentTeam.Name}");
                timer = new Timer(ReleaseCurrentTeam, null, TimeSpan.FromMinutes(2.9), TimeSpan.FromMilliseconds(-1));
            }
        }

        private void ReleaseCurrentTeam(object state)
        {
            lock (TeamQueue)
            {
                Console.WriteLine($"Timer stopped for team: {currentTeam.Name}");
                currentTeam = null;
                if (TeamQueue.Count > 0)
                {
                    StartNextTeamTimer();
                }
            }
        }

        public string RemoveTeamFromQueue(Team team)
        {
            lock (TeamQueue)
            {
                string response = "there is no such team";
                Console.WriteLine("Curteam" + currentTeam.IdTeam + "team" + team.IdTeam);
                if (currentTeam != null && currentTeam.IdTeam == team.IdTeam)
                {
                    currentTeam = null;
                    timer?.Dispose();
                    Console.WriteLine($"Timer stopped for team: {team.Name}");
                    response = $"Timer stopped for team: {team.Name}\n Team {team.Name} removed";
                    if (currentTeam == null && TeamQueue.Count > 0)
                    {
                        StartNextTeamTimer();
                    }
                }
                else
                {
                    if (TeamQueue.Contains(team))
                    {
                        var removedTeam = TeamQueue.Dequeue();
                        Console.WriteLine($"Team {removedTeam.Name} removed from the queue.");
                        response = $"Team {removedTeam.Name} removed from the queue successfully.";
                    }
                }
                    return response;    
            }
        }

        public Queue<Team> GetTeamQueue()
        {
            return TeamQueue;
        }

        public virtual ICollection<Board> Boards { get; set; } = new List<Board>();
        public virtual User IdUserNavigation { get; set; } = null!;
        public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
        public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}