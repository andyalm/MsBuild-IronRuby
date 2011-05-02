using System.Collections.Generic;
using System.IO;

using FakeItEasy;

using Machine.Fakes;
using Machine.Specifications.Annotations;

using Microsoft.Build.Framework;

namespace MsBuild.IronRuby
{
    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    public class SampleTaskBehavior
    {
        static IFileSystem _fileSystem;
        static IBuildEngine _buildEngine;
        static bool _initReturnValue;
        
        OnEstablish context = fakeAccessor =>
        {
            _fileSystem = fakeAccessor.The<IFileSystem>();
            _fileSystem
                .WhenToldTo(fs => fs.GetFileContent(A<string>.Ignored))
                .Return(GetSampleTaskContents);

            _buildEngine = fakeAccessor.The<IBuildEngine>();
            _buildEngine
                .WhenToldTo(e => e.ProjectFileOfTaskNode)
                .Return("c:\\mybuilddir\\build.proj");
        };

        public bool Initialize(IronRubyTaskFactory taskFactory)
        {
            var emptyPropertyDictionary = new Dictionary<string, TaskPropertyInfo>();
            const string usingTaskBody = @"<Script Path=""sample_task.rb"" />";
            _initReturnValue = taskFactory.Initialize("SampleTask", emptyPropertyDictionary, usingTaskBody, _buildEngine);

            return _initReturnValue;
        }

        private static string GetSampleTaskContents()
        {
            using (var stream = typeof(when_initializing_task).Assembly.GetManifestResourceStream("MsBuild.IronRuby.sample_task.rb"))
            {
                return new StreamReader(stream).ReadToEnd();
            }
        }
    }
}