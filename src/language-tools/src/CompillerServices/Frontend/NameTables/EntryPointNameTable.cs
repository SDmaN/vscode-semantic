namespace CompillerServices.Frontend.NameTables
{
    public class EntryPointNameTable : NameTable<EntryPointNameTableRow>
    {
    }

    public class EntryPointNameTableRow : RoutineNameTableRow
    {
        public EntryPointNameTableRow(int line, int column, ModuleNameTableRow parentModule)
            : base(line, column, null, null, parentModule)
        {
        }
    }
}