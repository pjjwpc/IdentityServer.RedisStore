namespace RigoFunc.IdentityServer.Api {
    public static class Constants {
        public static readonly string[] ApiPaths = new string[] {
            // can this by reflection rather than hard code?
            "api/account/register",
            "api/account/sendcode",
            "api/account/login",
            "api/account/verifycode",
            "api/account/changepassword",
            "api/account/resetpassword",
            "api/account/update",
        };
    }
}
