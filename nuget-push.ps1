dotnet restore
dotnet build .\src\RigoFunc.IdentityServer
dotnet build .\src\RigoFunc.IdentityServer.DistributedStore
dotnet build .\src\RigoFunc.IdentityServer.RedisStore
dotnet build .\src\RigoFunc.IdentityServer.Services.EntityFrameworkCore

dotnet test .\test\RigoFunc.IdentityServer.IntergrationTests
dotnet test .\test\RigoFunc.IdentityServer.Services.EntityFrameworkCore.UnitTests
dotnet test .\test\RigoFunc.IdentityServer.UnitTest

$project = Get-Content .\src\RigoFunc.IdentityServer\project.json | ConvertFrom-Json
$version = $project.version.Trim("-*")
nuget push .\src\RigoFunc.IdentityServer\bin\Debug\RigoFunc.IdentityServer.$version.nupkg -source nuget -apikey $env:NUGET_API_KEY