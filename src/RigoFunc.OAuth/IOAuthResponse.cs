namespace RigoFunc.OAuth {
    /// <summary>
    /// Defines the interface for OAuth response.
    /// </summary>
    public interface IOAuthResponse {
        /// <summary>
        /// Gets the access token.
        /// </summary>
        /// <value>The access token.</value>
        string AccessToken { get; }
        /// <summary>
        /// Gets the expires in.
        /// </summary>
        /// <value>The expires in.</value>
        long ExpiresIn { get; }
        /// <summary>
        /// Gets the raw.
        /// </summary>
        /// <value>The raw.</value>
        string Raw { get; }
        /// <summary>
        /// Gets the refresh token.
        /// </summary>
        /// <value>The refresh token.</value>
        string RefreshToken { get; }
        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        /// <value>The type of the token.</value>
        string TokenType { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is error.
        /// </summary>
        /// <value><c>true</c> if this instance is error; otherwise, <c>false</c>.</value>
        bool IsError { get; }
        /// <summary>
        /// Gets the error.
        /// </summary>
        /// <value>The error.</value>
        string Error { get; }
    }
}
