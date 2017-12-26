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

        void WriteStatementEnd(StatementType statementType);

        void WriteType(string type);
        void WriteIdentifier(string identifier);
        void WriteAssign();

        void WriteInput(string identifier);
        void WriteOutput();

        void WriteReturn();

        void WriteFunctionCallBegin(string functionName);
        void WriteCallArgSeparator();
        void WriteFunctionCallEnd();

        void WriteIfBegin();
        void WriteIfEnd();
        void WriteElse();

        void WriteWhileBegin();
        void WriteWhileEnd();

        void WriteDo();
        void WriteDoWhileBegin();
        void WriteDoWhileEnd();

        void WriteSum();
        void WriteSubstraction();
        void WriteMultiply();
        void WriteDivision();
        void WriteMod();

        void WriteBraceBegin();
        void WriteBraceEnd();

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

        string GetArrayType(string elementType, int dimentionsCount);
        void WriteArrayDimention(string type, int index, int dimentionsCount);
        void WriteArrayEnd(int dimentionsCount);

        void WriteArrayElementBegin();
        void WriteArrayElementEnd();

        void WriteRaw(string raw);
        void WriteLine();

        Task FlushAsync();
    }
}