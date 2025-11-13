using AlloyEngine;
using OpenTK.Audio.OpenAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlloyRuntime
{
    public sealed class AudioClip : EngineResource
    {
        private int audioBufferHandle;

        public AudioClip(byte[] pcmData, bool mono, int sampleRate)
        {
            audioBufferHandle = AL.GenBuffer();
            unsafe
            {
                fixed(byte* ptr = pcmData)
                {
                    AL.BufferData(audioBufferHandle, mono ? ALFormat.Mono16 : ALFormat.Stereo16, ptr, pcmData.Length, sampleRate);
                }
            }
        }

        protected override void Dispose()
        {
            
        }
    }
}
