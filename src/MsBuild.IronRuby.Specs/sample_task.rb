﻿class SampleTask
  include MsBuild::Task
        
  parameter :input_param
  parameter :required_param, :required => true
  parameter :output_param, :output => true
  parameter :item_param, :type => 'itaskitem'
end