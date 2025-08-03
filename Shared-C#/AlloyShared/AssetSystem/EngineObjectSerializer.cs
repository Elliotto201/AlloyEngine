using System;
using System.IO;
using System.Text;

namespace AlloyEngine3D_Internal
{
    // Struct to hold serialized data, AssetId, and Name
    public struct SerializedEngineObject
    {
        public byte[] Data { get; set; }
        public AssetId AssetId { get; set; }
        public string Name { get; set; }
    }

    public static class EngineObjectSerializer
    {
        // Serializes a SerializedEngineObject into a byte array
        public static byte[] Serialize(SerializedEngineObject serializedObject)
        {
            using (var memoryStream = new MemoryStream())
            using (var writer = new BinaryWriter(memoryStream, Encoding.UTF8, leaveOpen: false))
            {
                // Serialize Name (length-prefixed UTF-8 string)
                byte[] nameBytes = Encoding.UTF8.GetBytes(serializedObject.Name ?? string.Empty);
                writer.Write(nameBytes.Length); // 4-byte length
                writer.Write(nameBytes);       // Name bytes

                // Serialize AssetId (8-byte ulong)
                writer.Write(serializedObject.AssetId.Id);

                // Serialize Data (length-prefixed byte array)
                byte[] data = serializedObject.Data ?? Array.Empty<byte>();
                writer.Write(data.Length); // 4-byte length
                writer.Write(data);        // Data bytes

                return memoryStream.ToArray();
            }
        }

        // Deserializes a byte array into a SerializedEngineObject
        public static SerializedEngineObject Deserialize(byte[] serializedData)
        {
            if (serializedData == null || serializedData.Length == 0)
            {
                throw new ArgumentNullException(nameof(serializedData), "Serialized data cannot be null or empty.");
            }

            using (var memoryStream = new MemoryStream(serializedData))
            using (var reader = new BinaryReader(memoryStream, Encoding.UTF8, leaveOpen: false))
            {
                try
                {
                    // Deserialize Name
                    int nameLength = reader.ReadInt32();
                    if (nameLength < 0 || nameLength > memoryStream.Length - memoryStream.Position)
                    {
                        throw new InvalidDataException("Invalid name length in serialized data.");
                    }
                    byte[] nameBytes = reader.ReadBytes(nameLength);
                    string name = Encoding.UTF8.GetString(nameBytes);

                    // Deserialize AssetId
                    if (memoryStream.Position + 8 > memoryStream.Length)
                    {
                        throw new InvalidDataException("Insufficient data for AssetId.");
                    }
                    ulong assetIdValue = reader.ReadUInt64();
                    AssetId assetId = new AssetId(assetIdValue);

                    // Deserialize Data
                    int dataLength = reader.ReadInt32();
                    if (dataLength < 0 || dataLength > memoryStream.Length - memoryStream.Position)
                    {
                        throw new InvalidDataException("Invalid data length in serialized data.");
                    }
                    byte[] data = reader.ReadBytes(dataLength);

                    return new SerializedEngineObject
                    {
                        Name = name,
                        AssetId = assetId,
                        Data = data
                    };
                }
                catch (EndOfStreamException)
                {
                    throw new InvalidDataException("Serialized data is corrupted or incomplete.");
                }
            }
        }
    }
}
