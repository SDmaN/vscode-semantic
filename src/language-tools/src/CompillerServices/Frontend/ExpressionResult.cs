using System.Linq;
using CompillerServices.Frontend.NameTables.Types;

namespace CompillerServices.Frontend
{
    internal class ExpressionResult
    {
        public readonly ExpressionType ExpressionType;
        public readonly SlangType[] PossibleTypes;

        public ExpressionResult(ExpressionType expressionType, params SlangType[] possibleTypes)
        {
            ExpressionType = expressionType;
            PossibleTypes = possibleTypes;
        }

        public bool IsAssignableToType(SlangType other)
        {
            return PossibleTypes != null && PossibleTypes.Any(other.IsAssignable);
        }
    }
}