namespace Spoksy.Application.Commons.Results
{
    public record ForbiddenResult<T> : Result<T>
    {
        private ForbiddenResult(string error) : base(default, new List<string> { error }) { }

        public static ForbiddenResult<T> Create(string error) => new(error);
    }
}