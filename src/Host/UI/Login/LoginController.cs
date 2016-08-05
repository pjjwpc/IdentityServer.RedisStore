using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Host.EntityFrameworkCore;

namespace Host.UI.Login {
    public class LoginController : Controller {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ILogger<LoginController> _logger;
        private readonly IUserInteractionService _interaction;

        public LoginController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ILogger<LoginController> logger,
            IUserInteractionService interaction) {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _interaction = interaction;
        }

        [HttpGet("ui/login", Name = "Login")]
        public async Task<IActionResult> Index(string returnUrl) {
            var vm = new LoginViewModel();

            if (returnUrl != null) {
                var request = await _interaction.GetLoginContextAsync();
                if (request != null) {
                    vm.UserName = request.LoginHint;
                    vm.ReturnUrl = returnUrl;
                }
            }

            return View(vm);
        }

        [HttpPost("ui/login")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginInputModel model) {
            if (ModelState.IsValid) {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded) {
                    _logger.LogInformation(1, "User logged in.");
                    if (model.ReturnUrl != null && _interaction.IsValidReturnUrl(model.ReturnUrl)) {
                        return Redirect(model.ReturnUrl);
                    }

                    return Redirect("~/");
                }
                if (result.RequiresTwoFactor) {
                    //return RedirectToAction(nameof(SendCode), new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }
                if (result.IsLockedOut) {
                    _logger.LogWarning(2, "User account locked out.");
                    return View("Lockout");
                }
                else {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            var vm = new LoginViewModel(model);
            return View(vm);
        }
    }
}
