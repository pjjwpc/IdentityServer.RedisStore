﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Newtonsoft.Json;

namespace RigoFunc.IdentityServer.Services.Redis.Serialization {
    public class ScopeConverter : JsonConverter {
        private readonly IScopeStore _scopeStore;

        /// <exception cref="ArgumentNullException"><paramref name="scopeStore"/> is <see langword="null" />.</exception>
        public ScopeConverter(IScopeStore scopeStore) {
            if (scopeStore == null) throw new ArgumentNullException(nameof(scopeStore));
            _scopeStore = scopeStore;
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            var source = (Scope)value;
            var target = new ScopeLite {
                Name = source.Name
            };
            serializer.Serialize(writer, target);
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var source = serializer.Deserialize<ScopeLite>(reader);
            var factory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);
            return factory.StartNew(async () => await _scopeStore.FindScopesAsync(new[] { source.Name }))
                    .Unwrap()
                    .GetAwaiter()
                    .GetResult()
                    .Single();
        }

        /// <inheritdoc />
        public override bool CanConvert(Type objectType) {
            return typeof(Scope) == objectType;
        }
    }
}
