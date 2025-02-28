using exam.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IQuestionRepository
{
    Task<List<CaseStudyViewModel>> GetCaseStudiesWithQuestionsAsync(); // ✅ Updated return type
}
