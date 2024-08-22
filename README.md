
# Project Title

Simple result pattern for dealing with common errors without exceptions.

## Table of Contents
* [General Info](#general-information)
* [Usage](#usage)
* [Predefined Errors Types](#predefined-Errors-Types)
* [Examples](#examples)
* [Copyright](#Copyright)

## General Information

Simple result is a small library to solve a common problem. It returns an object indicating success or failure of an operation instead of throwing/using exceptions.


Reading the article:

[Exceptions for Flow Control by Vladimir Khorikov](https://enterprisecraftsmanship.com/posts/exceptions-for-flow-control/) i understood the benefit of using Result pattern over exceptions for flow controle.

There are really good implementations of the Result pattern like [FluentResults](https://github.com/altmann/FluentResults), but I didn't need all the functionality in my programs, so I decided to create my own implementation.

##  Usage

Creating `Result`.

Explicitly create `Result` object
```csharp
///Result object cannot be created with new(), only factory methods are available.

Result result = Result.Failure(Error.NotFound("some value", "some message"));
Result result1 = Result.Success();

Result result2 = Result.Failure<ExampleType>(Error.NotFound("some value", "some message"));
Result result3 = Result.Success(exampleType);//note: if exampleType is null you get failure result.
```

Implicitly create `Result` object without return value type.
```csharp 
public Result DoSomethingWithoutReturnType(string? value)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        //Implicitly return failed result with given error.
        return Error.Problem("Value_NullOrEmpty", "Cant be null or empty.");
    }

    //Implicitly return successful result.
    return true;
}

public void UseRetunedResultWithoutType(string? value)
{
    var result = DoSomethingWithoutReturnType(value);

    //if the result have error break the flow.
    if (result.IsFailure)
    {
            //break the flow
    }

    //continue with the flow
}
```


Implicitly create `Result` object with return value type.


```csharp 
 public Result<SomeType> DoSomethingWithReturnType(SomeType someType)
 {
     if (someType is not null)
     {
         //Implicitly return failed result with given error.
         return Error.Problem("Null_Value", "Incoming type cant be null.");
     }

     //Implicitly return success result with given value.
     return new SomeType();
 }

 public Result<SomeAnotherType> UseRetunedResultWitType(SomeType someType)
 {
     var result = DoSomethingWithReturnType(someType);

     if (result.IsFailure)
     {
         return result.Error;
     }

     return new SomeAnotherType();
 }
```
## Predefined Errors Types

```csharp
//Indicates that a business rule has been violated.
var resultProblem = Error.Problem("some value", "some message");

//Indicates that there is no record for the given input.
var resultNotFound = Error.NotFound("some value", "some message");

//Indicates that there is conflict creating/updating some resource.
var resultConflict = Error.Conflict("some value", "some message");

//Indicates that there is validation errors.
var resultValidation = Error.Validation("some value");
```
## Examples

Example how to use Result Object in service.
```csharp
public class ExampleTypeService
{
    private readonly Collection<ExampleType> _examples =
    [
        new ExampleType(){Id = 1},
        new ExampleType(){Id = 2},
        new ExampleType(){Id = 3},
        new ExampleType(){Id = 4},
    ];

    //instate of returning null we can return well detain error object
    public Result<ExampleType> GetExampleTypeById(int id)
    {
        var item = _examples.FirstOrDefault(x => x.Id == id);

        if (item == null)
        {
            return Error.NotFound("ExampleType_NotFound",
             "The record with the given id does not exist.");
        }

        return item;
    }
}
```

Example how to create Validator using Result Object.
```csharp
public static class ExampleTypeValidator
{
    public static Result ValidateCreate(ExampleType exampleType)
    {
        //Create empty failed Result object 
        var validationError = Error.Validation("ExampleType_Creation");

        //Try add validations with PropertyErrors.
        validationError.TryAddPropertiesErrors(
            ValidateFirstName(exampleType.FirstName),
            ValidateLastName(exampleType.LastName));

        //Try add validation with string Collection error messages.
        validationError.TryAddPropertyErrors(
            nameof(exampleType.Email), ValidateEmail(exampleType.Email));

        //if there is validation errors return them.
        if (validationError.HasErrors)
        {
            return validationError;
        }

        return true;
    }

    private static PropertyError ValidateFirstName(string? firstName)
    {
        var error = PropertyError.Create(nameof(ExampleType.FirstName));

        if (string.IsNullOrWhiteSpace(firstName))
        {
            error.AddMessage("Cant be null or Empty.");
            return error;
        }

        if (firstName.Length > 10)
        {
            error.AddMessage($"Cant be more then 10 characters.");
        }

        return error;
    }

    private static PropertyError ValidateLastName(string? lastName)
    {
        var error = PropertyError.Create(nameof(ExampleType.LastName));

        if (string.IsNullOrWhiteSpace(lastName))
        {
            error.AddMessage("Cant be null or Empty.");
            return error;
        }

        if (lastName.Length < 5)
        {
            error.AddMessage($"Cant be less then 5 characters.");
        }

        return error;
    }

    private static Collection<string> ValidateEmail(string? email)
    {
        var errorMessages = new Collection<string>();

        if (string.IsNullOrWhiteSpace(email))
        {
            errorMessages.Add("Cant be null or Empty.");
            return errorMessages;
        }

        if (email.Length < 7)
        {
            errorMessages.Add($"Cant be less then 7 characters.");
        }

        if (email.Length > 25)
        {
            errorMessages.Add($"Cant be more then 25 characters.");
        }

        return errorMessages;
    }
}
```

Example of how to use the Result object in a minimal api.
```csharp
app.MapPost("/api/ExampleType", (ExampleType exampleType) =>
{
    var validationResult = ExampleTypeValidator.ValidateCreate(exampleType);

    if (validationResult.IsFailure)
    {
        //helper method for mapping to specific IResult 
        //return type with well defined ProblemDetails object,
        //based on type of error you created.
        return MinimalApiResults.Problem(validationResult);
    }

    //it is now safe to use exampleType to write to db, calling services whatever.

    return Results.Ok();
});

app.MapGet("/api/ExampleType/{id}", (int id) =>
{
    var result = new ExampleTypeService().GetExampleTypeById(id);

    //if (result.IsFailure)
    //{
    //    return MinimalApiResults.Problem(result);
    //}

    //return Results.Ok(result.Value);

    //extension method for resolving result without (if and else).
    return result.Match(
        value => Results.Ok(value),
        MinimalApiResults.Problem
        );
});
```
## Copyright
Copyright (c) Vlatko Karcheski See [LICENSE](https://raw.githubusercontent.com/altmann/FluentResults/master/LICENSE) for details.