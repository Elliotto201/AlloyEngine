using AlloyEngine;
using MessagePack;
using MessagePack.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyRuntime.IO
{
    internal static class AssetEngine
    {
        private static AssetEngineContext Context;
        private static MessagePackSerializerOptions serializerOptions => ContractlessStandardResolverAllowPrivate.Options
            .WithSecurity(MessagePackSecurity.UntrustedData);

        internal static void InitializeEngine(AssetEngineContext context
            ,Dictionary<Type, AssetPostProcessor> _assetPostProcessors
            ,Dictionary<Type, CustomAssetSerializer> _assetSerializers)
        {
            Context = context;
        }

        internal static object LoadAsset(string assetName, Type type)
        {
            var path = Path.Combine(Context.AssetRootFolderPath, assetName);
            path = Path.GetFullPath(path);

            if (Context.IsYamlAssets)
            {
                var yamlAsset = File.ReadAllText(path);
                var deserializedAsset = YamlSerializer.Deserialize(yamlAsset, type);

                return deserializedAsset;
            }
            else
            {
                var binAsset = File.ReadAllBytes(path);
                var deserializedAsset = MessagePackSerializer.Deserialize(type, binAsset, serializerOptions);

                if (deserializedAsset == null)
                {
                    throw new Exception("Invalid Asset");
                }

                return deserializedAsset;
            }
        }

        internal static async Task<object> LoadAssetAsync(string assetName,Type type)
        {
            var path = Path.Combine(Context.AssetRootFolderPath, assetName);
            path = Path.GetFullPath(path);

            if (Context.IsYamlAssets)
            {
                var yamlAsset = await File.ReadAllTextAsync(path);
                var deserializedAsset = YamlSerializer.Deserialize(yamlAsset, type);

                return deserializedAsset;
            }
            else
            {
                using var binAssetStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                var deserializedAsset = await MessagePackSerializer.DeserializeAsync(type, binAssetStream, serializerOptions);

                if(deserializedAsset == null)
                {
                    throw new Exception("Invalid Asset");
                }

                return deserializedAsset;
            }
        }

        internal static byte[] LoadAssetRaw(string assetName)
        {
            var path = Path.Combine(Context.AssetRootFolderPath, assetName);
            path = Path.GetFullPath(path);

            return File.ReadAllBytes(path);
        }

        internal static async Task<byte[]> LoadAssetRawAsync(string assetName)
        {
            var path = Path.Combine(Context.AssetRootFolderPath, assetName);
            path = Path.GetFullPath(path);

            return await File.ReadAllBytesAsync(path);
        }

        internal static void WriteAsset<T>(string assetName, T asset)
        {
            var path = Path.Combine(Context.AssetRootFolderPath, assetName);
            path = Path.GetFullPath(path);

            if (Context.IsYamlAssets)
            {
                var serializedAsset = YamlSerializer.Serialize(asset);
                File.WriteAllText(path, serializedAsset);
            }
            else
            {
                var serializedAsset = MessagePackSerializer.Serialize(typeof(T), asset, serializerOptions);
                File.WriteAllBytes(path, serializedAsset);
            }
        }

        internal static void WriteAssetRaw(string assetName, byte[] data)
        {
            var path = Path.Combine(Context.AssetRootFolderPath, assetName);
            path = Path.GetFullPath(path);

            File.WriteAllBytes(path, data);
        }

        internal static void DeleteAsset(string assetName)
        {
            File.Delete(assetName);
        }
    }

    internal sealed class AssetEngineContext
    {
        public string AssetRootFolderPath;
        public bool IsYamlAssets;
    }
}
