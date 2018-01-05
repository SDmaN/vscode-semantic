using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompillerServices.Backend.Writers
{
    public class CppWriter : ISourceWriter
    {
        private readonly string _headerFileName;

        #region Standard library modules

        private static readonly IDictionary<string, string> ModuleStandardIncludes = new Dictionary<string, string>
        {
            { "Math", "cmath" }
        };

        #endregion

        #region Standard types

        private static readonly IDictionary<string, string> StandardTypes = new Dictionary<string, string>
        {
            { "bool", "bool" },
            { "int", "int" },
            { "real", "float" }
        };

        #endregion

        private readonly IndentedTextWriter _headerWriter;
        private readonly IndentedTextWriter _sourceWriter;

        private bool _shouldWriteHeader;
        private string _functionName;

        public CppWriter(string headerFileName, TextWriter headerWriter, TextWriter sourceWriter)
        {
            _headerFileName = headerFileName.Replace('.', '_').ToUpper();
            _headerWriter = new IndentedTextWriter(headerWriter);
            _sourceWriter = new IndentedTextWriter(sourceWriter);
        }

        public void Dispose()
        {
            _headerWriter?.Dispose();
            _sourceWriter?.Dispose();
        }

        public void WriteStart()
        {
            _headerWriter.WriteLine($"#ifndef {_headerFileName}");
            _headerWriter.WriteLine($"#define {_headerFileName}");
            _headerWriter.WriteLine();
        }

        public void WriteEnd()
        {
            _headerWriter.WriteLine();
            _headerWriter.WriteLine("#endif");
        }

        public void WriteSimpleType(string type)
        {
            WriteWithHeader($"{TranslateType(type)} ");
        }

        public void WriteFunctionTypeBegin()
        {
            WriteWithHeader("std::function<");
        }

        public void WriteFunctionTypeEnd()
        {
            WriteWithHeader(">");
        }

        public void WriteProcedureTypeBegin()
        {
            WriteFunctionTypeBegin();
        }

        public void WriteProcedureTypeEnd()
        {
            WriteFunctionTypeEnd();
        }

        public void WriteRoutineArgListBegin()
        {
            WriteWithHeader("(");
        }

        public void WriteRoutineArgDelimeter()
        {
            WriteWithHeader(", ");
        }

        public void WriteRoutineArgListEnd()
        {
            WriteWithHeader(")");
        }

        public void WriteImportBegin()
        {
            _sourceWriter.WriteLine("#include <iostream>");
            _sourceWriter.WriteLine("#include <vector>");
            _sourceWriter.WriteLine("#include <string>");
        }

        public void WriteImport(string importingModule)
        {
            bool isStandard = IsStandard(importingModule);

            string includeName = ModuleToInclude(importingModule);
            _headerWriter.WriteLine($"#include \"{includeName}\"");
            _sourceWriter.WriteLine($"#include \"{includeName}\"");

            if (isStandard)
            {
                return;
            }

            _headerWriter.WriteLine($"using namespace {importingModule};");
            _sourceWriter.WriteLine($"using namespace {importingModule};");
        }

        public void WriteImportEnd()
        {
            _headerWriter.WriteLine();
            _sourceWriter.WriteLine();
        }

        public void WriteModuleBegin(string moduleName)
        {
            _headerWriter.WriteLine($"namespace {moduleName}\n{{");
            _headerWriter.Indent++;

            _sourceWriter.WriteLine($"namespace {moduleName}\n{{");
            _sourceWriter.Indent++;
        }

        public void WriteModuleEnd()
        {
            _headerWriter.Indent--;
            _headerWriter.WriteLine("}");

            _sourceWriter.Indent--;
            _sourceWriter.WriteLine("}");
        }

        public void WriteFunctionDeclareBegin(string accessModifier, string name)
        {
            _shouldWriteHeader = true;
            _functionName = name;
        }

        public void WriteFunctionDeclareEnd(string accessModifier, string name)
        {
            _shouldWriteHeader = false;
        }

        public void WriteBlockBegin()
        {
            _sourceWriter.WriteLine("{");
            _sourceWriter.Indent++;
        }

        public void WriteBlockEnd()
        {
            _sourceWriter.Indent--;
            _sourceWriter.WriteLine("}");
        }

        public void WriteFunction(string accessModifier, string returningType, string name,
            IEnumerable<SubprogramArgument> arguments)
        {
            StringBuilder declarationBuilder = new StringBuilder();
            declarationBuilder.Append($"{TranslateType(returningType)} {name}(");

            AppendArguments(declarationBuilder, arguments);

            declarationBuilder.Append(")");
            string declaration = declarationBuilder.ToString();

            _headerWriter.WriteLine($"{declaration};");
            _sourceWriter.WriteLine(declaration);
        }

        public void WriteProcedure(string accessModifier, string name, IEnumerable<SubprogramArgument> arguments)
        {
            WriteFunction(accessModifier, "void", name, arguments);
        }

        public void WriteStatementEnd(StatementType statementType = StatementType.SingleStatement)
        {
            if (statementType != StatementType.BlockStatement)
            {
                _sourceWriter.WriteLine(";");
            }
        }

        public void WriteIdentifier(string identifier)
        {
            _sourceWriter.Write(identifier);
        }

        public void WriteAssign()
        {
            _sourceWriter.Write(" = ");
        }

        public void WriteInput(string identifier)
        {
            _sourceWriter.Write($"std::cin >> {identifier}");
        }

        public void WriteOutput()
        {
            _sourceWriter.Write("std::cout << ");
        }

        public void WriteReturn()
        {
            _sourceWriter.Write("return ");
        }

        public void WriteFunctionCallBegin(string functionName, string moduleName = null)
        {
            string modulePrefix = moduleName != null ? $"{moduleName}::" : string.Empty;
            _sourceWriter.Write($"{modulePrefix}{functionName}(");
        }

        public void WriteCallArgSeparator()
        {
            _sourceWriter.Write(", ");
        }

        public void WriteFunctionCallEnd()
        {
            WriteBraceEnd();
        }

        public void WriteIfBegin()
        {
            _sourceWriter.Write("if ");
            WriteBraceBegin();
        }

        public void WriteIfEnd()
        {
            WriteBraceEnd();
            WriteLine();
        }

        public void WriteElse()
        {
            _sourceWriter.WriteLine("else");
        }

        public void WriteWhileBegin()
        {
            _sourceWriter.Write("while ");
            WriteBraceBegin();
        }

        public void WriteWhileEnd()
        {
            WriteBraceEnd();
            WriteLine();
        }

        public void WriteDo()
        {
            _sourceWriter.WriteLine("do");
        }

        public void WriteDoWhileBegin()
        {
            _sourceWriter.Write("while ");
            WriteBraceBegin();
        }

        public void WriteDoWhileEnd()
        {
            WriteBraceEnd();
            WriteStatementEnd();
            WriteLine();
        }

        public void WriteSum()
        {
            _sourceWriter.Write(" + ");
        }

        public void WriteSubstraction()
        {
            _sourceWriter.Write(" - ");
        }

        public void WriteMultiply()
        {
            _sourceWriter.Write(" * ");
        }

        public void WriteDivision()
        {
            _sourceWriter.Write(" / ");
        }

        public void WriteMod()
        {
            _sourceWriter.Write(" % ");
        }

        public void WriteBraceBegin()
        {
            _sourceWriter.Write("(");
        }

        public void WriteBraceEnd()
        {
            _sourceWriter.Write(")");
        }

        public void WritePlus()
        {
            _sourceWriter.Write("+");
        }

        public void WriteMinus()
        {
            _sourceWriter.Write("-");
        }

        public void WriteLogicOr()
        {
            _sourceWriter.Write(" || ");
        }

        public void WriteLogicAnd()
        {
            _sourceWriter.Write(" && ");
        }

        public void WriteEquality()
        {
            _sourceWriter.Write(" == ");
        }

        public void WriteInequality()
        {
            _sourceWriter.Write(" != ");
        }

        public void WriteBigger()
        {
            _sourceWriter.Write(" > ");
        }

        public void WriteLesser()
        {
            _sourceWriter.Write(" < ");
        }

        public void WriteBiggerOrEqual()
        {
            _sourceWriter.Write(" >= ");
        }

        public void WriteLesserOrEqual()
        {
            _sourceWriter.Write(" <= ");
        }

        public void WriteNot()
        {
            _sourceWriter.Write("!");
        }

        public string GetArrayType(string elementType, int dimentionsCount)
        {
            return GetVectorForDimention(TranslateType(elementType), dimentionsCount);
        }

        public void WriteArrayDimention(string type, int index, int dimentionsCount)
        {
            if (index > 0)
            {
                _sourceWriter.Write(", ");
            }

            _sourceWriter.Write(GetVectorForDimention(TranslateType(type), dimentionsCount - index));
            WriteBraceBegin();
        }

        public void WriteArrayEnd(int dimentionsCount)
        {
            for (int i = 0; i < dimentionsCount; i++)
            {
                WriteBraceEnd();
            }
        }

        public void WriteArrayElementBegin()
        {
            _sourceWriter.Write("[");
        }

        public void WriteArrayElementEnd()
        {
            _sourceWriter.Write("]");
        }

        public void WriteRaw(string raw)
        {
            _sourceWriter.Write(raw);
        }

        public void WriteLine()
        {
            _sourceWriter.WriteLine();
        }

        public Task FlushAsync()
        {
            return Task.WhenAll(_headerWriter.FlushAsync(), _sourceWriter.FlushAsync());
        }

        private static string GetVectorForDimention(string type, int dimention)
        {
            StringBuilder vectorBuilder = new StringBuilder();

            for (int i = 0; i < dimention; i++)
            {
                vectorBuilder.Append("std::vector<");
            }

            vectorBuilder.Append(type);

            for (int i = 0; i < dimention; i++)
            {
                vectorBuilder.Append(">");
            }

            return vectorBuilder.ToString();
        }

        private static bool IsStandard(string module)
        {
            return ModuleStandardIncludes.ContainsKey(module);
        }

        private static string ModuleToInclude(string module)
        {
            return IsStandard(module) ? ModuleStandardIncludes[module] : $"{module}.h";
        }

        private static void AppendArguments(StringBuilder builder, IEnumerable<SubprogramArgument> arguments)
        {
            IEnumerable<SubprogramArgument> argumentsAsArray = arguments as SubprogramArgument[] ?? arguments.ToArray();

            if (arguments == null || !argumentsAsArray.Any())
            {
                return;
            }

            SubprogramArgument firstArg = argumentsAsArray.First();
            string reference = firstArg.PassModifier == "ref" ? "&" : string.Empty;
            builder.Append($"{TranslateType(firstArg.Type)} {reference}{firstArg.Name}");

            foreach (SubprogramArgument arg in argumentsAsArray.Skip(1))
            {
                reference = arg.PassModifier == "ref" ? "&" : string.Empty;
                builder.Append($", {TranslateType(arg.Type)} {reference}{arg.Name}");
            }
        }

        private static string TranslateType(string type)
        {
            return StandardTypes.TryGetValue(type, out string cppType) ? cppType : type;
        }

        private void WriteWithHeader(string text)
        {
            if (_shouldWriteHeader)
            {
                _headerWriter.Write(text);
            }

            _sourceWriter.Write(text);
        }
    }
}