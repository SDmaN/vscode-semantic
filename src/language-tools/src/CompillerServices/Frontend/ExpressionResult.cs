using CompillerServices.Frontend.NameTables.Types;

namespace CompillerServices.Frontend
{
    internal class ExpressionResult
    {
        public readonly SlangType SlangType;
        public readonly ExpressionType ExpressionType;

        public ExpressionResult(SlangType slangType, ExpressionType expressionType)
        {
            SlangType = slangType;
            ExpressionType = expressionType;
        }
    }
}