using System.Collections.Generic;
using System.Linq;
using CompillerServices.Frontend.NameTables.Types;

namespace CompillerServices.Frontend.NameTables
{
    public abstract class RoutineNameTableRow : NameTableRow
    {
        protected RoutineNameTableRow(int line, int column, string accessModifier, string name,
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

        public abstract SlangType ToSlangType();

        protected IList<RoutineTypeArg> GetRoutineTypeArgs()
        {
            return Arguments.Select(x => new RoutineTypeArg(x.PassModifier, x.Type)).ToList();
        }

        public ArgumentNameTableRow FindArgument(string name)
        {
            return Arguments.FirstOrDefault(x => x.Name == name);
        }

        public bool ContainsArgument(string name)
        {
            return FindArgument(name) != null;
        }
    }
}