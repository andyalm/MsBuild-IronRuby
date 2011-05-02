using System.Collections.Generic;

using Machine.Fakes;
using Machine.Specifications;

using Microsoft.Build.Framework;

using System.Linq;

namespace MsBuild.IronRuby
{
    [Subject("IronRubyTaskFactory")]
    public class when_discovering_parameters : WithSubject<IronRubyTaskFactory>
    {
        Establish context = () =>
        {
            _sampleTaskBehavior = new SampleTaskBehavior();
            With(_sampleTaskBehavior);
            _sampleTaskBehavior.Initialize(Subject);
            _discoveredParameters = Subject.GetTaskParameters().ToDictionary(p => p.Name);
        };

        It should_convert_names_to_camel_case = () =>
            _discoveredParameters.ContainsKey("InputParam").ShouldBeTrue();

        It should_set_default_type_to_string = () =>
            _discoveredParameters["InputParam"].PropertyType.ShouldEqual(typeof(string));

        private static SampleTaskBehavior _sampleTaskBehavior;
        private static IDictionary<string,TaskPropertyInfo> _discoveredParameters;
    }
}