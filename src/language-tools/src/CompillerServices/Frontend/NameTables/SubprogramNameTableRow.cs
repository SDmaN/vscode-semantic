using System.Collections.Generic;

namespace CompillerServices.Frontend.NameTables
{
    public class SubprogramNameTableRow : NameTableRow
    {
        public SubprogramNameTableRow(int line, int column, string accessModifier, string name,
            ModuleNameTableRow parent)
            : base(line, column)
        {
            AccessModifier = accessModifier;
            Name = name;
            Parent = parent;
        }

        public string AccessModifier { get; }
        public string Name { get; }

        public ModuleNameTableRow Parent { get; }
        public ICollection<ArgumentNameTableRow> Arguments { get; } = new List<ArgumentNameTableRow>();
        public ICollection<VariableNameTableRow> Variables { get; } = new List<VariableNameTableRow>();
    }
}