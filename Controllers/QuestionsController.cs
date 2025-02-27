using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

public class QuestionsController : Controller
{
    private readonly IQuestionRepository _questionRepository;

    public QuestionsController(IQuestionRepository questionRepository)
    {
        _questionRepository = questionRepository;
    }

    public async Task<IActionResult> Index()
    {
        var questions = await _questionRepository.GetQuestionsAsync();
        return View(questions);
    }
}
