namespace CompillerServices.Frontend.NameTables.Types
{
    public class RoutineTypeArg
    {
        public RoutineTypeArg(string modifier, SlangType type)
        {
            Modifier = modifier;
            Type = type;
        }

        public string Modifier { get; }
        public SlangType Type { get; }

        protected bool Equals(RoutineTypeArg other)
        {
            return string.Equals(Modifier, other.Modifier) && Equals(Type, other.Type);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((RoutineTypeArg) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Modifier != null ? Modifier.GetHashCode() : 0) * 397) ^
                       (Type != null ? Type.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return $"{Modifier} {Type}";
        }
    }
}