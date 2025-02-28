using System.ComponentModel;

namespace exam.Enums
{
    public enum QuestionType
    {
        Unknown,
        [Description("MCQ")]
        Choice,
        [Description("Fill Blanks")]
        Fill_Blanks,
        [Description("Reorder")]
        Arrangement,
        [Description("SELECT")]
        Selection,
        [Description("Matching")]
        Matching,
        [Description("Categorization")]
        Distribution,
        [Description("Rank Top")]
        Rank_Top
    }
}
