using CompillerServices.Frontend.NameTables.Types;

namespace CompillerServices.Frontend.NameTables
{
    public class FunctionNameTable : NameTable<FunctionNameTableRow>
    {
    }

    public class FunctionNameTableRow : SubprogramNameTableRow
    {
        public FunctionNameTableRow(int line, int column, string accessModifier, SlangType returningType, string name,
            ModuleNameTableRow parent)
            : base(line, column, accessModifier, name, parent)
        {
            ReturningType = returningType;
        }

        public SlangType ReturningType { get; }
    }
}