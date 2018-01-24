namespace CompillerServices.Exceptions
{
    public class RoutineAlreadyDefinedException : CompillerException
    {
        public RoutineAlreadyDefinedException(string message, string moduleName, int line, int column)
            : base(message, moduleName, line, column)
        {
        }
    }
}