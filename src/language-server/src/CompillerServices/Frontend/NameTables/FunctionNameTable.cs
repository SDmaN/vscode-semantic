namespace CompillerServices.Frontend.NameTables
{
    public class FunctionNameTable : NameTable<FunctionNameTableRow>
    {
    }

    public class FunctionNameTableRow : SubprogramNameTableRow
    {
        public FunctionNameTableRow(string accessModifier, string type, string name, ModuleNameTableRow parent)
            : base(accessModifier, name, parent)
        {
            Type = type;
        }

        public string Type { get; }
    }
}