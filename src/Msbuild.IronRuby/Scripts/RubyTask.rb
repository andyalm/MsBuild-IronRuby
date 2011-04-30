module MsBuildTask
  attr_accessor :log

  module ClassMethods
    def task_parameter(name, options={})
      task_parameters[name] = TaskParameter.new name, options
      attr_accessor name
    end

    def task_parameters
      @task_parameters = Hash.new if @task_parameters == nil
      @task_parameters
    end
  end

  def self.included(c)
    c.extend ClassMethods
  end

  class TaskParameter
    def initialize name, options={}
      @name = name
      @options = options
    end

    def name
      @name
    end

    def required?
      @options.fetch :required, false
    end

    def is_required
      required?
    end

    def output?
      @options[:output] || false
    end

    def is_output
      output?
    end

    def type
      @options[:type] || 'string'
    end
  end
end