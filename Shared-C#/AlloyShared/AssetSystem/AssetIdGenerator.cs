using System;
using System.Collections.Generic;

namespace AlloyEngine3D_Internal
{
    public readonly struct AssetId : IEquatable<AssetId>
    {
        public readonly ulong Id;

        public AssetId(ulong id) => Id = id;

        public override bool Equals(object? obj) => obj is AssetId other && Equals(other);
        public bool Equals(AssetId other) => Id == other.Id;
        public override int GetHashCode() => Id.GetHashCode();

        public static bool operator ==(AssetId left, AssetId right) => left.Equals(right);
        public static bool operator !=(AssetId left, AssetId right) => !(left == right);

        public override string ToString() => Id.ToString();
    }

    public static class AssetIdGenerator
    {
        private static readonly HashSet<AssetId> ids = new();
        private static readonly object lockObj = new();

        public static AssetId GetId(AssetId requestedId)
        {
            lock (lockObj)
            {
                if (requestedId.Id != 0 && !ids.Contains(requestedId))
                {
                    ids.Add(requestedId);
                    return requestedId;
                }

                AssetId newId;
                var rng = new Random();
                do
                {
                    newId = new AssetId((ulong)rng.NextInt64(1, long.MaxValue));
                } while (ids.Contains(newId));

                ids.Add(newId);
                return newId;
            }
        }

        public static bool AddUsedId(AssetId id)
        {
            if (id.Id == 0) return false;

            lock (lockObj)
            {
                return ids.Add(id);
            }
        }
    }
}
