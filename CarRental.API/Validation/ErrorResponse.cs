namespace CarRental.API.Validation
{
    public class ErrorResponse
    {
        public string Message { get; set; } = "Validation failed.";
        public List<ValidationErrorItem> Errors { get; set; } = new List<ValidationErrorItem>();
    }
}
