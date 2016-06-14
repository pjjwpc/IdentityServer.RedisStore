namespace RigoFunc.OAuth {
    /// <summary>
    /// Represents all the options you can user to configure the OAuth server.
    /// </summary>
    public class OAuthServerOptions {
        /// <summary>
        /// Gets or sets the OAuth server host URL.
        /// </summary>
        /// <value>The OAuth server host URL.</value>
        public string OAuthUrl { get; set; }
        /// <summary>
        /// Gets or sets the name of the scope.
        /// </summary>
        /// <value>The name of the scope.</value>
        public string ScopeName { get; set; }
        /// <summary>
        /// Gets or sets the scope secret.
        /// </summary>
        /// <value>The scope secret.</value>
        public string ScopeSecret { get; set; }
    }
}
