using System.Collections.Concurrent;

namespace CompillerServices.Frontend.NameTables
{
    public abstract class NameTable<TTableRow> : BlockingCollection<TTableRow>
    {
        public void Clear()
        {
            while (Count > 0)
            {
                TryTake(out _);
            }
        }
    }
}