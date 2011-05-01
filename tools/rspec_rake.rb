require "rspec/core/rake_task"

def rspec(args)
  task = RSpec::Core::RakeTask.new args
  yield(task) if block_given?
  task
end