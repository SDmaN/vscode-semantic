namespace CompillerServices
{
    public class SubprogramArgument
    {
        public SubprogramArgument(string passModifier, string type, string name)
        {
            PassModifier = passModifier;
            Type = type;
            Name = name;
        }

        public SubprogramArgument(string type, string name)
            : this(null, type, name)
        {
        }

        public string Type { get; }
        public string Name { get; }
        public string PassModifier { get; }
    }
}