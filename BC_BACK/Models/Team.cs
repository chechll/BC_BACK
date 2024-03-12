using System;
using System.Collections.Generic;

namespace BC_BACK.Models;

public partial class Team
{
    public int IdTeam { get; set; }

    public string Colour { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int? PositionX { get; set; }

    public int? PositionY { get; set; }

    public string Password { get; set; } = null!;

    public int IdGame { get; set; }

    public int Score { get; set; }

    public virtual ICollection<AnsweredTask> AnsweredTasks { get; set; } = new List<AnsweredTask>();

    public virtual Game IdGameNavigation { get; set; } = null!;
}
