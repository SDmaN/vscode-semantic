using System.Collections.Generic;
using System.Linq;

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
    }
}