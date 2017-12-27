namespace CompillerServices.Frontend.NameTables
{
    public class ArgumentNameTable : NameTable<ArgumentNameTableRow>
    {
    }

    public class ArgumentNameTableRow
    {
        public ArgumentNameTableRow(string passModifier, string type, string name, SubprogramNameTableRow parent)
        {
            PassModifier = passModifier;
            Type = type;
            Name = name;
            Parent = parent;
        }

        public string PassModifier { get; }
        public string Type { get; }
        public string Name { get; }

        public SubprogramNameTableRow Parent { get; }
    }
}