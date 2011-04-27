using System.Diagnostics.Contracts;
using IronRuby.Runtime;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Scripting.Hosting;

namespace MsBuild.IronRuby
{
    public class IronRubyTaskWrapper : Task, IGeneratedTask
    {
        private ScriptScope _taskScope;
        private readonly string _taskName;
        private dynamic _rubyTask;

        public IronRubyTaskWrapper(ScriptScope taskScope, string taskName)
        {
            _taskScope = taskScope;
            _taskName = taskName;
        }

        public override bool Execute()
        {
            RubyTask.execute();

            return true;
        }

        public object GetPropertyValue(TaskPropertyInfo property)
        {
            Contract.Requires(property != null);
//            var taskOperations = _taskScope.CreateOperations();
//            object rubyProperty;
//            if(!taskOperations.TryGetMember(RubyTask, RubyUtils.TryMangleName(property.Name), out rubyProperty))
//            {
//                if(!taskOperations.TryGetMember(RubyTask, property.Name, out rubyProperty))
//                {
//                    throw new NotSupportedException("The property '" + property.Name + "' is not defined");
//                }
//            }
//
//            return taskOperations.Invoke(rubyProperty);

            return string.Empty;
        }

        /// <summary>
        /// Sets the property value.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="value">The value to set.</param>
        public void SetPropertyValue(TaskPropertyInfo property, object value)
        {
            Contract.Requires(property != null);
            

//            var taskOperations = _taskScope.CreateOperations();
//            object rubyProperty;
//            if(!taskOperations.TryGetMember(RubyTask, RubyUtils.TryMangleName(property.Name), out rubyProperty))
//            {
//                if(!taskOperations.TryGetMember(RubyTask, property.Name, out rubyProperty))
//                {
//                    throw new NotSupportedException("The property '" + property.Name + "' is not defined");
//                }
//            }
//
//            taskOperations.Invoke(rubyProperty);
            int i = 0;
        }

        private dynamic RubyTask
        {
            get
            {
                if(_rubyTask == null)
                {
                    _rubyTask = _taskScope.Execute(_taskName + ".new");
                    _rubyTask.log = Log;
                }

                return _rubyTask;
            }
        }
    }
}