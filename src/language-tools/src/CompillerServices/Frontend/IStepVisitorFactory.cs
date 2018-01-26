using CompillerServices.Frontend.NameTables;
using CompillerServices.IO;

namespace CompillerServices.Frontend
{
    public interface IStepVisitorFactory
    {
        FirstStepVisitor CreateFirstStepVisitor(INameTableContainer nameTableContainer, SlangModule slangModule);
        SecondStepVisitor CreateSecondStepVisitor(INameTableContainer nameTableContainer, SlangModule slangModule);
    }
}