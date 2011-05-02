using System;
using System.IO;
using System.Reflection;

namespace MsBuild.IronRuby
{
    public interface IAssemblyResolver : IDisposable
    {
        void BeginResolving(params string[] searchPaths);

        void EndResolving();
    }

    class AssemblyResolver : IAssemblyResolver
    {
        private ResolveEventHandler _resolveEventHandler;
        
        public void EndResolving()
        {
            AppDomain.CurrentDomain.AssemblyResolve -= _resolveEventHandler;
        }

        public void Dispose()
        {
            EndResolving();
        }

        public void BeginResolving(params string[] searchPaths)
        {
            _resolveEventHandler = (sender, args) =>
            {
                var assemblySimpleName = GetSimpleName(args.Name);
                foreach (var searchPath in searchPaths)
                {
                    var assemblyPath = Path.Combine(searchPath, assemblySimpleName + ".dll");
                    try
                    {
                        return Assembly.LoadFrom(assemblyPath);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                return null;
            };

            AppDomain.CurrentDomain.AssemblyResolve += _resolveEventHandler;
        }

        private string GetSimpleName(string name)
        {
            var parts = name.Split(',');

            return parts[0];
        }
    }
}