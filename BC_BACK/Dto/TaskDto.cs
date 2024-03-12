namespace BC_BACK.Dto
{
    public class TaskDto
    {
        public int IdTask { get; set; }

        public int Number { get; set; }

        public string Answer { get; set; } = null!;

        public string? Question { get; set; }

        public int IdGame { get; set; }
    }
}
