namespace CompillerServices.Frontend.NameTables.Types
{
    public sealed class SimpleType : SlangType
    {
        public static readonly SimpleType Bool = new SimpleType(Constants.Keywords.Bool);
        public static readonly SimpleType Int = new SimpleType(Constants.Keywords.Int);
        public static readonly SimpleType Real = new SimpleType(Constants.Keywords.Real);

        public SimpleType(string typeKeyword)
        {
            TypeKeyword = typeKeyword;
        }

        public string TypeKeyword { get; }

        public override bool IsAssignable(SlangType other)
        {
            if (Equals(other))
            {
                return true;
            }

            if (!(other is SimpleType otherSimple))
            {
                return false;
            }

            return TypeKeyword == Constants.Keywords.Real && otherSimple.TypeKeyword == Constants.Keywords.Int;
        }

        public override bool Equals(SlangType other)
        {
            return other is SimpleType type && string.Equals(TypeKeyword, type.TypeKeyword);
        }

        public override int GetHashCode()
        {
            return TypeKeyword != null ? TypeKeyword.GetHashCode() : 0;
        }

        public override string ToString()
        {
            return TypeKeyword;
        }
    }
}