# RigoFunc.IdentityServer.Services.EntityFrameworkCore
使用EntityFrameworkCore实现IdentityServer的存储

# Feature
- [x] Clients & Scopes Store
- [ ] 

# Usage
```csharp

public void ConfigureServices(IServiceCollection services) {
    //...
    var identiyServerBuilder = services.AddIdentityServer()

    //add this
    var connectionString = @"Server=(local);Database=identityserver;Trusted_Connection=True;";
    identityServerBuilder.AddEntityFrameworkCoreServices(options => options.UseSqlServer(connectionString));

    //...

}
```

or (obsoleted)
```csharp
public void ConfigureServices(IServiceCollection services) {
    services.AddIdentityServer();

    // add this
    var connectionString = @"Server=(local);Database=identityserver;Trusted_Connection=True;";
    services.AddIdentityServerEntityFrameworkCoreServices(options => options.UseSqlServer(connectionString));
    //...
}
```
