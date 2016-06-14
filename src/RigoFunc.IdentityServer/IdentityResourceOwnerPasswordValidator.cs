// Copyright (c) RigoFunc (xuyingting). All rights reserved.

using System.Threading.Tasks;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace RigoFunc.IdentityServer {
    public class IdentityResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator {
        private readonly UserManager<IdentityUser<int>> _userManager;
        public IdentityResourceOwnerPasswordValidator(UserManager<IdentityUser<int>> userManager) {
            _userManager = userManager;
        }
        public async Task<CustomGrantValidationResult> ValidateAsync(string userName, string password, ValidatedTokenRequest request) {
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null && await _userManager.CheckPasswordAsync(user, password))
                return new CustomGrantValidationResult(user.Id.ToString(), "password");

            return new CustomGrantValidationResult("Invalid user name or password");
        }
    }
}
