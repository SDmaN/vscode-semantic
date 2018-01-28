using CompillerServices.Frontend.NameTables.Types;

namespace CompillerServices.Frontend.NameTables
{
    public class StatementVariableNameTable : NameTable<StatementVariableNameTableRow>
    {
    }

    public class StatementVariableNameTableRow : VariableNameTableRow
    {
        public StatementVariableNameTableRow(int line, int column, SlangType type, string name, bool isConstant,
            RoutineNameTableRow parentRoutine)
            : base(line, column, type, name, parentRoutine)
        {
            IsConstant = isConstant;
        }

        public bool IsConstant { get; }
    }
}