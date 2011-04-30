require 'rubygems'
require 'rspec'
require '../MsBuild.IronRuby/Scripts/RubyTask.rb'

class SampleTask
  include MsBuildTask
        
  task_parameter :input_param
  task_parameter :required_param, :required => true
  task_parameter :output_param, :output => true
  task_parameter :item_param, :type => 'itaskitem'
end

describe 'IronRuby MsBuild task' do
  describe 'task_parameter' do
    it 'should create a settable property for a task parameter' do
      task = SampleTask.new
      task.input_param = 'my value'
      task.input_param.should == 'my value'
    end
    it 'should default to input parameter' do
      SampleTask.task_parameters[:input_param].output?.should == false
    end
    it 'should default to optional' do
      SampleTask.task_parameters[:input_param].required?.should == false
    end
    it 'should be required when specified with required option' do
      SampleTask.task_parameters[:required_param].required?.should == true
    end
    it 'should be an output parameter if specified with output option' do
      SampleTask.task_parameters[:output_param].output?.should == true
    end
    it 'should default to type string' do
      SampleTask.task_parameters[:input_param].type.should == 'string'
    end
    it 'should be of specified type' do
      SampleTask.task_parameters[:item_param].type.should == 'itaskitem'
    end
  end
end