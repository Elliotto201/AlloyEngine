using AlloyEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyRuntime.IO
{
    internal static class AssetLoader
    {
        private static Dictionary<Type, ResourceSerializer> resourceSerializers = new();

        internal static void Initialize(AssetEngineContext context
            ,Dictionary<Type, AssetPostProcessor> _assetPostProcessors
            ,Dictionary<Type, CustomAssetSerializer> _assetSerializers
            ,Dictionary<Type, ResourceSerializer> _resourceSerializers)
        {
            AssetEngine.InitializeEngine(context, _assetPostProcessors, _assetSerializers);
            resourceSerializers = _resourceSerializers;
        }

        internal static T LoadEngineResource<T>(string assetName) where T : EngineResource
        {
            var type = typeof(T);
            return (T)LoadEngineResource(assetName, type);
        }

        internal static EngineResource LoadEngineResource(string assetName, Type type)
        {
            var resourceData = AssetEngine.LoadAssetRaw(assetName);

            if (resourceSerializers.TryGetValue(type, out var deserializer))
            {
                var resource = deserializer.Deserialize(resourceData);
                
                var engineResource = (EngineResource)resource;
                engineResource.CreateObject();

                return engineResource;
            }
            else
            {
                throw new Exception();
            }
        }

        internal static void WriteResource<T>(string assetName, T resource) where T : EngineResource
        {
            WriteResource(assetName, resource, typeof(T));
        }

        internal static void WriteResource(string assetName, EngineResource resource, Type type)
        {
            if (resourceSerializers.TryGetValue(type, out var serializer))
            {
                var resourceData = serializer.Serialize(resource);
                AssetEngine.WriteAssetRaw(assetName, resourceData);
            }
            else
            {
                throw new Exception();
            }
        }

        internal static T LoadAsset<T>(string assetName) where T : EngineObject
        {
            return (T)LoadAsset(assetName, typeof(T));
        }

        internal static EngineObject LoadAsset(string assetName, Type type)
        {
            return (EngineObject)AssetEngine.LoadAsset(assetName, type);
        }

        internal static void DeleteAsset(string assetName)
        {
            AssetEngine.DeleteAsset(assetName);
        }
    }
}
