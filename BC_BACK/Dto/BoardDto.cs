namespace BC_BACK.Dto
{
    public class BoardDto
    {
        public int IdBoard { get; set; }

        public string Board1 { get; set; } = null!;

        public int? IdGame { get; set; }

        public int Size { get; set; }
    }
}
