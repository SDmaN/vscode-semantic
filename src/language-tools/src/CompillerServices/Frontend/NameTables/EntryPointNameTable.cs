using CompillerServices.Exceptions;
using CompillerServices.Frontend.NameTables.Types;

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

        public override SlangType ToSlangType()
        {
            throw new System.NotImplementedException("Could not convert entry point to type");
        }
    }
}