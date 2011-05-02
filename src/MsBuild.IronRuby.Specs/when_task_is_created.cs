using Machine.Fakes;
using Machine.Specifications;

namespace MsBuild.IronRuby
{
    [Subject("IronRubyTaskFactory")]
    public class when_initializing_task : WithSubject<IronRubyTaskFactory>
    {
        Establish context = () =>
        {
            _sampleTaskBehavior = new SampleTaskBehavior();
            With(_sampleTaskBehavior);
            _initReturnValue = _sampleTaskBehavior.Initialize(Subject);
        };

        It should_return_true = () => _initReturnValue.ShouldBeTrue();

        It should_resolve_script_path_relative_to_project_file = () =>
            The<IFileSystem>().WasToldTo(fs => fs.GetFileContent("c:\\mybuilddir\\sample_task.rb"));

        private static SampleTaskBehavior _sampleTaskBehavior;
        private static bool _initReturnValue;
    }
}