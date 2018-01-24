using System;
using System.Collections.Generic;
using System.Linq;

namespace CompillerServices.Frontend.NameTables.Types
{
    public class FunctionType : SlangType
    {
        public FunctionType(SlangType returningType, IEnumerable<RoutineTypeArg> args)
        {
            ReturningType = returningType;
            Args = args;
        }

        public SlangType ReturningType { get; }
        public IEnumerable<RoutineTypeArg> Args { get; }

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
                return ((ReturningType != null ? ReturningType.GetHashCode() : 0) * 397) ^ (Args != null ? Args.GetHashCode() : 0);
            }
        }
    }
}