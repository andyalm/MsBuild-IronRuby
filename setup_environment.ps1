$result = &"where.exe" ir
if($result -eq $null)
{
    Write-Error "IronRuby is required.  Please add it to your path"
    return
}
bundle install
rake

Write-Host ""
Write-Host "**********************************************************"
Write-Host "Environment is set up.  From now on, you can just run rake"
Write-Host ""