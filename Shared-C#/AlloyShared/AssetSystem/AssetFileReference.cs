using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyEngine3D_Internal
{
    public struct AssetFileReference
    {
        public string Path { get; set; }

        public AssetFileReference(string assetPath)
        {
            Path = assetPath;
        }

        public void SetAssetPath(string newPath)
        {
            Path = newPath;
        }
    }
}
