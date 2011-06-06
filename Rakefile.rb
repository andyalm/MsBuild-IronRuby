require "albacore"
require "tools/rspec_rake"
require "tools/iron_xlinq"

configuration = :Debug

task :default => :package

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

task :test => [:test_rspec,:test_mspec]

rspec :test_rspec => [:import] do |t|
  t.pattern = "src/MsBuild.IronRuby.Specs/**/*_spec.rb"
  t.skip_bundler = true
end

msbuild :build_specs => [:build] do |t|
  t.properties :configuration => configuration
  t.solution = "src/MsBuild.IronRuby.Specs/MsBuild.IronRuby.Specs.csproj"
end

mspec :test_mspec => [:build_specs] do |t|
  t.command = "packages/Machine.Specifications.0.4.10.0/tools/mspec-clr4.exe"
  t.assemblies = ["src/MsBuild.IronRuby.Specs/bin/#{configuration}/MsBuild.IronRuby.Specs.dll"]
end

packaging_staging_directory = "output"

desc "Creating the NuGet package"
task :package => [:test,:package_stage_files,:package_create_nuspec, :package_create_package]

task :package_stage_files do
  FileUtils.mkdir_p "#{packaging_staging_directory}/lib"
  FileUtils.copy "src/MsBuild.IronRuby/bin/#{configuration}/MsBuild.IronRuby.dll", "#{packaging_staging_directory}/lib"
  FileUtils.copy "src/MsBuild.IronRuby/bin/#{configuration}/MsBuild.IronRuby.pdb", "#{packaging_staging_directory}/lib"
end

nuspec :package_create_nuspec do |t|
  t.id = "MsBuild.IronRuby"
  t.version = "0.1"
  t.authors = "Andy Alm, Greg Musick"
  t.description = "An experimental project to allow writing MSBuild tasks in IronRuby (differently than the DLRTaskFactory provides)"
  t.title = "IronRuby MSBuild Tasks"
  t.projectUrl = "https://github.com/andyalm/MsBuild-IronRuby"
  t.dependency "IronRuby", "1.1.3"
  t.working_directory = packaging_staging_directory
  t.output_file = "MsBuild.IronRuby.nuspec"
end

task :package_create_package do
  packages_output_directory = "#{packaging_staging_directory}\\packages"
  FileUtils.mkdir_p packages_output_directory
  nuget_pack_command = "tools/nuget.exe pack #{packaging_staging_directory}\\MsBuild.IronRuby.nuspec -OutputDirectory #{packages_output_directory}"
  puts "Executing '#{nuget_pack_command}'..."
  `#{nuget_pack_command}`
end

