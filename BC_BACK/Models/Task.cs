using System;
using System.Collections.Generic;

namespace BC_BACK.Models;

public partial class Task
{
    public int IdTask { get; set; }

    public int Number { get; set; }

    public string Answer { get; set; } = null!;

    public string? Question { get; set; }

    public int IdGame { get; set; }

    public virtual ICollection<AnsweredTask> AnsweredTasks { get; set; } = new List<AnsweredTask>();

    public virtual Game IdGameNavigation { get; set; } = null!;
}
