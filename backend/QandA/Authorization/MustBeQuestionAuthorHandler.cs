﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using QandA.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QandA.Authorization
{
    public class MustBeQuestionAuthorHandler : AuthorizationHandler<MustBeQuestionAuthorRequirement>
    {
        private  readonly IDataRepository _dataRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public MustBeQuestionAuthorHandler(IDataRepository dataRepository, IHttpContextAccessor httpContextAccessor)
        {
            _dataRepository = dataRepository;
            _httpContextAccessor = httpContextAccessor;

        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, MustBeQuestionAuthorRequirement requirement)
        {
           if(!context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return;
            }

            var questionId=_httpContextAccessor.HttpContext.Request.RouteValues["questionId"];
            int questionIdAsInt = Convert.ToInt32(questionId);

            var userId = context.User.FindFirst(ClaimTypes.NameIdentifier).Value;

           var question = await _dataRepository.GetQuestion(questionIdAsInt);
            if(question==null)
            {
                //let it through so the controller can return http404 error code
                context.Succeed(requirement);
                return; 
            }

            if(question.UserId  != userId)
            {
                context.Fail();
                return;

            }
            context.Succeed(requirement);
        }
      
    }
}