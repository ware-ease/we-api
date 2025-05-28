namespace API.Middlewares
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class AuthorizeGroupAttribute : Attribute
    {
        public string[] Groups { get; }

        public AuthorizeGroupAttribute(string groupsCsv)
        {
            Groups = groupsCsv.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        }
    }

}
