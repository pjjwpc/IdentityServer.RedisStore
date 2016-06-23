using System.Threading.Tasks;
using RigoFunc.Account.Models;
using RigoFunc.OAuth;

namespace RigoFunc.Account.Services {
    /// <summary>
    /// Provides the interface(s) for the account service.
    /// </summary>
    public interface IAccountService {
        /// <summary>
        /// Gets OAuth user by the specified user Id or Phone number asynchronous.
        /// </summary>
        /// <param name="model">The model contains the user Id or Phone number.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the get operation. Task result contains the found user.</returns>
        Task<OAuthUser> GetAsync(FindUserModel model);
        /// <summary>
        /// Registers a new user asynchronous.
        /// </summary>
        /// <param name="model">The register model.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the register operation. Task result contains the register response.</returns>
        Task<IResponse> RegisterAsync(RegisterModel model);
        /// <summary>
        /// Sends the specified code asynchronous.
        /// </summary>
        /// <param name="model">The send code model.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the send operation.</returns>
        Task<bool> SendCodeAsync(SendCodeModel model);
        /// <summary>
        /// Logins with the specified model asynchronous.
        /// </summary>
        /// <param name="model">The login model.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the login operation.</returns>
        Task<IResponse> LoginAsync(LoginModel model);
        /// <summary>
        /// Verifies the specified code asynchronous.
        /// </summary>
        /// <param name="model">The verify code model.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the verify operation.</returns>
        Task<IResponse> VerifyCodeAsync(VerifyCodeModel model);
        /// <summary>
        /// Changes the password for the specified user asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the change operation.</returns>
        Task<bool> ChangePasswordAsync(ChangePasswordModel model);
        /// <summary>
        /// Resets the password for specified user asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the reset operation.</returns>
        Task<IResponse> ResetPasswordAsync(ResetPasswordModel model);
        /// <summary>
        /// Updates the specified user asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the reset operation.</returns>
        Task<bool> UpdateAsync(OAuthUser model);
        /// <summary>
        /// Binds the open ID for the specified user asynchronous.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the bind operation.</returns>
        Task<bool> BindAsync(OpenIdBindingModel model);
        /// <summary>
        /// Logins with the specified model asynchronous.
        /// </summary>
        /// <param name="model">The login model.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the login operation.</returns>
        Task<IResponse> LoginAsync(OpenIdLoginModel model);
    }
}
