namespace CompillerServices.Frontend.NameTables
{
    public class VariableNameTable : NameTable<VariableNameTableRow>
    {
    }

    public class VariableNameTableRow : NameTableRow
    {
        public VariableNameTableRow(int line, int column, string type, string name, SubprogramNameTableRow parent)
            : base(line, column)
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