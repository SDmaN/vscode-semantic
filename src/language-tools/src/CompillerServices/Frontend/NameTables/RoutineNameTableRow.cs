using System.Collections.Generic;
using System.Linq;

namespace CompillerServices.Frontend.NameTables
{
    public class RoutineNameTableRow : NameTableRow
    {
        public RoutineNameTableRow(int line, int column, string accessModifier, string name,
            ModuleNameTableRow parentModule)
            : base(line, column)
        {
            AccessModifier = accessModifier;
            Name = name;
            ParentModule = parentModule;
        }

        public string AccessModifier { get; }
        public string Name { get; }

        public ModuleNameTableRow ParentModule { get; }
        public IList<ArgumentNameTableRow> Arguments { get; } = new List<ArgumentNameTableRow>();

        public ICollection<VariableNameTableRow> Variables { get; } = new List<VariableNameTableRow>();

        public VariableNameTableRow FindVariable(string name)
        {
            return Variables.FirstOrDefault(x => x.Name == name);
        }

        public bool ContainsVariable(string name)
        {
            return FindVariable(name) != null;
        }
    }
}