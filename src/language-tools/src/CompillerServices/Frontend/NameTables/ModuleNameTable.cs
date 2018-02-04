using System.Collections.Generic;
using System.Linq;
using CompillerServices.Frontend.NameTables.Types;

namespace CompillerServices.Frontend.NameTables
{
    public class ModuleNameTable : NameTable<ModuleNameTableRow>
    {
        public ModuleNameTableRow FindModule(string moduleName)
        {
            return this.FirstOrDefault(x => x.ModuleName == moduleName);
        }

        public bool Contains(string moduleName)
        {
            return this.Any(x => x.ModuleName == moduleName);
        }
    }

    public class ModuleNameTableRow : NameTableRow
    {
        public ModuleNameTableRow(int line, int column, string moduleName)
            : base(line, column)
        {
            ModuleName = moduleName;
        }

        public string ModuleName { get; }

        public ICollection<string> ImportingModules { get; } = new List<string>();
        public ICollection<FunctionNameTableRow> Functions { get; } = new List<FunctionNameTableRow>();
        public ICollection<ProcedureNameTableRow> Procedures { get; } = new List<ProcedureNameTableRow>();
        public EntryPointNameTableRow EntryPoint { get; set; }

        public bool IsImported(string moduleName)
        {
            return ImportingModules.Contains(moduleName);
        }

        public FunctionNameTableRow FindFunctionByPosition(string name, int line, int column)
        {
            return Functions.FirstOrDefault(x => x.Name == name && x.Line == line && x.Column == column);
        }

        public ProcedureNameTableRow FindProcedureByPosition(string name, int line, int column)
        {
            return Procedures.FirstOrDefault(x => x.Name == name && x.Line == line && x.Column == column);
        }

        public RoutineNameTableRow FindExactRoutine(string name, IList<SlangType> argTypes)
        {
            IEnumerable<RoutineNameTableRow> withSameName = FindRoutinesByName(name);
            return withSameName.FirstOrDefault(row => row.Arguments.Select(x => x.Type).SequenceEqual(argTypes));
        }

        public bool ContainsExactRoutine(string name, IList<SlangType> argTypes)
        {
            return FindExactRoutine(name, argTypes) != null;
        }

        public IEnumerable<RoutineNameTableRow> FindRoutinesByName(string name)
        {
            return Functions.Where(x => x.Name == name).Cast<RoutineNameTableRow>()
                .Concat(Procedures.Where(x => x.Name == name));
        }
    }
}