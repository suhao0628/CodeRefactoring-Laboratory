using AutoMapper.Internal;
using Delivery.Api.Exceptions;
using Delivery.Api.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
namespace Delivery.Api.Filters
{
    public class GlobalExceptionFilter : Attribute, IAsyncExceptionFilter
    {
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            var exceptionType = context.Exception.GetType();
            if (context.ExceptionHandled == false)
            {
                if (exceptionType == typeof(UnauthorizedException))
                {
                    context.Result = new ContentResult
                    {
                        StatusCode = StatusCodes.Status401Unauthorized,
                        ContentType = "application/json",
                    };
                }
                else if (exceptionType == typeof(NotFoundException))
                {
                    context.Result = new ContentResult
                    {
                        StatusCode = StatusCodes.Status404NotFound,
                        ContentType = "application/json",
                    };
                }
                else if (exceptionType == typeof(BadRequestException))
                {
                    Response res = new()
                    {
                        Status = StatusCodes.Status400BadRequest.ToString(),
                        Message = ""
                    };
                    context.Result = new ContentResult
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        ContentType = "application/json",
                        Content = JsonConvert.SerializeObject(res)
                    };
                }
                else if (exceptionType == typeof(ForbiddenException))
                {
                    context.Result = new ContentResult
                    {
                        StatusCode = StatusCodes.Status403Forbidden,
                        ContentType = "application/json",
                    };
                }
                else if(exceptionType == typeof(Exception))
                {
                    Response res = new()
                    {
                        Status = StatusCodes.Status500InternalServerError.ToString(),
                        Message = context.Exception.Message
                    };

                    context.Result = new ContentResult
                    {
                        StatusCode = StatusCodes.Status500InternalServerError,
                        ContentType = "application/json",
                        Content = JsonConvert.SerializeObject(res)
                    };
                }
            }
            context.ExceptionHandled = true;
            await Task.CompletedTask;
        }
    }
}


