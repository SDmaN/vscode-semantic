using System.Linq;
using System.Text;
using CompillerServices.Frontend.NameTables.Types;

namespace CompillerServices.Frontend
{
    public class ExpressionResult
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

        public string GetTypeText()
        {
            switch (ExpressionType)
            {
                case ExpressionType.Routine:
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(PossibleTypes[0]);

                    for (int i = 1; i < PossibleTypes.Length; i++)
                    {
                        builder.Append("; ");
                        builder.Append(PossibleTypes[i]);
                    }

                    return builder.ToString();
                }

                default:
                    return PossibleTypes.FirstOrDefault().ToString();
            }
        }
    }
}