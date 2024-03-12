using System;
using System.Collections.Generic;

namespace BC_BACK.Models;

public partial class Board
{
    public int IdBoard { get; set; }

    public string Board1 { get; set; } = null!;

    public int? IdGame { get; set; }

    public int Size { get; set; }

    public virtual Game? IdGameNavigation { get; set; }
}
