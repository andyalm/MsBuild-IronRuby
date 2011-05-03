using System.Linq;

using Machine.Fakes;
using Machine.Specifications;

using Microsoft.Build.Framework;

namespace MsBuild.IronRuby
{
    [Subject("IronRubyTaskFactory")]
    public class when_executing_task : WithSubject<IronRubyTaskFactory>
    {
        Establish context = () =>
        {
            var sampleTaskBehavior = new SampleTaskBehavior();
            With(sampleTaskBehavior);
            sampleTaskBehavior.Initialize(Subject);
            _properties = Subject.GetTaskParameters();
            _task = (IronRubyTaskWrapper) Subject.CreateTask(The<IBuildEngine>());
        };

        private It can_read_and_set_parameters = () =>
        {
            var inputParameter = _properties.Single(p => p.Name == "InputParam");
            _task.SetPropertyValue(inputParameter, "hello");
            var value = _task.GetPropertyValue(inputParameter);

            value.ShouldEqual("hello");
        };


        static IronRubyTaskWrapper _task;
        static TaskPropertyInfo[] _properties; 
    }
}