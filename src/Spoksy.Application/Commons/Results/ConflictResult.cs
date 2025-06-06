namespace Spoksy.Application.Commons.Results
{
    public record ConflictResult<T> : Result<T>
    {
        private ConflictResult(string error) : base(default, new List<string> { error }) { }

        public static ConflictResult<T> Create(string error) => new(error);
    }
}