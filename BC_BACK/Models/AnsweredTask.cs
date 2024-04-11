using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BC_BACK.Models;

public partial class AnsweredTask
{
    public int Id { get; set; }

    public int IdTask { get; set; }

    public int IdTeam { get; set; }
    [JsonIgnore]
    public virtual Task IdTaskNavigation { get; set; } = null!;
    [JsonIgnore]
    public virtual Team IdTeamNavigation { get; set; } = null!;
}
