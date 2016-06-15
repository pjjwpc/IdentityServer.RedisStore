using System;
using System.Threading.Tasks;

namespace RigoFunc.IdentityServer.Services {
    /// <summary>
    /// Represents the default email and Sms sender services.
    /// </summary>
    public class MessageSender : IEmailSender, ISmsSender {
        /// <summary>
        /// Sends the email asynchronous.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <returns>A <see cref="Task{TResult}" /> represents the send operation.</returns>
        public Task SendEmailAsync(string email, string subject, string message) {
            // Plug in your email service here to send an email.
            return Task.FromResult(0);
        }

        /// <summary>
        /// Sends the Sms message asynchronous.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the send operation.</returns>
        public async Task<SendSmsResult> SendSmsAsync(string template, string phoneNumber, params Tuple<string, string>[] parameters) {
            // Plug in your email service here to send an email.
            return await Task.FromResult(new SendSmsResult());
        }
    }
}
