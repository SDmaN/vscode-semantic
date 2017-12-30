namespace CompillerServices.Frontend.NameTables
{
    public class FunctionNameTable : NameTable<FunctionNameTableRow>
    {
    }

    public class FunctionNameTableRow : SubprogramNameTableRow
    {
        public FunctionNameTableRow(int line, int column, string accessModifier, string type, string name,
            ModuleNameTableRow parent)
            : base(line, column, accessModifier, name, parent)
        {
            Type = type;
        }

        public string Type { get; }
    }
}