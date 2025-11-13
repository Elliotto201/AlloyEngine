using NAudio.Wave;
using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyRuntime.IO
{
    internal sealed class AudioClipSerializer : ResourceSerializer
    {
        public override byte[] Serialize(byte[] rawAsset)
        {
            var ms = new MemoryStream(rawAsset);
            WaveStream reader = new WaveFileReader(ms);
            int sampleRate = reader.WaveFormat.SampleRate;
            bool mono = reader.WaveFormat.Channels == 1;
            var conv = new WaveFormatConversionStream(new WaveFormat(sampleRate, 16, reader.WaveFormat.Channels), reader);
            byte[] pcmData = new byte[conv.Length];
            conv.Read(pcmData, 0, pcmData.Length);

            byte[] finalData = new byte[pcmData.Length + sizeof(int) + 1];

            Array.Copy(pcmData, 0, finalData, 5, pcmData.Length);
            finalData[0] = Convert.ToByte(mono);
            BinaryPrimitives.WriteInt32BigEndian(new Span<byte>(finalData, 1, 4), sampleRate);

            return finalData;
        }

        public override object Deserialize(byte[] asset)
        {
            bool mono = asset[0] == 1;
            int sampleRate = BinaryPrimitives.ReadInt32BigEndian(new Span<byte>(asset, 1, 4));
            byte[] pcmData = new Span<byte>(asset, 5, asset.Length - 5).ToArray();

            return new AudioClip(pcmData, mono, sampleRate);
        }
    }
}
