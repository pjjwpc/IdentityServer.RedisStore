namespace RigoFunc.IdentityServer.Services.Redis {
    public class RedisStoreOptions {
        public string config { get; set; }
        public int db { get; set; }

        public RedisStoreOptions(string config = "localhost", int db = 0) {
            this.config = config;
            this.db = db;
        }
    }
}