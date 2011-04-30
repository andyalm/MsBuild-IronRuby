class HelloIronRuby
    include MsBuild::Task
    
    parameter :message, :required => true, :type => ''
    parameter :items, :output => true
    
    def execute
        log.log_message "hello #{message}"
    end
end