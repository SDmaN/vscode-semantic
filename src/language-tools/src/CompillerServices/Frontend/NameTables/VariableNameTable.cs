using System;
using CompillerServices.Frontend.NameTables.Types;

namespace CompillerServices.Frontend.NameTables
{
    public class VariableNameTable : NameTable<VariableNameTableRow>
    {
    }

    public class VariableNameTableRow : NameTableRow
    {
        public VariableNameTableRow(int line, int column, SlangType type, string name, bool isConstant,
            RoutineNameTableRow parentRoutine)
            : base(line, column)
        {
            Type = type;
            Name = name;
            IsConstant = isConstant;
            ParentRoutine = parentRoutine;
        }

        public SlangType Type { get; }
        public string Name { get; }
        public bool IsConstant { get; }

        public RoutineNameTableRow ParentRoutine { get; }
    }
}