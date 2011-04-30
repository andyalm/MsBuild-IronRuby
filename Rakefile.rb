require "albacore"
require "rspec/core/rake_task"

configuration = :Debug

task :default => :test

task :build => [:run_build]
msbuild :run_build do |b|
  b.properties :configuration => configuration
  b.solution = 'src/MsBuild.IronRuby/MsBuild.IronRuby.csproj'
end

task :test => [:build, :run_specs]

desc "Running RSpec tests"
RSpec::Core::RakeTask.new(:run_specs)  do |t|
  t.pattern = "./src/MsBuild.IronRuby.Specs/**/*_spec.rb"
  t.skip_bundler = true
end