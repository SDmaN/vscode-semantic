using System.Collections.Concurrent;

namespace CompillerServices.Frontend.NameTables
{
    public abstract class NameTable<TTableRow> : BlockingCollection<TTableRow> where TTableRow : NameTableRow
    {
        public void Clear()
        {
            while (Count > 0)
            {
                TryTake(out _);
            }
        }
    }

    public abstract class NameTableRow
    {
        protected NameTableRow(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public int Line { get; }
        public int Column { get; }
    }
}