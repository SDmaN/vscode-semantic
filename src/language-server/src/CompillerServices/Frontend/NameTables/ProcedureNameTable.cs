namespace CompillerServices.Frontend.NameTables
{
    public class ProcedureNameTable : NameTable<ProcedureNameTableRow>
    {
    }

    public class ProcedureNameTableRow : SubprogramNameTableRow
    {
        public ProcedureNameTableRow(string accessModifier, string name, ModuleNameTableRow parent)
            : base(accessModifier, name, parent)
        {
        }
    }
}