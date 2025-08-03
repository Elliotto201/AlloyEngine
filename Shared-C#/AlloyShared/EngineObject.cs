using AlloyEngine3D_Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyEngine3D
{
    public abstract class EngineObject<T>
    {
        public string Name { get; private set; }
        public AssetId AssetId { get; private set; }
        public AssetFileReference AssetFile { get; private set; }

        internal EngineObject(string assetPath)
        {
            DeSerialize();
        }

        public abstract void DeSerialize(byte[] assetBytes);

        public abstract byte[] Serialize();
    }
}
