using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BC_BACK.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string Username { get; set; } = null!;
    [JsonIgnore]
    public string Password { get; set; } = null!;

    public int Rights { get; set; }
    [JsonIgnore]
    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
