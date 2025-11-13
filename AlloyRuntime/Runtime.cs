using AlloyRuntime.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyRuntime
{
    internal static class Runtime
    {
        public static void Start(string windowTitle, int width, int height, Window.GameConfig config, string assetFolder, bool assetsInTextFormat, Window.WindowIcon icon = null)
        {
            var window = new Window(windowTitle, width, height, config, icon);

            AssetLoader.Initialize(
            new AssetEngineContext
            {
                AssetRootFolderPath = assetFolder,
                IsYamlAssets = assetsInTextFormat,
            },
            new Dictionary<Type, AssetPostProcessor>
            {

            },
            new Dictionary<Type, CustomAssetSerializer>
            {

            },
            new Dictionary<Type, ResourceSerializer>
            {
                { typeof(Texture2D), new Texture2DSerializer() },
            });

            window.Run();
        }
    }
}
