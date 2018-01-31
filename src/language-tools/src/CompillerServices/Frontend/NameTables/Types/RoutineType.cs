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

        public bool HasAssignableArgTypes(IList<SlangType[]> types)
        {
            int index = 0;
            return Args.Count == types.Count && Args.All(x => types[index++].Any(y => x.Type.IsAssignable(y)));
        }
    }
}