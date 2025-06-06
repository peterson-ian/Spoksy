namespace Spoksy.Application.Commons.Results
{
    public record NotFoundResult<T> : Result<T>
    {
        private NotFoundResult(string error) : base(default, new List<string> { error }) { }

        public static NotFoundResult<T> Create(string error) => new(error);
    }
}
