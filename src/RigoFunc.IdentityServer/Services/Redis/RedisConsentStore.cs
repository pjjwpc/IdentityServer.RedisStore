using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Contrib.Store.Redis;
using IdentityServer4.Models;
using IdentityServer4.Services;
using StackExchange.Redis;

namespace RigoFunc.IdentityServer.Services.Redis {
    public class RedisConsentStore : BaseTokenStore<Consent>, IConsentStore {
        private readonly IDatabase _db;
        public RedisConsentStore(IClientStore clientStore, IScopeStore scopeStore, string config, int db = 0) : base(clientStore, scopeStore) {
            var connectionMultiplexer = RedisConnectionMultiplexerStore.GetConnectionMultiplexer(config);
            _db = connectionMultiplexer.GetDatabase(db);
        }
        public Task<IEnumerable<Consent>> LoadAllAsync(string subject) {
            throw new NotImplementedException();
        }

        public Task RevokeAsync(string subject, string client) {
            throw new NotImplementedException();
        }

        public Task<Consent> LoadAsync(string subject, string client) {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Consent consent) {
            throw new NotImplementedException();
        }
    }
}
