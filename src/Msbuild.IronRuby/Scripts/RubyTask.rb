module MsBuild
  module Task
    attr_accessor :log unless defined? log
    attr_accessor :build_engine unless defined? build_engine
    
    module ClassMethods
      def parameter(name, options={})
        parameters[name] = TaskParameter.new(name, options)
        attr_accessor name
      end

      def parameters
        @@parameters ||= Hash.new
        @@parameters
      end
    end

    def self.included(base)
      base.extend ClassMethods
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