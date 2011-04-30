require "albacore"
require "rspec/core/rake_task"
require "tools/iron_xlinq"

configuration = :Debug

task :default => :test

task :import do
  repositories = XDocument.load "packages/repositories.config"
  repositories.root.elements("repository").each do |r|
    relative_path = r.attribute("path")
    packages_config_full_path = File.expand_path(File.join "packages", relative_path)
    result = `.\\tools\\nuget.exe install #{packages_config_full_path} -o .\\packages\\`
    puts result
  end
end

msbuild :build => [:import] do |b|
  b.properties :configuration => configuration
  b.solution = 'src/MsBuild.IronRuby/MsBuild.IronRuby.csproj'
end

RSpec::Core::RakeTask.new(:test => [:build])  do |t|
  t.pattern = "./src/MsBuild.IronRuby.Specs/**/*_spec.rb"
  t.skip_bundler = true
end