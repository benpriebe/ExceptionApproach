# Exception Approach to returning Translated JSON responses to a client.

This repository demonstrates an alternate approach the [Result Object](https://github.com/benpriebe/ResultObject) 
for handling common failure conditions and translating those failures to Http results.

## Key Features

- Store **multiple messages (info, warnings, errors, validation errors)** within a `ClientException`
- Create **localised, tokenised messages** instead of only messages in string format
- Comes with **fluent validator** to validate most primitive types and generate localised messages
- Use **Http Middleware** to convert `ClientException`s to Http `IActionResults`**

## Throwing a ClientException

Simplest case - throw a `ClientException` with a translated message.
```csharp
throw new ClientException(Resx.MsgRootError);
```

Throw a `ClientException` with a root message, and some extra messages.
```csharp
throw new ClientException(Resx.MsgRootError)                
    .WithInfo(Resx.MsgKeyInfo, new { type = "some dynamic juicy content just for you"})
    .WithWarning(Resx.MsgKeyWarning)
    .WithError(Resx.MsgKeyError);
```

Throw a `NotFoundClientException` passing in the identity of the entity that is not found.
```csharp
throw new NotFoundClientException<string>(id);
```

Throw a `ForbiddenClientException`.
```csharp
throw new ForbiddenClientException();
```

Throw a `NotAuthenticatedClientException`.
```csharp
throw new NotAuthenticatedClientException();
```

Use a validator to throw a `ValidationClientException`.
```csharp
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
```                            

### Using Asp.Net Core Middleware to convert ClientExceptions

Simply use the `UseI18nMiddleware` to handle ClientExceptions. 

```csharp
public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    app.UseI18NMiddleware();
}
```
The various implementations of `ClientException` will be mapped to a `ExceptionResult` 
instance and written to the output stream with the appropriate `HttpStatusCode`.

```csharp
public class ExceptionResult
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Message { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Message[] Extras { get; set; }
}
```

```
ClientException -> Status400BadRequest 
NotFoundClientException -> StatusCodes.Status404NotFound
NotAuthenticatedClientException -> StatusCodes.Status401Unauthorized
ForbiddenClientException -> StatusCodes.Status403Forbidden

All others -> Status500InternalServerError
```

### Other features

A `Result` object is included to handle success scenarios where `data` and `messages` are required.
The `messages` are options.

```csharp

var response = new Response { Name = "jimbo jones", Mobile = "0400 123 123" };
          
var result = new Result<Response>(response)
    .WithInfo(Resx.MsgKeyInfo, new { type = "some dynamic juicy content just for you" })
    .WithWarning(Resx.MsgKeyWarning);
    
```


## License

This code is subject to the Unlicense license - See [LICENSE](https://raw.githubusercontent.com/benpriebe/ResultObject/master/LICENSE) for details. 