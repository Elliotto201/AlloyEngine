using AlloyEngine3D_Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AlloyEngine3D
{
    internal static class AssetDatabase
    {
        private static Dictionary<AssetId, EngineObject> loadedAssets = new();

        private static string EditorAssetDirectory = string.Empty;
        private static string AssetPackPath = string.Empty;

        public static void InitEditor(string assetsPath)
        {
            EditorAssetDirectory = assetsPath;
        }

        public static void Init(string assetPackPath)
        {
            AssetPackPath = assetPackPath;
        }

        public static T CreateAssetEditor<T>(string path, string name, object[] constructorArguments) where T : EngineObject
        {
            T baseAsset = (T)FormatterServices.GetUninitializedObject(typeof(T));
            T asset = baseAsset.CreateNewAsset<T>(constructorArguments);

            var assetId = AssetIdGenerator.GetId();
            var assetFileReference = new AssetFileReference { Path = path };

            var type = typeof(T);
            var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public;

            type.GetProperty(nameof(EngineObject.Name), flags)?.SetValue(asset, name);
            type.GetProperty(nameof(EngineObject.AssetId), flags)?.SetValue(asset, assetId);
            type.GetProperty(nameof(EngineObject.AssetFile), flags)?.SetValue(asset, assetFileReference);

            loadedAssets.Add(assetId, asset);

            return asset;
        }
    }
}
