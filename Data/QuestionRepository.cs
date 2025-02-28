using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Linq;
using exam.Enums;
using exam.ViewModels;

public class QuestionRepository : IQuestionRepository
{
    private readonly string _connectionString;

    public QuestionRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException(nameof(configuration), "Connection string cannot be null");
    }

    public async Task<List<CaseStudyViewModel>> GetCaseStudiesWithQuestionsAsync()
    {
        var caseStudies = new Dictionary<int, CaseStudyViewModel>();

        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            string query = @"
                SELECT 
                    cs.Id AS CaseStudyId, cs.Name AS CaseStudyName, cs.Description AS CaseStudyDescription,
                    cq.Id AS QuestionId, cq.Description AS QuestionDescription, cq.ExtraContext, 
                    cq.QuestionType, cq.MediaPath AS QuestionMediaPath, cq.SortId AS QuestionSortId,
                    co.Id AS OptionId, co.Description AS OptionDescription, co.SortId AS OptionSortId, 
                    co.MediaPath AS OptionMediaPath, co.IsAnswer,
                    cq.SubSkillId, ss.Name AS SubSkillName, ss.SkillId,
                    sk.Name AS SkillName, sk.CompetencyId,
                    cp.Name AS CompetencyName
                FROM [PQT_DB].[dbo].[Content_CaseStudy] cs
                LEFT JOIN [PQT_DB].[dbo].[Content_Question] cq ON cs.Id = cq.CaseStudyId
                LEFT JOIN [PQT_DB].[dbo].[Content_Option] co ON cq.Id = co.QuestionId
                LEFT JOIN [PQT_DB].[dbo].[Content_SubSkill] ss ON cq.SubSkillId = ss.Id
                LEFT JOIN [PQT_DB].[dbo].[Content_Skill] sk ON ss.SkillId = sk.Id
                LEFT JOIN [PQT_DB].[dbo].[Content_Competency] cp ON sk.CompetencyId = cp.Id;";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    int caseStudyId = (int)reader.GetInt64(reader.GetOrdinal("CaseStudyId"));

                    if (!caseStudies.ContainsKey(caseStudyId))
                    {
                        caseStudies[caseStudyId] = new CaseStudyViewModel
                        {
                            CaseStudyId = caseStudyId,
                            CaseStudyName = reader.GetString(reader.GetOrdinal("CaseStudyName")),
                            CaseStudyDescription = reader.GetString(reader.GetOrdinal("CaseStudyDescription")),
                            Questions = new List<QuestionViewModel>()
                        };
                    }

                    if (!reader.IsDBNull(reader.GetOrdinal("QuestionId")))
                    {
                        int questionId = (int)reader.GetInt64(reader.GetOrdinal("QuestionId"));

                        var question = caseStudies[caseStudyId].Questions.FirstOrDefault(q => q.QuestionId == questionId);
                        if (question == null)
                        {
                            question = new QuestionViewModel
                            {
                                QuestionId = questionId,
                                QuestionDescription = reader.GetString(reader.GetOrdinal("QuestionDescription")),
                                ExtraContext = reader.IsDBNull(reader.GetOrdinal("ExtraContext")) ? null : reader.GetString(reader.GetOrdinal("ExtraContext")),
                                QuestionType = (QuestionType)reader.GetInt32(reader.GetOrdinal("QuestionType")),
                                QuestionMediaPath = reader.IsDBNull(reader.GetOrdinal("QuestionMediaPath")) ? null : reader.GetString(reader.GetOrdinal("QuestionMediaPath")),
                                QuestionSortId = (int)reader.GetInt64(reader.GetOrdinal("QuestionSortId")),
                                Options = new List<OptionViewModel>(),  // ✅ Ensure options list is initialized
                                SubSkill = new SubSkillViewModel
                                {
                                    SubSkillId = (int)reader.GetInt64(reader.GetOrdinal("SubSkillId")),
                                    SubSkillName = reader.GetString(reader.GetOrdinal("SubSkillName"))
                                },
                                Skill = new SkillViewModel
                                {
                                    SkillId = (int)reader.GetInt64(reader.GetOrdinal("SkillId")),
                                    SkillName = reader.GetString(reader.GetOrdinal("SkillName"))
                                },
                                Competency = new CompetencyViewModel
                                {
                                    CompetencyId = (int)reader.GetInt64(reader.GetOrdinal("CompetencyId")),
                                    CompetencyName = reader.GetString(reader.GetOrdinal("CompetencyName"))
                                }
                            };

                            caseStudies[caseStudyId].Questions.Add(question);
                        }

                        // ✅ Now add options to the question
                        if (!reader.IsDBNull(reader.GetOrdinal("OptionId")))
                        {
                            var option = new OptionViewModel
                            {
                                OptionId = (int)reader.GetInt64(reader.GetOrdinal("OptionId")),
                                OptionDescription = reader.GetString(reader.GetOrdinal("OptionDescription")),
                                OptionSortId = (int)reader.GetInt64(reader.GetOrdinal("OptionSortId")),
                                OptionMediaPath = reader.IsDBNull(reader.GetOrdinal("OptionMediaPath")) ? null : reader.GetString(reader.GetOrdinal("OptionMediaPath")),
                                IsAnswer = !reader.IsDBNull(reader.GetOrdinal("IsAnswer")) && reader.GetBoolean(reader.GetOrdinal("IsAnswer"))
                            };

                            question.Options.Add(option);
                        }
                    }
                }
            }
        }

        return caseStudies.Values.OrderBy(cs => cs.CaseStudyId).ToList();
    }
}
