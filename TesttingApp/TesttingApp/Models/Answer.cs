namespace TesttingApp.Models
{
    public partial class Answer
    {
        public int AnswerId { get; set; }
        public int QuestionId { get; set; }
        public string? Text { get; set; }
        public bool IsCorrect { get; set; }

        public virtual Question Question { get; set; } = null!;

    }
}
