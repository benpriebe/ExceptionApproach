using System.Linq;
using Exception.Core;
using Exception.Core.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Resx = Exception.Host.i18n.Message;

namespace Exception.Host.Controllers
{
    [ApiController]
    [Route("exception/example")]
    public class ExceptionController : ControllerBase
    {
        private readonly ILogger<ExceptionController> logger;

        public ExceptionController(ILogger<ExceptionController> logger) {
            this.logger = logger;
        }

        [HttpGet("no-content")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult GetNoContent()
        {
            Result result = Result.New().WithInfo(Resx.MsgKeyInfo, new { type = "some dynamic juicy content just for you" });
            if (result.Messages.Any())
                return new OkObjectResult(result);
            return NoContent();
        }

        [HttpGet("content")]
        public IActionResult GetContent()
        {
            var response = new Response { Name = "jimbo jones", Mobile = "0400 123 123" };
            return new OkObjectResult(response);
        }
        
        [HttpGet("result-content")]
        public IActionResult GetResultContent()
        {
            var response = new Response { Name = "jimbo jones", Mobile = "0400 123 123" };
            
            Result<Response> result = Result<Response>.New(response)
                .WithInfo(Resx.MsgKeyInfo, new { type = "some dynamic juicy content just for you" })
                .WithWarning(Resx.MsgKeyWarning);
            
            return new OkObjectResult(result);
        }

        [HttpGet("bad-request")]
        public IActionResult GetBadRequest()
        {
            throw new ClientException(
                Message.Info(Resx.MsgKeyInfo, new { type = "some dynamic juicy content just for you"}),
                Message.Warning(Resx.MsgKeyWarning),
                Message.Error(Resx.MsgKeyError)
            );
        }
        
        [HttpGet("not-found")]
        public IActionResult GetNotFound()
        {
            var id = "1000";
            throw new NotFoundClientException<string>(id);
        }
        
        [HttpGet("forbidden")]
        public IActionResult GetForbidden()
        {
            throw new ForbiddenClientException();
        }
        
        [HttpGet("unauthorised")]
        public IActionResult GetUnauthorised()
        {
            throw new NotAuthenticatedClientException();
        }
        
        [HttpPost("validator")]
        public IActionResult GetValidator(ValidateRequest request)
        {
            var validator = new Validator();
            validator
                .ValidatePropertyIsRequired(nameof(request.Name), request.Name)
                .ValidateStringLength(nameof(request.Name), request.Name, 5, 3)
                .ValidateCollectionHasValues(nameof(request.Emails), request.Emails)
                .Validate(
                    Resx.MsgKeyValidationError, 
                    new {email = request.Emails?.FirstOrDefault()}, 
                    request.Emails == null || request.Emails.Any() && request.Emails?.Any(email => email.EndsWith("gmail.com")) == true);

            if (validator.HasErrors)
            {
                throw new ValidationClientException(validator);
            }

            return new OkObjectResult("Ok Result");
        }
    }
    
    public class ValidateRequest
    {
        public string Name { get; set; }
        public string[] Emails { get; set; }
    }

    public class Response
    {
        public string Name { get; init; }
        public string Mobile { get; init; }
    }
}
