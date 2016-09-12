using System;
using System.IO;
using IdentityServer4.Models;

namespace RigoFunc.IdentityServer.DistributedStore {
    public class PersistedGrantSerializer : IDataSerializer<PersistedGrant> {
        private readonly IdentityServer4.Stores.Serialization.PersistentGrantSerializer _serializer;

        public PersistedGrantSerializer(IdentityServer4.Stores.Serialization.PersistentGrantSerializer serializer) {
            _serializer = serializer;
        }

        public virtual PersistedGrant Deserialize(byte[] data) {
            using (var memory = new MemoryStream(data)) {
                using (var reader = new BinaryReader(memory)) {
                    return Read(reader);
                }
            }
        }

        public virtual byte[] Serialize(PersistedGrant refreshToken) {
            using (var memory = new MemoryStream()) {
                using (var writer = new BinaryWriter(memory)) {
                    Write(writer, refreshToken);
                }
                return memory.ToArray();
            }
        }

        /// <exception cref="ArgumentNullException"><paramref name="writer"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="persistedGrant"/> is <see langword="null" />.</exception>
        public virtual void Write(BinaryWriter writer, PersistedGrant persistedGrant) {
            if (writer == null) {
                throw new ArgumentNullException(nameof(writer));
            }

            if (persistedGrant == null) {
                throw new ArgumentNullException(nameof(persistedGrant));
            }

            writer.Write(_serializer.Serialize(persistedGrant));
        }


        /// <exception cref="ArgumentNullException"><paramref name="reader"/> is <see langword="null" />.</exception>
        public virtual PersistedGrant Read(BinaryReader reader) {
            if (reader == null) {
                throw new ArgumentNullException(nameof(reader));
            }
            return _serializer.Deserialize<PersistedGrant>(reader.ReadString());
        }
    }
}
