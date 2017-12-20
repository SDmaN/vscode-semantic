using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CompillerServices.Backend.Writers
{
    public interface ISourceWriter : IDisposable
    {
        void WriteImportBegin();
        void WriteImport(string importingModule);
        void WriteImportEnd();

        void WriteModuleBegin(string moduleName);
        void WriteModuleEnd();

        void WriteBlockBegin();
        void WriteBlockEnd();

        void WriteFunction(string accessModifier, string returningType, string name,
            IEnumerable<FunctionArgument> arguments);

        void WriteProcedure(string accessModifier, string name, IEnumerable<FunctionArgument> arguments);

        void WriteStatementEnd();

        void WriteType(string type);
        void WriteIdentifier(string identifier);
        void WriteAssign();

        void WriteInput(string identifier);
        void WriteOutputBegin();
        void WriteOutputEnd();

        void WriteReturnBegin();
        void WriteReturnEnd();

        void WriteSum();
        void WriteSubstraction();
        void WriteMultiply();
        void WriteDivision();
        void WriteMod();

        void WriteBracketBegin();
        void WriteBracketEnd();

        void WritePlus();
        void WriteMinus();

        void WriteLogicOr();
        void WriteLogicAnd();
        void WriteEquality();
        void WriteInequality();
        void WriteBigger();
        void WriteLesser();
        void WriteBiggerOrEqual();
        void WriteLesserOrEqual();
        void WriteNot();

        void WriteRaw(string raw);

        Task FlushAsync();
    }
}