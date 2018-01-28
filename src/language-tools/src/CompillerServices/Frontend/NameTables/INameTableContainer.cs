using System.Threading.Tasks;

namespace CompillerServices.Frontend.NameTables
{
    public interface INameTableContainer
    {
        ModuleNameTable ModuleNameTable { get; }
        FunctionNameTable FunctionNameTable { get; }
        ProcedureNameTable ProcedureNameTable { get; }
        EntryPointNameTable EntryPointNameTable { get; }
        ArgumentNameTable ArgumentNameTable { get; }
        StatementVariableNameTable StatementVariableNameTable { get; }

        Task Clear();

        ModuleNameTableRow FindModule(string name);
    }
}