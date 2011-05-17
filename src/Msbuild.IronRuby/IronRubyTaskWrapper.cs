using System;

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
            return _taskScope.GetProperty((object) TaskInstance, property.Name);
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
                    _taskInstance.build_engine = BuildEngine;
                }

                return _taskInstance;
            }
        }
    }
}