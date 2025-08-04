using AlloyEngine3D_Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyEngine3D
{
    public abstract class EngineObject
    {
        public string Name { get; private set; }
        public AssetId AssetId { get; private set; }
        public AssetFileReference AssetFile { get; private set; }

        internal EngineObject(string assetPath)
        {
            var bytes = File.ReadAllBytes(assetPath);
            var deserializedEngineObjectData = EngineObjectSerializer.Deserialize(bytes);

            Name = deserializedEngineObjectData.Name;
            AssetId = deserializedEngineObjectData.AssetId;
            AssetIdGenerator.AddUsedId(AssetId);

            DeSerialize(deserializedEngineObjectData.Data);
        }

        public abstract void DeSerialize(byte[] assetBytes);

        public abstract byte[] Serialize();
    }
}
