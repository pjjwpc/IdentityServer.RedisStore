dotnet restore
dotnet build .\src\RigoFunc.IdentityServer.RedisStore
dotnet pack .\src\RigoFunc.IdentityServer.RedisStore


$project = Get-Content .\src\RigoFunc.IdentityServer.RedisStore\project.json | ConvertFrom-Json
$version = $project.version.Trim("-*")
nuget push .\src\RigoFunc.IdentityServer.RedisStore\bin\Debug\RigoFunc.IdentityServer.RedisStore.$version.nupkg -source nuget -apikey $env:NUGET_API_KEY
