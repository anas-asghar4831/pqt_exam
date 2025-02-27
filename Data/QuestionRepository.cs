using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using exam.ViewModels;
using Microsoft.Extensions.Configuration;

public class QuestionRepository : IQuestionRepository
{
    private readonly string _connectionString;

    public QuestionRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException(nameof(configuration), "Connection string cannot be null");
    }

    public async Task<List<QuestionViewModel>> GetQuestionsAsync()
    {
        var questions = new List<QuestionViewModel>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            string query = @"
                SELECT 
                    cq.Id AS QuestionId,
                    cq.CaseStudyId,
                    cs.Name AS CaseStudyName,
                    cs.Description AS CaseStudyDescription,
                    cq.Description AS QuestionDescription,
                    cq.ExtraContext,
                    cq.QuestionType,
                    cq.MediaPath AS QuestionMediaPath,
                    cq.SortId AS QuestionSortId,
                    co.Id AS OptionId,
                    co.Description AS OptionDescription,
                    co.SortId AS OptionSortId,
                    co.MediaPath AS OptionMediaPath,
                    co.IsAnswer
                FROM [PQT_DB].[dbo].[Content_Question] cq
                LEFT JOIN [PQT_DB].[dbo].[Content_CaseStudy] cs ON cq.CaseStudyId = cs.Id
                LEFT JOIN [PQT_DB].[dbo].[Content_Option] co ON cq.Id = co.QuestionId;";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                var questionDict = new Dictionary<int, QuestionViewModel>();

                while (await reader.ReadAsync())
                {
                    int questionId = reader.GetInt32(reader.GetOrdinal("QuestionId"));

                    if (!questionDict.ContainsKey(questionId))
                    {
                        questionDict[questionId] = new QuestionViewModel
                        {
                            QuestionId = questionId,
                            CaseStudyId = reader.GetInt32(reader.GetOrdinal("CaseStudyId")),
                            CaseStudyName = reader.GetString(reader.GetOrdinal("CaseStudyName")),
                            CaseStudyDescription = reader.GetString(reader.GetOrdinal("CaseStudyDescription")),
                            QuestionDescription = reader.GetString(reader.GetOrdinal("QuestionDescription")),
                            ExtraContext = reader.GetString(reader.GetOrdinal("ExtraContext")),
                            QuestionType = reader.GetString(reader.GetOrdinal("QuestionType")),
                            QuestionMediaPath = reader.IsDBNull(reader.GetOrdinal("QuestionMediaPath")) ? null : reader.GetString(reader.GetOrdinal("QuestionMediaPath")),
                            QuestionSortId = reader.GetInt32(reader.GetOrdinal("QuestionSortId")),
                            Options = new List<OptionViewModel>()
                        };
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("OptionId")))
                    {
                        questionDict[questionId].Options.Add(new OptionViewModel
                        {
                            OptionId = reader.GetInt32(reader.GetOrdinal("OptionId")),
                            OptionDescription = reader.GetString(reader.GetOrdinal("OptionDescription")),
                            OptionSortId = reader.GetInt32(reader.GetOrdinal("OptionSortId")),
                            OptionMediaPath = reader.IsDBNull(reader.GetOrdinal("OptionMediaPath")) ? null : reader.GetString(reader.GetOrdinal("OptionMediaPath")),
                            IsAnswer = reader.GetBoolean(reader.GetOrdinal("IsAnswer"))
                        });
                    }
                }

                questions = questionDict.Values.OrderBy(q => q.QuestionSortId).ToList();

                foreach (var question in questions)
                {
                    question.Options = question.Options.OrderBy(o => o.OptionSortId).ToList();
                }
            }
        }

        return questions;
    }
}
