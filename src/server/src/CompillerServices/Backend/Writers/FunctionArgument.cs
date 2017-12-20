namespace CompillerServices.Backend.Writers
{
    public class FunctionArgument
    {
        public FunctionArgument(string type, string name)
        {
            Type = type;
            Name = name;
        }

        public string Type { get; }
        public string Name { get; }
    }
}