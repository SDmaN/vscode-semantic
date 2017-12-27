using System.Collections.Generic;
using System.Linq;

namespace CompillerServices.Frontend.NameTables
{
    public class ModuleNameTable : NameTable<ModuleNameTableRow>
    {
        public ModuleNameTableRow GetModule(string name)
        {
            return this.FirstOrDefault(x => x.ModuleName == name);
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