using System;
using System.Threading.Tasks;

using IdentityModel.Client;
using IdentityServer4;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RigoFunc.OAuth;
using RigoFunc.OAuth.Services;

namespace Host.UI.OAuth {
    [Route("api/[controller]")]
    public class OAuthController : Controller {
        private readonly UserManager<IdentityUser<int>> _userManager;
        private readonly SignInManager<IdentityUser<int>> _signInManager;
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly ILogger<OAuthController> _logger;

        public OAuthController(UserManager<IdentityUser<int>> userManager,
            SignInManager<IdentityUser<int>> signInManager,
            IEmailSender emailSender,
            ISmsSender smsSender,
            ILogger<OAuthController> logger) {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _smsSender = smsSender;
            _logger = logger;
        }

        [HttpPost("[action]")]
        public async Task<IOAuthResponse> Register([FromBody]RegisterInputModel model) {
            if (model == null || !ModelState.IsValid) {
                throw new ArgumentNullException(nameof(model));
            }

            var user = await _userManager.FindByNameAsync(model.PhoneNumber);
            if (user != null) {
                throw new ArgumentException($"The user: {model.PhoneNumber} had been register.");
            }

            user = await _userManager.FindByIdAsync("1");
            if (user == null || !await _userManager.VerifyChangePhoneNumberTokenAsync(user, model.Code, model.PhoneNumber)) {
                throw new ArgumentException($"cannot verify the code: {model.Code} for the phone: {model.PhoneNumber}");
            }

            user = new IdentityUser<int> { UserName = model.UserName ?? model.PhoneNumber, PhoneNumber = model.PhoneNumber };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded) {
                await _signInManager.SignInAsync(user, isPersistent: false);

                return await RequestTokenAsync(user.UserName, model.Password);
            }

            _logger.LogError(result.ToString());

            throw new ArgumentException($"cannot to register new user for phone: {model.PhoneNumber} code: {model.Code}");
        }

        [HttpPost("[action]")]
        public async Task<bool> SendCode([FromBody]SendCodeInputModel model) {
            if (model == null || !ModelState.IsValid) {
                throw new ArgumentNullException(nameof(model));
            }
            var user = await _userManager.FindByNameAsync(model.PhoneNumber);
            if (user == null) {
                user = await _userManager.FindByIdAsync("1");
            }

            if (user != null) {
                var code = await _userManager.GenerateChangePhoneNumberTokenAsync(user, model.PhoneNumber);
                await _smsSender.SendCodeAsync(model.PhoneNumber, code);
                return true;
            }
            else {
                throw new ArgumentException($"cannot send code for the phone: {model.PhoneNumber}");
            }
        }

        [HttpPost("[action]")]
        public async Task<IOAuthResponse> Login([FromBody]LoginInputModel model) {
            if (model == null || !ModelState.IsValid) {
                throw new ArgumentNullException(nameof(model));
            }

            var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded) {
                _logger.LogInformation(1, "User logged in.");
                return await RequestTokenAsync(model.UserName, model.Password);
            }

            _logger.LogError(result.ToString());

            throw new InvalidOperationException("User login failed");
        }

        [HttpPost("[action]")]
        public async Task<IOAuthResponse> VerifyCode([FromBody]VerifyCodeInputModel model) {
            if (model == null || !ModelState.IsValid) {
                throw new ArgumentNullException(nameof(model));
            }

            // TODO: generate a randon password.
            var password = "Honglan@520";
            var user = await _userManager.FindByNameAsync(model.PhoneNumber);
            if (user == null) {
                user = await _userManager.FindByIdAsync("1");
                if (user == null || !await _userManager.VerifyChangePhoneNumberTokenAsync(user, model.Code, model.PhoneNumber)) {
                    throw new ArgumentException($"cannot verify the code: {model.Code} for the phone: {model.PhoneNumber}");
                }

                user = new IdentityUser<int> { UserName = model.PhoneNumber, PhoneNumber = model.PhoneNumber };
                var result = await _userManager.CreateAsync(user, password);
                if (result.Succeeded) {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    await _smsSender.SendPasswordAsync(model.PhoneNumber, password);

                    _logger.LogInformation(3, "User changed their password successfully.");

                    return await RequestTokenAsync(user.UserName, password);
                }

                _logger.LogError(result.ToString());

                throw new ArgumentException($"cannot to register new user for phone: {model.PhoneNumber} code: {model.Code}");
            }
            else {
                if (!await _userManager.VerifyChangePhoneNumberTokenAsync(user, model.Code, model.PhoneNumber)) {
                    throw new ArgumentException($"cannot verify the code: {model.Code} for the phone: {model.PhoneNumber}");
                }

                // because we doesn't known the password.
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, code, password);
                if (result.Succeeded) {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return await RequestTokenAsync(user.UserName, password);
                }

                _logger.LogError(result.ToString());

                throw new ArgumentNullException($"cannot login use code: {model.Code} for the user: {model.PhoneNumber}");
            }
        }

        [HttpPost("[action]")]
        public async Task<bool> ChangePassword([FromBody]ChangePasswordModel model) {
            if (model == null || !ModelState.IsValid) {
                throw new ArgumentNullException(nameof(model));
            }

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user != null) {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded) {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.LogInformation(3, "User changed their password successfully.");

                    return true;
                }

                _logger.LogError(result.ToString());
            }

            return false;
        }

        [HttpPost("[action]")]
        public async Task<IOAuthResponse> ResetPassword([FromBody]ResetPasswordModel model) {
            if (model == null || !ModelState.IsValid) {
                throw new ArgumentNullException(nameof(model));
            }

            var user = await _userManager.FindByNameAsync(model.PhoneNumber);
            if (user == null) {
                throw new ArgumentNullException($"cannot reset the password for user: {model.PhoneNumber}");
            }

            if (!await _userManager.VerifyChangePhoneNumberTokenAsync(user, model.Code, model.PhoneNumber)) {
                throw new ArgumentException($"The code: {model.Code} is invalide or timeout with 3 minutes.");
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, code, model.Password);
            if (result.Succeeded) {
                await _signInManager.SignInAsync(user, isPersistent: false);

                return await RequestTokenAsync(user.UserName, model.Password);
            }

            _logger.LogError(result.ToString());

            throw new ArgumentNullException($"cannot reset the password for user: {model.PhoneNumber}");
        }

        [HttpPost("[action]")]
        public async Task<bool> Update([FromBody]OAuthUser model) {
            if (model == null || !ModelState.IsValid) {
                throw new ArgumentNullException(nameof(model));
            }

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null) {
                throw new ArgumentException($"cannot found the user: {model.Id}");
            }

            var result = await _userManager.AddClaimsAsync(user, model.ToClaims());
            if (result.Succeeded) {
                return true;
            }

            _logger.LogError(result.ToString());

            throw new ArgumentNullException("Update User failed");
        }

        private async Task<IOAuthResponse> RequestTokenAsync(string userName, string password) {
            var tokenEndpoint = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}/{Constants.RoutePaths.Oidc.Token}";
            _logger.LogInformation($"token_endpoint: {tokenEndpoint}");
            var client = new TokenClient(tokenEndpoint, "system", "secret");
            var response = await client.RequestResourceOwnerPasswordAsync(userName, password, "doctor consultant finance order payment");

            return OAuthResponse.FromTokenResponse(response);
        }

        private Task<IdentityUser<int>> GetCurrentUserAsync() => _userManager.GetUserAsync(HttpContext.User);
    }
}
