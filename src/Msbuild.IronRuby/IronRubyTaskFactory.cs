using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using IronRuby;
using Microsoft.Build.Framework;
using Microsoft.Scripting.Hosting;
using System.Linq;

using MsBuild.IronRuby.Extensions;

namespace MsBuild.IronRuby
{
    public class IronRubyTaskFactory : ITaskFactory
    {
        private const string RubyTaskScript = "MsBuild.IronRuby.Scripts.RubyTask.rb";
        private ScriptScope _taskScriptScope;
        private dynamic _taskClass;
        private AssemblyResolver _assemblyResolver;
        
        public bool Initialize(string taskName, IDictionary<string, TaskPropertyInfo> parameterGroup, string taskBody, IBuildEngine taskFactoryLoggingHost)
        {
            var projectFileDirectory = Path.GetDirectoryName(taskFactoryLoggingHost.ProjectFileOfTaskNode);
            var thisAssemblyDirectory = Path.GetDirectoryName(this.GetType().Assembly.Location);
            _assemblyResolver = new AssemblyResolver(thisAssemblyDirectory, projectFileDirectory);

            try
            {
                var engine = Ruby.CreateEngine();
                _taskScriptScope = engine.CreateScope();
                _taskScriptScope.ExecuteEmbeddedScript(RubyTaskScript);
                var rubyTaskBody = TaskBodyParser.Parse(taskBody);
                var scriptFile = projectFileDirectory.CombinePath(rubyTaskBody.Script).ToFullPath();
                _taskScriptScope = _taskScriptScope.ExecuteFile(scriptFile);
                _taskClass = engine.Runtime.Globals.GetVariable(taskName);
            }
            catch(Exception)
            {
                _assemblyResolver.Dispose();
                throw;
            }

            return true;
        }

        public TaskPropertyInfo[] GetTaskParameters()
        {
            IEnumerable<dynamic> taskParameters = _taskClass.parameters().Values;
            return (from taskParameter in taskParameters
                    let paramInfo = new
                    {
                        Name = (string)taskParameter.name(),
                        Type = typeof(string),
                        IsRequired = (bool)taskParameter.is_required()
                    }
                    select new TaskPropertyInfo(paramInfo.Name.Unmangle(), paramInfo.Type, output: false, required: paramInfo.IsRequired)).ToArray();
        }

        public ITask CreateTask(IBuildEngine taskFactoryLoggingHost)
        {
            return new IronRubyTaskWrapper(_taskScriptScope, _taskClass) {BuildEngine = taskFactoryLoggingHost};
        }

        public void CleanupTask(ITask task)
        {
            if(_assemblyResolver != null)
                _assemblyResolver.Dispose();
        }

        public string FactoryName
        {
            get { return this.GetType().Name; }
        }

        public Type TaskType
        {
            get { return typeof(IronRubyTaskWrapper); }
        }

        private class AssemblyResolver : IDisposable
        {
            private readonly ResolveEventHandler _resolveEventHandler;
            
            public AssemblyResolver(params string[] searchPaths)
            {
                _resolveEventHandler = (sender, args) =>
                {
                    var assemblySimpleName = GetSimpleName(args.Name);
                    foreach(var searchPath in searchPaths)
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

            public void Dispose()
            {
                AppDomain.CurrentDomain.AssemblyResolve -= _resolveEventHandler;
            }
        }
    }
}