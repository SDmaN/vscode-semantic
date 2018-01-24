using System.Collections.Generic;
using System.Linq;
using CompillerServices.Frontend.NameTables.Types;

namespace CompillerServices.Frontend.NameTables
{
    public class ModuleNameTable : NameTable<ModuleNameTableRow>
    {
        public ModuleNameTableRow GetModuleRow(string moduleName)
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

        public bool ContainsSameRoutine(string name, ICollection<SlangType> argTypes)
        {
            IEnumerable<SubprogramNameTableRow> withSameName = Functions.Where(x => x.Name == name)
                .Cast<SubprogramNameTableRow>().Concat(Procedures.Where(x => x.Name == name));

            return withSameName.Select(row => row.Arguments.Select(x => x.Type))
                .Any(rowTypes => rowTypes.SequenceEqual(argTypes));
        }
    }
}