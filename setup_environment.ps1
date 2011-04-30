$result = &"where.exe" ir
if($result -eq $null)
{
    Write-Error "IronRuby is required.  Please add it to your path"
    return
}
ir -S bundle install
ir -S rake

Write-Host ""
Write-Host "**********************************************************"
Write-Host "Environment is set up.  From now on, you can just run rake (or run build.ps1)"
Write-Host ""