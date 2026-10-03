using System.ComponentModel.DataAnnotations;

namespace StudySphere.Models.ViewModels
{
    public class QuizAttemptViewModel
    {
        public int QuizId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public List<QuizAttemptQuestionViewModel> Questions { get; set; } = new();

        [Required]
        public Dictionary<int, string> Answers { get; set; } = new();
    }

    public class QuizAttemptQuestionViewModel
    {
        public int QuizQuestionId { get; set; }

        public string Prompt { get; set; } = string.Empty;

        public decimal Marks { get; set; }

        public string OptionA { get; set; } = string.Empty;

        public string OptionB { get; set; } = string.Empty;

        public string OptionC { get; set; } = string.Empty;

        public string OptionD { get; set; } = string.Empty;
    }
}
