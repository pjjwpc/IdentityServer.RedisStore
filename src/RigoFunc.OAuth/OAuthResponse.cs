using IdentityModel.Client;

namespace RigoFunc.OAuth {
    /// <summary>
    /// Represents the default implementation of the <see cref="IOAuthResponse"/> interface.
    /// </summary>
    public class OAuthResponse : IOAuthResponse {
        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <value>The access token.</value>
        public string AccessToken { get; }

        /// <summary>
        /// Gets the expires in.
        /// </summary>
        /// <value>The expires in.</value>
        public long ExpiresIn { get; }

        /// <summary>
        /// Gets the raw.
        /// </summary>
        /// <value>The raw.</value>
        public string Raw { get; }

        /// <summary>
        /// Gets the refresh token.
        /// </summary>
        /// <value>The refresh token.</value>
        public string RefreshToken { get; }

        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        /// <value>The type of the token.</value>
        public string TokenType { get; }

        /// <summary>
        /// Gets the is error.
        /// </summary>
        /// <value>The is error.</value>
        public bool IsError { get; }

        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>The error.</value>
        public string Error { get; }

        private OAuthResponse(TokenResponse response) {
            AccessToken = response.AccessToken;
            ExpiresIn = response.ExpiresIn;
            Raw = response.Raw;
            RefreshToken = response.RefreshToken;
            TokenType = response.TokenType;
            IsError = response.IsError;
            Error = response.Error;
        }

        private OAuthResponse(string error) {
            IsError = true;
            Error = error;
        }

        /// <summary>
        /// Creates an <see cref="IOAuthResponse"/> from the specified token response.
        /// </summary>
        /// <param name="response">The token response.</param>
        /// <returns>A <see cref="IOAuthResponse"/> indicating a OAuth response.</returns>
        public static IOAuthResponse FromTokenResponse(TokenResponse response) => new OAuthResponse(response);

        /// <summary>
        /// Creates an <see cref="IOAuthResponse"/> indicating a failed OAuth response, with a <paramref name="error"/> if applicable.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>A <see cref="IOAuthResponse"/> indicating a failed OAuth response.</returns>
        public static IOAuthResponse Failed(string error) => new OAuthResponse(error);
    }
}
