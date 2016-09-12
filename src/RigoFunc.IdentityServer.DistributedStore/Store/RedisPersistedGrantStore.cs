using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Caching.Distributed;

namespace RigoFunc.IdentityServer.DistributedStore.Store {
    /// <summary>
    /// In-memory persisted grant store
    /// </summary>
    public class PersistedGrantStore : IPersistedGrantStore {
        private readonly IDistributedCache _cache;
        private readonly IDataSerializer<PersistedGrant> _serializer;
        private DistributedCacheEntryOptions _dceo;
        public PersistedGrantStore(IDistributedCache cache, IDataSerializer<PersistedGrant> serializer) {
            _cache = cache;
            _serializer = serializer;
            _dceo = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(15));
        }

        public Task StoreAsync(PersistedGrant grant) {
            var data = _serializer.Serialize(grant);
            return _cache.SetAsync(grant.Key, data, _dceo);
        }

        public async Task<PersistedGrant> GetAsync(string key) {
            var data = await _cache.GetAsync(key);
            if (data == null) {
                return null;
            }
            return _serializer.Deserialize(data);
        }

        public async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId) {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(string key) {
            return _cache.RemoveAsync(key);
        }

        public Task RemoveAllAsync(string subjectId, string clientId) {
            throw new NotImplementedException();
        }

        public Task RemoveAllAsync(string subjectId, string clientId, string type) {
            throw new NotImplementedException();
        }
    }
}
