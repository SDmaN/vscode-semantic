using CompillerServices.Frontend.NameTables.Types;

namespace CompillerServices.Frontend.NameTables
{
    public class ArgumentNameTable : NameTable<ArgumentNameTableRow>
    {
    }

    public class ArgumentNameTableRow : VariableNameTableRow
    {
        public ArgumentNameTableRow(int line, int column, string passModifier, SlangType type, string name,
            RoutineNameTableRow parent)
            : base(line, column, type, name, parent)
        {
            PassModifier = passModifier;
        }

        public string PassModifier { get; }
    }
}