namespace Spoksy.Domain.ValueObjects
{
    public sealed class Language
    {
        public string Code { get; }
        public string Name { get; }
        public TextDirection Direction { get; }

        private Language(string code, string name, TextDirection direction = TextDirection.LTR)
        {
            Code = code;
            Name = name;
            Direction = direction;
        }

        public static readonly Language English = new Language("en", "English");
        public static readonly Language Portuguese = new Language("pt", "Portuguese");
        public static readonly Language Spanish = new Language("es", "Spanish");

        public static readonly List<Language> All = new List<Language>() { English, Portuguese, Spanish };

        public static Language GetByCode(string code)
        {
            return All.FirstOrDefault(x => x.Code.ToLower() == code.ToLower());
        }
    }

    public enum TextDirection
    {
        LTR, // Left to Right
        RTL // Right to Left
    }
}
