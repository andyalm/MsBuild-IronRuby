using Machine.Fakes;
using Machine.Specifications;

using Microsoft.Build.Framework;

namespace MsBuild.IronRuby
{
    [Subject("IronRubyTaskFactory")]
    public class when_parsing_parameter_types : WithSubject<ParameterTypeDeserializer>
    {
        It should_recognize_taskitem_parameters = () =>
            Subject.DeserializeType("taskitem").ShouldEqual(typeof(ITaskItem));

        It should_recognize_array_parameters = () =>
            Subject.DeserializeType("taskitem[]").ShouldEqual(typeof(ITaskItem[]));

        It should_recognize_int_parameters = () =>
            Subject.DeserializeType("int").ShouldEqual(typeof(int));

        It should_recognize_fully_qualified_types = () =>
            Subject.DeserializeType("System.Double").ShouldEqual(typeof(double));
    }
}