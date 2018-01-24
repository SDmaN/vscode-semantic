using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CompillerServices.Frontend.NameTables.Types
{
    public class ProcedureType : SlangType
    {
        public ProcedureType(IEnumerable<RoutineTypeArg> args)
        {
            Args = args;
        }

        public IEnumerable<RoutineTypeArg> Args { get; }

        public override bool IsAssignable(SlangType other)
        {
            if (!(other is ProcedureType type))
            {
                return false;
            }

            return Args.SequenceEqual(type.Args);
        }

        public override bool Equals(SlangType other)
        {
            return IsAssignable(other);
        }

        public override int GetHashCode()
        {
            return Args != null ? Args.GetHashCode() : 0;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("proc (");

            RoutineTypeArg firstArg = Args.FirstOrDefault();

            if (firstArg != null)
            {
                builder.Append(firstArg);

                foreach (RoutineTypeArg nextArg in Args.Skip(1))
                {
                    builder.Append($", {nextArg}");
                }
            }

            builder.Append($")");

            return builder.ToString();
        }
    }
}