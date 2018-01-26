namespace CompillerServices.Frontend.NameTables
{
    public class ProcedureNameTable : NameTable<ProcedureNameTableRow>
    {
    }

    public class ProcedureNameTableRow : RoutineNameTableRow
    {
        public ProcedureNameTableRow(int line, int column, string accessModifier, string name,
            ModuleNameTableRow parentModule)
            : base(line, column, accessModifier, name, parentModule)
        {
        }
    }
}