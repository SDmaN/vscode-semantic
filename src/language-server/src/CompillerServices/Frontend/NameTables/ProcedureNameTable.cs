namespace CompillerServices.Frontend.NameTables
{
    public class ProcedureNameTable : NameTable<ProcedureNameTableRow>
    {
    }

    public class ProcedureNameTableRow : SubprogramNameTableRow
    {
        public ProcedureNameTableRow(int line, int column, string accessModifier, string name,
            ModuleNameTableRow parent)
            : base(line, column, accessModifier, name, parent)
        {
        }
    }
}