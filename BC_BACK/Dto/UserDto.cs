using System.Text.Json.Serialization;

namespace BC_BACK.Dto
{
    public class UserDto
    {
        public int IdUser { get; set; }

        public string Username { get; set; } = null!;

        public string Password { get; set; }

        public int Rights { get; set; }
    }
}
