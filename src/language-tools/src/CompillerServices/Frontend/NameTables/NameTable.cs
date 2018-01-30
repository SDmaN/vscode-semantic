using System.Collections;
using System.Collections.Generic;

namespace CompillerServices.Frontend.NameTables
{
    public abstract class NameTable<TTableRow> : ICollection<TTableRow> where TTableRow : NameTableRow
    {
        private readonly ICollection<TTableRow> _set;

        protected NameTable()
        {
            _set = new HashSet<TTableRow>();
        }

        protected NameTable(IEnumerable<TTableRow> other)
        {
            _set = new HashSet<TTableRow>(other);
        }

        public void Add(TTableRow item)
        {
            _set.Add(item);
        }

        public void Clear()
        {
            _set.Clear();
        }

        public bool Contains(TTableRow item)
        {
            return _set.Contains(item);
        }

        public void CopyTo(TTableRow[] array, int arrayIndex)
        {
            _set.CopyTo(array, arrayIndex);
        }

        public bool Remove(TTableRow item)
        {
            return _set.Remove(item);
        }

        public int Count => _set.Count;
        public bool IsReadOnly => _set.IsReadOnly;

        public IEnumerator<TTableRow> GetEnumerator()
        {
            return _set.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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