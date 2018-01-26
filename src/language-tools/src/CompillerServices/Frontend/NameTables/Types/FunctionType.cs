using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompillerServices.Frontend.NameTables.Types
{
    public class FunctionType : RoutineType
    {
        public FunctionType(SlangType returningType, IList<RoutineTypeArg> args)
            : base(args)
        {
            ReturningType = returningType;
        }

        public SlangType ReturningType { get; }

        public override bool IsAssignable(SlangType other)
        {
            if (!(other is FunctionType type))
            {
                return false;
            }

            return ReturningType.Equals(type.ReturningType) && Args.SequenceEqual(type.Args);
        }

        public override bool Equals(SlangType other)
        {
            return IsAssignable(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ReturningType != null ? ReturningType.GetHashCode() : 0) * 397) ^
                       (Args != null ? Args.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("fun (");

            RoutineTypeArg firstArg = Args.FirstOrDefault();

            if (firstArg != null)
            {
                builder.Append(firstArg);

                foreach (RoutineTypeArg nextArg in Args.Skip(1))
                {
                    builder.Append($", {nextArg}");
                }
            }

            builder.Append($") : {ReturningType}");

            return builder.ToString();
        }
    }
}