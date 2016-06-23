using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RigoFunc.Account.Models;
using RigoFunc.Account.Services;
using RigoFunc.OAuth;

namespace RigoFunc.Account {
    [Route("api/[controller]")]
    public class AccountController {
        private readonly IAccountService _service;

        public AccountController(IAccountService service) {
            _service = service;
        }

        [HttpGet]
        public async Task<OAuthUser> Get([FromQuery]FindUserModel model) {
            if (model == null) {
                throw new ArgumentNullException(nameof(model));
            }

            if (model.Id == null && string.IsNullOrEmpty(model.PhoneNumber)) {
                throw new ArgumentException("must provide user id or phone number. e.g. ?userId=111&phonenumber=phone");
            }

            return await _service.GetAsync(model);
        }

        [HttpPost("[action]")]
        public async Task<IResponse> Register([FromBody]RegisterModel model) {
            if (model == null) {
                throw new ArgumentNullException(nameof(model));
            }

            return await _service.RegisterAsync(model);
        }

        [HttpPost("[action]")]
        public async Task<bool> SendCode([FromBody]SendCodeModel model) {
            if (model == null) {
                throw new ArgumentNullException(nameof(model));
            }

            return await _service.SendCodeAsync(model);
        }

        [HttpPost("[action]")]
        public async Task<IResponse> Login([FromBody]LoginModel model) {
            if (model == null) {
                throw new ArgumentNullException(nameof(model));
            }

            return await _service.LoginAsync(model);
        }

        [HttpPost("[action]")]
        public async Task<IResponse> VerifyCode([FromBody]VerifyCodeModel model) {
            if (model == null) {
                throw new ArgumentNullException(nameof(model));
            }

            return await _service.VerifyCodeAsync(model);
        }

        [HttpPost("[action]")]
        public async Task<bool> ChangePassword([FromBody]ChangePasswordModel model) {
            if (model == null) {
                throw new ArgumentNullException(nameof(model));
            }

            return await _service.ChangePasswordAsync(model);
        }

        [HttpPost("[action]")]
        public async Task<IResponse> ResetPassword([FromBody]ResetPasswordModel model) {
            if (model == null) {
                throw new ArgumentNullException(nameof(model));
            }

            return await _service.ResetPasswordAsync(model);
        }

        [HttpPost("[action]")]
        public async Task<bool> Update([FromBody]OAuthUser model) {
            if (model == null) {
                throw new ArgumentNullException(nameof(model));
            }

            return await _service.UpdateAsync(model);
        }
    }
}
