using System.Threading.Tasks;

namespace RigoFunc.OAuth.Services {
    /// <summary>
    /// Represents an email sender services.
    /// </summary>
    public interface IEmailSender {
        /// <summary>
        /// Sends the email asynchronous.
        /// </summary>
        /// <param name="email">The email.</param>
        /// <param name="subject">The subject.</param>
        /// <param name="message">The message.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the send operation.</returns>
        Task SendEmailAsync(string email, string subject, string message);
    }
}
