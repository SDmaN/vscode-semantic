using System;
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
        #region Standard library modules

        private static readonly IDictionary<string, string> _moduleStandardIncludes;

        #endregion

        private readonly IndentedTextWriter _headerWriter;
        private readonly IndentedTextWriter _sourceWriter;

        static CppWriter()
        {
            Dictionary<string, string> moduleIncludes = new Dictionary<string, string>
            {
                { "Math", "cmath" }
            };

            _moduleStandardIncludes = new ReadOnlyDictionary<string, string>(moduleIncludes);
        }

        public CppWriter(TextWriter headerWriter, TextWriter sourceWriter)
        {
            _headerWriter = new IndentedTextWriter(headerWriter);
            _sourceWriter = new IndentedTextWriter(sourceWriter);
        }

        public void Dispose()
        {
            _headerWriter?.Dispose();
            _sourceWriter?.Dispose();
        }

        public void WriteImportBegin()
        {
            _sourceWriter.WriteLine("#include <iostream>");
        }

        public void WriteImport(string importingModule)
        {
            bool isStandard = IsStandard(importingModule);

            string includeName = ModuleToInclude(importingModule);
            _headerWriter.WriteLine($"#include <{includeName}>");
            _sourceWriter.WriteLine($"#include <{includeName}>");

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

        public void WriteBlockBegin()
        {
            _sourceWriter.WriteLine("{");
            _sourceWriter.Indent++;
        }

        public void WriteBlockEnd()
        {
            _sourceWriter.Indent--;
            _sourceWriter.WriteLine("}");
            _sourceWriter.WriteLine();
        }

        public void WriteFunction(string accessModifier, string returningType, string name,
            IEnumerable<FunctionArgument> arguments)
        {
            StringBuilder declarationBuilder = new StringBuilder();
            declarationBuilder.Append($"{returningType} {name}(");

            IEnumerable<FunctionArgument> argumentsAsArray = arguments as FunctionArgument[] ?? arguments.ToArray();

            if (arguments != null && argumentsAsArray.Any())
            {
                FunctionArgument firstArg = argumentsAsArray.First();
                declarationBuilder.Append($"{firstArg.Type} {firstArg.Name}");

                foreach (FunctionArgument arg in argumentsAsArray.Skip(1))
                {
                    declarationBuilder.Append($", {arg.Type} {arg.Name}");
                }
            }

            declarationBuilder.Append(")");
            string declaration = declarationBuilder.ToString();

            _headerWriter.WriteLine($"{declaration};");
            _sourceWriter.WriteLine(declaration);
        }

        public void WriteProcedure(string accessModifier, string name, IEnumerable<FunctionArgument> arguments)
        {
            WriteFunction(accessModifier, "void", name, arguments);
        }

        public void WriteStatementEnd()
        {
            _sourceWriter.WriteLine(";");
        }

        public void WriteType(string type)
        {
            _sourceWriter.Write($"{type} ");
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
            WriteStatementEnd();
        }

        public void WriteOutputBegin()
        {
            _sourceWriter.Write("std::cout << ");
        }

        public void WriteOutputEnd()
        {
            WriteStatementEnd();
        }

        public void WriteReturnBegin()
        {
            _sourceWriter.Write("return ");
        }

        public void WriteReturnEnd()
        {
            WriteStatementEnd();
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

        public void WriteBracketBegin()
        {
            _sourceWriter.Write("(");
        }

        public void WriteBracketEnd()
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

        public void WriteRaw(string raw)
        {
            _sourceWriter.Write(raw);
        }

        public Task FlushAsync()
        {
            return Task.WhenAll(_headerWriter.FlushAsync(), _sourceWriter.FlushAsync());
        }

        private static bool IsStandard(string module)
        {
            return _moduleStandardIncludes.ContainsKey(module);
        }

        private static string ModuleToInclude(string module)
        {
            return IsStandard(module) ? _moduleStandardIncludes[module] : $"{module}.h";
        }
    }
}