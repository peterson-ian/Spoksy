namespace Spoksy.Domain.ValueObjects
{
    public sealed class Country
    {
        public string Code { get; }
        public string Name { get; }
        public Language OfficialLanguage { get; }
        public int MinimumAge { get; }
        private Country(string code, string name, Language officialLanguage, int minimumAge)
        {
            Code = code;
            Name = name;
            OfficialLanguage = officialLanguage;
            MinimumAge = minimumAge;
        }

        public static readonly Country UnitedStates = new Country("US", "United States", Language.English, 21);
        public static readonly Country Brazil = new Country("BR", "Brazil", Language.Portuguese, 18);
        public static readonly Country Spain = new Country("ES", "Spain", Language.Spanish, 18);

        public static readonly List<Country> All = new List<Country>() { UnitedStates, Brazil, Spain };

        public static Country GetByCode(string code)
        {
            return All.FirstOrDefault(x => x.Code.ToLower() == code.ToLower());
        }

    }
}
