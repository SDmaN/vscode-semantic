using CompillerServices.Frontend.NameTables;
using CompillerServices.IO;
using Microsoft.Extensions.Localization;

namespace CompillerServices.Frontend
{
    internal class StepVisitorFactory : IStepVisitorFactory
    {
        private readonly IStringLocalizer<FirstStepVisitor> _firstStepLocalizer;
        private readonly IStringLocalizer<SecondStepVisitor> _secondStepLocalizer;

        public StepVisitorFactory(IStringLocalizer<FirstStepVisitor> firstStepLocalizer,
            IStringLocalizer<SecondStepVisitor> secondStepLocalizer)
        {
            _firstStepLocalizer = firstStepLocalizer;
            _secondStepLocalizer = secondStepLocalizer;
        }

        public FirstStepVisitor CreateFirstStepVisitor(INameTableContainer nameTableContainer, SlangModule slangModule)
        {
            return new FirstStepVisitor(_firstStepLocalizer, nameTableContainer, slangModule);
        }

        public SecondStepVisitor CreateSecondStepVisitor(INameTableContainer nameTableContainer,
            SlangModule slangModule)
        {
            return new SecondStepVisitor(_secondStepLocalizer, nameTableContainer, slangModule);
        }
    }
}