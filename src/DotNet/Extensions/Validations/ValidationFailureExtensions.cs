using FluentValidation.Results;
using Core.DotNet.AggregatesModel.ExceptionAggregate;

namespace Core.DotNet.Extensions.Validations;

public static class ValidationFailureExtensions
{
    public static List<ErrorField> ToErrorFields(this IEnumerable<ValidationFailure> failures)
    {
        var errorFields = failures.Select(error => new ErrorField(error.PropertyName, error.ErrorMessage)).ToList();

        return errorFields;
    }
}