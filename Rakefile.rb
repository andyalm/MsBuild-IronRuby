require "albacore"
require "tools/rspec_rake"
require "tools/iron_xlinq"

configuration = :Debug

task :default => :test

task :import do
  repositories = XDocument.load "packages/repositories.config"
  repositories.root.elements("repository").each do |r|
    relative_path = r.attribute("path")
    packages_config_path = File.expand_path(File.join "packages", relative_path)
    result = `tools/nuget install #{packages_config_path} -o packages`
    puts result
  end
end

msbuild :build => [:import] do |t|
  t.properties :configuration => configuration
  t.solution = "src/MsBuild.IronRuby/MsBuild.IronRuby.csproj"
end

task :test => [:rspec_test, :mspec_test]

rspec :rspec_test do |t|
  t.pattern = "src/MsBuild.IronRuby.Specs/**/*_spec.rb"
  t.skip_bundler = true
end

msbuild :build_specs => [:build] do |t|
  t.properties :configuration => configuration
  t.solution = "src/MsBuild.IronRuby.Specs/MsBuild.IronRuby.Specs.csproj"
end

mspec :mspec_test => [:build_specs] do |t|
  t.command = "packages/Machine.Specifications.0.4.10.0/tools/mspec-clr4.exe"
  t.assemblies = ["src/MsBuild.IronRuby.Specs/bin/#{configuration}/MsBuild.IronRuby.Specs.dll"]
end

