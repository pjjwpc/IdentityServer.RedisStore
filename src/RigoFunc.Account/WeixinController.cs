using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RigoFunc.Account.Models;
using RigoFunc.Account.Services;

namespace RigoFunc.Account {
    [Route("api/[controller]")]
    public class WeixinController {
        private readonly IAccountService _service;

        public WeixinController(IAccountService service) {
            _service = service;
        }

        [HttpPost("[action]")]
        public async Task<bool> Bind([FromBody]OpenIdBindingModel model) {
            if (model == null) {
                throw new ArgumentNullException(nameof(model));
            }

            return await _service.BindAsync(model);
        }

        [HttpPost("[action]")]
        public async Task<IResponse> Login([FromBody]OpenIdLoginModel model) {
            if (model == null) {
                throw new ArgumentNullException(nameof(model));
            }

            return await _service.LoginAsync(model);
        }
    }
}
