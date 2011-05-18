module MsBuild
  module Task
    attr_accessor :log
    attr_accessor :build_engine
    
    module ClassMethods
      def parameter(name, options={})
        parameters[name] = TaskParameter.new name, options
        attr_accessor name
      end

      def parameters
        @parameters = Hash.new if @parameters == nil
        @parameters
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
end