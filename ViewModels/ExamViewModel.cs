using exam.Enums;
using System.Collections.Generic;
using System.ComponentModel;
namespace exam.ViewModels
{
    public class CaseStudyViewModel
    {
        public int CaseStudyId { get; set; }
        public string CaseStudyName { get; set; }
        public string CaseStudyDescription { get; set; }
        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
    }

    public class QuestionViewModel
    {
        public int QuestionId { get; set; }
        public string QuestionDescription { get; set; }
        public string ExtraContext { get; set; }
        public QuestionType QuestionType { get; set; }
        public string QuestionMediaPath { get; set; }
        public int QuestionSortId { get; set; }
        public List<OptionViewModel> Options { get; set; } = new List<OptionViewModel>();

        // Tags for Question
        public SubSkillViewModel SubSkill { get; set; }
        public SkillViewModel Skill { get; set; }
        public CompetencyViewModel Competency { get; set; }
    }

    public class OptionViewModel
    {
        public int OptionId { get; set; }
        public string OptionDescription { get; set; }
        public int OptionSortId { get; set; }
        public string OptionMediaPath { get; set; }
        public bool IsAnswer { get; set; }
    }

    public class SubSkillViewModel
    {
        public int SubSkillId { get; set; }
        public string SubSkillName { get; set; }
    }

    public class SkillViewModel
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; }
    }
    
    public class CompetencyViewModel
    {
        public int CompetencyId { get; set; }
        public string CompetencyName { get; set; }
    }

}
