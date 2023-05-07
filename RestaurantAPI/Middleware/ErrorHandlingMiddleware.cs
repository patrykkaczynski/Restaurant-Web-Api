using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RestaurantAPI.Exceptions;
using System.Net;
using System.Text.Json;

namespace RestaurantAPI.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ErrorHandlingMiddleware> _logger; 
        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
			try
			{
				await next.Invoke(context);
			}
            catch(ForbidException forbidException)
            {
                var problemDetails = new ValidationProblemDetails(new Dictionary<string, string[]> { { "Error", new[] { forbidException.Message } } })
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.3",
                    Title = "Forbidden",
                    Status = (int)HttpStatusCode.Forbidden,
                    Instance = context.Request.Path
                };

                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await HandleExceptionAsync(context, problemDetails);

            }
            catch (BadRequestException badRequestException)
            {
                var problemDetails = new ValidationProblemDetails(new Dictionary<string, string[]> { { "Error", new[] { badRequestException.Message } } })
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
                    Title = "Bad Request",
                    Status = (int)HttpStatusCode.BadRequest,
                    Instance = context.Request.Path
                };

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await HandleExceptionAsync(context, problemDetails);
            }
            catch (NotFoundException notFoundException)
            {
                var problemDetails = new ValidationProblemDetails(new Dictionary<string, string[]> { { "Error", new[] { notFoundException.Message } } })
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                    Title = "Not Found",
                    Status = (int)HttpStatusCode.NotFound,
                    Instance = context.Request.Path
                };

                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                await HandleExceptionAsync(context, problemDetails);
            }
			catch (Exception e)
			{

                _logger.LogError(e, e.Message);

                var problemDetails = new ValidationProblemDetails(new Dictionary<string, string[]> { { "Error", new[] { e.Message } } })
                {
                    Type = "https://tools.ietf.org/html/rfc7231#section-6.6.1",
                    Title = "Internal Server Error",
                    Status = (int)HttpStatusCode.InternalServerError,
                    Instance = context.Request.Path
                };

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await HandleExceptionAsync(context, problemDetails);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, ValidationProblemDetails problemDetails)
        {
            var result = JsonSerializer.Serialize(problemDetails);
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(result);
        }
    }
}
