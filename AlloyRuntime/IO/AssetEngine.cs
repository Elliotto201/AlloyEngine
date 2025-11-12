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

        internal static void InitializeEngine(AssetEngineContext context)
        {
            Context = context;
            Type interfaceType = typeof(IAssetPostProcessor<>);
            AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch { return Array.Empty<Type>(); }
                })
                .Where(t => t.IsClass && !t.IsAbstract && interfaceType.IsAssignableFrom(t));
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

        internal static void CreateAsset<T>(string assetName, T asset)
        {
            var path = Path.Combine(Context.AssetRootFolderPath, assetName);
            path = Path.GetFullPath(path);

            if (Context.IsYamlAssets)
            {
                using var yamlAssetStream = File.CreateText(path);
                var serializedAsset = YamlSerializer.Serialize(asset);

                yamlAssetStream.Write(serializedAsset);

                yamlAssetStream.Flush();
            }
            else
            {
                using var binAssetStream = File.Create(path);
                var serializedAsset = MessagePackSerializer.Serialize(typeof(T), asset, serializerOptions);

                binAssetStream.Write(serializedAsset);

                binAssetStream.Flush(true);
            }
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
