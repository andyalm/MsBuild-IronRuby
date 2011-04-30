Write your MSBuild tasks completely in IronRuby (no XML or CDATA required):

HelloWorld.rb
---------------------------------

    class HelloWorld
       include MsBuild::Task

       parameter :message

       def execute
          log.log_message :message
       end
    end

Build.proj
----------------------------------
    <?xml version="1.0" encoding="utf-8"?>
    <Project ToolsVersion="4.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
        <Target Name="Build">
            <HelloWorld Message="Hello IronRuby task" />
        </Target>
    </Project>

