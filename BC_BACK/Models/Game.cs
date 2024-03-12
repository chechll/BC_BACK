using System;
using System.Collections.Generic;

namespace BC_BACK.Models;

public partial class Game
{
    public int IdGame { get; set; }

    public DateTime DateGame { get; set; }

    public int IdUser { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Board> Boards { get; set; } = new List<Board>();

    public virtual User IdUserNavigation { get; set; } = null!;

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
}
