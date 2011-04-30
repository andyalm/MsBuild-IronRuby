class HelloIronRuby
    include MsBuildTask
    
    task_parameter :message, :required => true, :type => ''
    task_parameter :items, :output => true
    
    def execute
        log.log_message "hello #{message}"
    end
end