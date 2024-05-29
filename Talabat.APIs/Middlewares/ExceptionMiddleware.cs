using System.Net;
using System.Text.Json;
using Talabat.APIs.Errors;

namespace Talabat.APIs.Middlewares
{
    public class ExceptionMiddleware
    {
        /*
         * ده من خلال حاجتين : Middleware طب هو هيعرف منين انها 
         * 1.Middleware بعده اسم  Class ان اسم ال
         * 2.InvokeAsync اسمها Method عندها 
         * 3. app.UseMiddleware<ExceptionMiddleware>(); بالشكل ده Middleware ضفناها ك  Program في ملف ال
         */

        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            //الي بعدها Middleware السيناريو اني هقوله لو الرد بتاعك مافيهوش مشكله ابعتي لل
            //console في شاشة ال log ولو فيه ايرور ابعتلي الايرور واعمله 
            try
            {
                await _next.Invoke(context); //الي بعدي ايا كان فيه ايرور Middleware معناها اني بقوله اتنقل لل 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message); //Console السطر ده بيخلي ال يطلعلعي في ال
                //عندي يتخزن في الداتابيز log والصح ان ال    
                context.Response.ContentType = "application/json";
                context.Response.StatusCode= (int)HttpStatusCode.InternalServerError;

                var response = _env.IsDevelopment() ? new ApiExceptionResponse(500, ex.Message, ex.StackTrace.ToString())
                                                    : new ApiExceptionResponse(500);
                var jsonResult = JsonSerializer.Serialize(response); //String To json ده علشان احول من ال
                
                await context.Response.WriteAsync(jsonResult);
            }
        }
    }
}
