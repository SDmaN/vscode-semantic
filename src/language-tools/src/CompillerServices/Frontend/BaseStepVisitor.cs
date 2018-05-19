using System.Collections.Generic;
using Antlr4.Runtime;
using CompillerServices.Exceptions;
using CompillerServices.Frontend.NameTables.Types;
using CompillerServices.IO;
using SlangGrammar;

namespace CompillerServices.Frontend
{
    internal abstract class BaseStepVisitor : SlangBaseVisitor<object>
    {
        private readonly SlangModule _slangModule;

        protected BaseStepVisitor(SlangModule slangModule)
        {
            _slangModule = slangModule;
        }

        public override object VisitSimpleType(SlangParser.SimpleTypeContext context)
        {
            return new SimpleType(context.SimpleType().GetText());
        }

        public override object VisitFuncType(SlangParser.FuncTypeContext context)
        {
            IList<RoutineTypeArg> routineTypeArgs = (IList<RoutineTypeArg>) Visit(context.routineArgList());
            SlangType returningType = (SlangType) Visit(context.type());

            return new FunctionType(returningType, routineTypeArgs);
        }

        public override object VisitProcType(SlangParser.ProcTypeContext context)
        {
            IList<RoutineTypeArg> routineTypeArgs = (IList<RoutineTypeArg>) Visit(context.routineArgList());
            return new ProcedureType(routineTypeArgs);
        }

        public override object VisitRoutineArgList(SlangParser.RoutineArgListContext context)
        {
            IList<RoutineTypeArg> routineTypeArgs = new List<RoutineTypeArg>(context.routineArg().Length);

            foreach (SlangParser.RoutineArgContext arg in context.routineArg())
            {
                routineTypeArgs.Add((RoutineTypeArg) Visit(arg));
            }

            return routineTypeArgs;
        }

        public override object VisitRoutineArg(SlangParser.RoutineArgContext context)
        {
            string modifier = context.ArgPassModifier().GetText();
            SlangType type = (SlangType) Visit(context.type());

            return new RoutineTypeArg(modifier, type);
        }

        public override object VisitArrayType(SlangParser.ArrayTypeContext context)
        {
            SlangType elementType = (SlangType) Visit(context.scalarType());
            int dimentions = context.arrayDimention().Length;

            return new ArrayType(elementType, dimentions);
        }

        protected void ThrowCompillerException(string message, IToken symbol)
        {
            throw new CompillerException(message, _slangModule.ModuleFile, symbol.Line, symbol.Column);
        }
    }
}