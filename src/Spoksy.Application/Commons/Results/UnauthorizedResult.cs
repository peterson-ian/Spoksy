namespace Spoksy.Application.Commons.Results
{
    public record UnauthorizedResult<T> : Result<T>
    {
        private UnauthorizedResult(string error) : base(default, new List<string> { error }) { }

        public static UnauthorizedResult<T> Create(string error) => new(error);
    }
}
