class HelloIronRuby
    include MsBuildTask
    
    task_property :Message
    
    def execute
        log.log_message "hello #{Message}"
    end
end