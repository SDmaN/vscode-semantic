using CompillerServices.Frontend.NameTables.Types;

namespace CompillerServices.Frontend.NameTables
{
    public abstract class VariableNameTableRow : NameTableRow
    {
        protected VariableNameTableRow(int line, int column, SlangType type, string name,
            RoutineNameTableRow parentRoutine)
            : base(line, column)
        {
            Type = type;
            Name = name;
            ParentRoutine = parentRoutine;
        }

        public SlangType Type { get; }
        public string Name { get; }

        public RoutineNameTableRow ParentRoutine { get; }
    }
}