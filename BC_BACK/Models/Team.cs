using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
    [NotMapped]
    public int Steps { get; set; }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }

        var otherTeam = (Team)obj;
        return IdTeam == otherTeam.IdTeam;
    }

    public override int GetHashCode()
    {
        return IdTeam.GetHashCode();
    }
    [JsonIgnore]
    public virtual ICollection<AnsweredTask> AnsweredTasks { get; set; } = new List<AnsweredTask>();
    [JsonIgnore]
    public virtual Game IdGameNavigation { get; set; } = null!;
}
