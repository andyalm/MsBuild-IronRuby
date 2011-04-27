using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;
using IronRuby;
using Microsoft.Build.Framework;
using Microsoft.Scripting.Hosting;
using System.Linq;

namespace MsBuild.IronRuby
{
    public class IronRubyTaskFactory : ITaskFactory
    {
        private ScriptScope _taskScriptScope;
        private string _taskName;
        private const string RubyTaskScript = "MsBuild.IronRuby.Scripts.RubyTask.rb";
        
        public bool Initialize(string taskName, IDictionary<string, TaskPropertyInfo> parameterGroup, string taskBody, IBuildEngine taskFactoryLoggingHost)
        {
            Debugger.Launch();
            var engine = Ruby.CreateEngine();
            _taskScriptScope = engine.CreateScope();
            _taskScriptScope.ExecuteEmbeddedScript(RubyTaskScript);
            var rubyTaskBody = TaskBodyParser.Parse(taskBody);
            var taskProjectDir = Path.GetDirectoryName(taskFactoryLoggingHost.ProjectFileOfTaskNode);
            var scriptFile = Path.GetFullPath(Path.Combine(taskProjectDir, rubyTaskBody.Script));
            _taskScriptScope = _taskScriptScope.Engine.ExecuteFile(scriptFile, _taskScriptScope);
            _taskName = taskName;

            return true;
        }

        public TaskPropertyInfo[] GetTaskParameters()
        {
            var propertyInfo = _taskScriptScope.Execute<IEnumerable<dynamic>>(_taskName + ".task_properties.values")
                .Select(paramInfo => new TaskPropertyInfo(paramInfo.name().ToString().Unmangle(), typeof(string), false, paramInfo.is_required()))
                .ToArray();

            return propertyInfo;
            return new TaskPropertyInfo[0];
        }

        public ITask CreateTask(IBuildEngine taskFactoryLoggingHost)
        {
            return new IronRubyTaskWrapper(_taskScriptScope, _taskName) {BuildEngine = taskFactoryLoggingHost};
        }

        public void CleanupTask(ITask task)
        {
            
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

    public class TaskBodyParser
    {
        public static TaskBodyParser Parse(string taskBody)
        {
            return new TaskBodyParser(XElement.Parse(taskBody));
        }

        private XElement _taskElement;

        public TaskBodyParser(XElement taskElement)
        {
            _taskElement = taskElement;
        }

        public string Script
        {
            get { return (string)_taskElement.Attribute("Path"); }
        }
    }
}