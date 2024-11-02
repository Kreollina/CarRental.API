using FluentValidation.Results;

namespace CarRental.API.Validation
{
    public static class ValidationErrorHandler
    {
        public static ErrorResponse ErrorHandler(List<ValidationFailure> errors)
        {
            var errorResponse = new ErrorResponse();
            foreach (var x in errors)
            {
                var validationErrorItem = new ValidationErrorItem();
                validationErrorItem.ErrorMessage = x.ErrorMessage;
                validationErrorItem.PropertyName = x.PropertyName;
                errorResponse.Errors.Add(validationErrorItem);
            }
            return errorResponse;
        }
    }
}
