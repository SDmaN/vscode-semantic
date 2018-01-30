using System.Collections.Generic;
using System.Linq;
using CompillerServices.Frontend.NameTables.Types;

namespace CompillerServices.Frontend.NameTables
{
    public class StatementVariableNameTable : NameTable<StatementVariableNameTableRow>
    {
        public StatementVariableNameTable()
        {
        }

        public StatementVariableNameTable(IEnumerable<StatementVariableNameTableRow> other)
            : base(other)
        {
        }

        public StatementVariableNameTableRow FindVariable(string name)
        {
            return this.FirstOrDefault(x => x.Name == name);
        }

        public bool ContainsVariable(string name)
        {
            return FindVariable(name) != null;
        }
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