using Microsoft.Extensions.Caching.Memory;
using QandA.Data.Models;
using System;

namespace QandA.Data
{
    public class QuestionCache : IQuestionCache
    {
        private readonly IMemoryCache memoryCache;
        private string GetCacheKey(int questionId) => $"Question-{questionId}";
        public QuestionCache(IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
        }
        public QuestionGetSingleResponse Get(int questionId)
        {
            QuestionGetSingleResponse response= new QuestionGetSingleResponse();
            memoryCache.TryGetValue(GetCacheKey(questionId), out response);
            return response;
        }

        public void Remove(int questionId)
        {
            memoryCache.Remove(GetCacheKey(questionId));
        }

        public void Set(QuestionGetSingleResponse question)
        {
            memoryCache.Set(GetCacheKey(question.QuestionId), question, new MemoryCacheEntryOptions().SetAbsoluteExpiration(DateTime.Now.AddSeconds(1000)).SetSize(1));
        }
    }
}
