using System;
using System.Diagnostics.Contracts;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Scripting.Hosting;

using MsBuild.IronRuby.Extensions;

namespace MsBuild.IronRuby
{
    public class IronRubyTaskWrapper : Task, IGeneratedTask
    {
        private readonly ScriptScope _taskScope;
        private readonly dynamic _taskClass;
        private dynamic _taskInstance;

        public IronRubyTaskWrapper(ScriptScope taskScope, dynamic taskClass)
        {
            _taskScope = taskScope;
            _taskClass = taskClass;
        }

        public override bool Execute()
        {
            TaskInstance.execute();

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
            _taskScope.SetProperty((object)TaskInstance, property.Name, value);
        }

        private dynamic TaskInstance
        {
            get
            {
                if(_taskInstance == null)
                {
                    _taskInstance = _taskClass.@new();
                    _taskInstance.log = Log;
                }

                return _taskInstance;
            }
        }
    }
}