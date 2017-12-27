namespace CompillerServices.Frontend.NameTables
{
    public class VariableNameTable : NameTable<VariableNameTableRow>
    {
    }

    public class VariableNameTableRow
    {
        public VariableNameTableRow(string type, string name, SubprogramNameTableRow parent)
        {
            Type = type;
            Name = name;
            Parent = parent;
        }

        public string Type { get; }
        public string Name { get; }

        public SubprogramNameTableRow Parent { get; }
    }
}