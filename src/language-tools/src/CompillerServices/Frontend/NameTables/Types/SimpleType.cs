namespace CompillerServices.Frontend.NameTables.Types
{
    public sealed class SimpleType : SlangType
    {
        public SimpleType(string typeKeyword)
        {
            TypeKeyword = typeKeyword;
        }

        public string TypeKeyword { get; }

        public override bool IsAssignable(SlangType other)
        {
            return other is SimpleType type && string.Equals(TypeKeyword, type.TypeKeyword);
        }

        public override bool Equals(SlangType other)
        {
            return IsAssignable(other);
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