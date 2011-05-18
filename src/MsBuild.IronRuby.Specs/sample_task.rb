require 'Microsoft.Build.Utilities.v4.0, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a'

class SampleTask
  include MsBuild::Task
  include Microsoft::Build::Utilities
        
  parameter :input_param
  parameter :required_param, :required => true
  parameter :output_param, :output => true
  parameter :item_param, :type => 'taskitem', :output => true
  parameter :items_param, :type => 'taskitem[]', :output => true
  
  def execute
    @output_param = "goodbye"
    @typed_param = 5
    @item_param = TaskItem.new('item 1')
    @items_param = [ TaskItem.new('item 1'), TaskItem.new('item 2') ]
  end
end