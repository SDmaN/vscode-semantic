using System;
using System.Collections.Generic;

namespace PluginServer.LanguageServices
{
    public interface IFileContentKeeper
    {
        void Add(Uri fileUri, string text);
        void Remove(Uri fileUri);
        string Get(Uri fileUri);
    }

    public class FileContentKeeper : IFileContentKeeper
    {
        private readonly IDictionary<Uri, string> _fileContents = new Dictionary<Uri, string>();

        public void Add(Uri fileUri, string text)
        {
            if (!_fileContents.ContainsKey(fileUri))
            {
                _fileContents.Add(fileUri, text);
            }
        }

        public void Remove(Uri fileUri)
        {
            if (_fileContents.ContainsKey(fileUri))
            {
                _fileContents.Remove(fileUri);
            }
        }

        public string Get(Uri fileUri)
        {
            return _fileContents.TryGetValue(fileUri, out string text) ? text : null;
        }
    }
}