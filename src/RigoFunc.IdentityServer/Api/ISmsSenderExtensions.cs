using System;
using System.Threading.Tasks;
using RigoFunc.ApiCore.Services;

namespace RigoFunc.IdentityServer.Api {
    public static class ISmsSenderExtensions {
        public async static Task<SendSmsResult> SendCodeAsync(this ISmsSender sender, string phoneNumber, string code) {
            return await sender.SendSmsAsync("SMS_5265397", phoneNumber, Tuple.Create("code", code));
        }

        public async static Task<SendSmsResult> SendPasswordAsync(this ISmsSender sender, string phoneNumber, string password) {
            return await sender.SendSmsAsync("SMS_5265397", phoneNumber, Tuple.Create("password", password));
        }
    }
}
