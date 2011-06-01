using System;
using System.Collections.Generic;
using System.IO;
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
        private readonly IParameterTypeDeserializer _typeDeserializer;
        private IFileSystem _fileSystem;

        public IronRubyTaskFactory() : this(new FileSystem(), new AssemblyResolver(), new ParameterTypeDeserializer()) {}

        public IronRubyTaskFactory(IFileSystem fileSystem, IAssemblyResolver assemblyResolver, IParameterTypeDeserializer typeDeserializer)
        {
            _fileSystem = fileSystem;
            _assemblyResolver = assemblyResolver;
            _typeDeserializer = typeDeserializer;
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
                    select new TaskPropertyInfo(
                        name : ((string) taskParameter.name()).Unmangle(),
                        typeOfParameter : _typeDeserializer.DeserializeType((string)taskParameter.type()),
                        output : (bool) taskParameter.is_output(),
                        required : (bool) taskParameter.is_required()
                        )).ToArray();
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