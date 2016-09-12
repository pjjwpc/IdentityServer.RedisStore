using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;

namespace RigoFunc.IdentityServer {
    /// <summary>
    /// Handles validation of resource owner password credentials
    /// </summary>
    /// <typeparam name="TUser">The type of the t user.</typeparam>
    public class IdentityResourceOwnerPasswordValidator<TUser> : IResourceOwnerPasswordValidator where TUser : class {
        private readonly UserManager<TUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="IdentityResourceOwnerPasswordValidator{TUser}"/> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        public IdentityResourceOwnerPasswordValidator(UserManager<TUser> userManager) {
            _userManager = userManager;
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context) {
            var userName = context.UserName;
            var secret = context.Password;
            var user = _userManager.FindByNameAsync(userName).Result;
            if (user != null) {
                var userId = _userManager.GetUserIdAsync(user).Result;
                var claims = new List<Claim>();//todo
                if (_userManager.CheckPasswordAsync(user, secret).Result) {
                    context.Result = new GrantValidationResult(userId, "password", claims);
                }
                else {
                    var phoneNumber = _userManager.GetPhoneNumberAsync(user).Result;
                    if (_userManager.VerifyChangePhoneNumberTokenAsync(user, secret, phoneNumber).Result) {
                        context.Result = new GrantValidationResult(userId, "code", claims);
                    }
                }
            }
            return Task.FromResult(0);
        }
    }
}
