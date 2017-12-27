using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace CompillerServices.IO
{
    public class SourceContainer : IEnumerable<SlangModule>
    {
        private readonly IDictionary<string, SlangModule> _sources =
            new ConcurrentDictionary<string, SlangModule>();

        public SlangModule this[string moduleName] =>
            _sources.TryGetValue(moduleName, out SlangModule value) ? value : null;

        public bool ContainsMainModule { get; private set; }
        public string MainModuleName { get; internal set; }

        public IEnumerator<SlangModule> GetEnumerator()
        {
            return _sources.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        internal void Add(SlangModule slangModule)
        {
            _sources.Add(slangModule.ModuleName, slangModule);
            ContainsMainModule = ContainsMainModule | slangModule.IsMain;

            if (slangModule.IsMain)
            {
                MainModuleName = slangModule.ModuleName;
            }
        }
    }
}