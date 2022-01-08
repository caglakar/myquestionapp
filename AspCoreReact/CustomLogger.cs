using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreReact
{
    public class CustomLogger
    {
        private readonly RequestDelegate _next;
        public CustomLogger(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            //yukarıdaki kodda next null ise hata fırlatacak değilse next'i _next'e atayacak.
            //name of(next) bize "next" şeklinde parametrenin adını yazacak. nameof yerine direk "next" de yazabilirdik.
            //ama kod içerisinde direk string değer yazmak doğru değildir yanlışlıkla next2 desek kodda hiç olmaya bir değişken adı yazlır  hataya
            //bu nedenle nameof(next) yazmak daha doğru

        }

        public async Task Invoke(HttpContext httpContext)
        {   if(httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            //TODO log the request
            await _next(httpContext);
            //TODO log the response

        }
    }
}
