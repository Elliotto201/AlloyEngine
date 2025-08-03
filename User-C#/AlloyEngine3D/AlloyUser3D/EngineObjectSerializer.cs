using AlloyEngine3D_Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyEngine3D
{
    // Struct to hold serialized data, AssetId, and Name
    internal struct SerializedEngineObject
    {
        public byte[] Data { get; set; }
        public AssetId AssetId { get; set; }
        public string Name { get; set; }
    }

    internal static class EngineObjectSerializer
    {
        // Serializes an EngineObject<T> into a SerializedEngineObject with efficient binary serialization
        public static SerializedEngineObject Serialize<T>(EngineObject<T> engineObject)
        {
            if (engineObject == null)
            {
                throw new ArgumentNullException(nameof(engineObject));
            }

            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream, Encoding.UTF8, leaveOpen: false))
            {
                // Serialize Name (length-prefixed UTF-8 string)
                byte[] nameBytes = Encoding.UTF8.GetBytes(engineObject.Name ?? string.Empty);
                writer.Write(nameBytes.Length); // 4-byte length
                writer.Write(nameBytes);       // Name bytes

                // Serialize AssetId (8-byte ulong)
                writer.Write(engineObject.AssetId.Id);

                // Serialize Data (placeholder: empty array or custom data)
                // TODO: Customize this if specific data is needed for Data field
                byte[] data = Array.Empty<byte>(); // Placeholder
                writer.Write(data.Length); // 4-byte length
                writer.Write(data);        // Data bytes

                // Create and return SerializedEngineObject
                return new SerializedEngineObject
                {
                    Data = memoryStream.ToArray(),
                    AssetId = engineObject.AssetId,
                    Name = engineObject.Name
                };
            }
        }

        // Deserializes an EngineObject<T> into a SerializedEngineObject with efficient binary serialization
        public static SerializedEngineObject Deserialize<T>(EngineObject<T> engineObject)
        {
            // Since Deserialize returns SerializedEngineObject and takes EngineObject<T>,
            // we serialize the input object's Name and AssetId into a binary format
            // This mirrors the Serialize method's logic
            if (engineObject == null)
            {
                throw new ArgumentNullException(nameof(engineObject));
            }

            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream, Encoding.UTF8, leaveOpen: false))
            {
                // Serialize Name (length-prefixed UTF-8 string)
                byte[] nameBytes = Encoding.UTF8.GetBytes(engineObject.Name ?? string.Empty);
                writer.Write(nameBytes.Length); // 4-byte length
                writer.Write(nameBytes);       // Name bytes

                // Serialize AssetId (8-byte ulong)
                writer.Write(engineObject.AssetId.Id);

                // Serialize Data (placeholder: empty array or custom data)
                // TODO: Customize this if specific data is needed for Data field
                byte[] data = Array.Empty<byte>(); // Placeholder
                writer.Write(data.Length); // 4-byte length
                writer.Write(data);        // Data bytes

                // Create and return SerializedEngineObject
                return new SerializedEngineObject
                {
                    Data = memoryStream.ToArray(),
                    AssetId = engineObject.AssetId,
                    Name = engineObject.Name
                };
            }
        }
    }
}
