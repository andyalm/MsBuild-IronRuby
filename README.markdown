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
