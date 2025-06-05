namespace Spoksy.Application.Commons
{
    public abstract record Result
    {
        public bool IsSuccess => Errors.Count == 0;
        public List<string> Errors { get; }

        protected Result(List<string>? errors = null)
        {
            Errors = errors ?? new();
        }
    }
    public record Result<T> : Result
    {
        private readonly T? _value;

        public T Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException(
                $"Cannot access the value of a failed result. Errors: {string.Join(", ", Errors)}"
            );

        protected internal Result(T? value, List<string>? errors = null)
            : base(errors)
        {
            _value = value;
        }

        public static Result<T> Success(T value) => new(value);
        public static Result<T> Failure(string error) => new(default, new() { error });
        public static Result<T> Failure(IEnumerable<string> errors) => new(default, errors.ToList());
    }
}
