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

        It can_set_parameters_correctly =()=>
        {
            var inputParameter = _properties.Single(p => p.Name == "InputParam");
            _task.SetPropertyValue(inputParameter, "hello");

            var value = _task.GetPropertyValue(inputParameter);
            value.ShouldEqual("hello");
        };

        private It can_return_simple_output_parameters_correctly =()=>
        {
            var parameter = _properties.Single(p => p.Name == "OutputParam");

            _task.Execute();

            var value = _task.GetPropertyValue(parameter);
            value.ShouldEqual("goodbye");
        };

        It can_return_typed_output_parameters_correctly =()=>
        {
            var typedParameter = _properties.Single(p => p.Name == "ItemParam");

            _task.Execute();

            var value = _task.GetPropertyValue(typedParameter) as ITaskItem;
            value.ItemSpec.ShouldEqual("item 1");
        };

        It can_return_a_typed_output_arrays_correctly = () =>
        {
            var itemsParameter = _properties.Single(p => p.Name == "ItemsParam");

            _task.Execute();

            var list = (object[])_task.GetPropertyValue(itemsParameter);

            var x = list.Cast<ITaskItem>();

            x.First().ItemSpec.ShouldEqual("item 1");
            x.Last().ItemSpec.ShouldEqual("item 2");
        };

        static IronRubyTaskWrapper _task;
        static TaskPropertyInfo[] _properties; 
    }
}