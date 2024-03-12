using System;
using System.Collections.Generic;

namespace BC_BACK.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int Rights { get; set; }

    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
