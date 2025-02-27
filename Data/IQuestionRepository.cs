using exam.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IQuestionRepository
{
    Task<List<QuestionViewModel>> GetQuestionsAsync();
}
