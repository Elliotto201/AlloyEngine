using AlloyEngine;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyRuntime.IO
{
    internal sealed class Texture2DSerializer : ResourceSerializer
    {
        public override byte[] Serialize(byte[] rawAsset)
        {
            using var image = Image.Load<Rgba32>(rawAsset);

            image.Mutate(x => x.Flip(FlipMode.Vertical));

            int width = image.Width;
            int height = image.Height;

            byte[] pixelData = new byte[(width * height * 4) + sizeof(int) * 2];
            image.CopyPixelDataTo(new Span<byte>(pixelData, sizeof(int) * 2, width * height * 4));
            
            BinaryPrimitives.WriteInt32BigEndian(new Span<byte>(pixelData, 0, 4), width);
            BinaryPrimitives.WriteInt32BigEndian(new Span<byte>(pixelData, 4, 4), height);

            return pixelData;
        }

        public override object Deserialize(byte[] asset)
        {
            int width = BinaryPrimitives.ReadInt32BigEndian(new Span<byte>(asset, 0, 4));
            int height = BinaryPrimitives.ReadInt32BigEndian(new Span<byte>(asset, 4, 4));
            byte[] textureData = new Span<byte>(asset, 8, asset.Length - 8).ToArray();

            var texture = new Texture2D(width, height, textureData);
            return texture;
        }
    }
}
