namespace exam.ViewModels
{
    //public class ExamViewModel
    //{
    //}
    public class QuestionViewModel
    {
        public int QuestionId { get; set; }
        public int CaseStudyId { get; set; }
        public string? CaseStudyName { get; set; }
        public string? CaseStudyDescription { get; set; }
        public string? QuestionDescription { get; set; }
        public string? ExtraContext { get; set; }
        public string? QuestionType { get; set; }
        public string? QuestionMediaPath { get; set; }
        public int QuestionSortId { get; set; }
        public List<OptionViewModel> Options { get; set; } = new List<OptionViewModel>();
    }

    public class OptionViewModel
    {
        public int OptionId { get; set; }
        public string? OptionDescription { get; set; }
        public int OptionSortId { get; set; }
        public string? OptionMediaPath { get; set; }
        public bool IsAnswer { get; set; }
    }

}
