namespace BC_BACK.Dto
{
    public class GameDto
    {
        public int IdGame { get; set; }

        public DateTime? DateGame { get; set; }

        public int IdUser { get; set; }

        public string Name { get; set; } = null!;
    }
}
