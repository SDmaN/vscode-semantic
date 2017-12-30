using System.Threading.Tasks;

namespace CompillerServices.Frontend.NameTables
{
    public class NameTableContainer : INameTableContainer
    {
        public ModuleNameTable ModuleNameTable { get; } = new ModuleNameTable();
        public FunctionNameTable FunctionNameTable { get; } = new FunctionNameTable();
        public ProcedureNameTable ProcedureNameTable { get; } = new ProcedureNameTable();
        public ArgumentNameTable ArgumentNameTable { get; } = new ArgumentNameTable();
        public VariableNameTable VariableNameTable { get; } = new VariableNameTable();

        public Task Clear()
        {
            return Task.Run(() =>
            {
                ModuleNameTable.Clear();
                FunctionNameTable.Clear();
                ProcedureNameTable.Clear();
                ArgumentNameTable.Clear();
                VariableNameTable.Clear();
            });
        }
    }
}