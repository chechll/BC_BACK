using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BC_BACK.Dto
{
    public class TeamDto
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
    }
}
