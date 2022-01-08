using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using QandA.Data;
using QandA.Data.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace QandA.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionsController : ControllerBase
    {
        private readonly IDataRepository _dataRepository;
        private readonly IQuestionCache questionCache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _auth0UserInfo;

        public QuestionsController(IDataRepository dataRepository,IQuestionCache questionCache,IHttpClientFactory httpClientFactory,IConfiguration configuration)
        {
            _dataRepository = dataRepository;
            this.questionCache = questionCache;
            _httpClientFactory = httpClientFactory;
            _auth0UserInfo = $"{configuration["Auth0:Authority"]}userInfo";   
        }
        
        private async Task<string> GetUserName()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, _auth0UserInfo);
            request.Headers.Add("Authorization", Request.Headers["Authorization"].First());
            var client = _httpClientFactory.CreateClient();
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var jsonContent = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<User>(jsonContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return user.Name;
            }
            else return "";
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IEnumerable<QuestionGetManyResponse>> GetAll(string search, bool includeAnswers, int pageSize=20,int pageNumber=1)
        {
            IEnumerable<QuestionGetManyResponse> questions;
            if (string.IsNullOrEmpty(search))
            {
                if (includeAnswers)
                    questions =await _dataRepository.GetQuestionsWithAnswers();
                else
                    questions = await _dataRepository.GetQuestions();
            }
            else {
                //questions = _dataRepository.GetQuestionsBySearch(search); 
                questions = await _dataRepository.GetQuestionsBySearchWithPaging(search,pageNumber,pageSize);
            }
            return questions;
        }

        [AllowAnonymous]
        [HttpGet("unanswered")]
        public async Task<IEnumerable<QuestionGetManyResponse>> GetUnAnswered()
        {
               return await _dataRepository.GetUnansweredQuestionsAsync();
        }

        [AllowAnonymous]
        [HttpGet("{questionId}")]
        public async Task<ActionResult<QuestionGetSingleResponse>> GetQuestionById(int questionId)
        {
            var question = questionCache.Get(questionId);
            if (question == null) 
            {
                question= await _dataRepository.GetQuestion(questionId);
                if (question is null)
                    return NotFound();
                questionCache.Set(question);
            }
            return question;
        }

        
        [HttpPost]
        public async Task<IActionResult> PostQuestion(QuestionPostRequest question)
        {
            var newQuestion = await _dataRepository.PostQuestion(new QuestionPostFullRequest()
            {

                Title = question.Title,
                Content = question.Content,
                UserId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value,
                UserName =await GetUserName(),
                Created = System.DateTime.UtcNow
            });
            return CreatedAtAction(nameof(GetQuestionById), new {questionId=newQuestion.QuestionId }, newQuestion);
        }

        [Authorize(Policy = "MustBeQuestionAuthor")]
        [HttpPut("{questionId}")]
        public async Task<ActionResult<QuestionGetSingleResponse>> PutQuestion(int questionId, QuestionPutRequest questionPutRequest)
        {
            
            var currQuestion = await _dataRepository.GetQuestion(questionId);
            if (currQuestion == null)
                return NotFound();
            questionPutRequest.Title = string.IsNullOrEmpty(questionPutRequest.Title) ? currQuestion.Title : questionPutRequest.Title;
            questionPutRequest.Content = string.IsNullOrEmpty(questionPutRequest.Content) ? currQuestion.Content : questionPutRequest.Content;
             var updateQuest= await _dataRepository.PutQuestion(questionId, questionPutRequest);
            questionCache.Remove(questionId);
            return updateQuest;
        }
        
        [Authorize(Policy ="MustBeQuestionHandler")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int questionId)
        {
            var question=_dataRepository.GetQuestion(questionId);
            if (question == null)
                return NotFound();
          await  _dataRepository.DeleteQuestion(questionId);
            questionCache.Remove(questionId);
            return NoContent();
        }

        
        //to update answer first method
        
        [HttpPost("answer")]

        public async Task<ActionResult<AnswerGetResponse>> PostAnswer(AnswerPostRequest answer)
        {

            var questionExists = await _dataRepository.GetQuestion(answer.QuestionId.Value);
            if (questionExists == null)
                return NotFound();
            var answerReponse= await _dataRepository.PostAnswer(new AnswerPostFullRequest()
            {

                QuestionId = answer.QuestionId.Value,
                Content = answer.Content,
                UserId = HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier),
                UserName = await GetUserName(),
                Created = System.DateTime.UtcNow
            });

            questionCache.Remove(answer.QuestionId.Value);
            return answerReponse;
        }

        //to update answer second method
        [HttpPost("{questionId}/answer")]

        public async Task<ActionResult<AnswerGetResponse>> PostAnswer2(int questionId,AnswerPostRequest answer)
        {

            var questionExists = await _dataRepository.GetQuestion(questionId);
            if (questionExists == null)
                return NotFound();
            return await _dataRepository.PostAnswer(new AnswerPostFullRequest()
            {

                QuestionId = answer.QuestionId.Value,
                Content = answer.Content,
                UserId = "1",
                UserName = "cagla.test@test.com",
                Created = System.DateTime.UtcNow
            });
        }
    }
}

