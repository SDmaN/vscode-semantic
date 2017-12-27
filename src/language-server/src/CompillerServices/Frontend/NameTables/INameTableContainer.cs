using System.Threading.Tasks;

namespace CompillerServices.Frontend.NameTables
{
    public interface INameTableContainer
    {
        ModuleNameTable ModuleNameTable { get; }
        FunctionNameTable FunctionNameTable { get; }
        ProcedureNameTable ProcedureNameTable { get; }
        ArgumentNameTable ArgumentNameTable { get; }
        VariableNameTable VariableNameTable { get; }

        Task Clear();
    }
}