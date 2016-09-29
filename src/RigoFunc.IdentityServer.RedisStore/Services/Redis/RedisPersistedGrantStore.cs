using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using StackExchange.Redis;

namespace RigoFunc.IdentityServer.Services.Redis {
    /// <summary>
    /// In-memory persisted grant store
    /// </summary>
    public class RedisPersistedGrantStore : BaseTokenStore<PersistedGrant>, IPersistedGrantStore {
        private readonly IDatabase _db;
        public RedisPersistedGrantStore(IClientStore clientStore, IScopeStore scopeStore, RedisStoreOptions options)
            : this(clientStore, scopeStore, options.Configuration, options.Db) {

        }

        internal RedisPersistedGrantStore(IClientStore clientStore, IScopeStore scopeStore, string config, int db = 0) : base(clientStore, scopeStore) {
            var connectionMultiplexer = RedisConnectionMultiplexerStore.GetConnectionMultiplexer(config);
            _db = connectionMultiplexer.GetDatabase(db);
        }

        public Task StoreAsync(PersistedGrant grant) {
            var redisKey = $"PersistedGrant_{grant.Key}";
            _db.StringSetAsync(redisKey, ToJson(grant));
            return Task.FromResult(0);
        }

        public async Task<PersistedGrant> GetAsync(string key) {
            var redisKey = $"PersistedGrant_{key}";
            var json = await _db.StringGetAsync(redisKey);
            return FromJson(json);
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId) {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(string key) {
            return _db.KeyDeleteAsync(key);
        }

        public Task RemoveAllAsync(string subjectId, string clientId) {
            throw new NotImplementedException();
        }

        public Task RemoveAllAsync(string subjectId, string clientId, string type) {
            throw new NotImplementedException();
        }
    }
}
