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
        private IAssemblyResolver _assemblyResolver;
        private IFileSystem _fileSystem;

        public IronRubyTaskFactory() : this(new FileSystem(), new AssemblyResolver()) {}

        public IronRubyTaskFactory(IFileSystem fileSystem, IAssemblyResolver assemblyResolver)
        {
            _fileSystem = fileSystem;
            _assemblyResolver = assemblyResolver;
        }

        public bool Initialize(string taskName, IDictionary<string, TaskPropertyInfo> parameterGroup, string taskBody, IBuildEngine taskFactoryLoggingHost)
        {
            var projectFileDirectory = Path.GetDirectoryName(taskFactoryLoggingHost.ProjectFileOfTaskNode);
            var thisAssemblyDirectory = Path.GetDirectoryName(this.GetType().Assembly.Location);
            _assemblyResolver.BeginResolving(thisAssemblyDirectory, projectFileDirectory);

            try
            {
                var engine = Ruby.CreateEngine();
                _taskScriptScope = engine.CreateScope();
                _taskScriptScope.ExecuteEmbeddedScript(RubyTaskScript);
                var rubyTaskBody = TaskBodyParser.Parse(taskBody);
                var scriptFile = projectFileDirectory.CombinePath(rubyTaskBody.Script).ToFullPath();
                var scriptContents = _fileSystem.GetFileContent(scriptFile);
                _taskScriptScope.Execute(scriptContents);
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

    }
}