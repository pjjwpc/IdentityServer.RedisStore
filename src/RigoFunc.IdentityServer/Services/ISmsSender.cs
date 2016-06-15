using System;
using System.Threading.Tasks;

namespace RigoFunc.IdentityServer.Services {
    /// <summary>
    /// Represents the Sms sender services.
    /// </summary>
    public interface ISmsSender {
        /// <summary>
        /// Sends the Sms message asynchronous.
        /// </summary>
        /// <param name="template">The template.</param>
        /// <param name="phoneNumber">The phone number.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A <see cref="Task{TResult}"/> represents the send operation.</returns>
        Task<SendSmsResult> SendSmsAsync(string template, string phoneNumber, params Tuple<string, string>[] parameters);
    }

    /// <summary>
    /// Represents the Sms send result.
    /// </summary>
    public class SendSmsResult {
        /// <summary>
        /// Gets or sets a value indicating whether this instance is success send.
        /// </summary>
        /// <value><c>true</c> if this instance is success send; otherwise, <c>false</c>.</value>
        public bool IsSuccessSend { get; set; }
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Creates an <see cref="SendSmsResult"/> indicating a failed identity operation, with a <paramref name="message"/> if applicable.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>A <see cref="SendSmsResult"/> indicating a failed send operation.</returns>
        public static SendSmsResult Failed(string message) {
            return new SendSmsResult {
                IsSuccessSend = false,
                ErrorMessage = message
            };
        }
    }
}
