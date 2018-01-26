using System.Collections.Generic;
using System.Linq;

namespace CompillerServices.Frontend.NameTables.Types
{
    public abstract class RoutineType : SlangType
    {
        protected RoutineType(IList<RoutineTypeArg> args)
        {
            Args = args;
        }

        public IList<RoutineTypeArg> Args { get; }

        public bool IsSuitable(IList<SlangType> argTypes)
        {
            int index = 0;
            return Args.Count == argTypes.Count && Args.All(x => x.Type.IsAssignable(argTypes[index++]));
        }
    }
}