using System.Collections.Generic;

namespace CompillerServices.Frontend.NameTables
{
    public class SubprogramNameTableRow
    {
        public SubprogramNameTableRow(string accessModifier, string name, ModuleNameTableRow parent)
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