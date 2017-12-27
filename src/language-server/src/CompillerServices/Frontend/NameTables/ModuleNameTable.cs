using System.Collections.Generic;
using System.Linq;

namespace CompillerServices.Frontend.NameTables
{
    public class ModuleNameTable : NameTable<ModuleNameTableRow>
    {
        public bool Contains(string moduleName)
        {
            return this.Any(x => x.ModuleName == moduleName);
        }
    }

    public class ModuleNameTableRow
    {
        public ModuleNameTableRow(string moduleName)
        {
            ModuleName = moduleName;
        }

        public string ModuleName { get; }
        public ICollection<FunctionNameTableRow> Functions { get; } = new List<FunctionNameTableRow>();
        public ICollection<ProcedureNameTableRow> Procedures { get; } = new List<ProcedureNameTableRow>();
    }
}