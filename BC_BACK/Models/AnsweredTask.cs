using System;
using System.Collections.Generic;

namespace BC_BACK.Models;

public partial class AnsweredTask
{
    public int Id { get; set; }

    public int IdTask { get; set; }

    public int IdTeam { get; set; }

    public virtual Task IdTaskNavigation { get; set; } = null!;

    public virtual Team IdTeamNavigation { get; set; } = null!;
}
