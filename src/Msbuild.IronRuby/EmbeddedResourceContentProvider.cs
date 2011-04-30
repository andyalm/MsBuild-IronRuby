using System.IO;
using System.Reflection;
using Microsoft.Scripting;

namespace MsBuild.IronRuby
{
    /// <summary>
    /// DLR hook to load a script from an embedded resource
    /// </summary>
    public class EmbeddedResourceContentProvider : StreamContentProvider
    {
        private readonly Assembly _assembly;
        private readonly string _fileName;

        public EmbeddedResourceContentProvider(string fileName, Assembly assembly)
        {
            _fileName = fileName;
            _assembly = assembly;
        }

        public override Stream GetStream()
        {
            return _assembly.GetManifestResourceStream(_fileName);
        }
    }
}