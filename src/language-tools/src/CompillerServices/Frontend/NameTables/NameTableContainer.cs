using System.Linq;
using System.Threading.Tasks;

namespace CompillerServices.Frontend.NameTables
{
    public class NameTableContainer : INameTableContainer
    {
        public ModuleNameTable ModuleNameTable { get; } = new ModuleNameTable();
        public FunctionNameTable FunctionNameTable { get; } = new FunctionNameTable();
        public ProcedureNameTable ProcedureNameTable { get; } = new ProcedureNameTable();
        public EntryPointNameTable EntryPointNameTable { get; } = new EntryPointNameTable();
        public ArgumentNameTable ArgumentNameTable { get; } = new ArgumentNameTable();
        public StatementVariableNameTable StatementVariableNameTable { get; } = new StatementVariableNameTable();

        public async Task Clear()
        {
            await Task.WhenAll(
                Task.Run(() => ModuleNameTable.Clear()),
                Task.Run(() => FunctionNameTable.Clear()),
                Task.Run(() => ProcedureNameTable.Clear()),
                Task.Run(() => EntryPointNameTable.Clear()),
                Task.Run(() => ArgumentNameTable.Clear()),
                Task.Run(() => StatementVariableNameTable.Clear())
            );
        }

        public ModuleNameTableRow FindModule(string name)
        {
            return ModuleNameTable.FirstOrDefault(x => x.ModuleName == name);
        }
    }
}