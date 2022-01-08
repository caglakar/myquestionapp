﻿using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using QandA.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace QandA.Data
{
    public class DataRepository : IDataRepository
    {
        private readonly string _connectionString;

        public DataRepository(IConfiguration configuration)
        {
            _connectionString = configuration["ConnectionStrings:DefaultConnection"];

        }

        public async Task DeleteQuestion(int questionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(@"EXEC dbo.Question_Delete @QuestionId=@QuestionId", new {QuestionId=questionId});
                
            }
        }

        public async Task<AnswerGetResponse> GetAnswer(int answerId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                return await connection.QueryFirstOrDefaultAsync<AnswerGetResponse>(@"EXEC dbo.Answer_Get_ByAnswerId @AnswerId=@AnswerId", new { AnswerId = answerId });

                
            }
        }

        //public QuestionGetSingleResponse GetQuestion(int questionId)
        //{
        //    using (var connection = new SqlConnection(_connectionString))
        //    {
        //        connection.Open();
        //        using (GridReader results = connection.QueryMultiple(@"EXEC dbo.Question_GetSingle @QuestionId=@QuestionId; EXEC dbo.Answer_Get_ByQuestionId @QuestionId=@QuestionId", new { QuestionId = questionId }))
        //        {
        //            var question = results.Read<QuestionGetSingleResponse>().FirstOrDefault();
        //            if (question != null)
        //            {
        //                question.Answers = results.Read<AnswerGetResponse>().ToList();
        //            }

        //            return question;
        //        }
        //    } }

        public async Task<QuestionGetSingleResponse> GetQuestion(int questionId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (GridReader results = await connection.QueryMultipleAsync(@"EXEC dbo.Question_GetSingle @QuestionId=@QuestionId; EXEC dbo.Answer_Get_ByQuestionId @QuestionId=@QuestionId", new { QuestionId = questionId }))
                {
                    var question = results.Read<QuestionGetSingleResponse>().FirstOrDefault();
                    if (question != null)
                    {
                        question.Answers = results.Read<AnswerGetResponse>().ToList();
                    }

                    return question;
                }
            }
        }

        public async Task<IEnumerable<QuestionGetManyResponse>> GetQuestions()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
               await connection.OpenAsync();
                return await connection.QueryAsync<QuestionGetManyResponse>(@"EXEC dbo.Question_GetMany");
            }
        }

        public async Task<IEnumerable<QuestionGetManyResponse>> GetQuestionsWithAnswers()
        {
            #region This method is classic N+1 trap!
            //using(var connection =  new SqlConnection(_connectionString))
            //{
            //    connection.Open();
            //    var questions = connection.Query<QuestionGetManyResponse>(@"EXEC dbo.Question_GetMany");

            //    foreach (var question in questions)
            //    {
            //        question.Answers = connection.Query<AnswerGetResponse>(@"EXEC dbo.Answer_Get_ByQuestionId @QuestionId=@QuestionId", new { QuestionId = question.QuestionId }).ToList();
            //    }
            //    return questions;
            //}
            #endregion This method is classic N+1 trap!

            using (var connection = new SqlConnection(_connectionString))
            {
               await connection.OpenAsync();
                var questionsDictionary = new Dictionary<int, QuestionGetManyResponse>();

               return (await connection.QueryAsync<QuestionGetManyResponse, AnswerGetResponse, QuestionGetManyResponse>(@"EXEC dbo.Question_GetMany_WithAnswers",
                   map: (q, a) =>
                {
                    QuestionGetManyResponse question;
                    if (!questionsDictionary.TryGetValue(q.QuestionId, out question))
                    {
                        question = q;
                        question.Answers = new List<AnswerGetResponse>();
                        questionsDictionary.Add(question.QuestionId, question);
                    }
                    question.Answers.Add(a);
                    return question;
                },
                splitOn: "QuestionId")).Distinct().ToList(); 
            }
        }

        public async Task<IEnumerable<QuestionGetManyResponse>> GetQuestionsBySearch(string search)
        {
         using(var connection = new SqlConnection(_connectionString))
            {
               await connection.OpenAsync();
                return await connection.QueryAsync<QuestionGetManyResponse>(@"EXEC dbo.Question_GetMany_BySearch @Search=@Search", new { Search = search });
            }
        }


        public async Task<IEnumerable<QuestionGetManyResponse>> GetQuestionsBySearchWithPaging(string search, int pageNumber, int pageSize)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection .OpenAsync();
                var parameters = new
                {
                    Search = search,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
                return await connection.QueryAsync<QuestionGetManyResponse>(@"EXEC dbo.Question_GetMany_BySearch_WithPaging @Search=@Search,@PageNumber=@PageNumber,@PageSize=@PageSize", parameters);
            }
        }


            public async Task<IEnumerable<QuestionGetManyResponse>> GetUnansweredQuestions()
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                   await connection.OpenAsync();
                    return await connection.QueryAsync<QuestionGetManyResponse>(@"EXEC dbo.Question_GetUnanswered");
                }
            }


            public async Task<IEnumerable<QuestionGetManyResponse>> GetUnansweredQuestionsAsync()
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                 await   connection.OpenAsync();
                    var resonse= await connection.QueryAsync<QuestionGetManyResponse>(@"EXEC dbo.Question_GetUnanswered");
                    return resonse;
                }
            }

        public async Task<AnswerGetResponse> PostAnswer(AnswerPostFullRequest answer)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
            return await connection.QueryFirstAsync<AnswerGetResponse>(@"EXEC dbo.Answer_Post @QuestionId=@QuestionId, @Content=@Content, @UserId=@UserId, @UserName=@UserName,@Created=@Created", answer);

                
            }
        }

        public async Task<QuestionGetSingleResponse> PostQuestion(QuestionPostFullRequest question)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection .OpenAsync();
                var questionId = await connection.QueryFirstAsync<int>(@"EXEC dbo.Question_Post @Title=@Title, @Content=@Content, @UserId=@UserId, @UserName=@UserName,@Created=@Created",question);
                
                return await GetQuestion(questionId);
            }
        }

            
        public async Task<QuestionGetSingleResponse> PutQuestion(int questionId, QuestionPutRequest question)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
               await  connection.OpenAsync();
               await connection.ExecuteAsync(@"EXEC dbo.Question_Put @QuestionId=@QuestionId, @Title=@Title, @Content=@Content",new { QuestionId = questionId,Title= question.Title, Content=question.Content });
                return await GetQuestion(questionId);
            }
        }

        public async Task<bool> QuestionExists(int questionId)
        {

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection .OpenAsync();
                var question = await connection.QueryFirstAsync<bool>(@"EXEC dbo.Question_Exists @QuestionId=@QuestionId", new { QuestionId = questionId });
                
                return question;
            }
        }
    }
}
