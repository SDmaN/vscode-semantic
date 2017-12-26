namespace CompillerServices.Backend.Writers
{
    public class FunctionArgument
    {
        public FunctionArgument(string passModifier, string type, string name)
        {
            PassModifier = passModifier;
            Type = type;
            Name = name;
        }

        public FunctionArgument(string type, string name)
            : this(null, type, name)
        {
        }

        public string Type { get; }
        public string Name { get; }
        public string PassModifier { get; }
    }
}