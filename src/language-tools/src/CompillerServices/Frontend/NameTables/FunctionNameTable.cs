using CompillerServices.Frontend.NameTables.Types;

namespace CompillerServices.Frontend.NameTables
{
    public class FunctionNameTable : NameTable<FunctionNameTableRow>
    {
    }

    public class FunctionNameTableRow : RoutineNameTableRow
    {
        public FunctionNameTableRow(int line, int column, string accessModifier, SlangType returningType, string name,
            ModuleNameTableRow parentModule)
            : base(line, column, accessModifier, name, parentModule)
        {
            ReturningType = returningType;
        }

        public SlangType ReturningType { get; }

        public override SlangType ToSlangType()
        {
            return new FunctionType(ReturningType, GetRoutineTypeArgs());
        }
    }
}