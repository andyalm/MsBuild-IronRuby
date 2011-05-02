require 'rubygems'
require 'rspec'
require File.dirname(__FILE__) + '/../MsBuild.IronRuby/Scripts/RubyTask'
require File.dirname(__FILE__) + '/sample_task'

describe MsBuild::Task do
  describe 'parameter' do
    it 'should create a settable property for a task parameter' do
      task = SampleTask.new
      task.input_param = 'my value'
      task.input_param.should == 'my value'
    end
    it 'should default to input parameter' do
      SampleTask.parameters[:input_param].output?.should == false
    end
    it 'should default to optional' do
      SampleTask.parameters[:input_param].required?.should == false
    end
    it 'should be required when specified with required option' do
      SampleTask.parameters[:required_param].required?.should == true
    end
    it 'should be an output parameter if specified with output option' do
      SampleTask.parameters[:output_param].output?.should == true
    end
    it 'should default to type string' do
      SampleTask.parameters[:input_param].type.should == 'string'
    end
    it 'should be of specified type' do
      SampleTask.parameters[:item_param].type.should == 'itaskitem'
    end
  end
end