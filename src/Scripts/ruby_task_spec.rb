require "rspec"
require "RubyTask"

class FakeLog
  def initialize
    @messages = []
  end

  def log_message(msg)
    @messages << msg
  end

  def messages
    @messages
  end
end

describe "Setting a task property" do
  before :all do
    @task = HelloMsBuild.new
    @task.log = FakeLog.new
  end

  it "should be accessible via a getter" do
    @task.text = "hello world"
    @task.execute

    @task.log.messages.count.should == 1
    @task.log.messages[0].should == "hello world"
  end

  it "should identify required properties" do
    properties = HelloMsBuild.task_properties
    properties[:text].required?.should == true
    properties[:severity].required?.should == false
  end

end