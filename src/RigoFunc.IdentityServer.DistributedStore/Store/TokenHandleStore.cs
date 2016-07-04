using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Caching.Distributed;

namespace RigoFunc.IdentityServer.DistributedStore {
    public class TokenHandleStore : ITokenHandleStore {
        private readonly IDistributedCache _cache;
        private readonly IDataSerializer<Token> _serializer;

        public TokenHandleStore(IDistributedCache cache, IDataSerializer<Token> serializer) {
            _cache = cache;
            _serializer = serializer;
        }

        public async Task StoreAsync(string key, Token value) {
            var data = _serializer.Serialize(value);

            await _cache.SetAsync(key, data);
        }

        public async Task<Token> GetAsync(string key) {
            var data = await _cache.GetAsync(key);

            return _serializer.Deserialize(data);
        }

        public async Task RemoveAsync(string key) {
            await _cache.RemoveAsync(key);
        }

        public Task RevokeAsync(string subject, string client) {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ITokenMetadata>> GetAllAsync(string subject) {
            throw new NotImplementedException();
        }
    }
}
