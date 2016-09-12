dotnet restore
dotnet build .\src\RigoFunc.IdentityServer
dotnet build .\src\RigoFunc.IdentityServer.DistributedStore
dotnet build .\src\RigoFunc.IdentityServer.RedisStore
dotnet build .\src\RigoFunc.IdentityServer.Services.EntityFrameworkCore

dotnet test .\test\RigoFunc.IdentityServer.IntergrationTests
dotnet test .\test\RigoFunc.IdentityServer.Services.EntityFrameworkCore.UnitTests
dotnet test .\test\RigoFunc.IdentityServer.UnitTest

dotnet pack .\src\RigoFunc.IdentityServer
$project = Get-Content .\src\RigoFunc.IdentityServer\project.json | ConvertFrom-Json
$version = $project.version.Trim("-*")
nuget push .\src\RigoFunc.IdentityServer\bin\Debug\RigoFunc.IdentityServer.$version.nupkg -source nuget -apikey $env:NUGET_API_KEY


dotnet pack .\src\RigoFunc.IdentityServe.DistributedStore
$project = Get-Content .\src\RigoFunc.IdentityServer.DistributedStore\project.json | ConvertFrom-Json
$version = $project.version.Trim("-*")
nuget push .\src\RigoFunc.IdentityServer.DistributedStore\bin\Debug\RigoFunc.IdentityServer.DistributedStore.$version.nupkg -source nuget -apikey $env:NUGET_API_KEY

dotnet pack .\src\RigoFunc.IdentityServer.RedisStore
$project = Get-Content .\src\RigoFunc.IdentityServer.RedisStore\project.json | ConvertFrom-Json
$version = $project.version.Trim("-*")
nuget push .\src\RigoFunc.IdentityServer.RedisStore\bin\Debug\RigoFunc.IdentityServer.RedisStore.$version.nupkg -source nuget -apikey $env:NUGET_API_KEY

dotnet pack .\src\RigoFunc.IdentityServer.Services.EntityFrameworkCore
$project = Get-Content .\src\RigoFunc.IdentityServer.Services.EntityFrameworkCore\project.json | ConvertFrom-Json
$version = $project.version.Trim("-*")
nuget push .\src\RigoFunc.IdentityServer.Services.EntityFrameworkCore\bin\Debug\RigoFunc.IdentityServer.Services.EntityFrameworkCore.$version.nupkg -source nuget -apikey $env:NUGET_API_KEY

