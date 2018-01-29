namespace CompillerServices.Frontend
{
    internal class StatementResult
    {
        public StatementResult(bool returnsValue)
        {
            ReturnsValue = returnsValue;
        }

        public bool ReturnsValue { get; }
    }
}