dotnet restore
dotnet build .\src\RigoFunc.IdentityServer.Services.EntityFrameworkCore
dotnet pack .\src\RigoFunc.IdentityServer.Services.EntityFrameworkCore


$project = Get-Content .\src\RigoFunc.IdentityServer.Services.EntityFrameworkCore\project.json | ConvertFrom-Json
$version = $project.version.Trim("-*")
nuget push .\src\RigoFunc.IdentityServer.Services.EntityFrameworkCore\bin\Debug\RigoFunc.IdentityServer.Services.EntityFrameworkCore.$version.nupkg -source nuget -apikey $env:NUGET_API_KEY
