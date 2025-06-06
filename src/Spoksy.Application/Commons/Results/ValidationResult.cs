namespace Spoksy.Application.Commons.Results
{
    public record ValidationResult<T> : Result<T>
    {
        private ValidationResult(T? value, List<string>? errors = null) : base(value, errors) { }

        public static ValidationResult<T> Success(T value) => new(value);

        public static ValidationResult<T> Failure(IEnumerable<string> errors) => new(default, errors.ToList());

        public static ValidationResult<T> Failure(string error) => new(default, new List<string> { error });
    }

    public record ValidationResult : Result
    {
        private ValidationResult(List<string>? errors = null) : base(errors) { }

        public static ValidationResult Success() => new();

        public static ValidationResult Failure(IEnumerable<string> errors) => new(errors.ToList());

        public static ValidationResult Failure(string error) => new(new List<string> { error });
    }

}
