


# sample google oauth login


- follow http://www.asp.net/mvc/overview/security/create-an-aspnet-mvc-5-app-with-facebook-and-google-oauth2-and-openid-sign-on
https://github.com/aspnet/Security/blob/dev/samples/SocialSample/Startup.cs#L200
- add dependencies
```
    "Microsoft.AspNetCore.Authentication.Google": "1.0.0"

```
- add configure
```
        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory) {
            //...
            app.UseGoogleAuthentication(new GoogleOptions() {
                ClientId = "481836841077-cb09qcd0khin228ais4vg71oni5luq8m.apps.googleusercontent.com",
                ClientSecret = "PIJZOUveCa8m3ZE4k6FaDSxc"
            });
        }

```


# facebook
- follow https://docs.asp.net/en/latest/security/authentication/sociallogins.html
- add dependencies
```
    "Microsoft.AspNetCore.Authentication.Facebook": "1.0.0"
```
- add configurate
